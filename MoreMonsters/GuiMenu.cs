using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
            // oddly enough doesn't work here
            if (!guiIsHost) { return; }
            if (menuStyle == null) { initMenu(); }

            if (b_menu)
            {
                
                //GUI.Box(new Rect(MENUX, MENUY, MENUWIDTH, MENUHEIGHT), "MoreMonsters Menu", menuStyle);

                tabIndex = GUI.Toolbar(new Rect(MENUX, MENUY - 30, MENUWIDTH, 30), tabIndex, tabNames, buttonStyle);

                switch (tabIndex)
                {
                    case 0:
                        // direct vs coefficient mob scaling
                        // allow user to set nMobs directly or
                        
                        // if use playerScaling is toggled, then use a custom coefficient with a default of .6 * nPlayers to increase mob count
                        // maybe if use playerScaling is enabled, then guiNMobs is set to coefficient * nPlayers
                        // else it's set directly

                        GUI.Label(new Rect(CENTERX, MENUY, ITEMWIDTH, 30), "Number of Mobs", labelStyle);
                        GUI.Label(new Rect(CENTERX, MENUY + 30, ITEMWIDTH, 30), ""+guiNMobs, textStyle);
                        guiNMobs = (int)GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 60, ITEMWIDTH, 30), (int)guiNMobs, (int)1, (int)100);
                        //MoreMonstersBase.mls.LogInfo("[+] guiNMobs: " + guiNMobs);

                        // should maybe move the order of these around to be optimal. have the var assigned before the menu shows. Although it doesn't
                        // matter since a default value is set anyways

                        GUI.Label(new Rect(CENTERX, MENUY + 90, ITEMWIDTH, 40), "Time between Mob Spawns", labelStyle);
                        GUI.Label(new Rect(CENTERX, MENUY + 130, ITEMWIDTH, 30), "" + (guiTimeBetweenMobSpawns / 100f) + " hours in-game", textStyle);
                        guiTimeBetweenMobSpawns = GUI.HorizontalSlider(new Rect(CENTERX, MENUY + 160, ITEMWIDTH, 30), (int)guiTimeBetweenMobSpawns, (int)0, (int)800);
                        //MoreMonstersBase.mls.LogInfo("[+] guiTimeBetweenMobSpawns: " + guiTimeBetweenMobSpawns);
                        guiEnableSpawnMobsAsScrapIsFound = GUI.Toggle(new Rect(CENTERX, MENUY + 190, ITEMWIDTH, 45), guiEnableSpawnMobsAsScrapIsFound, "Spawn an extra mob after finding 25%, 50%, and 75% of total scrap.", toggleStyle);

                        //guiScaleMonsterCountByPlayerCount = GUI.Toggle(new Rect(CENTERX, MENUY + 220, ITEMWIDTH, 30), guiEnableSpawnMobsAsScrapIsFound, "Enable mob scaling based on player number", toggleStyle);
                        

                        break;
                }

            }

        }
    }
}
