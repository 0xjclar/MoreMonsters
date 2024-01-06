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
using System.Security.Cryptography;
using System.Xml.Schema;
using GameNetcodeStuff;

namespace MoreMonsters
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class MoreMonstersBase : BaseUnityPlugin
    {
        private const string modGUID = "Quokka.MoreMonsters";
        private const string modName = "MoreMonsters";
        private const string modVersion = "1.2.1";

        private readonly Harmony harmony = new Harmony(modGUID);

        internal static MoreMonstersBase Instance;

        private static ConfigEntry<int> nMobsLabel;
        private static ConfigEntry<float> timeBetweenMobSpawns;
        private static ConfigEntry<bool> enableSpawnMobsAsScrapIsFound;
        private static ConfigEntry<int> maxCentipedesLabel;
        private static ConfigEntry<int> maxSandSpiderLabel;
        private static ConfigEntry<int> maxHoarderBugLabel;
        private static ConfigEntry<int> maxFlowermanLabel;
        private static ConfigEntry<int> maxCrawlerLabel;
        private static ConfigEntry<int> maxBlobLabel;
        private static ConfigEntry<int> maxSpringManLabel;
        private static ConfigEntry<int> maxPufferLabel;
        private static ConfigEntry<int> maxNutcrackerLabel;
        private static ConfigEntry<int> maxDressGirlLabel;
        private static ConfigEntry<int> maxJesterLabel;
        private static ConfigEntry<int> maxMaskedPlayerEnemyLabel;
        private static ConfigEntry<int> maxLassoManLabel;

        private static bool hasGuiSynced = false;
        internal static bool isHost;

        public static ManualLogSource mls;

        private static RoundManager currentRound;

        public static bool firstTier = false;
        public static bool secondTier = false;
        public static bool thirdTier = false;

        public static float timeToSpawn = 120f; // slightly after spawntime
        public static int ventIndex = 0;

        internal static GuiMenu myGUI;

        public static Dictionary<string, int> maxEnemyList = new Dictionary<string, int>();
        public static Dictionary<string, int> currentEnemyList = new Dictionary<string, int>();
        public static int[] pureEnemyList = new int[13];
        public static int[] pureMaxEnemyList = new int[13];

        public static int spawnedMonsterTotal = 0;

        void Awake()
        {
            /*
            if(Instance == null)
            {
                Instance = this;
            }
            */

            // Major credit to @lawrencea13 for his Lethal Company Game Master code. Made understanding
            // how gui works much easier.

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

            

            maxEnemyList.Add("Centipede", 0);
            maxEnemyList.Add("SandSpider", 0);
            maxEnemyList.Add("HoarderBug", 0);
            maxEnemyList.Add("Flowerman", 0);
            maxEnemyList.Add("Crawler", 0);
            maxEnemyList.Add("Blob", 0);
            maxEnemyList.Add("DressGirl", 0);
            maxEnemyList.Add("Puffer", 0);
            maxEnemyList.Add("SpringMan", 0);
            maxEnemyList.Add("Nutcracker", 0);
            maxEnemyList.Add("Jester", 0);
            maxEnemyList.Add("MaskedPlayerEnemy", 0);
            maxEnemyList.Add("LassoMan", 0);



            currentEnemyList.Add("Centipede", 0);
            currentEnemyList.Add("SandSpider", 0);
            currentEnemyList.Add("HoarderBug", 0);
            currentEnemyList.Add("Flowerman", 0);
            currentEnemyList.Add("Crawler", 0);
            currentEnemyList.Add("Blob", 0);
            currentEnemyList.Add("DressGirl", 0);
            currentEnemyList.Add("Puffer", 0); 
            currentEnemyList.Add("SpringMan", 0);
            currentEnemyList.Add("Nutcracker", 0);
            currentEnemyList.Add("Jester", 0);
            currentEnemyList.Add("MaskedPlayerEnemy", 0);
            currentEnemyList.Add("LassoMan", 0);



        }

        void setGuiVars() // sets vars initially. update handles further changes
        {
            // settings the vars doesn't help
            myGUI.guiNMobs = nMobsLabel.Value;
            myGUI.guiTimeBetweenMobSpawns = timeBetweenMobSpawns.Value;
            myGUI.guiEnableSpawnMobsAsScrapIsFound = enableSpawnMobsAsScrapIsFound.Value;
            hasGuiSynced = true;
            myGUI.guiMaxCentipedes = maxCentipedesLabel.Value;
            myGUI.guiMaxSandSpiders = maxSandSpiderLabel.Value;
            myGUI.guiMaxHoarderBugs = maxHoarderBugLabel.Value;
            myGUI.guiMaxFlowerMen = maxFlowermanLabel.Value;
            myGUI.guiMaxCrawlers = maxCrawlerLabel.Value;
            myGUI.guiMaxBlobs = maxBlobLabel.Value;
            myGUI.guiMaxSpringMen = maxSpringManLabel.Value;
            myGUI.guiMaxPuffers = maxPufferLabel.Value;
            myGUI.guiMaxNutcrackers = maxNutcrackerLabel.Value;
            myGUI.guiMaxDressGirls = maxDressGirlLabel.Value;
            myGUI.guiMaxJesters = maxJesterLabel.Value;
            myGUI.guiMaxMaskedPlayerEnemies = maxMaskedPlayerEnemyLabel.Value;
            myGUI.guiMaxLassoMen = maxLassoManLabel.Value;
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
            maxCentipedesLabel.Value = myGUI.guiMaxCentipedes;
            maxSandSpiderLabel.Value = myGUI.guiMaxSandSpiders;
            maxHoarderBugLabel.Value = myGUI.guiMaxHoarderBugs;
            maxFlowermanLabel.Value = myGUI.guiMaxFlowerMen;
            maxCrawlerLabel.Value = myGUI.guiMaxCrawlers;
            maxBlobLabel.Value = myGUI.guiMaxBlobs;
            maxSpringManLabel.Value = myGUI.guiMaxSpringMen;
            maxPufferLabel.Value = myGUI.guiMaxPuffers;
            maxNutcrackerLabel.Value = myGUI.guiMaxNutcrackers;
            maxDressGirlLabel.Value = myGUI.guiMaxDressGirls;
            maxJesterLabel.Value = myGUI.guiMaxJesters;
            maxMaskedPlayerEnemyLabel.Value = myGUI.guiMaxMaskedPlayerEnemies;
            maxLassoManLabel.Value = myGUI.guiMaxLassoMen;
        }

        void Update() // not needed for now
        {}

        private void SetBindings()
        {
            nMobsLabel = Config.Bind("Mob Settings", "TotalNumber of Mobs", 8, new ConfigDescription("Base number of mobs to spawn in total", new AcceptableValueRange<int>(1, 100)));
            timeBetweenMobSpawns = Config.Bind("Mob Settings", "Time between each Mob Spawn", 100f, new ConfigDescription("Time between each mob spawn where 0.1 = 1.5 hours", new AcceptableValueRange<float>(0, 800)));
            enableSpawnMobsAsScrapIsFound = Config.Bind("Mob Settings", "Toggle whether more mobs spawn as scrap is found.", false, "If true, an additional mob will spawn at 25%, 50%, and 75% of scrap found in level");
            //maxEnemyListLabel = Config.Bind("Mob Settings", "List of Max Enemy Counts", new Dictionary<string, int>(), "contains a list of every enemy and a way to modify their count"); doesn't work with dictionaries
            maxCentipedesLabel = Config.Bind("Mob Settings", "Max Centipedes", 0, new ConfigDescription("Max number of centipedes allowed", new AcceptableValueRange<int>(0, 100)));
            maxSandSpiderLabel = Config.Bind("Mob Settings", "Max Sandspiders", 0, new ConfigDescription("Max number of SandSpiders allowed", new AcceptableValueRange<int>(0, 100)));
            maxHoarderBugLabel = Config.Bind("Mob Settings", "Max Hoarderbugs", 0, new ConfigDescription("Max number of Hoarderbugs allowed", new AcceptableValueRange<int>(0, 100)));
            maxFlowermanLabel = Config.Bind("Mob Settings", "Max Flowermen", 0, new ConfigDescription("Max number of Flowermen allowed", new AcceptableValueRange<int>(0, 100)));
            maxCrawlerLabel = Config.Bind("Mob Settings", "Max Crawlers", 0, new ConfigDescription("Max number of Crawlers allowed", new AcceptableValueRange<int>(0, 100)));
            maxBlobLabel = Config.Bind("Mob Settings", "Max Blobs", 0, new ConfigDescription("Max number of Blobs allowed", new AcceptableValueRange<int>(0, 100)));
            maxSpringManLabel = Config.Bind("Mob Settings", "Max SpringMen", 0, new ConfigDescription("Max number of SpringMen allowed", new AcceptableValueRange<int>(0, 100)));
            maxNutcrackerLabel = Config.Bind("Mob Settings", "Max Nutcrackers", 0, new ConfigDescription("Max number of Nutcrackers allowed", new AcceptableValueRange<int>(0, 100)));
            maxPufferLabel = Config.Bind("Mob Settings", "Max Puffers", 0, new ConfigDescription("Max number of Puffers allowed", new AcceptableValueRange<int>(0, 100)));
            maxDressGirlLabel = Config.Bind("Mob Settings", "Max DressGirls", 0, new ConfigDescription("Max number of DressGirls allowed", new AcceptableValueRange<int>(0, 100)));
            maxJesterLabel = Config.Bind("Mob Settings", "Max Jesters", 0, new ConfigDescription("Max number of Jesters allowed", new AcceptableValueRange<int>(0, 100)));
            maxMaskedPlayerEnemyLabel = Config.Bind("Mob Settings", "Max MaskedPlayerEnemies", 0, new ConfigDescription("Max number of MaskedPlayerEnemies allowed", new AcceptableValueRange<int>(0, 100)));
            maxLassoManLabel = Config.Bind("Mob Settings", "Max LassoMen", 0, new ConfigDescription("Max number of LassoMen allowed", new AcceptableValueRange<int>(0, 100)));
        }


        [HarmonyPatch(typeof(RoundManager), nameof(RoundManager.LoadNewLevel))]
        [HarmonyPostfix]
        static void ModifyLevel(ref SelectableLevel newLevel)
        {

            currentRound = RoundManager.Instance;
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.ChangeLevel))]
        [HarmonyPostfix]
        static void ChangeLevel(ref SelectableLevel ___currentLevel, ref SelectableLevel[] ___levels)
        {
            for(int i = 0; i < ___levels.Length; i++)
            {
                mls.LogInfo(___levels[i].PlanetName);
            }
            
            ___currentLevel.Enemies = ___levels[8].Enemies;
            spawnedMonsterTotal = 0;
        }

        [HarmonyPatch(typeof(StartOfRound), nameof(StartOfRound.StartGame))]
        [HarmonyPostfix]
        static void modifiedStart()
        {
            spawnedMonsterTotal = 0;
            Instance.updateCFGVarsViaGui();
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
            if(!isHost)
            {
                return;
            }

            currentRound = RoundManager.Instance;
            //currentRound.timeScript.globalTimeSpeedMultiplier = 4f;
            //currentRound.currentLevel.maxEnemyPowerCount = 250; this was why mobs weren't limited lmao

            

            maxEnemyList["Centipede"] = maxCentipedesLabel.Value;
            maxEnemyList["SandSpider"] = maxSandSpiderLabel.Value;
            maxEnemyList["HoarderBug"] = maxHoarderBugLabel.Value;
            maxEnemyList["Flowerman"] = maxFlowermanLabel.Value;
            maxEnemyList["Crawler"] = maxCrawlerLabel.Value;
            maxEnemyList["Blob"] = maxBlobLabel.Value;
            maxEnemyList["SpringMan"] = maxSpringManLabel.Value;
            maxEnemyList["Puffer"] = maxPufferLabel.Value;
            maxEnemyList["Nutcracker"] = maxNutcrackerLabel.Value;
            maxEnemyList["DressGirl"] = maxDressGirlLabel.Value;
            maxEnemyList["Jester"] = maxJesterLabel.Value;
            maxEnemyList["MaskedPlayerEnemy"] = maxMaskedPlayerEnemyLabel.Value;
            maxEnemyList["LassoMan"] = maxLassoManLabel.Value;

            

            int monsterSum = 0;

            

            foreach(KeyValuePair<string, int> entry in maxEnemyList)
            {
                monsterSum += entry.Value;
            }

            

            
            
            int enemyListLength = currentRound.currentLevel.Enemies.Count;
           

            if ((spawnedMonsterTotal < monsterSum) && (currentRound.timeScript.currentDayTime > timeToSpawn) && (currentRound.allEnemyVents.Length > 0))
            {
                
                Vector3 pos = currentRound.allEnemyVents[ventIndex].floorNode.position;
                float y = currentRound.allEnemyVents[ventIndex].floorNode.eulerAngles.y;
                
                

                
                

                if(enemyListLength > 0)
                {
                    int random = UnityEngine.Random.Range(0, 13);
                    
                    string currEnemyName = currentRound.currentLevel.Enemies[random].enemyType.name;

                    // advance hour sets currentEnemySpawnIndex to 0 lmao. this loop then runs endlessly since the max mobs were reached but it was still run due to advance hour's shenanigans

                    while (currentRound.currentLevel.Enemies[random].enemyType.numberSpawned >= maxEnemyList[currEnemyName]) // stuck here
                    {
                        random = UnityEngine.Random.Range(0, 13);
                        currEnemyName = currentRound.currentLevel.Enemies[random].enemyType.name;
                    }
                   
                    
                    mls.LogInfo("Spawning " + random + " name: " + currEnemyName + " currentEnemyList: " + currentEnemyList[currEnemyName]); // never made it here
                    currentRound.SpawnEnemyOnServer(pos, y, random);
                    
                    currentRound.currentLevel.Enemies[random].enemyType.numberSpawned++;
                                       

                    currentRound.currentEnemySpawnIndex++;
                    spawnedMonsterTotal++;
                    ventIndex++;
                    ventIndex = ventIndex % currentRound.allEnemyVents.Length;

                    timeToSpawn = currentRound.timeScript.currentDayTime + timeBetweenMobSpawns.Value; // adds delay for mob spawning so that they don't spawn all at once

                    mls.LogInfo("currentEnemySpawnIndex: " + currentRound.currentEnemySpawnIndex);
                    mls.LogInfo("spawnedMonsterTotal: " + spawnedMonsterTotal);

                    if (enableSpawnMobsAsScrapIsFound.Value)
                    {
                        if ((currentRound.valueOfFoundScrapItems > (int)(0.25 * currentRound.totalScrapValueInLevel)) && !firstTier)
                        {
                            random = (ventIndex * (int)currentRound.timeScript.currentDayTime + (19 * (ventIndex + (int)currentRound.totalScrapValueInLevel) + 21)) % enemyListLength;
                            currentRound.SpawnEnemyOnServer(pos, y, random);
                            ventIndex++;
                            ventIndex = ventIndex % currentRound.allEnemyVents.Length;
                            firstTier = true;
                        }
                        if ((currentRound.valueOfFoundScrapItems > (int)(0.5 * currentRound.totalScrapValueInLevel)) && !secondTier)
                        {
                            random = (ventIndex * (int)currentRound.timeScript.currentDayTime + (19 * (ventIndex + (int)currentRound.totalScrapValueInLevel) + 21)) % enemyListLength;
                            currentRound.SpawnEnemyOnServer(pos, y, random);
                            ventIndex++;
                            ventIndex = ventIndex % currentRound.allEnemyVents.Length;
                            secondTier = true;
                        }
                        if ((currentRound.valueOfFoundScrapItems > (int)(0.75 * currentRound.totalScrapValueInLevel)) && !thirdTier)
                        {
                            random = (ventIndex * (int)currentRound.timeScript.currentDayTime + (19 * (ventIndex + (int)currentRound.totalScrapValueInLevel) + 21)) % enemyListLength;
                            currentRound.SpawnEnemyOnServer(pos, y, random);
                            ventIndex++;
                            ventIndex = ventIndex % currentRound.allEnemyVents.Length;
                            thirdTier = true;
                        }
                    }
                }
            }
        }
    }
}
