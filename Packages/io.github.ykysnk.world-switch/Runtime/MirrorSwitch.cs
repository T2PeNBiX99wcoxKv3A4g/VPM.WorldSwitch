using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldSwitch
{
    [AddComponentMenu("yky/World Switch/Mirror Switch")]
    public class MirrorSwitch : BaseSwitch
    {
        private const string TransparencyProp = "_Transparency";
        private const string TransparencyKey = "transparency";
        private const string ShaderName = "VRCPlayersOnlyMirror";

        [Space] [SerializeField] private GameObject[] mirrors;

        [Space] [Header("UI")] [SerializeField]
        private Slider transparencySlider;

        [SerializeField] [Range(0, 1)] private float transparency;
        [SerializeField] private GameObject mirrorTransparencyUI;
        [SerializeField] private GameObject mirrorOptionUI;
        [SerializeField] [HideInInspector] private int transparencyPropID;
        private bool _showMirrorUI;

        protected override int MaxMode => mirrors.Length;

        protected override void Start()
        {
            if (Utilities.IsValid(mirrorTransparencyUI))
                mirrorTransparencyUI.SetActive(false);
            if (Utilities.IsValid(mirrorOptionUI))
                mirrorOptionUI.SetActive(false);
            if (Utilities.IsValid(transparencySlider))
                transparencySlider.value = transparency;
            base.Start();
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        protected override void OnChange()
        {
            if (Utilities.IsValid(transparencySlider))
                transparencySlider.value = transparency;

            transparencyPropID = Shader.PropertyToID(TransparencyProp);
            base.OnChange();
        }
#endif

        private static Material GetMaterial(GameObject mirror)
        {
            var meshRenderer = mirror.GetComponent<MeshRenderer>();
            if (!Utilities.IsValid(meshRenderer)) return null;
            var mat = meshRenderer.material;
            return mat;
        }

        private void ChangeTransparency(GameObject mirror)
        {
            if (!Utilities.IsValid(mirror)) return;
            var mat = GetMaterial(mirror);
            if (!Utilities.IsValid(mat))
            {
                _showMirrorUI = false;
                return;
            }

            var shaderName = mat.shader.name;
            _showMirrorUI = shaderName.Contains(ShaderName);

            if (!_showMirrorUI) return;
            if (Utilities.IsValid(transparencySlider))
                mat.SetFloat(transparencyPropID, transparencySlider.value);
            else
                mat.SetFloat(transparencyPropID, 0);
        }

        protected override void OnSwitchMode()
        {
            if (mirrors.Length < 1) return;
            if (Mode < 1)
            {
                if (Utilities.IsValid(mirrorTransparencyUI))
                    mirrorTransparencyUI.SetActive(false);
                if (Utilities.IsValid(mirrorOptionUI))
                    mirrorOptionUI.SetActive(false);
                if (mirrors.Length < 2) return;
                if (LastMode == Mode || LastMode < 1) return;
                var lastMirror = mirrors[LastMode - 1];
                if (!Utilities.IsValid(lastMirror)) return;
                lastMirror.SetActive(false);
                return;
            }

            var mirror = mirrors[Mode - 1];
            if (!Utilities.IsValid(mirror)) return;

            mirror.SetActive(true);
            ChangeTransparency(mirror);

            if (Utilities.IsValid(mirrorTransparencyUI))
                mirrorTransparencyUI.SetActive(_showMirrorUI);
            if (Utilities.IsValid(mirrorOptionUI))
                mirrorOptionUI.SetActive(true);
            if (mirrors.Length < 2) return;
            if (LastMode == Mode || LastMode < 1) return;
            var lastMirror2 = mirrors[LastMode - 1];
            if (!Utilities.IsValid(lastMirror2)) return;
            lastMirror2.SetActive(false);
        }

        [PublicAPI]
        public override void OnValueChanged()
        {
            foreach (var mirror in mirrors)
                ChangeTransparency(mirror);
            base.OnValueChanged();
        }

        [PublicAPI]
        public void ToggleMirror_1() => ToggleMode(1);

        [PublicAPI]
        public void ToggleMirror_2() => ToggleMode(2);

        [PublicAPI]
        public void ToggleMirror_3() => ToggleMode(3);

        [PublicAPI]
        public void ToggleMirror_4() => ToggleMode(4);

        [PublicAPI]
        public void ToggleMirror_5() => ToggleMode(5);

        [PublicAPI]
        public void ToggleMirror_6() => ToggleMode(6);

        protected override void OnLoad(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(transparencySlider) ||
                !PlayerData.TryGetFloat(player, SaveKey(TransparencyKey), out var transparencySaved)) return;
            transparencySlider.value = transparencySaved;
        }

        protected override void OnSave(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(transparencySlider)) return;
            PlayerData.SetFloat(SaveKey(TransparencyKey), transparencySlider.value);
        }
    }
}