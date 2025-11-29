using io.github.ykysnk.WorldBasic.Udon;
using MimyLab.FukuroUdon;
using UdonSharp;
using UnityEngine;
using VRC.SDK3.Components;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldSwitch
{
    [AddComponentMenu("yky/World Switch/Reset Button")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ResetButton : BasicUdonSharpBehaviour
    {
        [Header("Button")] [SerializeField] private bool isLocal;
        [Space] [SerializeField] private GameObject[] objects;

        private void Press()
        {
            foreach (var obj in objects)
            {
                var sync = obj.GetComponent<VRCObjectSync>();

                if (Utilities.IsValid(sync))
                {
                    sync.Respawn();
                    continue;
                }

                var sync2 = obj.GetComponent<ManualObjectSync>();

                if (Utilities.IsValid(sync2))
                    sync2.Respawn();
            }
        }

        public override void Interact() => InteractCheck();
        protected override void InteractAntiCheat() => Synchronize(isLocal);
        protected override void AfterSynchronize(bool isOwner) => Press();
    }
}