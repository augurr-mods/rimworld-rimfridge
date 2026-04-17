using RimWorld;
using System.Collections.Generic;
using System.Reflection;
using Verse;

namespace RimFridge
{
    internal static class RimFridgeSettingsUtil
    {
        public static Dictionary<string, float> BaseEnergy { get; set; }
        public static Dictionary<string, ThingDef> FridgeDefs { get; set; }

        private static readonly FieldInfo powerBaseConsumptionField = typeof(CompProperties_Power).GetField(
            "basePowerConsumption",
            BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        static RimFridgeSettingsUtil()
        {
            BaseEnergy = null;
        }

        private static void CreateBaseEnergyMap()
        {
            if (BaseEnergy == null)
            {
                BaseEnergy = new Dictionary<string, float>();
                FridgeDefs = new Dictionary<string, ThingDef>();
                foreach (ThingDef def in DefDatabase<ThingDef>.AllDefsListForReading)
                {
                    if (def.defName.StartsWith("RimFridge"))
                    {
                        CompProperties_Power power = def.GetCompProperties<CompProperties_Power>();
                        if (power != null)
                        {
                            BaseEnergy.Add(def.defName, power.PowerConsumption);
                            FridgeDefs.Add(def.defName, def);
                        }
                    }
                }
            }
        }

        public static void ApplyFactor(float newFactor)
        {
            CreateBaseEnergyMap();

            foreach (KeyValuePair<string, float> basePower in BaseEnergy)
            {
                ThingDef def = FridgeDefs[basePower.Key];
                CompProperties_Power power = def.GetCompProperties<CompProperties_Power>();
                float scaled = basePower.Value * newFactor;
                if (powerBaseConsumptionField != null)
                {
                    powerBaseConsumptionField.SetValue(power, scaled);
                }
                else
                {
                    Log.Warning("RimFridge: could not set fridge power (basePowerConsumption field missing); power factor setting ignored.");
                }
            }
        }
    }
}
