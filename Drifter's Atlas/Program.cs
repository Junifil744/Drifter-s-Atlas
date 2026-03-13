using Newtonsoft.Json.Linq;
using Raylib_cs;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;
using static Drifters_Atlas.Button;

namespace Drifters_Atlas {
    class Program {
        [STAThread]
        public static unsafe void Main() {
            // Setup Window
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(800, 480, "Drifter's Atlas");
            Raylib.SetWindowMinSize(490, 400);
            Raylib.SetWindowSize(
                (int)(Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor()) * 0.7),
                (int)(Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()) * 0.7)
            );
            Raylib.SetTargetFPS(30);
            Raylib.SetWindowIcon(Raylib.LoadImage("Resources/icon.png"));
            Raylib.SetExitKey(0);
            // TODO Clean this area
            // Setup Variables
            string v = "Indev: " + System.Reflection.Assembly.GetExecutingAssembly().GetName().Version ?? "Alpha 0.4".ToString();
            // string v = "Alpha 0.3.9558";
            bool debug = true;
            int currentMenu = 0;
            int windowWidth = Raylib.GetScreenWidth();
            int windowHeight = Raylib.GetScreenHeight();

            int fontSize = 25;
            
            // Map Values
            string saveData = string.Empty;
            Vector2 drawPos = new Vector2(0, 0);
            float zoom = 1;
            float moveIncrement = 0;
            bool underground = false;
            bool lastFrameUGSw = false;
            bool lastFrameInteracted = false;

            // Menu Values
            int imgBrightness = 0;
            bool brightnessIncreasing = true;
            Vector2 bgPos = new Vector2(0, 0);
            Vector2 windowCenter = new Vector2(windowWidth / 2f, windowHeight / 2f);
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
            
            // Map Menu Values
            Rectangle mapMenuRect = new Rectangle(
                new Vector2(10, windowHeight - windowHeight / 10f - 10),
                new Vector2(windowWidth - 20, windowHeight / 10f)
            );
            
            float menuBoxScale = MathF.Min(
                windowWidth / menuBoxTexture.Width / 1.2f,
                windowHeight / menuBoxTexture.Height / 1.2f
            );

            Rectangle menuRect = new Rectangle(
                windowWidth / 2f - menuBoxTexture.Width * menuBoxScale / 2,
                windowHeight / 2f - menuBoxTexture.Height * menuBoxScale / 2,
                menuBoxTexture.Width * menuBoxScale,
                menuBoxTexture.Height * menuBoxScale
            );

            // Map related Images loading
            Texture2D mapTex = Raylib.LoadTexture("Resources/spr_Map_0.png");
            Texture2D mapLabTex = Raylib.LoadTexture("Resources/spr_Map_Lab_0.png");
            Texture2D mapRoomOverlayTex = Raylib.LoadTexture("Resources/spr_Map_roomOverlay.png");
            Texture2D mapDrawTex = mapTex;
            Vector2 mapPos = new Vector2(-mapTex.Width / 2f, -mapTex.Height / 2f);

            Texture2D sprDrifterTex = Raylib.LoadTexture("Resources/spr_charstandside.png");
            Texture2D sprADrifterTex = Raylib.LoadTexture("Resources/spr_ADStandSide.png");
            // Texture2D sprGhostTex = Raylib.LoadTexture("Resources/spr_ghost_0.png");
            Texture2D reticuleTex = Raylib.LoadTexture("Resources/spr_MapReticule_0.png");
            Color retTint = Color.White;
            
            // Animated images.
            byte animLoopCounter = 0;
            Texture2D mapShine = Raylib.LoadTexture("Resources/spr_MapShine.png");
            Texture2D mapArrowUp = Raylib.LoadTexture("Resources/spr_MapArrows_0.png");
            Texture2D mapArrowRight = Raylib.LoadTexture("Resources/spr_MapArrows_1.png");
            Texture2D mapArrowDown = Raylib.LoadTexture("Resources/spr_MapArrows_2.png");
            Texture2D mapArrowLeft = Raylib.LoadTexture("Resources/spr_MapArrows_3.png");

            // Map Menu related images.
            Texture2D roomOverlayOn = Raylib.LoadTexture("Resources/mapControls/roomOverlayOn.png");
            Texture2D roomOverlayOff = Raylib.LoadTexture("Resources/mapControls/roomOverlayOff.png");
            Texture2D moduleMarkerUG = Raylib.LoadTexture("Resources/mapControls/moduleMarkerUg.png");
            Texture2D moduleAny = Raylib.LoadTexture("Resources/mapControls/moduleAny.png");
            Texture2D moduleVisOff = Raylib.LoadTexture("Resources/mapControls/moduleVisOff.png");
            Texture2D monolithMarkerUG = Raylib.LoadTexture("Resources/mapControls/monolithMarkerUg.png");
            Texture2D monolithAny = Raylib.LoadTexture("Resources/mapControls/monolithAny.png");
            Texture2D monolithVisOff = Raylib.LoadTexture("Resources/mapControls/monolithVisOff.png");
            Texture2D gunMarkerUG = Raylib.LoadTexture("Resources/mapControls/gunMarkerUg.png");
            Texture2D gunAny = Raylib.LoadTexture("Resources/mapControls/gunAny.png");
            Texture2D gunVisOff = Raylib.LoadTexture("Resources/mapControls/gunVisOff.png");
            Texture2D cloakMarkerUG = Raylib.LoadTexture("Resources/mapControls/cloakMarkerUg.png");
            Texture2D cloakAny = Raylib.LoadTexture("Resources/mapControls/cloakAny.png");
            Texture2D cloakVisOff = Raylib.LoadTexture("Resources/mapControls/cloakVisOff.png");
            
            Texture2D mapUgOff = Raylib.LoadTexture("Resources/mapControls/spr_MapLabIcon_0.png");
            Texture2D mapUgOn = Raylib.LoadTexture("Resources/mapControls/spr_MapLabIcon_1.png");

