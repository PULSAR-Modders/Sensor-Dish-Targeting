using HarmonyLib;
using PulsarModLoader.Patches;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace Sensor_Dish_Targeting
{
    //PLShipInfo lines 5777, 5799, 5821
    [HarmonyPatch(typeof(PLShipInfo), "UpdateSensorDish")]
    internal class ShipInfo
    {
        static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            //if (this.SensorDishCurrentTarget == null &&
            List<CodeInstruction> targetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0), //this
                new CodeInstruction(OpCodes.Ldfld, AccessTools.Field(typeof(PLShipInfo), "SensorDishCurrentTarget")),
                new CodeInstruction(OpCodes.Ldnull), //null
                new CodeInstruction(OpCodes.Call), // ==
                new CodeInstruction(OpCodes.Brfalse)
            };

            List<CodeInstruction> patchSequence = new List<CodeInstruction>();

            instructions = HarmonyHelpers.PatchBySequence(instructions, targetSequence, patchSequence,
                HarmonyHelpers.PatchMode.REPLACE, HarmonyHelpers.CheckMode.NONNULL);

            //this.SensorDishCurrentSecondaryTarget_Scrap = plspaceScrap;
            //if (plplayer != 
            targetSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Leave_S),
                new CodeInstruction(OpCodes.Ldloca_S),
                new CodeInstruction(OpCodes.Constrained),
                new CodeInstruction(OpCodes.Callvirt),
                new CodeInstruction(OpCodes.Endfinally),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ldnull)
            };

            patchSequence = new List<CodeInstruction>()
            {
                new CodeInstruction(OpCodes.Ldarg_0), //this
                new CodeInstruction(OpCodes.Ldloc_1), //normalized
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(ShipInfo), "ScrapOverrideShip"))
            };

            return HarmonyHelpers.PatchBySequence(instructions, targetSequence, patchSequence,
                HarmonyHelpers.PatchMode.AFTER, HarmonyHelpers.CheckMode.NEVER);
        }

        public static void ScrapOverrideShip(PLShipInfo instance, Vector3 sensorDir)
        {
            if (instance != PLEncounterManager.Instance.PlayerShip ||
                instance.SensorDishCurrentTarget == null ||
                instance.SensorDishCurrentSecondaryTarget_Scrap == null) return;

            Vector3 shipPos = instance.SensorDishCurrentTarget.Exterior.transform.position;
            Vector3 scrapPos = instance.SensorDishCurrentSecondaryTarget_Scrap.transform.position;
            Vector3 sensorPos = instance.GetSensorDishPosition();
            float shipDot = Vector3.Dot(Vector3.Normalize(shipPos - sensorPos), sensorDir);
            float scrapDot = Vector3.Dot(Vector3.Normalize(scrapPos - sensorPos), sensorDir);

            if (scrapDot > shipDot)
            {
                instance.SensorDishCurrentTarget = null;
            }
            else
            {
                instance.SensorDishCurrentSecondaryTarget_Scrap = null;
            }
        }
    }
}
