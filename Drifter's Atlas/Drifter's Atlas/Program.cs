using Newtonsoft.Json.Linq;
using Raylib_cs;
using System.Numerics;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.JavaScript;
using System.Text;

namespace Drifters_Atlas
{
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

            // Setup Variables
            bool fileError = false;
            int currentMenu = 0;
            string menuName = string.Empty;

            string saveData = "";
            float xPos = 0;
            float yPos = 0;
            float zoom = 0;
            bool underground = false;
            int imgBrightness = 0;
            bool brightnessIncreasing = true;
            int windowWidth = Raylib.GetScreenWidth();
            int windowHeight = Raylib.GetScreenHeight();


            // Load Saves
            string SafeReadFile(string path) {
                try {
                    return Encoding.UTF8.GetString(Convert.FromBase64String(File.ReadAllText(path))).Substring(54);
                } catch {
                    return string.Empty;
                }
            }

            string savePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HyperLightDrifter");
            string save0 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_0.sav");
            string save1 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_1.sav");
            string save2 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_2.sav");
            string save3 = SafeReadFile(savePath + "\\HyperLight_RecordOfTheDrifter_3.sav");

            // Load Images
            Image bgImg = Raylib.LoadImage("Resources/bg_Title.png");
            Texture2D menuBgTexture = Raylib.LoadTextureFromImage(bgImg);
            float menuBgScale = 0;
            Image menuBoxImg = Raylib.LoadImage("Resources/spr_MEN_Frame_Reg.png");
            Texture2D menuBoxTexture = Raylib.LoadTextureFromImage(menuBoxImg);
            float menuBoxScale = 0;
            int textX = 0;
            int textY = 0;

            Image sprDrifterImg = Raylib.LoadImage("Resources/spr_charstandside.png");
            Texture2D sprDrifterTex = Raylib.LoadTextureFromImage(sprDrifterImg);
            Image sprGhostImg = Raylib.LoadImage("Resources/spr_ghost_0.png");
            Texture2D sprGhostTex = Raylib.LoadTextureFromImage(sprGhostImg);

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
            Raylib.SetSoundVolume(loadSaveSfx, 0.5f);
            Sound acceptSound = Raylib.LoadSound("Resources/snd_MenuloadComplete.wav");

            #region Create controls
            // Main Menu Page
            Button loadButton = new Button();
            Button exitButton = new Button();
            Button helpButton = new Button();
            Button creditsButton = new Button();
            loadButton.text = "Load Save";
            loadButton.type = Button.ButtonType.Menu;
            loadButton.ID = 0;
            loadButton.onClick += mainMenuButtonClick;

            exitButton.text = "Exit Tool";
            exitButton.type = Button.ButtonType.Menu;
            exitButton.ID = 1;
            exitButton.onClick += mainMenuButtonClick;

            helpButton.text = "Information";
            helpButton.type = Button.ButtonType.Menu;
            helpButton.ID = 2;
            helpButton.onClick += mainMenuButtonClick;

            creditsButton.text = "Credits";
            creditsButton.type = Button.ButtonType.Menu;
            creditsButton.ID = 3;
            creditsButton.onClick += mainMenuButtonClick;

            // Load Save Page
            Button save0Button = new Button();
            save0Button.type = Button.ButtonType.SaveSelect;
            save0Button.text = "Empty";
            save0Button.ID = 0;
            save0Button.drifterImage = sprDrifterImg;
            save0Button.ghostImage = sprGhostImg;
            save0Button.onClick += loadSave;

            Button save1Button = new Button();
            save1Button.type = Button.ButtonType.SaveSelect;
            save1Button.text = "Empty";
            save1Button.ID = 1;
            save1Button.drifterImage = sprDrifterImg;
            save1Button.ghostImage = sprGhostImg;
            save1Button.onClick += loadSave;

            Button save2Button = new Button();
            save2Button.type = Button.ButtonType.SaveSelect;
            save2Button.text = "Empty";
            save2Button.ID = 2;
            save2Button.drifterImage = sprDrifterImg;
            save2Button.ghostImage = sprGhostImg;
            save2Button.onClick += loadSave;

            Button save3Button = new Button();
            save3Button.type = Button.ButtonType.SaveSelect;
            save3Button.text = "Empty";
            save3Button.ID = 3;
            save3Button.drifterImage = sprDrifterImg;
            save3Button.ghostImage = sprGhostImg;
            save3Button.onClick += loadSave;

