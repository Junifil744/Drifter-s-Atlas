using Newtonsoft.Json.Linq;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace Drifters_Atlas {
    class Program {
        [STAThread]
        public unsafe static void Main() {
            // Setup Window
            Raylib.InitWindow(800, 480, "Drifter's Atlas");
            Raylib.SetWindowSize(
                (int)(Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor()) * 0.7),
                (int)(Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()) * 0.7)
            );
            Raylib.SetTargetFPS(30);
            Raylib.SetWindowIcon(Raylib.LoadImage("Resources/icon.png"));
            Raylib.SetExitKey(0);

            // Setup Variables
            string v = "Indev: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString();
            // string v = "Alpha 0.1.9390";
            bool debug = false;
            int currentMenu = 0;
            string menuName = string.Empty;

            string saveData = string.Empty;
            Vector2 drawPos = new Vector2(0, 0);
            int fontSize = 25;
            if(Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()) < 1080) fontSize = 20; // shit fix. TODO: fix this better.
            float zoom = 1;
            float moveIncrement = 0;
            bool underground = false;
            int imgBrightness = 0;
            bool brightnessIncreasing = true;
            bool lastFrameUGSw = false;
            bool lastFrameInteracted = false;
            Vector2 bgPos = new Vector2(0, 0);
            int windowWidth = Raylib.GetScreenWidth();
            int windowHeight = Raylib.GetScreenHeight();
            Vector2 windowCenter = new Vector2(windowWidth / 2, windowHeight / 2);
            KeyboardKey lastKey = KeyboardKey.Null;

            // Load Saves
            string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HyperLightDrifter");
            string save0 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_0.sav");
            string save1 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_1.sav");
            string save2 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_2.sav");
            string save3 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_3.sav");
            int currentSave = 0;

            // Load Images
            Image bgImg = Raylib.LoadImage("Resources/bg_Title.png");
            Texture2D menuBgTexture = Raylib.LoadTextureFromImage(bgImg);
            float menuBgScale = 0;
            Texture2D menuBoxTexture = Raylib.LoadTexture("Resources/spr_MEN_Frame_Reg.png");
            float menuBoxScale = 0;
            int textX = 0;
            int textY = 0;

            // Map related Images loading
            Texture2D mapTex = Raylib.LoadTexture("Resources/spr_Map_0.png");
            Texture2D mapLabTex = Raylib.LoadTexture("Resources/spr_Map_Lab_0.png");
            Texture2D mapDrawTex = mapTex;
            Vector2 mapPos = new Vector2(-mapTex.Width / 2, -mapTex.Height / 2);

            Texture2D sprDrifterTex = Raylib.LoadTexture("Resources/spr_charstandside.png");
            Texture2D sprADrifterTex = Raylib.LoadTexture("Resources/spr_ADStandSide.png");
            Texture2D sprGhostTex = Raylib.LoadTexture("Resources/spr_ghost_0.png");
            Texture2D reticuleTex = Raylib.LoadTexture("Resources/spr_MapReticule_0.png");
            Color retTint = Color.White;

            #region Load Sprites List
            // Textures
            Texture2D mapDrifter = Raylib.LoadTexture("Resources/spr_MapDrifter_0.png");
            Texture2D mapADrifter = Raylib.LoadTexture("Resources/spr_MapDrifter_2.png");
            Image mapWarpZone = Raylib.LoadImage("Resources/spr_MapWarpPillar_0.png");
            Image mapMonolith = Raylib.LoadImage("Resources/monolith.png");
            Texture2D mapAbyssBase = Raylib.LoadTexture("Resources/spr_MapAbyssBase_0.png");
            Texture2D mapAbyssBaseActive = Raylib.LoadTexture("Resources/spr_MapAbyssBase_1.png");
            Texture2D mapAbyssPillar = Raylib.LoadTexture("Resources/spr_MapAbyssPillar_0.png");
            Texture2D mapAbyssPillarActive = Raylib.LoadTexture("Resources/spr_MapAbyssPillar_1.png");
            Texture2D mapAbyssModule = Raylib.LoadTexture("Resources/spr_MapAbyssModule_0.png");
            Texture2D mapAbyssModuleActive = Raylib.LoadTexture("Resources/spr_MapAbyssModule_1.png");
            Texture2D mapModule = Raylib.LoadTexture("Resources/spr_MapModuleMarker_1.png");
            Texture2D mapModuleActive = Raylib.LoadTexture("Resources/spr_MapModuleMarker_0.png");
            // Controls
            List<MapSprite> spriteList = new List<MapSprite>();
            // spriteList.Add(new MapSprite(mapDrifter, MapSprite.SpriteType.AbyssCenter, new Vector2(10, 0), mapADrifter)); // FOR A LATER UPDATE.
            spriteList.Add(new MapSprite(mapAbyssBaseActive, MapSprite.SpriteType.AbyssCenter, new Vector2(-13.5f, 33.5f), false, mapAbyssBase));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(3.5f, 31.5f), false, mapAbyssPillar, 0));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(-13.5f, 14.5f), false, mapAbyssPillar, 1));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(-30.5f, 31.5f), false, mapAbyssPillar, 2));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(-13.5f, 48.5f), false, mapAbyssPillar, 3));
            // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-6.5f, 34f), false, mapAbyssModule, 1)); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-8.5f, 36f), false, mapAbyssModule, 2)); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-10.5f, 34f), false, mapAbyssModule, 3)); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-8.5f, 32f), false, mapAbyssModule, 4)); // North
            // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-11.5f, 39f), false, mapAbyssModule, 5)); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 41f), false, mapAbyssModule, 6)); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-15.5f, 39f), false, mapAbyssModule, 7)); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 37f), false, mapAbyssModule, 8)); // North
            // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-16.5f, 34f), false, mapAbyssModule, 9)); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-18.5f, 36f), false, mapAbyssModule, 10)); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-20.5f, 34f), false, mapAbyssModule, 11)); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-18.5f, 32f), false, mapAbyssModule, 12)); // North
            // North
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-11.5f, 30f), false, mapAbyssModule, 13)); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 32f), false, mapAbyssModule, 14)); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-15.5f, 30f), false, mapAbyssModule, 15)); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 28f), false, mapAbyssModule, 16)); // North

            for (int i = 0; i < 21; i++) {
                spriteList[i].tag = "noUG";
            }

            // Warp Zones
            spriteList.Add(new MapSprite(mapWarpZone, MapSprite.SpriteType.Warp, new Vector2(-67.5f, 2), false, 0));
            spriteList.Add(new MapSprite(mapWarpZone, MapSprite.SpriteType.Warp, new Vector2(8.5f, -478), false, 1));
            spriteList.Add(new MapSprite(mapWarpZone, MapSprite.SpriteType.Warp, new Vector2(-432.5f, 93), false, 2));
            spriteList.Add(new MapSprite(mapWarpZone, MapSprite.SpriteType.Warp, new Vector2(-39.5f, 359), false, 3));
            spriteList.Add(new MapSprite(mapWarpZone, MapSprite.SpriteType.Warp, new Vector2(501.5f, 1), false, 4));

            // North Monoliths
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(27, -214.5f), false, 1));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(110, -283.5f), false, 2));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(19, -466.5f), true, 3));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(104, -437.5f), true, 4));
            // East Monoliths
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(450, 69.5f), false, 9));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(501, -54.5f), false, 10));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(679, -37.5f), false, 11));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(391, -135.5f), false, 12));
            // West Monoliths
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(-337, -25.5f), false, 13));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(-445, 181.5f), false, 14));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(-548, 35.5f), false, 15));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(-645, -109.5f), true, 16));
            // South Monoliths
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(-25, 302.5f), false, 5));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(97, 365.5f), true, 6));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(-73, 434.5f), true, 7));
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(-128, 420.5f), true, 8));

            // North Modules
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(50.5f, -241), true, mapModule, -1137428));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(45.5f, -343), false, mapModule, -1084059));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(18.5f, -409), false, mapModule, -767783));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(17.5f, -526), false, mapModule, -1047430));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-61.5f, -396), true, mapModule, -1895481));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-21.5f, -388), true, mapModule, -932471));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-30.5f, -468), true, mapModule, -902212));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(5.5f, -442), true, mapModule, -813235));
            // East Modules
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(423.5f, 70), true, mapModule, -255100));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(585.5f, 79), true, mapModule, -187905));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(560.5f, 170), true, mapModule, -167326));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(490.5f, -158), true, mapModule, -118694));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(424.5f, -85), false, mapModule, -18778));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(577.5f, 6), true, mapModule, -68841));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(591.5f, -100), false, mapModule, -53392));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(531.5f, -98), true, mapModule, -88709));
            // West Modules
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-439.5f, 51), true, mapModule, 266784));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-353.5f, 111), false, mapModule, 185267));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-300.5f, 81), true, mapModule, 101387));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-449.5f, 125), false, mapModule, 206139));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-534.5f, 172), true, mapModule, 335443));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-557.5f, 61), false, mapModule, 353953));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-580.5f, -27), true, mapModule, 403666));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-677.5f, -16), false, mapModule, 435082));
            // South Modules
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-21.5f, 488), true, mapModule, -416223));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-94.5f, 455), true, mapModule, -602007));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-105.5f, 424), true, mapModule, -596678));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-110.5f, 336), true, mapModule, -555279));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-157.5f, 471), true, mapModule, -676357));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(154.5f, 346), true, mapModule, -386457));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(109.5f, 355), true, mapModule, -398635));
            spriteList.Add(new MapSprite(mapModuleActive, MapSprite.SpriteType.Module, new Vector2(-13.5f, 499), true, mapModule, -417825));
            #endregion

            // Load font
            Font menuFont = Raylib.LoadFont("Resources/Imagine.ttf");
            Raylib.SetTextureFilter(menuFont.Texture, TextureFilter.Point);

            // Load Sounds
            Raylib.InitAudioDevice();
            Sound menuSound = Raylib.LoadSound("Resources/snd_TitleScreenStart.wav");
            Raylib.PlaySound(menuSound);
            Sound menuAction = Raylib.LoadSound("Resources/snd_MenuAction.wav");
            Sound errorSound = Raylib.LoadSound("Resources/snd_GenericError.wav");
            Sound loadSaveSfx = Raylib.LoadSound("Resources/snd_CollectModule_V3.wav");
            Sound menuOpenSfx = Raylib.LoadSound("Resources/snd_MenuOpen.wav");
            Sound menuCloseSfx = Raylib.LoadSound("Resources/snd_MenuClose.wav");
            Raylib.SetSoundVolume(loadSaveSfx, 0.5f);
            Sound acceptSound = Raylib.LoadSound("Resources/snd_MenuloadComplete.wav");

            #region Create controls
            // Main Menu Page
            Button loadButton = new Button("Load Save", 0, Button.ButtonType.Menu, null);
            Button exitButton = new Button("Exit Tool", 1, Button.ButtonType.Menu, null);
            Button helpButton = new Button("Information", 2, Button.ButtonType.Menu, null);
            Button creditsButton = new Button("Credits", 3, Button.ButtonType.Menu, null);
            loadButton.onClick += mainMenuButtonClick;
            exitButton.onClick += mainMenuButtonClick;
            helpButton.onClick += mainMenuButtonClick;
            creditsButton.onClick += mainMenuButtonClick;

            // Load Save Page
            Button save0Button = new Button("Empty", 0, Button.ButtonType.SaveSelect, sprDrifterTex);
            Button save1Button = new Button("Empty", 1, Button.ButtonType.SaveSelect, sprDrifterTex);
            Button save2Button = new Button("Empty", 2, Button.ButtonType.SaveSelect, sprDrifterTex);
            Button save3Button = new Button("Empty", 3, Button.ButtonType.SaveSelect, sprDrifterTex);
            save0Button.onClick += loadSave;
            save1Button.onClick += loadSave;
            save2Button.onClick += loadSave;
            save3Button.onClick += loadSave;

            // Others
            Button backButton = new Button("Back", 0, Button.ButtonType.Menu, null, true);
            backButton.onClick += backButtonClick;
            #endregion

            // Run the program
            while (!Raylib.WindowShouldClose()) {
                #region Updating Values
                Vector2 menuBoxPos = new Vector2(
                    Raylib.GetScreenWidth() / 2 - menuBoxTexture.Width * menuBoxScale / 2,
                    Raylib.GetScreenHeight() / 2 - menuBoxTexture.Height * menuBoxScale / 2
                );
                if (currentMenu != 6) {
                    float scaleX = (float)windowWidth / menuBgTexture.Width;
                    float scaleY = (float)windowHeight / menuBgTexture.Height;
                    if (currentMenu != 7) {
                        // Menu background image properties
                        if (brightnessIncreasing) {
                            if (imgBrightness < 60) { imgBrightness++; } else { brightnessIncreasing = false; }
                        } else {
                            if (imgBrightness > -60) { imgBrightness--; } else { brightnessIncreasing = true; }
                        }
                        Image tempImg = Raylib.ImageCopy(bgImg);
                        Raylib.ImageColorBrightness(&tempImg, Math.Clamp(imgBrightness / 2, -15, 15));
                        Raylib.UpdateTexture(menuBgTexture, tempImg.Data);
                        Raylib.UnloadImage(tempImg);
                        menuBgScale = MathF.Max(scaleX, scaleY);
                        bgPos = new Vector2(
                            (windowWidth - menuBgTexture.Width * menuBgScale) / 2,
                            (windowHeight - menuBgTexture.Height * menuBgScale) / 2
                        );
                    }
                    // Menu box properties
                    scaleX = (float)windowWidth / menuBoxTexture.Width / (float)1.4;
                    scaleY = (float)windowHeight / menuBoxTexture.Height / (float)1.4;
                    menuBoxScale = MathF.Min(scaleX, scaleY);
                    textX = windowWidth / 2 - (int)(menuBoxTexture.Width * menuBoxScale / 2) + (int)(12 * menuBoxScale);
                    textY = windowHeight / 2 - (int)(menuBoxTexture.Height * menuBoxScale / 2) - (int)(8 * menuBoxScale);

                }

                // Update menus
                // Main Menu
                if (currentMenu == 0) {
                    menuName = "Main Menu";

                    // Updating controls
                    loadButton.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 64, menuBoxTexture.Height * menuBoxScale / 11); // this last number is how many buttons fit in the box height-wise btw
                    loadButton.localPosition = new Vector2(33, menuBoxTexture.Height * menuBoxScale / 11 * 3);                                  // And the last number here is the index in the box
                    loadButton.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width, menuBoxTexture.Height));
                    loadButton.Update();

                    helpButton.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 64, menuBoxTexture.Height * menuBoxScale / 11);
                    helpButton.localPosition = new Vector2(33, menuBoxTexture.Height * menuBoxScale / 11 * 4);
                    helpButton.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width, menuBoxTexture.Height));
                    helpButton.Update();

                    creditsButton.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 64, menuBoxTexture.Height * menuBoxScale / 11);
                    creditsButton.localPosition = new Vector2(33, menuBoxTexture.Height * menuBoxScale / 11 * 5);
                    creditsButton.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width, menuBoxTexture.Height));
                    creditsButton.Update();

                    exitButton.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 64, menuBoxTexture.Height * menuBoxScale / 11);
                    exitButton.localPosition = new Vector2(33, menuBoxTexture.Height * menuBoxScale / 11 * 6);
                    exitButton.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width, menuBoxTexture.Height));
                    exitButton.Update();
                }
                // Load Save
                else if (currentMenu == 1) {
                    menuName = "Load Save";
                    backButton.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale / 4, menuBoxTexture.Height * menuBoxScale / 10);
                    backButton.localPosition = new Vector2(menuBoxTexture.Width * menuBoxScale - 27 - backButton.rect.Width, menuBoxTexture.Height * menuBoxScale + 1);
                    backButton.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width, menuBoxTexture.Height));
                    backButton.Update();

                    if ((string)parseSave(0, "gameName") != "invalid") {
                        getSaveInfo(ref save0Button);
                    }
                    save0Button.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 58, menuBoxTexture.Height * menuBoxScale / 4);
                    save0Button.localPosition = new Vector2(30, menuBoxTexture.Height * menuBoxScale / 4 * 0);
                    save0Button.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width * menuBoxScale, menuBoxTexture.Height * menuBoxScale));
                    save0Button.Update();

                    if ((string)parseSave(1, "gameName") != "invalid") {
                        getSaveInfo(ref save1Button);
                    }
                    save1Button.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 58, menuBoxTexture.Height * menuBoxScale / 4);
                    save1Button.localPosition = new Vector2(30, menuBoxTexture.Height * menuBoxScale / 4 * 1);
                    save1Button.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width * menuBoxScale, menuBoxTexture.Height * menuBoxScale));
                    save1Button.Update();

                    if ((string)parseSave(2, "gameName") != "invalid") {
                        getSaveInfo(ref save2Button);
                    }
                    save2Button.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 58, menuBoxTexture.Height * menuBoxScale / 4);
                    save2Button.localPosition = new Vector2(30, menuBoxTexture.Height * menuBoxScale / 4 * 2);
                    save2Button.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width * menuBoxScale, menuBoxTexture.Height * menuBoxScale));
                    save2Button.Update();

                    if ((string)parseSave(3, "gameName") != "invalid") {
                        getSaveInfo(ref save3Button);
                    }
                    save3Button.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 58, menuBoxTexture.Height * menuBoxScale / 4);
                    save3Button.localPosition = new Vector2(30, menuBoxTexture.Height * menuBoxScale / 4 * 3);
                    save3Button.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width * menuBoxScale, menuBoxTexture.Height * menuBoxScale));
                    save3Button.Update();
                }
                // Information
                else if (currentMenu == 2) {
                    menuName = "Information";
                    backButton.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale / 4, menuBoxTexture.Height * menuBoxScale / 10);
                    backButton.localPosition = new Vector2(menuBoxTexture.Width * menuBoxScale - 27 - backButton.rect.Width, menuBoxTexture.Height * menuBoxScale + 1);
                    backButton.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width, menuBoxTexture.Height));
                    backButton.Update();
                }
                // Credits
                else if (currentMenu == 3) {
                    menuName = "Credits";
                    backButton.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale / 4, menuBoxTexture.Height * menuBoxScale / 10);
                    backButton.localPosition = new Vector2(menuBoxTexture.Width * menuBoxScale - 27 - backButton.rect.Width, menuBoxTexture.Height * menuBoxScale + 1);
                    backButton.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width, menuBoxTexture.Height));
                    backButton.Update();
                }
                // The map.. Dear lord...
                else if (currentMenu == 6) {
                    backButton.ID = 0;
                    // Manage movement
                    bool up = Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up);
                    bool down = Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down);
                    bool left = Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left);
                    bool right = Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right);
                    bool zoomIn = Raylib.IsKeyDown(KeyboardKey.Q) || Raylib.IsKeyDown(KeyboardKey.Kp1);
                    bool zoomOut = Raylib.IsKeyDown(KeyboardKey.E) || Raylib.IsKeyDown(KeyboardKey.Kp2);

                    if(Raylib.IsKeyDown(KeyboardKey.Escape)) {
                        currentMenu = 7;
                        Raylib.PlaySound(menuOpenSfx);
                        Raylib.ShowCursor();
                    }

                    if (Raylib.IsMouseButtonDown(MouseButton.Left) && lastFrameInteracted == false) {
                        lastFrameInteracted = true;
                        retTint = new Color(252, 45, 193);
                        Raylib.PlaySound(errorSound); // Not fully implemented yet. Wait for Beta 1.0
                    } else if (Raylib.IsMouseButtonUp(MouseButton.Left)) {
                        lastFrameInteracted = false;
                        retTint = Color.White;
                    }
                    // debug = !Raylib.IsKeyDown(KeyboardKey.F);

                    if (up && mapPos.Y < 0) {
                        lastKey = KeyboardKey.W;
                        mapPos.Y += moveIncrement;
                    } else if (down && mapPos.Y > -mapTex.Height) {
                        lastKey = KeyboardKey.S;
                        mapPos.Y -= moveIncrement;
                    }
                    if (left && mapPos.X < 0) {
                        lastKey = KeyboardKey.A;
                        mapPos.X += moveIncrement;
                    } else if (right && mapPos.X > -mapTex.Width) {
                        lastKey = KeyboardKey.D;
                        mapPos.X -= moveIncrement;
                    }
                    if (zoomIn && zoom < 13f) {
                        zoom += 0.1f;
                    } else if (zoomOut && zoom > 0.6f) {
                        zoom -= 0.1f;
                    }
                    if (zoom < 0.6f) zoom = 0.6f;
                    if ((Raylib.IsKeyDown(KeyboardKey.R) || Raylib.IsKeyDown(KeyboardKey.Kp3)) && lastFrameUGSw == false) {
                        underground = !underground;
                        lastFrameUGSw = true;
                        Raylib.PlaySound(acceptSound);
                    }
                    if (Raylib.IsKeyUp(KeyboardKey.R) || Raylib.IsKeyDown(KeyboardKey.Kp3)) {
                        lastFrameUGSw = false;
                    }
                    if (!up && !down && !left && !right) {
                        if (lastKey == KeyboardKey.W && mapPos.Y < 0) mapPos.Y += moveIncrement;
                        if (lastKey == KeyboardKey.A && mapPos.X < 0) mapPos.X += moveIncrement;
                        if (lastKey == KeyboardKey.S && mapPos.Y > -mapTex.Height) mapPos.Y -= moveIncrement;
                        if (lastKey == KeyboardKey.D && mapPos.X > -mapTex.Width) mapPos.X -= moveIncrement;
                        if (moveIncrement > 0) moveIncrement -= 0.4f;
                        else if (moveIncrement < 0) moveIncrement = 0;
                    } else {
                        if (moveIncrement < 5) moveIncrement += 0.2f;
                    }
                    drawPos = new Vector2(
                        windowCenter.X + mapPos.X * zoom,
                        windowCenter.Y + mapPos.Y * zoom
                    );

                    if (underground) mapDrawTex = mapLabTex;
                    else mapDrawTex = mapTex;

                    // Update sprites
                    foreach (MapSprite sprite in spriteList) {
                        if (sprite.type == MapSprite.SpriteType.Warp) {
                            sprite.collected = ((string)parseSave(currentSave, "warp")).Contains(sprite.ID.ToString());
                        } else if (sprite.type == MapSprite.SpriteType.Monolith) {
                            sprite.collected = ((string)parseSave(currentSave, "tablet")).Contains(sprite.ID.ToString());
                        } else if (sprite.type == MapSprite.SpriteType.Module) {
                            for (byte i = 6; i <= 9; i++) {
                                sprite.collected = parseModule(currentSave, i, sprite.ID);
                                if (sprite.collected) break;
                            }
                            
                        } else if (sprite.type == MapSprite.SpriteType.AbyssPillar) {
                            sprite.collected = ((string)parseSave(currentSave, "well")).Contains(sprite.ID.ToString());
                        } else if (sprite.type == MapSprite.SpriteType.AbyssCenter) {
                            sprite.collected = parseCL(currentSave, 6).Count() >= 4 && parseCL(currentSave, 7).Count() >= 4 && parseCL(currentSave, 8).Count() >= 4 && parseCL(currentSave, 9).Count() >= 4;
                        } else if (sprite.type == MapSprite.SpriteType.AbyssModule) {
                            for (int section = 6; section <= 9; section++) {
                                byte count = (byte)Math.Min(parseCL(currentSave, (byte)section).Count(), 4);
                                for (int i = 0; i < count; i++) {
                                    int targetID = (section-6)*4 + i + 1;
                                    sprite.collected = targetID == sprite.ID;
                                    if (sprite.collected) break;
                                }
                                if (sprite.collected) break;
                            }
                        }
                        sprite.scale = zoom;
                        sprite.parentObject = new Rectangle(drawPos, new Vector2(mapTex.Width * zoom, mapTex.Height * zoom));
                        sprite.Update();
                    }
                }
                else if (currentMenu == 7) {
                    menuName = "Settings";
                    backButton.ID = 2;
                    backButton.Update();
                }



                // Managing drag N drop.
                if (Raylib.IsFileDropped() && currentMenu != 6) {
                    var files = Raylib.LoadDroppedFiles();
                    try {
                        saveData = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(Marshal.PtrToStringAnsi((IntPtr)files.Paths[0])))).Substring(54);
                        Raylib.UnloadDroppedFiles(files);
                        loadSave(10);
                    } catch {
                        Raylib.PlaySound(errorSound);
                        Raylib.UnloadDroppedFiles(files);

                    }
                }
                #endregion

                #region Drawing things
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);
                
                // Draw the background if the map isnt meant to be drawn
                if (currentMenu != 6) {
                    if(currentMenu != 7) Raylib.DrawTextureEx(menuBgTexture, bgPos, 0, menuBgScale, new Color(Math.Min(220 - imgBrightness / 2, 255), Math.Min(220 - imgBrightness / 2, 255), Math.Min(220 - imgBrightness / 2, 255)));
                }
                if (currentMenu == 0) {
                    drawMenuBox();
                    exitButton.Draw();
                    loadButton.Draw();
                    helpButton.Draw();
                    creditsButton.Draw();
                } else if (currentMenu == 1) {
                    drawMenuBox();
                    save0Button.Draw();
                    save1Button.Draw();
                    save2Button.Draw();
                    save3Button.Draw();

                    Raylib.DrawLine((int)save1Button.rect.X, (int)save1Button.rect.Y, (int)save1Button.rect.X + (int)save1Button.rect.Width, (int)save1Button.rect.Y, new Color(255, 255, 255, 166));
                    Raylib.DrawLine((int)save2Button.rect.X, (int)save2Button.rect.Y, (int)save2Button.rect.X + (int)save2Button.rect.Width, (int)save2Button.rect.Y, new Color(255, 255, 255, 166));
                    Raylib.DrawLine((int)save3Button.rect.X, (int)save3Button.rect.Y, (int)save3Button.rect.X + (int)save3Button.rect.Width, (int)save3Button.rect.Y, new Color(255, 255, 255, 166));
                } else if (currentMenu == 2) {
                    drawMenuBox();
                    Raylib.DrawTextEx(menuFont, "Controls\nWASD/Arrow Keys = Move\nQ/Numpad1 = Zoom In\nE/Numpad2 = Zoom out\nR/Numpad3 = Go Underground\n\n\nTHIS IS A VERY EARLY BUILD\nTHINGS WILL CHANGE\nI'm open to any feedback\nYou can contact me at my Discord or at\njunifil@middlemouse.click\nFor any bug reports!\n\n\nAlpha 0.1\nBuilt September 16 2025", new Vector2(menuBoxPos.X + 60, menuBoxPos.Y + 10), fontSize, -2, Color.White);
                } else if (currentMenu == 3) {
                    drawMenuBox();
                    Raylib.DrawTextEx(menuFont, "Credits!\n\nJunifil: Coding the whole thing and\n     putting everything together.\nTK: Moral support dawg.", new Vector2(menuBoxPos.X + 60, menuBoxPos.Y + 10), fontSize, -2, Color.White);
                } else if (currentMenu == 6) {
                    drawMap();
                    Raylib.DrawTextureEx(reticuleTex, new Vector2(Raylib.GetMousePosition().X - reticuleTex.Width * 1.5f, Raylib.GetMousePosition().Y - reticuleTex.Height * 1.5f), 0, 3f, retTint);
                } else if (currentMenu == 7) {
                    drawMap();
                    Raylib.DrawRectangle(0, 0, windowWidth, windowHeight, new Color(0, 0, 0, 120));
                    drawMenuBox();
                }
                if (currentMenu != 6 && currentMenu != 0) {
                    // Draw back button when necessary.
                    backButton.Draw();
                }
                Vector2 mouseMap = (Raylib.GetMousePosition() - windowCenter) / zoom - mapPos - new Vector2(mapTex.Width / 2, mapTex.Height / 2);
                if (debug) Raylib.DrawText($"currentMenu: {currentMenu}\n\nMap: {mapPos.X},{mapPos.Y}\nScale: {mapTex.Width * zoom},{mapTex.Height * zoom}\nIncrement: {moveIncrement}\nZoom: {zoom}\nScreenCenter: {windowCenter}\n\nMouse\n{mouseMap}\n\nSave\nMonoliths: {(string)parseSave(currentSave, "tablet")}\nCL: {(string)parseSave(currentSave, "cl")}", 0, 0, 20, Color.Pink);

                Raylib.EndDrawing();
                #endregion
            }

            Raylib.CloseWindow();

            // Menu Drawing Functions
            void drawMap() {
                Raylib.ClearBackground(Color.Black);
                Raylib.DrawTextureEx(mapDrawTex, drawPos, 0, zoom, Color.White);
                foreach (MapSprite sprite in spriteList) {
                    if (underground == sprite.underground || sprite.tag.Contains("noUG")) {
                        sprite.Draw();
                        if (debug) Raylib.DrawRectangleLinesEx(sprite.rect, 2 * (zoom / 10), Color.Purple);
                    }
                }
            }
            void drawMenuBox() {
                Raylib.DrawTextEx(menuFont, menuName, new Vector2(textX, textY), 20, -2, new Color(252, 45, 193, 166));
                Raylib.DrawTextureEx(
                    menuBoxTexture,
                    new Vector2(
                        windowWidth / 2 - menuBoxTexture.Width * menuBoxScale / 2,
                        windowHeight / 2 - menuBoxTexture.Height * menuBoxScale / 2
                    ),
                    0,
                    menuBoxScale,
                    Color.White
                );

                Raylib.DrawTextEx(menuFont, v, new Vector2(windowWidth - Raylib.MeasureTextEx(menuFont, v, fontSize, -2).X, menuBgTexture.Height * menuBgScale - fontSize), fontSize, -2, Color.White);
            }

            // Menu Button functions.
            void mainMenuButtonClick(int ID) {
                Raylib.SetSoundPitch(menuAction, 1f);
                Raylib.PlaySound(menuAction);
                if (ID == 0) {
                    currentMenu = 1;
                } else if (ID == 1) {
                    Thread.Sleep(50); // this line is just to make the funny sfx barely heard before closing, like in game!
                    Environment.Exit(0);
                } else if (ID == 2) {
                    currentMenu = 2;
                } else if (ID == 3) {
                    currentMenu = 3;
                }
            }
            void backButtonClick(int ID) {
                if (backButton.ID == 0) {
                    Raylib.SetSoundPitch(menuAction, 0.5f);
                    Raylib.PlaySound(menuAction);
                    currentMenu = 0;
                } else {
                    Raylib.PlaySound(menuCloseSfx);
                    lastFrameInteracted = true;
                    currentMenu = 6;
                }
            }
            void loadSave(int ID) {
                if (ID == 10) {
                    Raylib.PlaySound(acceptSound);
                    currentMenu = 6;
                    currentSave = 10;
                } else if ((string)parseSave(ID, "gameName") != "invalid") {
                    Raylib.PlaySound(loadSaveSfx);
                    Raylib.PlaySound(acceptSound);
                    currentMenu = 6;
                    currentSave = ID;
                    Raylib.HideCursor();
                } else Raylib.PlaySound(errorSound);
            }

            // Helper functions
            object parseSave(int ID, string parameter) {
                try {
                    dynamic save = null;
                    switch (ID) {
                        case 0:
                            save = JObject.Parse(save0);
                            break;
                        case 1:
                            save = JObject.Parse(save1);
                            break;
                        case 2:
                            save = JObject.Parse(save2);
                            break;
                        case 3:
                            save = JObject.Parse(save3);
                            break;
                    }


                    return save[parameter].Value;
                } catch {
                    return "invalid";
                }
            }
            bool parseModule(int saveID, byte section, int ID) {
                try {
                    string CL = (string)parseSave(saveID, "cl");
                    string[] sections = CL.Split(">");
                    foreach (string s in sections) {
                        if (s.Contains(section.ToString() + "=")) {
                            
                            return s.Contains(Convert.ToString(ID));
                        }
                    }
                    return false;
                } catch {
                    return false;
                }
            }
            string[] parseCL(int saveID, byte section) {
                try {
                    string CL = (string)parseSave(saveID, "cl");
                    string[] sections = CL.Split(">");
                    foreach (string s in sections) {
                        if (s.Contains(section.ToString() + "=")) {
                            return s.Split("&").Take(s.Split("&").Length - 1).ToArray();
                        }
                    }
                    return System.Array.Empty<string>();
                } catch {
                    return System.Array.Empty<string>();
                }
            }
            string SafeReadFile(string path) {
                try {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(path))).Substring(54);
                } catch {
                    return string.Empty;
                }
            }
            void getSaveInfo(ref Button button) {
                TimeSpan span = TimeSpan.FromMinutes((double)parseSave(button.ID, "playT"));
                string time;
                if (span.Hours < 1) time = $"{span.Minutes} Minutes";
                else time = $"{span.Hours} Hours {span.Minutes} Minutes";

                string difficulty;
                if ((double)parseSave(button.ID, "noviceMode") != 0) difficulty = "Newcomer";
                if ((double)parseSave(button.ID, "checkHP") == 3) {
                    difficulty = "New Game Alt";
                    button.image = sprADrifterTex;
                } 
                else if ((double)parseSave(button.ID, "cape") == 11) difficulty = "New Game +";
                else difficulty = "Standard";

                button.text =
                (string)parseSave(button.ID, "gameName") + "\n" +
                time + "\n" +
                DateTime.FromOADate((double)parseSave(button.ID, "dateTime")).ToString("MM/dd/yy") + " . " + difficulty;
            }
        }
    }

    unsafe class Button{
        public enum ButtonType : ushort {
            None = 0, // Never to be used. Wont load anything. Its just useful to define a variable to be none when initialized.
            Menu = 1, // Menu buttons
            SaveSelect = 2, // Selecting a save, this one has support for images and will need more than 1 label. Text content is not chooseable
            Map = 3 // Smaller square buttons for map functions.
        }
        public Rectangle rect;
        public string text;
        public bool hovering = false;
        public ButtonType type;
        public int ID;
        public Texture2D ghostImage;
        public Texture2D image;
        public event buttonClick onClick;
        public Vector2 localPosition;
        public Rectangle parentObject;
        public bool border;

        private Sound hoverSfx = Raylib.LoadSound("Resources/snd_MenuMove.wav");
        private Vector2 textPos = Vector2.Zero;
        private Vector2 namePos = Vector2.Zero;
        private Vector2 timePos = Vector2.Zero;
        private bool lastFrameHover = false;
        private Font menuFont = Raylib.LoadFont("Resources/Imagine.ttf");
        private string[] props;
        private Vector2 drifterPos = Vector2.Zero;
        private int fontSize = 25;

        public Button(string text, int ID, ButtonType type, Texture2D? image, bool border = false) {
            this.ID = ID;
            this.text = text;
            this.type = type;
            this.image = image ?? Raylib.LoadTextureFromImage(Raylib.GenImageColor(0,0,Color.Blank));
            this.border = border;

            Raylib.SetTextureFilter(menuFont.Texture, TextureFilter.Point);
            if (Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()) < 1080) fontSize = 20;
        }

        public void Update() {
            rect.X = parentObject.X + localPosition.X;
            rect.Y = parentObject.Y + localPosition.Y;

            Rectangle mouseRect = new Rectangle(Raylib.GetMousePosition().X, Raylib.GetMousePosition().Y, 1, 1);
            hovering = Raylib.CheckCollisionRecs(mouseRect, rect);

            if (type == ButtonType.Menu) {
                textPos = new Vector2(
                    rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, text, fontSize, -2f).X) / 2,
                    rect.Y + (rect.Height - fontSize) / 2
                );
            }
            else if (type == ButtonType.SaveSelect) {
                props = text.Split('\n');
                namePos = new Vector2(
                    rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[0], fontSize, -2f).X) / 2,
                    rect.Y + fontSize
                );
                if (text != "Empty") {
                    timePos = new Vector2(
                        rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[1], fontSize, -2f).X) / 2,
                        rect.Y + 60
                    );
                    textPos = new Vector2(
                        rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[2], fontSize, -2f).X) / 2,
                        rect.Y + 90
                    );
                    drifterPos = new Vector2(rect.X + rect.Width - image.Width * 3 - 30, rect.Y + rect.Height/2 - image.Height*3/2);
                }
            }
            if (hovering && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                onClick?.Invoke(ID);
            }
            if (hovering && !lastFrameHover) {
                Raylib.PlaySound(hoverSfx);
            }

            lastFrameHover = hovering;
        }

        public void Draw() {
            if (type == ButtonType.Menu) {
                if (hovering) {
                    Raylib.DrawRectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, new Color(211, 6, 128, 230));
                }
                if (border) Raylib.DrawRectangleLinesEx(rect, 2f, new Color(211, 6, 128, 110));
                Raylib.DrawTextEx(menuFont, text, new Vector2(textPos.X, textPos.Y), fontSize, -2, Color.White);
            } else if (type == ButtonType.SaveSelect) {
                if (hovering) {
                    Raylib.DrawRectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, new Color(211, 6, 128, 130));
                }
                if(text != "Empty") {
                    Raylib.DrawTextEx(menuFont, props[0], new Vector2(namePos.X, namePos.Y), fontSize, -2, Color.White);
                    Raylib.DrawTextEx(menuFont, props[1], new Vector2(timePos.X, timePos.Y), fontSize, -2, new Color(44, 197, 177));
                    Raylib.DrawTextEx(menuFont, props[2], new Vector2(textPos.X, textPos.Y), fontSize, -2, new Color(190, 190, 190));
                    Raylib.DrawTextureEx(image, drifterPos, 0, 3f, Color.White);
                } else {
                    Raylib.DrawTextEx(menuFont, text, new Vector2(namePos.X, namePos.Y), fontSize, -2, new Color(252, 45, 193, 136));
                }
            }
        }
        public delegate void buttonClick(int ID);
    }

    unsafe class MapSprite {
        public enum SpriteType : ushort {
            None = 0,
            Drifter = 1,
            Boss = 2,
            Gearbit = 3,
            Key = 4,
            Monolith = 5,
            Module = 6,
            ModuleDoor = 7,
            Gun = 8,
            ColorSet = 9,
            Warp = 10,
            Tower = 11,

            AbyssCenter = 12,
            AbyssModule = 13,
            AbyssPillar = 14
        }
        public Rectangle rect;
        public SpriteType type;
        public int ID = 0;
        public string tag = string.Empty;
        public Texture2D sprite;
        public Texture2D? altSprite;
        public event collectibleClick onClick;
        public Vector2 localPosition;
        public Rectangle parentObject;
        public float scale;
        public string tip;
        public bool underground = false;
        public bool collected = false;

        public MapSprite(Texture2D tex, SpriteType sType, Vector2 pos, bool underground, Texture2D? altTex = null, int? id = null) {
            ID = id ?? 0;
            type = sType;
            sprite = tex;
            altSprite = altTex;
            localPosition = pos;
            this.underground = underground;
        }
        public MapSprite(Image tex, SpriteType sType, Vector2 pos, bool underground, int? id = null) {
            ID = id ?? 0;
            this.underground = underground;
            type = sType;
            localPosition = pos;
            Image tmpImg = Raylib.ImageCopy(tex);
            sprite = Raylib.LoadTextureFromImage(tmpImg);
            Raylib.ImageFormat(ref tmpImg, PixelFormat.UncompressedGrayAlpha);
            altSprite = Raylib.LoadTextureFromImage(tmpImg);
            Raylib.UnloadImage(tmpImg);
        }

        public void Update() {
            rect.Width = sprite.Width * scale;
            rect.Height = sprite.Height * scale;

            Vector2 worldPos = new Vector2(
                parentObject.X + parentObject.Width * 0.5f + localPosition.X * scale,
                parentObject.Y + parentObject.Height * 0.5f + localPosition.Y * scale
            );

            // convert center -> top-left
            rect.X = worldPos.X - rect.Width * 0.5f;
            rect.Y = worldPos.Y - rect.Height * 0.5f;
        }

        public void Draw() {
            if(collected) Raylib.DrawTextureEx(sprite, new Vector2(rect.X, rect.Y), 0, scale, Color.White);
            else Raylib.DrawTextureEx(altSprite ?? sprite, new Vector2(rect.X, rect.Y), 0, scale, Color.White);
        }
        public delegate void collectibleClick();
    }
}
