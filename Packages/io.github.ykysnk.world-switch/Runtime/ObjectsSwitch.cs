using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldSwitch
{
    [AddComponentMenu("yky/World Switch/Objects Switch")]
    public class ObjectsSwitch : BaseSwitch
    {
        [Space] [Header("Object")] [SerializeField]
        private GameObject[] objects;

        protected override void OnSwitchMode()
        {
            foreach (var obj in objects)
            {
                if (!Utilities.IsValid(obj)) continue;
                obj.SetActive(Mode != 0);
            }
        }
    }
}