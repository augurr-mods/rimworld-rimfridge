using RimWorld;
using Verse;

namespace RimFridge
{
    class RimFridge_Building : Building_Storage
    {
        public RimFridge_Building() : base() { }

        public override void DeSpawn(DestroyMode mode = DestroyMode.Vanish)
        {
            Map map = Map;
            CompRefrigerator fridgeComp = ThingCompUtility.TryGetComp<CompRefrigerator>(this);
            base.DeSpawn(mode);
            if (fridgeComp != null && map != null)
            {
                FridgeCache.RemoveFridge(fridgeComp, map);
            }
        }

        /*public override IEnumerable<Gizmo> GetGizmos()
        {
            List<Gizmo> l = new List<Gizmo>(base.GetGizmos());
            return SaveStorageSettingsGizmoUtil.AddSaveLoadGizmos(l, "fridge", this.settings.filter);
        }*/
    }
}