            #region Load Sprites List
            // Textures
            // Texture2D mapDrifter = Raylib.LoadTexture("Resources/spr_MapDrifter_0.png");
            // Texture2D mapADrifter = Raylib.LoadTexture("Resources/spr_MapDrifter_2.png");
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
            Texture2D mapBoss = Raylib.LoadTexture("Resources/spr_MapTowerLock_0.png");
            Texture2D mapBossActive = Raylib.LoadTexture("Resources/spr_MapTowerLock_1.png");
            Image mapPistol = Raylib.LoadImage("Resources/spr_Pistol.png");
            Image mapZeliska = Raylib.LoadImage("Resources/spr_Zeliska.png");
            Image mapRailgun = Raylib.LoadImage("Resources/spr_Railgun.png");
            Image mapBlunderbuss = Raylib.LoadImage("Resources/spr_Blunderbuss.png");
            Image mapDiamShotgun = Raylib.LoadImage("Resources/spr_DiamShotgun.png");
            Image mapImpactRailgun = Raylib.LoadImage("Resources/spr_ImpactRailgun.png");
            Image mapCloakBlue = Raylib.LoadImage("Resources/spr_CloakBlue.png");
            Image mapCloakBlack = Raylib.LoadImage("Resources/spr_CloakBlack.png");
            Image mapCloakFuchsia = Raylib.LoadImage("Resources/spr_CloakFuchsia.png");
            Image mapCloakGreen = Raylib.LoadImage("Resources/spr_CloakGreen.png");
            Image mapCloakOchre = Raylib.LoadImage("Resources/spr_CloakOchre.png"); // I hate the name of this color. It's ugly. - Ash
            Image mapCloakOrange = Raylib.LoadImage("Resources/spr_CloakOrange.png");
            Image mapCloakPink = Raylib.LoadImage("Resources/spr_CloakPink.png");
            Image mapCloakPurple = Raylib.LoadImage("Resources/spr_CloakPurple.png");
            Image mapCloakWhite = Raylib.LoadImage("Resources/spr_CloakWhite.png");
            Image mapCloakYellow = Raylib.LoadImage("Resources/spr_CloakFuchsia.png");
            
