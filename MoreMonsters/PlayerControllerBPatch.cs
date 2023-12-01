using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreMonsters.PlayerBControllerPatches
{
    [HarmonyPatch(typeof(PlayerControllerB))]
    public class PlayerControllerBPatch
    {
        [HarmonyPatch("Update")]
        [HarmonyPrefix]
        static void patchControllerUpdate()
        {
            MoreMonstersBase.myGUI.guiIsHost = MoreMonstersBase.isHost;
            MoreMonstersBase.Instance.updateCFGVarsViaGui();
        }
    }
}
