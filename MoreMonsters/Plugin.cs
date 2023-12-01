using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using MoreMonsters.GuiMenuComponent;
using BepInEx.Configuration;
using System.Runtime.CompilerServices;
using MoreMonsters.PlayerBControllerPatches;

namespace MoreMonsters
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MoreMonstersBase : BaseUnityPlugin
    {
        private const string modGUID = "Quokka.MoreMonsters";
        private const string modName = "MoreMonsters";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static MoreMonstersBase Instance;

        private static ConfigEntry<int> nMobsLabel;
        private static ConfigEntry<float> timeBetweenMobSpawns;
        private static ConfigEntry<bool> enableSpawnMobsAsScrapIsFound;

        private static bool hasGuiSynced = false;
        internal static bool isHost;

        public static ManualLogSource mls;

        private static RoundManager currentRound;

        public static bool firstTier = false;
        public static bool secondTier = false;
        public static bool thirdTier = false;

        public static float timeToSpawn = 120f;
        public static int ventIndex = 0;

        internal static GuiMenu myGUI;

        void Awake()
        {
            /*
            if(Instance == null)
            {
                Instance = this;
            }
            */
            Instance = this;

            mls = BepInEx.Logging.Logger.CreateLogSource(modGUID);

            

            harmony.PatchAll(typeof(MoreMonstersBase));
            harmony.PatchAll(typeof(PlayerControllerBPatch)); // this is necessary. No clue how other guy avoided it

            var gameObject = new UnityEngine.GameObject("GuiMenu"); // was GUIMenu before
            UnityEngine.GameObject.DontDestroyOnLoad(gameObject);
            gameObject.hideFlags = HideFlags.HideAndDontSave;
            gameObject.AddComponent<GuiMenu>();
            myGUI = (GuiMenu)gameObject.GetComponent("GuiMenu");
            SetBindings();
            setGuiVars();

            
        }

        void setGuiVars() // sets vars initially. update handles further changes
        {
            // settings the vars doesn't help
            myGUI.guiNMobs = nMobsLabel.Value;
            myGUI.guiTimeBetweenMobSpawns = timeBetweenMobSpawns.Value;
            myGUI.guiEnableSpawnMobsAsScrapIsFound = enableSpawnMobsAsScrapIsFound.Value;
            hasGuiSynced = true;
        }

        internal void updateCFGVarsViaGui()
        {
            if(!hasGuiSynced)
            {
                setGuiVars();
            }

            nMobsLabel.Value = myGUI.guiNMobs;
            timeBetweenMobSpawns.Value = myGUI.guiTimeBetweenMobSpawns;
            enableSpawnMobsAsScrapIsFound.Value = myGUI.guiEnableSpawnMobsAsScrapIsFound;
        }

        void Update() // not needed for now
        {}

        private void SetBindings()
        {
            nMobsLabel = Config.Bind("Mob Settings", "TotalNumber of Mobs", 8, new ConfigDescription("Base number of mobs to spawn in total", new AcceptableValueRange<int>(1, 100)));
            timeBetweenMobSpawns = Config.Bind("Mob Settings", "Time between each Mob Spawn", 100f, new ConfigDescription("Time between each mob spawn where 0.1 = 1.5 hours", new AcceptableValueRange<float>(0, 800)));
            enableSpawnMobsAsScrapIsFound = Config.Bind("Mob Settings", "Toggle whether more mobs spawn as scrap is found.", false, "If true, an additional mob will spawn at 25%, 50%, and 75% of scrap found in level");
            
        }

        
        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        [HarmonyPostfix]
        static void ModifyLevel(ref SelectableLevel newLevel)
        {

            currentRound = RoundManager.Instance;
        }
        
        

        [HarmonyPatch(typeof(RoundManager), "Start")]
        [HarmonyPostfix]
        static void setIsHost()
        {
            mls.LogInfo("Host Status: " + RoundManager.Instance.NetworkManager.IsHost.ToString());
            isHost = RoundManager.Instance.NetworkManager.IsHost;
            MoreMonstersBase.myGUI.guiIsHost = isHost;
            Instance.updateCFGVarsViaGui(); // only called once - playerController makes sense as best place to call from
        }
        


        [HarmonyPatch(typeof(RoundManager), "SpawnInsideEnemiesFromVentsIfReady")]
        [HarmonyPostfix]
        static void SpawnInsideEnemiesFromVentsIfReadyPatch()
        {
            currentRound = RoundManager.Instance;
            //currentRound.timeScript.globalTimeSpeedMultiplier = 1f;
            currentRound.currentLevel.maxEnemyPowerCount = 250;

            int nPlayers = currentRound.playersManager.connectedPlayersAmount;
            /*
            int nMobs = (int)(0.75 * nPlayers);
            if (nPlayers < 4)
                nMobs = 6;
            */
            

            if ((currentRound.currentEnemySpawnIndex < nMobsLabel.Value) && (currentRound.timeScript.currentDayTime > timeToSpawn) && (currentRound.allEnemyVents.Length > 0))
            {
                //mls.LogInfo("Vent Length: " + currentRound.allEnemyVents.Length);
                Vector3 pos = currentRound.allEnemyVents[ventIndex].floorNode.position;
                float y = currentRound.allEnemyVents[ventIndex].floorNode.eulerAngles.y;
                int random = (ventIndex * (int)currentRound.timeScript.currentDayTime + (19 * (ventIndex + 3))) % 8;
                while (currentRound.currentLevel.Enemies[random].enemyType.numberSpawned > 4)
                {
                    random++;
                    random = random % currentRound.allEnemyVents.Length;
                }


                currentRound.SpawnEnemyOnServer(pos, y, random);

                currentRound.currentEnemySpawnIndex++;
                ventIndex++;
                ventIndex = ventIndex % currentRound.allEnemyVents.Length;
                currentRound.currentLevel.Enemies[random].enemyType.numberSpawned++;

                timeToSpawn = currentRound.timeScript.currentDayTime + timeBetweenMobSpawns.Value; // adds delay for mob spawning so that they don't spawn all at once



                if (enableSpawnMobsAsScrapIsFound.Value)
                {
                    if ((currentRound.valueOfFoundScrapItems > (int)(0.25 * currentRound.totalScrapValueInLevel)) && !firstTier)
                    {
                        nMobsLabel.Value++;
                        firstTier = true;
                    }
                    if ((currentRound.valueOfFoundScrapItems > (int)(0.5 * currentRound.totalScrapValueInLevel)) && !secondTier)
                    {
                        nMobsLabel.Value++;
                        secondTier = true;
                    }
                    if ((currentRound.valueOfFoundScrapItems > (int)(0.75 * currentRound.totalScrapValueInLevel)) && !thirdTier)
                    {
                        nMobsLabel.Value++;
                        thirdTier = true;
                    }
                }
            }


            /*

                for (int i = 0; i < currentRound.allEnemyVents.Length; i++)
            {

                
                if ((currentRound.currentEnemySpawnIndex < nMobsLabel.Value) && (currentRound.timeScript.currentDayTime > timeToSpawn))
                {
                    //mls.LogInfo("nMobsLabel.Value" + nMobsLabel.Value);
                    
                }
                
            }
            */
        }
    }
}
