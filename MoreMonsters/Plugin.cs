using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace MoreMonsters
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MoreMonstersBase : BaseUnityPlugin
    {
        private const string modGUID = "Quokka.MoreMonsters";
        private const string modName = "MoreMonsters";
        private const string modVersion = "1.0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        private static MoreMonstersBase Instance;

        public static ManualLogSource mls;

        private static RoundManager currentRound;

        public static bool firstTier = false;
        public static bool secondTier = false;
        public static bool thirdTier = false;

        void Awake()
        {
            if(Instance == null)
            {
                Instance = this;
            }

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            

            harmony.PatchAll(typeof(MoreMonstersBase));
        }

        
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        [HarmonyPostfix]
        static void ModifyLevel(ref SelectableLevel newLevel)
        {

            currentRound = RoundManager.Instance;


            
            
            
        }

        


        [HarmonyPatch(typeof(RoundManager), "SpawnInsideEnemiesFromVentsIfReady")]
        [HarmonyPostfix]
        static void SpawnInsideEnemiesFromVentsIfReadyPatch()
        {
            currentRound = RoundManager.Instance;
            currentRound.timeScript.globalTimeSpeedMultiplier = 1f;
            currentRound.currentLevel.maxEnemyPowerCount = 250;

            int nPlayers = currentRound.playersManager.connectedPlayersAmount;
            int nMobs = (int)(0.75 * nPlayers);
            if (nPlayers < 4)
                nMobs = 6;

            

            for (int i = 0; i < currentRound.allEnemyVents.Length; i++)
            {
                
                
                if ((currentRound.currentEnemySpawnIndex < nMobs) && (currentRound.timeScript.currentDayTime / currentRound.timeScript.totalTime > 0.185f))
                {
                    
                    Vector3 pos = currentRound.allEnemyVents[i].floorNode.position;
                    float y = currentRound.allEnemyVents[i].floorNode.eulerAngles.y;
                    int random = (i * (int)currentRound.timeScript.currentDayTime + (19 * (i+3))) % 8;
                    while(currentRound.currentLevel.Enemies[random].enemyType.numberSpawned > 4)
                    {
                        random++;
                    }
                  
                    
                    currentRound.SpawnEnemyOnServer(pos, y, random);
                    
                    currentRound.currentEnemySpawnIndex++;
                    currentRound.currentLevel.Enemies[random].enemyType.numberSpawned++;

                    if((currentRound.valueOfFoundScrapItems > (int)(0.25*currentRound.totalScrapValueInLevel)) && !firstTier)
                    {
                        nMobs++;
                        firstTier = true;
                    }
                    if ((currentRound.valueOfFoundScrapItems > (int)(0.5 * currentRound.totalScrapValueInLevel)) && !secondTier)
                    {
                        nMobs++;
                        secondTier = true;
                    }
                    if ((currentRound.valueOfFoundScrapItems > (int)(0.75 * currentRound.totalScrapValueInLevel)) && !thirdTier)
                    {
                        nMobs++;
                        thirdTier = true;
                    }

                }
                
            }

        }
    }
}
