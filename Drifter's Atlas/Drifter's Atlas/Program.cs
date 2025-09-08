using Raylib_cs;
using System.Buffers.Text;
using System.Runtime.InteropServices;
using System.Text;

namespace Drifters_Atlas
{
    class Program {
        [STAThread]
        public unsafe static void Main() {
            // Setup Window
            Raylib.SetConfigFlags(ConfigFlags.ResizableWindow);
            Raylib.InitWindow(800, 480, "Drifter's Atlas");
            Raylib.SetWindowSize(
                (int)(Raylib.GetMonitorWidth(Raylib.GetCurrentMonitor()) * 0.7),
                (int)(Raylib.GetMonitorHeight(Raylib.GetCurrentMonitor()) * 0.7)
            );
            Raylib.SetTargetFPS(30);

            // Setup Variables
            bool drawingMap = false;
            bool fileError = false;

            string saveData = "";
            float xPos = 0;
            float yPos = 0;
            float zoom = 0;
            bool underground = false;
            int imgBrightness = 0;
            bool brightnessIncreasing = true;
            float scale =0; // i hate this

            // Load Images
            Image bgImg = Raylib.LoadImage("Resources/bg_TitleBlank.png");
            Texture2D menuBgTexture = Raylib.LoadTextureFromImage(bgImg);


            // Load Sounds
            Raylib.InitAudioDevice();
            Sound menuSound = Raylib.LoadSound("Resources/snd_TitleScreenStart.wav");
            Raylib.PlaySound(menuSound);
            // Raylib.UnloadSound(menuSound);

            // Run the program(?) I never used raylib. I'm confuse.
            while (!Raylib.WindowShouldClose()) {
                // Preparing variables.

                int windowWidth = Raylib.GetScreenWidth();
                int windowHeight = Raylib.GetScreenHeight();

                // Menu background image properties
                if(!drawingMap) {
                    if (brightnessIncreasing) {
                        if (imgBrightness < 30) { imgBrightness++; }
                        else { brightnessIncreasing=false; }
                    } else {
                        if (imgBrightness > -30) { imgBrightness--; }
                        else { brightnessIncreasing=true; }
                    }
                    Image tempImg = Raylib.ImageCopy(bgImg);
                    Raylib.ImageColorBrightness(&tempImg, Math.Clamp(imgBrightness, -15, 5));
                    Raylib.UpdateTexture(menuBgTexture, tempImg.Data);
                    Raylib.UnloadImage(tempImg);
                    float scaleX = (float)windowWidth / menuBgTexture.Width;
                    float scaleY = (float)windowHeight / menuBgTexture.Height;
                    scale = MathF.Min(scaleX, scaleY);
                   
                }
                
                // Managing drag N drop.
                if (Raylib.IsFileDropped()) {
                    var retard = Raylib.LoadDroppedFiles();
                    string filePath = Marshal.PtrToStringAnsi((IntPtr)retard.Paths[0]);
                    string fileData = File.ReadAllText(filePath);
                    saveData = fileData.Substring(80);
                }

                // Drawing things
                Raylib.BeginDrawing();
                Raylib.ClearBackground(Color.White);
                if (!drawingMap) {
                    Raylib.DrawTextureEx(menuBgTexture, new System.Numerics.Vector2(0,0), 0, scale, new Color(Math.Min(220 - imgBrightness, 255), Math.Min(220 - imgBrightness, 255), Math.Min(220 - imgBrightness, 255)));
                }
                Raylib.DrawText(
                    Encoding.UTF8.GetString(Convert.FromBase64String(saveData)),
                    32,
                    32,
                    20,
                    Color.Black
                );
                


                Raylib.DrawText("Hello, world!", 12, 12, 20, Color.Black);

                Raylib.EndDrawing();
            }

            Raylib.CloseWindow();
        }
    }
}
