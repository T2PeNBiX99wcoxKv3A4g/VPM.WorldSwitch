using io.github.ykysnk.utils;
using io.github.ykysnk.utils.Extensions;
using UnityEngine;
using VRC.SDKBase;

namespace io.github.ykysnk.WorldSwitch
{
    [AddComponentMenu("yky/World Switch/Collision Switch")]
    public class CollisionSwitch : BaseSwitch
    {
        [Header("Layer")] [SerializeField] private LayerMask defaultCollisionLayerMask;
        [SerializeField] private LayerMask changeCollisionLayerMask;

        [Header("Chair")] [SerializeField] private Collider[] chairs;

        [SerializeField] [HideInInspector] private int defaultCollisionLayer;
        [SerializeField] [HideInInspector] private int changeCollisionLayer;

#if !COMPILER_UDONSHARP && UNITY_EDITOR
        protected override void OnChange()
        {
            defaultCollisionLayer = Utils.ToLayer(defaultCollisionLayerMask);
            changeCollisionLayer = Utils.ToLayer(changeCollisionLayerMask);
            base.OnChange();
        }
#endif

        protected override void OnSwitchMode() => CollisionLayerToggle();

        private void CollisionLayerToggle()
        {
            foreach (var chair in chairs)
            {
                if (!Utilities.IsValid(chair)) continue;
                var obj = chair.gameObject;
                var newLayer = Mode == 0 ? defaultCollisionLayer : changeCollisionLayer;

                Log($"Collision layer of '{obj.FullName()}' toggle to '{LayerMask.LayerToName(newLayer)}'");
                obj.layer = newLayer;
            }
        }
    }
}