using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Schema;
using UnityEngine;
using UnityEngine.Windows;

// credits to creator of LethalCompanyGameMaster. The following GUI code is based off of his implementation.

namespace MoreMonsters.GuiMenuComponent
{
    internal class GuiMenu : MonoBehaviour
    {
        private KeyboardShortcut toggleMenu;
        private bool b_menu;
        internal bool wasKeyDown;

        private int tabIndex = 0;
        private string[] tabNames = { "Mob Settings" };

        private int MENUWIDTH = 600;
        private int MENUHEIGHT = 800;
        private int MENUX;
        private int MENUY;
        private int CENTERX;

        private int ITEMWIDTH = 300;

        
        
        public int guiNMobs;
        public float guiTimeBetweenMobSpawns;
        public bool guiEnableSpawnMobsAsScrapIsFound;
        public int guiMaxCentipedes;
        public int guiMaxSandSpiders;
        //public string guiMaxSandSpidersString;
        public int guiMaxHoarderBugs;
        public int guiMaxFlowerMen;
        public int guiMaxCrawlers;
        public int guiMaxBlobs;
        public int guiMaxSpringMen;
        public int guiMaxPuffers;
        public int guiMaxNutcrackers;
        public int guiMaxDressGirls;
        public int guiMaxJesters;
        public int guiMaxMaskedPlayerEnemies;
        public int guiMaxLassoMen;

        //public bool guiScaleMonsterCountByPlayerCount;

        private GUIStyle menuStyle;
        private GUIStyle buttonStyle;
        private GUIStyle labelStyle;
        private GUIStyle toggleStyle;
        private GUIStyle hScrollStyle;
        private GUIStyle textStyle;

        public bool guiIsHost;