            // Others
            Button backButton = new Button();
            backButton.text = "Back";
            backButton.type = Button.ButtonType.Menu;
            backButton.ID = 0;
            backButton.onClick += backButtonClick;
            backButton.border = true;
            #endregion

            // Run the program
            while (!Raylib.WindowShouldClose()) {
                Vector2 menuBoxPos = new Vector2(
                    Raylib.GetScreenWidth() / 2 - menuBoxTexture.Width * menuBoxScale / 2,
                    Raylib.GetScreenHeight() / 2 - menuBoxTexture.Height * menuBoxScale / 2
                );
                if(currentMenu != 6) {
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
                    float scaleX = (float)windowWidth / menuBgTexture.Width;
                    float scaleY = (float)windowHeight / menuBgTexture.Height;
                    menuBgScale = MathF.Min(scaleX, scaleY);

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
                        TimeSpan span = TimeSpan.FromMinutes((double)parseSave(0, "playT"));
                        string time;
                        if (span.Hours < 1) time = $"{span.Minutes} Minutes";
                        else time = $"{span.Hours} Hours {span.Minutes} Minutes";

                        string difficulty;
                        if ((double)parseSave(0, "noviceMode") != 0) difficulty = "Newcomer";
                        if ((double)parseSave(0, "checkHP") == 3) {
                            if ((double)parseSave(0, "cape") == 11) difficulty = "New Game +";
                            else difficulty = "New Game Alt";
                        } else difficulty = "Standard";

                        save0Button.text =
                        (string)parseSave(0, "gameName") + "\n" +
                        time + "\n" +
                        DateTime.FromOADate((double)parseSave(0, "dateTime")).ToString("MM/dd/yy") + " . " + difficulty;
                    }
                    save0Button.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 58, menuBoxTexture.Height * menuBoxScale / 4);
                    save0Button.localPosition = new Vector2(30, menuBoxTexture.Height * menuBoxScale / 4 * 0);
                    save0Button.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width * menuBoxScale, menuBoxTexture.Height * menuBoxScale));
                    save0Button.Update();

                    if ((string)parseSave(1, "gameName") != "invalid") {
                        TimeSpan span = TimeSpan.FromMinutes((double)parseSave(1, "playT"));
                        string time;
                        if(span.Hours < 1) time = $"{span.Minutes} Minutes";
                        else time = $"{span.Hours} Hours {span.Minutes} Minutes";
                        
                        string difficulty;
                        if ((double)parseSave(1, "noviceMode") != 0) difficulty = "Newcomer";
                        if ((double)parseSave(1, "checkHP") == 3) {
                            if ((double)parseSave(1, "cape") == 11) difficulty = "New Game +";
                            else difficulty = "New Game Alt"; 
                        } else difficulty = "Standard"; 

                        save1Button.text =
                        (string)parseSave(1, "gameName") + "\n" +
                        time + "\n" +
                        DateTime.FromOADate((double)parseSave(1, "dateTime")).ToString("MM/dd/yy") + " . " + difficulty;
                    }
                    save1Button.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 58, menuBoxTexture.Height * menuBoxScale / 4);
                    save1Button.localPosition = new Vector2(30, menuBoxTexture.Height * menuBoxScale / 4 * 1);
                    save1Button.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width * menuBoxScale, menuBoxTexture.Height * menuBoxScale));
                    save1Button.Update();

                    if ((string)parseSave(2, "gameName") != "invalid") {
                        TimeSpan span = TimeSpan.FromMinutes((double)parseSave(2, "playT"));
                        string time;
                        if (span.Hours < 1) time = $"{span.Minutes} Minutes";
                        else time = $"{span.Hours} Hours {span.Minutes} Minutes";

                        string difficulty;
                        if ((double)parseSave(2, "noviceMode") != 0) difficulty = "Newcomer";
                        if ((double)parseSave(2, "checkHP") == 3) {
                            if ((double)parseSave(2, "cape") == 11) difficulty = "New Game +";
                            else difficulty = "New Game Alt";
                        } else difficulty = "Standard";

                        save2Button.text =
                        (string)parseSave(2, "gameName") + "\n" +
                        time + "\n" +
                        DateTime.FromOADate((double)parseSave(2, "dateTime")).ToString("MM/dd/yy") + " . " + difficulty;
                    }
                    save2Button.rect = new Rectangle(0, 0, menuBoxTexture.Width * menuBoxScale - 58, menuBoxTexture.Height * menuBoxScale / 4);
                    save2Button.localPosition = new Vector2(30, menuBoxTexture.Height * menuBoxScale / 4 * 2);
                    save2Button.parentObject = new Rectangle(menuBoxPos, new Vector2(menuBoxTexture.Width * menuBoxScale, menuBoxTexture.Height * menuBoxScale));
                    save2Button.Update();

                    if ((string)parseSave(3, "gameName") != "invalid") {
                        TimeSpan span = TimeSpan.FromMinutes((double)parseSave(3, "playT"));
                        string time;
                        if (span.Hours < 1) time = $"{span.Minutes} Minutes";
                        else time = $"{span.Hours} Hours {span.Minutes} Minutes";

                        string difficulty;
                        if ((double)parseSave(3, "noviceMode") != 0) difficulty = "Newcomer";
                        if ((double)parseSave(3, "checkHP") == 3) {
                            if ((double)parseSave(3, "cape") == 11) difficulty = "New Game +";
                            else difficulty = "New Game Alt";
                        } else difficulty = "Standard";

                        save3Button.text =
                        (string)parseSave(3, "gameName") + "\n" +
                        time + "\n" +
                        DateTime.FromOADate((double)parseSave(3, "dateTime")).ToString("MM/dd/yy") + " . " + difficulty;
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

                // Managing drag N drop.
                if (Raylib.IsFileDropped()) {
                    var files = Raylib.LoadDroppedFiles();
                    string filePath = Marshal.PtrToStringAnsi((IntPtr)files.Paths[0]);
                    string fileData = File.ReadAllText(filePath);
                    saveData = fileData.Substring(80);
                }

                // Drawing things
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);
                // Draw the background
                if (currentMenu != 6) {
                    Raylib.DrawTextureEx(menuBgTexture, new Vector2(0, 0), 0, menuBgScale, new Color(Math.Min(220 - imgBrightness / 2, 255), Math.Min(220 - imgBrightness / 2, 255), Math.Min(220 - imgBrightness / 2, 255)));
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
                }
                if (currentMenu == 0) {
                    // Draw the controls for main menu
                    exitButton.Draw();
                    loadButton.Draw();
                    helpButton.Draw();
                    creditsButton.Draw();
                } else if (currentMenu == 1) {
                    save0Button.Draw();
                    save1Button.Draw();
                    save2Button.Draw();
                    save3Button.Draw();

                    Raylib.DrawLine((int)save1Button.rect.X, (int)save1Button.rect.Y, (int)save1Button.rect.X + (int)save1Button.rect.Width, (int)save1Button.rect.Y, new Color(255, 255, 255, 166));
                    Raylib.DrawLine((int)save2Button.rect.X, (int)save2Button.rect.Y, (int)save2Button.rect.X + (int)save2Button.rect.Width, (int)save2Button.rect.Y, new Color(255, 255, 255, 166));
                    Raylib.DrawLine((int)save3Button.rect.X, (int)save3Button.rect.Y, (int)save3Button.rect.X + (int)save3Button.rect.Width, (int)save3Button.rect.Y, new Color(255, 255, 255, 166));
                } else if (currentMenu == 2) {
                    Raylib.DrawTextEx(menuFont, "Test :3", new Vector2(menuBoxPos.X + 60, menuBoxPos.Y + 10), 25, -2, Color.White);
                }
                if (currentMenu != 6 && currentMenu != 0) {
                    // Draw back button
                    backButton.Draw();
                }
                    Raylib.DrawText($"save1Button\n{save1Button.rect.X},{save1Button.rect.Y}\n\nMouse\n{Raylib.GetMousePosition().X},{Raylib.GetMousePosition().Y}", 0, 0, 20, Color.White);
                // Temporary
                /*Raylib.DrawText(
                    Encoding.UTF8.GetString(Convert.FromBase64String(saveData)).Replace(",", "\n"),
                    32,
                    32,
                    20,
                    Color.White
                );*/


                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();

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
            void loadMenuButtonClick(int ID) {
                Raylib.SetSoundPitch(menuAction, 1f);
                Raylib.PlaySound(menuAction);
            }
            void backButtonClick(int ID) {
                Raylib.SetSoundPitch(menuAction, 0.5f);
                Raylib.PlaySound(menuAction);
                currentMenu = 0;
            }
            void loadSave(int ID) {
                if((string)parseSave(ID, "gameName") != "invalid") {
                    Raylib.PlaySound(loadSaveSfx);
                    Raylib.PlaySound(acceptSound);
                    currentMenu = 6;
                } else Raylib.PlaySound(errorSound);
            }

            // Helper functions
            // TODO: dont fucking do this. store the 4 saves in RAM, do NOT fucking load all of them SEVERAL TIMES, MULTIPLE TIMES PER FRAME
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
                } catch (Exception e) {
                    return "invalid";
                }
                return "halp"; // placeholder, dont you bitch about this
            }
        }
    }

    unsafe class Button{
        public enum ButtonType : ushort {
            None = 0, // Never to be used. You will fuck things up. Its just useful to define a variable to be none when initialized.
            Menu = 1, // Menu buttons
            SaveSelect = 2, // Selecting a save, this one has support for images and will need more than 1 label. Text content is not chooseable
            Map = 3 // Smaller square buttons for map functions.
        }
        public Rectangle rect;
        public string text;
        public bool hovering = false;
        public ButtonType type;
        public int ID;
        public Image? drifterImage;
        public Image? ghostImage;
        public event buttonClick onClick;
        public Vector2 localPosition;
        public Rectangle parentObject;
        public bool border = false;

        private Sound hoverSfx = Raylib.LoadSound("Resources/snd_MenuMove.wav");
        private Vector2 textPos = Vector2.Zero;
        private Vector2 namePos = Vector2.Zero;
        private Vector2 timePos = Vector2.Zero;
        private bool lastFrameHover = false;
        private Font menuFont = Raylib.LoadFont("Resources/Imagine.ttf");
        private string[] props;

        public Button() {
            Raylib.SetTextureFilter(menuFont.Texture, TextureFilter.Point);
        }

        public void Update() {
            rect.X = parentObject.X + localPosition.X;
            rect.Y = parentObject.Y + localPosition.Y;

            Rectangle mouseRect = new Rectangle(Raylib.GetMousePosition().X, Raylib.GetMousePosition().Y, 1, 1);
            hovering = Raylib.CheckCollisionRecs(mouseRect, rect);

            if (type == ButtonType.Menu) {
                textPos = new Vector2(
                    rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, text, 25, -2f).X) / 2,
                    rect.Y + (rect.Height - 25) / 2
                );
            }
            else if (type == ButtonType.SaveSelect) {
                props = text.Split('\n');
                namePos = new Vector2(
                    rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[0], 25, -2f).X) / 2,
                    rect.Y + 25
                );
                if (text != "Empty") {
                    timePos = new Vector2(
                        rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[1], 25, -2f).X) / 2,
                        rect.Y + 60
                    );
                    textPos = new Vector2(
                        rect.X + (rect.Width - Raylib.MeasureTextEx(menuFont, props[2], 25, -2f).X) / 2,
                        rect.Y + 90
                    );
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
                Raylib.DrawTextEx(menuFont, text, new Vector2(textPos.X, textPos.Y), 25, -2, Color.White);
            } else if (type == ButtonType.SaveSelect) {
                if (hovering) {
                    Raylib.DrawRectangle((int)rect.X, (int)rect.Y, (int)rect.Width, (int)rect.Height, new Color(211, 6, 128, 130));
                }
                if(text != "Empty") {
                    Raylib.DrawTextEx(menuFont, props[0], new Vector2(namePos.X, namePos.Y), 25, -2, Color.White);
                    Raylib.DrawTextEx(menuFont, props[1], new Vector2(timePos.X, timePos.Y), 25, -2, new Color(44, 197, 177));
                    Raylib.DrawTextEx(menuFont, props[2], new Vector2(textPos.X, textPos.Y), 25, -2, new Color(190, 190, 190));
                } else {
                    Raylib.DrawTextEx(menuFont, text, new Vector2(namePos.X, namePos.Y), 25, -2, new Color(252, 45, 193, 136));
                }
            }
        }
        public delegate void buttonClick(int ID);
    }
}