            // Controls
            List<MapSprite> spriteList = new List<MapSprite>();
            // spriteList.Add(new MapSprite(mapDrifter, MapSprite.SpriteType.AbyssCenter, new Vector2(10, 0), mapADrifter)); // FOR A LATER UPDATE.
            spriteList.Add(new MapSprite(mapAbyssBaseActive, MapSprite.SpriteType.AbyssCenter, new Vector2(-13.5f, 33.5f), false, mapAbyssBase, 0, "noUG"));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(3.5f, 31.5f), false, mapAbyssPillar, 0, "noUG"));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(-13.5f, 14.5f), false, mapAbyssPillar, 1, "noUG"));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(-30.5f, 31.5f), false, mapAbyssPillar, 2, "noUG"));
            spriteList.Add(new MapSprite(mapAbyssPillarActive, MapSprite.SpriteType.AbyssPillar, new Vector2(-13.5f, 48.5f), false, mapAbyssPillar, 3, "noUG"));

            // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-11.5f, 39f), false, mapAbyssModule, 1, "noUG")); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 41f), false, mapAbyssModule, 2, "noUG")); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-15.5f, 39f), false, mapAbyssModule, 3, "noUG")); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 37f), false, mapAbyssModule, 4, "noUG")); // North
            // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-6.5f, 34f), false, mapAbyssModule, 5, "noUG")); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-8.5f, 36f), false, mapAbyssModule, 6, "noUG")); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-10.5f, 34f), false, mapAbyssModule, 7, "noUG")); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-8.5f, 32f), false, mapAbyssModule, 8, "noUG")); // North
            // North
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-11.5f, 30f), false, mapAbyssModule, 9, "noUG")); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 32f), false, mapAbyssModule, 10, "noUG")); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-15.5f, 30f), false, mapAbyssModule, 11, "noUG")); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-13.5f, 28f), false, mapAbyssModule, 12, "noUG")); // North
            // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-16.5f, 34f), false, mapAbyssModule, 13, "noUG")); // East
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-18.5f, 36f), false, mapAbyssModule, 14, "noUG")); // South
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-20.5f, 34f), false, mapAbyssModule, 15, "noUG")); // West
            spriteList.Add(new MapSprite(mapAbyssModuleActive, MapSprite.SpriteType.AbyssModule, new Vector2(-18.5f, 32f), false, mapAbyssModule, 16, "noUG")); // North

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
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(450, 69.5f), false, 9)); // funny number
            spriteList.Add(new MapSprite(mapMonolith, MapSprite.SpriteType.Monolith, new Vector2(497, -57.5f), false, 10));
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

            // Boss
            spriteList.Add(new MapSprite(mapBoss, MapSprite.SpriteType.Boss, new Vector2(639, -149), true, mapBossActive, 0, "noUG"));
            spriteList.Add(new MapSprite(mapBoss, MapSprite.SpriteType.Boss, new Vector2(8, -531), false, mapBossActive, 1, "noUG"));
            spriteList.Add(new MapSprite(mapBoss, MapSprite.SpriteType.Boss, new Vector2(-688, -129), false, mapBossActive, 2, "noUG"));
            spriteList.Add(new MapSprite(mapBoss, MapSprite.SpriteType.Boss, new Vector2(33, 324), false, mapBossActive, 3, "noUG"));

            // Guns
            spriteList.Add(new MapSprite(mapPistol, MapSprite.SpriteType.Gun, new Vector2(-145.5f, -126), true, 1)); // Pistol
            spriteList.Add(new MapSprite(mapZeliska, MapSprite.SpriteType.Gun, new Vector2(-688, -87.5f), false, 2)); // Zeliska
            spriteList.Add(new MapSprite(mapRailgun, MapSprite.SpriteType.Gun, new Vector2(8.5f, -500), true, 21)); // Railgun
            spriteList.Add(new MapSprite(mapBlunderbuss, MapSprite.SpriteType.Gun, new Vector2(640, -133.5f), true, 43)); // Blunderbuss
            spriteList.Add(new MapSprite(mapDiamShotgun, MapSprite.SpriteType.Gun, new Vector2(388.5f, -185), true, 41)); // Diamond Shotgun
            spriteList.Add(new MapSprite(mapImpactRailgun, MapSprite.SpriteType.Gun, new Vector2(99.5f, 218.5f), true, 23)); // Impact Railgun
            
            // Gear
            spriteList.Add(new MapSprite(mapCloakBlue, MapSprite.SpriteType.ColorSet, new Vector2(11, -295), false, 1));
            spriteList.Add(new MapSprite(mapCloakFuchsia, MapSprite.SpriteType.ColorSet, new Vector2(-80, 450), true, 2));
            spriteList.Add(new MapSprite(mapCloakWhite, MapSprite.SpriteType.ColorSet, new Vector2(403, -50), true, 3));
            spriteList.Add(new MapSprite(mapCloakYellow, MapSprite.SpriteType.ColorSet, new Vector2(-577, 134), true, 4));
            spriteList.Add(new MapSprite(mapCloakOrange, MapSprite.SpriteType.ColorSet, new Vector2(575, 163), true, 5));
            spriteList.Add(new MapSprite(mapCloakGreen, MapSprite.SpriteType.ColorSet, new Vector2(-420, 117), true, 6)); // funny number
            spriteList.Add(new MapSprite(mapCloakPink, MapSprite.SpriteType.ColorSet, new Vector2(-7.5f, 282), false, 7));
            spriteList.Add(new MapSprite(mapCloakBlack, MapSprite.SpriteType.ColorSet, new Vector2(18.5f, 99), false, 8));
            spriteList.Add(new MapSprite(mapCloakOchre, MapSprite.SpriteType.ColorSet, new Vector2(-149, -209), false, 9));
            spriteList.Add(new MapSprite(mapCloakPurple, MapSprite.SpriteType.ColorSet, new Vector2(9.5f, -14), false, 10));
            
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
            // Load Save Page
            Menu loadMenu = new Menu("Load Save", Menu.MenuType.Menu, menuRect, 4, menuBoxTexture);
            Button save0Button = new Button("Empty", 0, ButtonType.SaveSelect, sprDrifterTex, loadMenu, loadSave);
            Button save1Button = new Button("Empty", 1, ButtonType.SaveSelect, sprDrifterTex, loadMenu, loadSave);
            Button save2Button = new Button("Empty", 2, ButtonType.SaveSelect, sprDrifterTex, loadMenu, loadSave);
            Button save3Button = new Button("Empty", 3, ButtonType.SaveSelect, sprDrifterTex, loadMenu, loadSave);

            // Help Page
            Menu helpMenu = new Menu("Information", Menu.MenuType.Menu, menuRect, 0, menuBoxTexture);

            // Credits Page
            Menu creditsMenu = new Menu("Credits", Menu.MenuType.Menu, menuRect, 0, menuBoxTexture);
            
            // Others
            Button backButton = new Button("Back", 99, ButtonType.Back, null, loadMenu);
            backButton.onClick += backButtonClick;

            // Main Menu Page
            // HACK: moving this thing downwards makes it not shit the bed because its spawned after mainMenuButtonClick is fully init - Ash
            Menu mainMenu = new Menu("Main Menu", Menu.MenuType.Menu, menuRect, 11, menuBoxTexture);
            Button loadButton = new Button("Load Save", 3, ButtonType.Menu, null, mainMenu, mainMenuButtonClick);
            Button helpButton = new Button("Information", 4, ButtonType.Menu, null, mainMenu, mainMenuButtonClick);
            Button creditsButton = new Button("Credits", 5, ButtonType.Menu, null, mainMenu, mainMenuButtonClick);
            Button exitButton = new Button("Exit Tool", 6, ButtonType.Menu, null, mainMenu, mainMenuButtonClick);
            
            // Settings Page(?) Pause Page?
            Menu pauseMenu = new Menu("Settings", Menu.MenuType.Menu, menuRect, 11, menuBoxTexture);
            Button menuButton = new Button("Exit to Menu", 4, ButtonType.Menu, null, pauseMenu, pauseMenuButtonClick);

            // Map Menu Buttons
            Menu mapMenu = new Menu("Map", Menu.MenuType.Map, mapMenuRect, 30);
            Button ugToggleButton = new Button(0, mapUgOff, mapUgOn, ref mapMenu, mapButtonClick, 0);
            Button roomOverlayButton = new Button(1, roomOverlayOff, roomOverlayOn, ref mapMenu, mapButtonClick, 0);
            Button moduleVisibilityButton = new Button(2, moduleVisOff, moduleMarkerUG, ref mapMenu, mapButtonClick, 1, "Modules are hidden", "Modules can only be seen in their respective layer (above/underground)", "Modules can be seen in both layers") { terImage = moduleAny };
            Button monolithVisibilityButton = new Button(3, monolithVisOff, monolithMarkerUG, ref mapMenu, mapButtonClick, 1, "Monoliths are hidden", "Monoliths can only be seen in their respective layer (above/underground)", "Monoliths can be seen in both layers") { terImage = monolithAny };
            Button gunVisibilityButton = new Button(4, gunVisOff, gunMarkerUG, ref mapMenu, mapButtonClick, 2, "Guns are hidden", "Guns can only be seen in their respective layer (above/underground)", "Guns can be seen in both layers") { terImage = gunAny };
            Button cloakVisibilityButton = new Button(5, cloakVisOff, cloakMarkerUG, ref mapMenu, mapButtonClick, 1, "Color Sets are hidden", "Color Sets can only be seen in their respective layer (above/underground)", "Color Sets can be seen in both layers") { terImage = cloakAny };

            #endregion

            // Run the program
            while (!Raylib.WindowShouldClose()) {
                #region Updating Values
                windowWidth = Raylib.GetScreenWidth();
                windowHeight = Raylib.GetScreenHeight();
                
                menuBoxScale = MathF.Min(
                    windowWidth / menuBoxTexture.Width / 1.2f,
                    windowHeight / menuBoxTexture.Height / 1.2f
                );

                menuRect = new Rectangle(
                    windowWidth / 2f - menuBoxTexture.Width * menuBoxScale / 2,
                    windowHeight / 2f - menuBoxTexture.Height * menuBoxScale / 2,
                    menuBoxTexture.Width * menuBoxScale,
                    menuBoxTexture.Height * menuBoxScale
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
                }
                // Update menus
                // Main Menu
                if (currentMenu == 0)
                {
                    mainMenu.Update(menuRect);
                }
                // Load Save
                else if (currentMenu == 1) {
                    backButton.rect = new Rectangle(
                        menuRect.X + menuRect.Width - (menuRect.Width / 4) - (10 * menuBoxScale),
                        menuRect.Y + menuRect.Height,
                        menuRect.Width / 4,
                        menuRect.Height / 10
                    );
                    if ((string)parseSave(0, "gameName") != "invalid") getSaveInfo(ref save0Button);
                    if ((string)parseSave(1, "gameName") != "invalid") getSaveInfo(ref save1Button);
                    if ((string)parseSave(2, "gameName") != "invalid") getSaveInfo(ref save2Button);
                    if ((string)parseSave(3, "gameName") != "invalid") getSaveInfo(ref save3Button);
                    backButton.Update();
                    loadMenu.Update(menuRect);
                }
                // Information
                else if (currentMenu == 2) {
                    backButton.rect = new Rectangle(
                        menuRect.X + menuRect.Width - (menuRect.Width / 4) - (10 * menuBoxScale),
                        menuRect.Y + menuRect.Height,
                        menuRect.Width / 4,
                        menuRect.Height / 10
                    );
                    backButton.Update();
                    helpMenu.Update(menuRect);
                }
                // Credits
                else if (currentMenu == 3) {
                    backButton.rect = new Rectangle(
                        menuRect.X + menuRect.Width - (menuRect.Width / 4) - (10 * menuBoxScale),
                        menuRect.Y + menuRect.Height,
                        menuRect.Width / 4,
                        menuRect.Height / 10
                    );
                    backButton.Update();
                    creditsMenu.Update(menuRect);
                }
                // The map.. Dear lord...
                else if (currentMenu == 6) {
                    // Manage movement
                    bool up = Raylib.IsKeyDown(KeyboardKey.W) || Raylib.IsKeyDown(KeyboardKey.Up);
                    bool down = Raylib.IsKeyDown(KeyboardKey.S) || Raylib.IsKeyDown(KeyboardKey.Down);
                    bool left = Raylib.IsKeyDown(KeyboardKey.A) || Raylib.IsKeyDown(KeyboardKey.Left);
                    bool right = Raylib.IsKeyDown(KeyboardKey.D) || Raylib.IsKeyDown(KeyboardKey.Right);
                    bool zoomIn = Raylib.IsKeyDown(KeyboardKey.E) || Raylib.IsKeyDown(KeyboardKey.Kp1);
                    bool zoomOut = Raylib.IsKeyDown(KeyboardKey.Q) || Raylib.IsKeyDown(KeyboardKey.Kp2);
                    debug = Raylib.IsKeyDown(KeyboardKey.F);

                    if (Raylib.IsKeyDown(KeyboardKey.Escape)) {
                        currentMenu = 7;
                        backButton.ID = 100;
                        Raylib.PlaySound(menuOpenSfx);
                        Raylib.ShowCursor();
                    }
                    if (Raylib.IsMouseButtonDown(MouseButton.Left) && lastFrameInteracted == false) {
                        lastFrameInteracted = true;
                        retTint = new Color(252, 45, 193);
                        bool valid = false;
                        foreach (Button button in mapMenu.buttonList)
                        {
                            if (!button.hovering) continue;
                            valid = true;
                            break;
                        }

                        Raylib.PlaySound(valid ? acceptSound : errorSound);
                    } else if (Raylib.IsMouseButtonUp(MouseButton.Left)) {
                        lastFrameInteracted = false;
                        retTint = Color.White;
                    }
                    
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
                        ugToggleButton.state = Convert.ToByte(underground); 
                        Raylib.PlaySound(menuOpenSfx);
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
                    windowCenter = new Vector2(windowWidth / 2f, windowHeight / 2f);
                    drawPos = new Vector2(
                        windowCenter.X + mapPos.X * zoom,
                        windowCenter.Y + mapPos.Y * zoom
                    );

                    mapDrawTex = underground ? mapLabTex : mapTex;

                    // Update sprites
                    animLoopCounter += 2;
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
                        } else if (sprite.type == MapSprite.SpriteType.AbyssPillar || sprite.type == MapSprite.SpriteType.Boss) { // Pillar && Boss collected
                            sprite.collected = ((string)parseSave(currentSave, "well")).Contains(sprite.ID.ToString());
                        } else if (sprite.type == MapSprite.SpriteType.AbyssCenter) {
                            sprite.collected = parseCL(currentSave, 6).Length >= 4 && parseCL(currentSave, 7).Length >= 4 && parseCL(currentSave, 8).Length >= 4 && parseCL(currentSave, 9).Length >= 4;
                        } else if (sprite.type == MapSprite.SpriteType.AbyssModule) {
                            for (int section = 6; section <= 9; section++) {
                                byte count = (byte)Math.Min(parseCL(currentSave, (byte)section).Length, 4);
                                for (int i = 0; i < count; i++) {
                                    int targetID = (section-6)*4 + i + 1;
                                    sprite.collected = targetID == sprite.ID;
                                    if (sprite.collected) break;
                                }
                                if (sprite.collected) break;
                            }
                        } else if (sprite.type == MapSprite.SpriteType.Gun) {
                            sprite.collected = ((string)parseSave(currentSave, "sc")).Contains(sprite.ID.ToString());
                        } else if (sprite.type == MapSprite.SpriteType.ColorSet) {
                            sprite.collected = ((string)parseSave(currentSave, "cCapes")).Contains(sprite.ID.ToString());
                        }

                        sprite.scale = zoom;
                        sprite.parentObject = new Rectangle(drawPos, new Vector2(mapTex.Width * zoom, mapTex.Height * zoom));
                        sprite.Update();
                    }

                    // Update Menu
                    // Map Menu Values
                    mapMenuRect = new Rectangle(
                        new Vector2(10, windowHeight - (windowHeight / 10f) - 10),
                        new Vector2(windowWidth - 20, windowHeight / 10f)
                    );
                    mapMenu.Update(mapMenuRect);
                }
                else if (currentMenu == 7) {
                    // menuName = "Settings";
                    backButton.rect = new Rectangle(
                        menuRect.X + menuRect.Width - (menuRect.Width / 4) - (10 * menuBoxScale),
                        menuRect.Y + menuRect.Height,
                        menuRect.Width / 4,
                        menuRect.Height / 10
                    );
                    foreach (Button button in pauseMenu.buttonList) button.Update();
                    pauseMenu.Update(menuRect);
                    backButton.Update();
                }

                // Managing drag N drop.
                if (Raylib.IsFileDropped() && currentMenu != 6) {
                    var files = Raylib.LoadDroppedFiles();
                    try {
                        saveData = Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(Marshal.PtrToStringAnsi((IntPtr)files.Paths[0]) ?? "Invalid").Substring(80)));
                        Raylib.UnloadDroppedFiles(files);
                        loadSave(save0Button, 10);
                    } catch {
                        Raylib.PlaySound(errorSound);
                        Raylib.UnloadDroppedFiles(files);

                    }
                }
                #endregion

                #region Drawing things
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);

                // Draw the background if the map isn't meant to be drawn
                if (currentMenu != 7 && currentMenu != 6) {
                    Raylib.DrawTextureEx(menuBgTexture, bgPos, 0, menuBgScale, new Color(Math.Min(220 - imgBrightness / 2, 255), Math.Min(220 - imgBrightness / 2, 255), Math.Min(220 - imgBrightness / 2, 255)));
                    Raylib.DrawTextEx(menuFont, v, new Vector2(windowWidth - Raylib.MeasureTextEx(menuFont, v, fontSize, -2).X, windowHeight - fontSize), fontSize, -2, Color.White);
                }
                if (currentMenu == 0) {
                    mainMenu.Draw();
                }
                else if (currentMenu == 1) {
                    loadMenu.Draw();
                    backButton.Draw();

                    Raylib.DrawLine((int)save1Button.rect.X, (int)save1Button.rect.Y, (int)save1Button.rect.X + (int)save1Button.rect.Width, (int)save1Button.rect.Y, new Color(255, 255, 255, 166));
                    Raylib.DrawLine((int)save2Button.rect.X, (int)save2Button.rect.Y, (int)save2Button.rect.X + (int)save2Button.rect.Width, (int)save2Button.rect.Y, new Color(255, 255, 255, 166));
                    Raylib.DrawLine((int)save3Button.rect.X, (int)save3Button.rect.Y, (int)save3Button.rect.X + (int)save3Button.rect.Width, (int)save3Button.rect.Y, new Color(255, 255, 255, 166));
                }
                else if (currentMenu == 2) {
                    helpMenu.Draw();
                    Raylib.DrawTextEx(menuFont, "Controls\nWASD/Arrow Keys = Move\nE/Numpad1 = Zoom In\nQ/Numpad2 = Zoom out\nR/Numpad3 = Go Underground\nESC = Show pause menu\n\nTHIS IS A VERY EARLY BUILD\nI'm open to any feedback\nYou can contact me at:\nMy Discord (Junifil)\nEmail (junifil@middlemouse.click)\n\nAlpha 0.4\nBuilt September 25 2025", new Vector2(menuRect.X + (20 * menuBoxScale), menuRect.Y + (10 * menuBoxScale)), fontSize * (menuBoxScale/2.5f), -2, Color.White);
                    // TODO: CHANGE THIS!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
                }
                else if (currentMenu == 3) {
                    creditsMenu.Draw();
                    Raylib.DrawTextEx(menuFont, "Credits!\n\nJunifil: Coding the whole thing and\n     putting everything together.\n\nu/Andy6000: Made the original\n     Pickup Map on Reddit\n\nMy love: For being there for me\n\nTK: Moral support dawg.", new Vector2(menuRect.X + (20*menuBoxScale), menuRect.Y + (10 * menuBoxScale)), fontSize * (menuBoxScale/2.5f), -2, Color.White);
                }
                else if (currentMenu == 6) {
                    // Draw the map itself
                    drawMap();

                    // Draw the map shine. // TODO: Move this to the updating area. Lots of calculations we shouldn't be doing here.
                    bool abyssOpen = true;
                    for (int i = 1; i < 5; i++) if (spriteList[i].collected == false) abyssOpen = false;
                    
                    if(abyssOpen) Raylib.DrawTextureEx(
                        mapShine, 
                        new Vector2(
                            spriteList[0].rect.X + spriteList[0].rect.Width/2 // Center it to the sprite
                            - mapShine.Width * animLoopCounter/10f * zoom / 2f, // Add offset for the rescaling.
                            
                            spriteList[0].rect.Y + spriteList[0].rect.Height/2 // Same here.
                            - mapShine.Height * animLoopCounter/10f * zoom / 2f
                        ),
                        0, 
                        animLoopCounter/10f * zoom, 
                        new Color(255, 255, 255, 255 - animLoopCounter)
                    );
                    
                    // Draw the map arrows
                    Raylib.DrawTextureEx(mapArrowUp,
                        new Vector2(windowWidth / 2f - mapArrowUp.Width * 2, // Center of screen for X
                                    20), // Top of screen + 20px for Y
                        0, 4, new Color(255, 255, 255, Math.Clamp(Math.Abs(128 - animLoopCounter) * 2, 63, 170))
                    );
                    Raylib.DrawTextureEx(mapArrowDown,
                        new Vector2(windowWidth / 2f - mapArrowUp.Width * 2, // Center of screen for X
                                    windowHeight - mapArrowUp.Height * 4 - 20), // Bottom of screen - 20px for Y
                        0, 4, new Color(255, 255, 255, Math.Clamp(Math.Abs(128 - animLoopCounter) * 2, 63, 170))
                    );
                    Raylib.DrawTextureEx(mapArrowLeft,
                        new Vector2(20, // 20 Left of the screen + 20px for X
                            windowHeight / 2f - mapArrowUp.Height * 2), // Center of the screen for Y
                        0, 4, new Color(255, 255, 255, Math.Clamp(Math.Abs(128 - animLoopCounter) * 2, 63, 170))
                    );
                    Raylib.DrawTextureEx(mapArrowRight,
                        new Vector2(windowWidth - mapArrowUp.Width * 4 - 20, // Right of the screen - 20px for X
                            windowHeight / 2f - mapArrowUp.Height * 2), // Center of the screen for Y
                        0, 4, new Color(255, 255, 255, Math.Clamp(Math.Abs(128 - animLoopCounter) * 2, 63, 170))
                    );
                    
                    // Draw the menu
                    mapMenu.Draw();
                    // Draw the little tooltip for the buttons
                    foreach (Button button in mapMenu.buttonList) if(button.hovering) button.DrawToolTip();
                    // Draw the mouse position
                    Raylib.DrawTextureEx(reticuleTex, new Vector2(Raylib.GetMousePosition().X - reticuleTex.Width * 1.5f, Raylib.GetMousePosition().Y - reticuleTex.Height * 1.5f), 0, 3f, retTint);
                }
                else if (currentMenu == 7) {
                    drawMap();
                    Raylib.DrawRectangle(0, 0, windowWidth, windowHeight, new Color(0, 0, 0, 120));
                    pauseMenu.Draw();
                    menuButton.Draw();
                }
                if (currentMenu != 6 && currentMenu != 0) {
                    // Draw back button when necessary.
                    backButton.Draw();
                }
                Vector2 mouseMap = (Raylib.GetMousePosition() - windowCenter) / zoom - mapPos - new Vector2(mapTex.Width / 2f, mapTex.Height / 2f);
                if (debug) Raylib.DrawText($"currentMenu: {currentMenu}\n\nMap: {mapPos.X},{mapPos.Y}\nIncrement: {moveIncrement}\nZoom: {zoom}\nScreenCenter: {windowCenter}\n\nMouse\n{mouseMap}\n\n{backButton.ID}\n\n{windowHeight},{windowWidth}", 0, 0, 20, Color.Pink);
                Raylib.EndDrawing();
                #endregion
            }

            Raylib.CloseWindow();

            // Menu Button functions.
            void mainMenuButtonClick(Button sender, int ID) {
                Raylib.SetSoundPitch(menuAction, 1f);
                Raylib.PlaySound(menuAction);
                if (ID == 3) {
                    currentMenu = 1;
                    backButton.parentMenu = loadMenu;
                } else if (ID == 4) {
                    currentMenu = 2;
                    backButton.parentMenu = helpMenu;
                } else if (ID == 5) {
                    currentMenu = 3;
                    backButton.parentMenu = creditsMenu;
                } else if (ID == 6) {
                    Thread.Sleep(50); // this line is just to make the funny sfx barely heard before closing, like in game!
                    Environment.Exit(0);
                }
            }
            void backButtonClick(Button sender, int ID) {
                if (backButton.ID != 100) {
                    Raylib.SetSoundPitch(menuAction, 0.5f);
                    Raylib.PlaySound(menuAction);
                    currentMenu = 0;
                } else {
                    Raylib.PlaySound(menuCloseSfx);
                    lastFrameInteracted = true;
                    currentMenu = 6;
                    backButton.ID = 99;
                }
            }
            void loadSave(Button sender, int ID) {
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
            void pauseMenuButtonClick(Button sender, int ID) {
                if(ID == 4) {
                    // Set menu back to main menu
                    currentMenu = 0;
                    // Reset values
                    drawPos = new Vector2(0, 0);
                    zoom = 1;
                    moveIncrement = 0;
                    underground = false;
                    lastFrameUGSw = false;
                    lastFrameInteracted = false;
                    // Play sfx
                    Raylib.PlaySound(acceptSound);
                    Raylib.PlaySound(menuSound);
                    // Reset Sprites
                    foreach (MapSprite sprite in spriteList) sprite.collected = false;
                    backButton.ID = 99;
                }
            }
            void mapButtonClick(Button sender, int ID)
            {
                if (ID == 0) underground = !underground;
                if(ID >= 2 && sender.state == 2) sender.state = 0;
                else if (ID < 2 && sender.state == 1) sender.state = 0;
                else sender.state++;
            }
            // Helper functions
            void drawMap() {
                Raylib.ClearBackground(Color.Black);
                Raylib.DrawTextureEx(mapDrawTex, drawPos, 0, zoom, Color.White);
                if(roomOverlayButton.state == 1 && !underground) Raylib.DrawTextureEx(mapRoomOverlayTex, drawPos, 0, zoom, Color.White);
                foreach (MapSprite sprite in spriteList) {
                    if (sprite.type == MapSprite.SpriteType.Module && moduleVisibilityButton.state == 0) continue;
                    if (sprite.type == MapSprite.SpriteType.Monolith && monolithVisibilityButton.state == 0) continue;
                    if (sprite.type == MapSprite.SpriteType.Gun && gunVisibilityButton.state == 0) continue;
                    if (sprite.type == MapSprite.SpriteType.ColorSet && cloakVisibilityButton.state == 0) continue;
                    if (underground == sprite.underground || sprite.tag.Contains("noUG")
                        || (moduleVisibilityButton.state == 2 && sprite.type == MapSprite.SpriteType.Module)
                        || (monolithVisibilityButton.state == 2 && sprite.type == MapSprite.SpriteType.Monolith)
                        || (gunVisibilityButton.state == 2 && sprite.type == MapSprite.SpriteType.Gun)
                        || (cloakVisibilityButton.state == 2 && sprite.type == MapSprite.SpriteType.ColorSet)
                    ) {
                        sprite.Draw();
                        if (debug) Raylib.DrawRectangleLinesEx(sprite.rect, 2 * (zoom / 10), Color.Purple);
                    }
                }
            }
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
                        case 10:
                            save = JObject.Parse(saveData);
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
                    return [];
                } catch {
                    return [];
                }
            }
            string SafeReadFile(string path) {
                try {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(path).Substring(80)));
                } catch(Exception err) {
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
        public enum ButtonType : byte {
            None = 0, // Never to be used. Won't load anything. It's just useful to define a variable to be none when initialized.
            Menu = 1, // Menu buttons
            SaveSelect = 2, // Selecting a save, this one has support for images and will need more than 1 label. Text content is not chooseable
            Map = 3, // Smaller square buttons for map functions.
            Back = 4 // The back button
        }
        public Rectangle rect;
        public string text;
        public bool hovering = false;
        public ButtonType type;
        public int ID;
        // public Texture2D ghostImage;
        public Texture2D image;
        public Texture2D altImage;
        public Texture2D terImage;
        public event buttonClick onClick;
        public Menu parentMenu;
        public byte state = 0;

        private Sound hoverSfx = Raylib.LoadSound("Resources/snd_MenuMove.wav");
        private Vector2 textPos = Vector2.Zero;
        private Vector2 namePos = Vector2.Zero;
        private Vector2 timePos = Vector2.Zero;
        private bool lastFrameHover = false;
        private Font menuFont = Raylib.LoadFont("Resources/Imagine.ttf");
        private string[] props;
        private Vector2 drifterPos = Vector2.Zero;
        private int fontSize = 25;
        private string enabledToolTip = string.Empty;
        private string disabledToolTip = string.Empty;
        public string tertiaryToolTip = string.Empty;
        
        
        public Button(string text, int ID, ButtonType type, Texture2D? image, Menu? parentMenu = null, buttonClick onClick = null, Texture2D? altImage = null) {
            this.ID = ID;
            this.text = text;
            this.type = type;
            this.image = image ?? Raylib.LoadTextureFromImage(Raylib.GenImageColor(1,1,Color.Blank));
            this.altImage = altImage ?? Raylib.LoadTextureFromImage(Raylib.GenImageColor(1, 1, Color.Blank));
            // this.border = border;
            this.onClick = onClick;

            Raylib.SetTextureFilter(menuFont.Texture, TextureFilter.Point);
            if (Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()) < 1080) fontSize = 20;
            if (parentMenu != null) {
                this.parentMenu = parentMenu;
                this.parentMenu.buttonList.Add(this);
            }
        }

        public Button(int ID, Texture2D disabledImage, Texture2D enabledImage, ref Menu parentMenu, buttonClick onClick, byte? state = null, string? disabledToolTip = null, string? enabledToolTip = null, string? tertiaryToolTip = null) {
            this.ID = ID;
            image = disabledImage;
            altImage = enabledImage;
            this.parentMenu = parentMenu;
            parentMenu.buttonList.Add(this);
            this.onClick = onClick;
            this.state = state ?? 0;
            this.enabledToolTip = enabledToolTip ?? string.Empty;
            this.disabledToolTip = disabledToolTip ?? string.Empty;
            this.tertiaryToolTip = tertiaryToolTip ?? string.Empty;
            type = ButtonType.Map;
            Raylib.SetTextureFilter(menuFont.Texture, TextureFilter.Point);
        }

        public void Update() {
            if(type != ButtonType.Map && type != ButtonType.Back) {
                rect = new Rectangle(
                    parentMenu.rect.X + 32 * (parentMenu.menuBoxScale/2.5f),
                    parentMenu.rect.Y + (parentMenu.rect.Height / parentMenu.indexMax * ID),
                    parentMenu.rect.Width - 64 * (parentMenu.menuBoxScale/2.5f),
                    parentMenu.rect.Height / parentMenu.indexMax - 1
                );
            } else if(type == ButtonType.Map) {
                rect = new Rectangle(
                    parentMenu.rect.X + ((parentMenu.rect.Height - 6) * ID) + 4,
                    parentMenu.rect.Y + 4,
                    parentMenu.rect.Height - 8,
                    parentMenu.rect.Height - 8
                );
                if (state == 2) text = tertiaryToolTip;
                else if (state == 1) text = enabledToolTip;
                else text = disabledToolTip;
                namePos = Raylib.MeasureTextEx(menuFont, text, fontSize / 2, -2f);
                textPos = new Vector2(Raylib.GetMousePosition().X + 30, Raylib.GetMousePosition().Y - namePos.Y/2);
            }

            Rectangle mouseRect = new Rectangle(Raylib.GetMousePosition().X, Raylib.GetMousePosition().Y, 1, 1);
            hovering = Raylib.CheckCollisionRecs(mouseRect, rect);

            if (type == ButtonType.SaveSelect) {
                props = text.Split('\n');
                namePos = new Vector2(
                    rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[0], fontSize * (parentMenu.menuBoxScale/2.5f), -2f).X) / 2,
                    rect.Y + fontSize * (parentMenu.menuBoxScale/2.5f)
                );
                if (text != "Empty") {
                    timePos = new Vector2(
                        rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[1], fontSize * (parentMenu.menuBoxScale/2.5f), -2f).X) / 2,
                        rect.Y + 60 * (parentMenu.menuBoxScale/2.5f)
                    );
                    textPos = new Vector2(
                        rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[2], fontSize * (parentMenu.menuBoxScale/2.5f), -2f).X) / 2,
                        rect.Y + 90 * (parentMenu.menuBoxScale/2.5f)
                    );
                    drifterPos = new Vector2(rect.X + rect.Width - image.Width * 3 - 30, rect.Y + rect.Height/2 - image.Height*3/2);
                }
            } else if (type != ButtonType.Map) {
                textPos = new Vector2(
                    rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, text, fontSize * (parentMenu.menuBoxScale/2.5f), -2f).X) / 2,
                    rect.Y + (rect.Height - fontSize * (parentMenu.menuBoxScale/2.5f)) / 2
                );
            }
            if (hovering && Raylib.IsMouseButtonPressed(MouseButton.Left)) {
                onClick?.Invoke(this, ID);
            }
            if (hovering && !lastFrameHover) {
                if(type == ButtonType.Back) {
                    Raylib.SetSoundPitch(hoverSfx, 1);
                    Raylib.PlaySound(hoverSfx);
                } else {
                    Raylib.SetSoundPitch(hoverSfx, (ID - parentMenu.buttonList[0].ID) / 30f + 1);
                    Raylib.PlaySound(hoverSfx);
                }
            }

            lastFrameHover = hovering;
        }

        public void Draw() {
            if (type == ButtonType.Menu || type == ButtonType.Back) {
                if (type == ButtonType.Back) {
                    Raylib.DrawRectangleLinesEx(rect, 2f, new Color(211, 6, 128, 110));
                }
                if (hovering) {
                    Raylib.DrawRectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, new Color(211, 6, 128, 230));
                }
                Raylib.DrawTextEx(menuFont, text, new Vector2(textPos.X, textPos.Y), fontSize * (parentMenu.menuBoxScale/2.5f), -2, Color.White);
            } else if (type == ButtonType.SaveSelect) {
                if (hovering) {
                    Raylib.DrawRectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, new Color(211, 6, 128, 130));
                }
                if(text != "Empty") {
                    if(props == null) throw new NullReferenceException("Props was undefined. Have you forgotten to update the button?"); // Yes. Yes I have. Several times. Thanks past me.
                    Raylib.DrawTextEx(menuFont, props[0], new Vector2(namePos.X, namePos.Y), fontSize * (parentMenu.menuBoxScale/2.5f), -2, Color.White);
                    Raylib.DrawTextEx(menuFont, props[1], new Vector2(timePos.X, timePos.Y), fontSize * (parentMenu.menuBoxScale/2.5f), -2, new Color(44, 197, 177));
                    Raylib.DrawTextEx(menuFont, props[2], new Vector2(textPos.X, textPos.Y), fontSize * (parentMenu.menuBoxScale/2.5f), -2, new Color(190, 190, 190));
                    Raylib.DrawTextureEx(image, drifterPos, 0, 3f, Color.White);
                } else {
                    Raylib.DrawTextEx(menuFont, text, new Vector2(namePos.X, namePos.Y), fontSize * (parentMenu.menuBoxScale/2.5f), -2, new Color(252, 45, 193, 136));
                }
            } else if (type == ButtonType.Map) {
                float scale = Math.Min(
                    rect.Width / image.Width,
                    rect.Height / image.Height
                );
                Color tint = new Color(200, 200, 200);
                if (!hovering) tint = Color.White;

                if (state == 1) Raylib.DrawTextureEx(altImage, new Vector2(rect.X, rect.Y), 0, scale, tint);
                else if (state == 2) Raylib.DrawTextureEx(terImage, new Vector2(rect.X, rect.Y), 0, scale, tint);
                else Raylib.DrawTextureEx(image, new Vector2(rect.X, rect.Y), 0, scale, tint);
            }
        }

        public void DrawToolTip() {
            Raylib.DrawRectangle((int)textPos.X, (int)textPos.Y, (int)namePos.X, (int)namePos.Y, new Color(32, 32, 32));
            Raylib.DrawRectangleLinesEx(new Rectangle(textPos, namePos), 2, new Color(22, 22, 22));
            Raylib.DrawTextEx(menuFont, text, textPos, fontSize / 2, -2f, Color.White);
        }
        public delegate void buttonClick(Button sender, int ID);
    }

    unsafe class MapSprite {
        public enum SpriteType : byte {
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

            AbyssCenter = 11,
            AbyssModule = 12,
            AbyssPillar = 13
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

        public MapSprite(Texture2D tex, SpriteType sType, Vector2 pos, bool underground, Texture2D? altTex = null, int? id = null, string? tag = null) {
            ID = id ?? 0;
            type = sType;
            sprite = tex;
            altSprite = altTex;
            localPosition = pos;
            this.tag = tag ?? string.Empty;
            this.underground = underground;
        }
        public MapSprite(Image tex, SpriteType sType, Vector2 pos, bool underground, int? id = null, string? tag = null) {
            ID = id ?? 0;
            this.underground = underground;
            type = sType;
            localPosition = pos;
            this.tag = tag ?? string.Empty;
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

    unsafe class Menu {
        public enum MenuType : byte { // solely used to define what the box is meant to look like.
            None = 0, 
            Menu = 1,
            Map = 2,
        }
        public Rectangle rect;
        public string name;
        public List<Button> buttonList = new List<Button>();
        public MenuType type;
        public ushort indexMax;

        public float menuBoxScale;
        private Font menuFont = Raylib.LoadFont("Resources/Imagine.ttf");
        private int windowWidth = Raylib.GetScreenWidth();
        private int windowHeight = Raylib.GetScreenHeight();
        private int textX, textY;

        // sheer simplicity
        public int count => buttonList?.Count ?? 0;

        private Texture2D image;

        public Menu(string name, MenuType type, Rectangle rect, ushort indexMax, Texture2D? image = null) {
            windowWidth = Raylib.GetScreenWidth();
            windowHeight = Raylib.GetScreenHeight();

            this.name = name;
            this.type = type;
            this.rect = rect;
            this.indexMax = indexMax;

            // Menu box properties
            Raylib.SetTextureFilter(menuFont.Texture, TextureFilter.Point);
            this.image = image ?? Raylib.LoadTextureFromImage(Raylib.GenImageColor(1,1,Color.Blank));

            menuBoxScale = MathF.Min(
                windowWidth / this.image.Width / 1.2f,
                windowHeight / this.image.Height / 1.2f
            );
        }

        public void Update(Rectangle newRect) {
            windowWidth = Raylib.GetScreenWidth();
            windowHeight = Raylib.GetScreenHeight();
            menuBoxScale = MathF.Min(
                windowWidth / image.Width / 1.2f,
                windowHeight / image.Height / 1.2f
            );
            rect = newRect;
            textX = (int)rect.X + (int)(12 * menuBoxScale);
            textY = (int)rect.Y - (int)(20 * menuBoxScale/2.5f);
            
            foreach (var button in buttonList) {
                button.Update();
            }
        }

        public void Draw() {
            if (type == MenuType.Menu) {
                Raylib.DrawTextEx(menuFont, name, new Vector2(textX, textY), 20 * menuBoxScale/2.5f, -2, new Color(252, 45, 193, 166));
                Raylib.DrawTextureEx(
                    image,
                    new Vector2(rect.X, rect.Y),
                    0,
                    menuBoxScale,
                    Color.White
                );
            } else if (type == MenuType.Map) {
                Raylib.DrawRectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, new Color(32, 32, 32, 150));
                Raylib.DrawRectangleLinesEx(rect, 2, new Color(22, 22, 22));
            }
            foreach (var button in buttonList) button.Draw();
        }
    }
}