        private void Awake()
        {
            MoreMonstersBase.mls.LogInfo(" [+] GUI Loaded");
            toggleMenu = new KeyboardShortcut(KeyCode.Insert);
            b_menu = false; // I'll set it here just in case

            MENUX = Screen.width / 2;
            MENUY = Screen.height / 2;

            CENTERX = MENUX + ((MENUWIDTH / 2) - (ITEMWIDTH / 2));
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void initMenu()
        {
            if (menuStyle == null)
            {
                menuStyle = new GUIStyle(GUI.skin.box);
                buttonStyle = new GUIStyle(GUI.skin.button);
                labelStyle = new GUIStyle(GUI.skin.label);
                toggleStyle = new GUIStyle(GUI.skin.toggle);
                hScrollStyle = new GUIStyle(GUI.skin.horizontalSlider);
                textStyle = new GUIStyle();

                textStyle.fontSize = 18;
                textStyle.alignment = TextAnchor.MiddleCenter;
                textStyle.normal.textColor = Color.white;

                menuStyle.normal.textColor = Color.white;
                menuStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                menuStyle.fontSize = 18;
                menuStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                buttonStyle.normal.textColor = Color.white;
                buttonStyle.fontSize = 18;

                labelStyle.normal.textColor = Color.white;
                labelStyle.normal.background = MakeTex(2, 2, new Color(0.01f, 0.01f, 0.1f, .9f));
                labelStyle.fontSize = 18;
                labelStyle.alignment = TextAnchor.MiddleCenter;
                labelStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;

                toggleStyle.normal.textColor = Color.white;
                toggleStyle.fontSize = 18;
                toggleStyle.wordWrap = true;

                hScrollStyle.normal.textColor = Color.white;
                hScrollStyle.normal.background = MakeTex(2, 2, new Color(0.0f, 0.0f, 0.2f, .9f));
                hScrollStyle.normal.background.hideFlags = HideFlags.HideAndDontSave;
            }
        }
        public void OnDestroy()
        {
            MoreMonstersBase.mls.LogInfo("[-] The GUILoader was destroyed.");
        }
        public void Update()
        {

            
            if (toggleMenu.IsDown())
            {
                if (!wasKeyDown)
                {
                    wasKeyDown = true;
                }
            }
            if (toggleMenu.IsUp())
            {
                if (wasKeyDown)
                {
                    wasKeyDown = false;
                    b_menu = !b_menu;
                    if (b_menu)
                    {
                        Cursor.visible = true;
                        Cursor.lockState = CursorLockMode.Confined;
                    }
                    else
                    {
                        Cursor.visible = false;
                        //Cursor.lockState = CursorLockMode.Locked;
                    }
                }
            }


        } // end of update

        public void OnGUI()
        {
            
            if (!guiIsHost) 
            { 
                return; 
            }
            if (menuStyle == null) 
            { 
                initMenu(); 
            }

            if (b_menu)
            {
                
                

                tabIndex = GUI.Toolbar(new Rect(MENUX, MENUY - 30, MENUWIDTH, 30), tabIndex, tabNames, buttonStyle);

                switch (tabIndex)
                {
                    case 0:
                        

                        GUI.Label(new Rect(CENTERX, MENUY, ITEMWIDTH, 30), "Max Number of Centipedes: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY, ITEMWIDTH, 30), "" + guiMaxCentipedes, textStyle);
                        guiMaxCentipedes = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 30, ITEMWIDTH, 30), (int)guiMaxCentipedes, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 60, ITEMWIDTH, 30), "Max Number of SandSpiders: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 60, ITEMWIDTH, 30), "" + guiMaxSandSpiders, textStyle);
                        guiMaxSandSpiders = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 90, ITEMWIDTH, 30), (int)guiMaxSandSpiders, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 120, ITEMWIDTH, 30), "Max Number of HoarderBugs: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 120, ITEMWIDTH, 30), "" + guiMaxHoarderBugs, textStyle);
                        guiMaxHoarderBugs = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 150, ITEMWIDTH, 30), (int)guiMaxHoarderBugs, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 180, ITEMWIDTH, 30), "Max Number of Flowermen: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 180, ITEMWIDTH, 30), "" + guiMaxFlowerMen, textStyle);
                        guiMaxFlowerMen = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 210, ITEMWIDTH, 30), (int)guiMaxFlowerMen, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 240, ITEMWIDTH, 30), "Max Number of Crawlers: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 240, ITEMWIDTH, 30), "" + guiMaxCrawlers, textStyle);
                        guiMaxCrawlers = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 270, ITEMWIDTH, 30), (int)guiMaxCrawlers, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 300, ITEMWIDTH, 30), "Max Number of Blobs: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 300, ITEMWIDTH, 30), "" + guiMaxBlobs, textStyle);
                        guiMaxBlobs = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 330, ITEMWIDTH, 30), (int)guiMaxBlobs, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 360, ITEMWIDTH, 30), "Max Number of DressGirls: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 360, ITEMWIDTH, 30), "" + guiMaxDressGirls, textStyle);
                        guiMaxDressGirls = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 390, ITEMWIDTH, 30), (int)guiMaxDressGirls, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 420, ITEMWIDTH, 30), "Max Number of Puffers: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 420, ITEMWIDTH, 30), "" + guiMaxPuffers, textStyle);
                        guiMaxPuffers = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 450, ITEMWIDTH, 30), (int)guiMaxPuffers, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 480, ITEMWIDTH, 30), "Max Number of SpringMen: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 480, ITEMWIDTH, 30), "" + guiMaxSpringMen, textStyle);
                        guiMaxSpringMen = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 510, ITEMWIDTH, 30), (int)guiMaxSpringMen, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 540, ITEMWIDTH, 30), "Max Number of Nutcrackers: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 540, ITEMWIDTH, 30), "" + guiMaxNutcrackers, textStyle);
                        guiMaxNutcrackers = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 570, ITEMWIDTH, 30), (int)guiMaxNutcrackers, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 600, ITEMWIDTH, 30), "Max Number of Jesters: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 600, ITEMWIDTH, 30), "" + guiMaxJesters, textStyle);
                        guiMaxJesters = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 630, ITEMWIDTH, 30), (int)guiMaxJesters, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 660, ITEMWIDTH, 45), "Max Number of MaskedPlayerEnemies: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 660, ITEMWIDTH, 45), "" + guiMaxMaskedPlayerEnemies, textStyle);
                        guiMaxMaskedPlayerEnemies = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 700, ITEMWIDTH, 30), (int)guiMaxMaskedPlayerEnemies, (int)0, (int)50);

                        GUI.Label(new Rect(CENTERX, MENUY + 720, ITEMWIDTH, 30), "Max Number of LassoMen: ", labelStyle);
                        GUI.Label(new Rect(CENTERX + 165, MENUY + 720, ITEMWIDTH, 30), "" + guiMaxLassoMen, textStyle);
                        guiMaxLassoMen = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 750, ITEMWIDTH, 30), (int)guiMaxLassoMen, (int)0, (int)50);


                        // maybe use later guiMaxSandSpiders = Int32.Parse(GUI.TextField(new Rect(CENTERX, MENUY + 90, ITEMWIDTH, 30), guiMaxSandSpiders + ""));
                        

                        GUI.Label(new Rect(CENTERX, MENUY + 780, ITEMWIDTH, 40), "Time Between Mob Spawns", labelStyle);
                        GUI.Label(new Rect(CENTERX + 240, MENUY + 780, ITEMWIDTH, 40), "" + (guiTimeBetweenMobSpawns / 100f) + " hours in-game", textStyle);
                        guiTimeBetweenMobSpawns = (float)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 820, ITEMWIDTH, 30), (int)guiTimeBetweenMobSpawns, (int)0, (int)800);
                        
                        guiEnableSpawnMobsAsScrapIsFound = GUI.Toggle(new Rect(CENTERX, MENUY + 850, ITEMWIDTH, 45), guiEnableSpawnMobsAsScrapIsFound, "Spawn an extra mob after finding 25%, 50%, and 75% of total scrap.", toggleStyle);

                        
                        

                        break;
                }

            }

        }
    }
}
