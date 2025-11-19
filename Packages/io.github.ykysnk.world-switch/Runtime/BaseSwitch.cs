using io.github.ykysnk.WorldBasic.Udon;
using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldSwitch
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    [PublicAPI]
    public abstract class BaseSwitch : BasicUdonSharpBehaviour
    {
        [Header("Button")] [SerializeField] protected GameObject turnOn;
        [SerializeField] protected GameObject turnOff;
        [SerializeField] protected bool isOn;
        [SerializeField] protected bool isLocal;
        [SerializeField] protected bool isSave;

        [UdonSynced] protected int LastMode;
        [UdonSynced] protected int Mode;

        protected virtual int MaxMode => 1;

        protected virtual void Start()
        {
            if (isOn)
                Mode = 1;
            SwitchMode();
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        protected override void OnChange()
        {
            // SendMessage cannot be called during Awake, CheckConsistency, or OnValidate (On: OnBecameInvisible)
            if (Utilities.IsValid(turnOn))
                turnOn.SetActive(isOn);
            if (Utilities.IsValid(turnOff))
                turnOff.SetActive(!isOn);
        }
#endif

        private void SwitchMode()
        {
            if (Utilities.IsValid(turnOn))
                turnOn.SetActive(Mode != 0);
            if (Utilities.IsValid(turnOff))
                turnOff.SetActive(Mode == 0);
            OnSwitchMode();
        }

        protected virtual void OnSwitchMode()
        {
        }

        protected void ToggleMode(int mode)
        {
            Mode = mode;

            if (Mode > MaxMode)
                Mode = 0;

            Log($"Last mode is {LastMode}");
            Log($"Now mode is {Mode}");
            Synchronize(isLocal);
            LastMode = mode;
        }

        [PublicAPI]
        public virtual void OnValueChanged() => Synchronize(isLocal);

        public override void Interact() => InteractCheck();

        protected override void InteractAntiCheat() => ToggleMode(Mode + 1);

        public override void OnPlayerRestored(VRCPlayerApi player) => Load(player);
        public override void OnPlayerLeft(VRCPlayerApi player) => Save(player);

        protected override void Load(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return;
            if (!isLocal && !IsFirstMaster(player)) return;

            GetOwner();

            if (!isSave || !player.isLocal || !PlayerData.TryGetInt(player, SaveMode(), out var mode) ||
                mode > MaxMode) return;
            Mode = mode;
            LastMode = mode;

            OnLoad(player);
            Log($"Loaded from save: {SaveMode()}, {mode}");
            Synchronize(isLocal);
        }

        protected override void Save(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(player)) return;
            if (!isLocal && !IsFirstMaster(player)) return;
            if (!isSave || !player.isLocal) return;
            PlayerData.SetInt(SaveMode(), Mode);
            OnSave(player);
        }

        protected override void AfterSynchronize(bool isOwner)
        {
            SwitchMode();
            Save();
            OnAfterSynchronize(isOwner);
        }

        protected virtual void OnLoad(VRCPlayerApi player)
        {
        }

        protected virtual void OnSave(VRCPlayerApi player)
        {
        }

        protected virtual void OnAfterSynchronize(bool isOwner)
        {
        }
    }
}