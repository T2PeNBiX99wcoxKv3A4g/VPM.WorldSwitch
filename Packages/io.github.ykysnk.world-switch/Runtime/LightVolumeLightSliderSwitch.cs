using JetBrains.Annotations;
using UdonSharp;
using UnityEngine;
using UnityEngine.UI;
using VRC.SDK3.Persistence;
using VRC.SDKBase;
using VRCLightVolumes;

namespace io.github.ykysnk.WorldSwitch
{
    [AddComponentMenu("yky/World Switch/VRChat Light Volume Light Slider Switch")]
    public class LightVolumeLightSliderSwitch : BaseSwitch
    {
        private const string IntensityKey = "intensity";

        [Space] [Header("Light")] [SerializeField]
        private PointLightVolumeInstance[] lights;

        [Space] [Header("UI")] [SerializeField]
        private Slider intensitySlider;

        [SerializeField] [Range(0, 1)] private float intensity = 0.3f;
        [SerializeField] private GameObject lightIntensityUI;

        [UdonSynced] private float _intensity;

        protected override void Start()
        {
            if (Utilities.IsValid(lightIntensityUI))
                lightIntensityUI.SetActive(false);
            if (Utilities.IsValid(intensitySlider))
                intensitySlider.value = intensity;
            base.Start();
            ChangeIntensity();
        }

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        protected override void OnChange()
        {
            if (Utilities.IsValid(intensitySlider))
                intensitySlider.value = intensity;

            foreach (var light2 in lights)
            {
                if (!Utilities.IsValid(light2)) continue;
                light2.Intensity = intensity;
            }

            base.OnChange();
        }
#endif

        protected override void OnSwitchMode()
        {
            foreach (var light2 in lights)
            {
                if (!Utilities.IsValid(light2)) continue;
                light2.gameObject.SetActive(Mode != 0);
            }

            if (Utilities.IsValid(lightIntensityUI))
                lightIntensityUI.SetActive(Mode != 0);
        }

        private void ChangeIntensity()
        {
            if (!Utilities.IsValid(intensitySlider)) return;
            _intensity = intensitySlider.value;

            foreach (var light2 in lights)
            {
                if (!Utilities.IsValid(light2)) continue;
                light2.Intensity = intensitySlider.value;
            }
        }

        [PublicAPI]
        public override void OnValueChanged()
        {
            ChangeIntensity();
            base.OnValueChanged();
        }

        protected override void OnLoad(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(intensitySlider) ||
                !PlayerData.TryGetFloat(player, IntensityKey, out var intensitySaved)) return;
            intensitySlider.value = _intensity = intensitySaved;
        }

        protected override void OnSave(VRCPlayerApi player)
        {
            if (!Utilities.IsValid(intensitySlider)) return;
            PlayerData.SetFloat(SaveKey(IntensityKey), intensitySlider.value);
        }

        protected override void OnAfterSynchronize(bool isOwner)
        {
            if (isOwner) return;
            intensitySlider.value = _intensity;
        }
    }
}