using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;

namespace DinoBotTester
{
    class DinoBot
    {
        // Using External Win API Key Down and Key Up
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        //Key Hex codes
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;
        private const int WM_SPACE = 0x20;
        private const int WM_ARROWDOWN = 0x28;


        private int level = 0;
        private bool innerLevelState = false;
        private int whitePixels = 0;
        private int halfWhitePixels = 0;
        private int blackPixels = 0;
        private int halfBlackPixels = 0;
        private Bitmap screenCatch = new Bitmap(60, 60);
        private int ten_percent = 0;
        private int seven_percent = 0;
        private int five_percent = 0;
        private int tree_percent = 0;

        // Public Propertyies for BOT control 
        public Process Process { get; set; } // current chrome process
        public Graphics Graph { get; set; } // Graphical implementation of Bitmap catch Screen
        public int JumpDelay { get; set; } // Jump Delay
        public int CameraDistance { get; set; } // Distance Camera from left to right
        public bool DinoBotState { get; set; } // Dino bot State about work

        public DinoBot()
        {
            ten_percent = screenCatch.Width * screenCatch.Height * 10 / 100; //Get 10% of pixels 
            seven_percent = screenCatch.Width * screenCatch.Height * 7 / 100; //Get 7% of pixels 
            five_percent = screenCatch.Width * screenCatch.Height * 5 / 100; //Get 5% of pixels 
            tree_percent = screenCatch.Width * screenCatch.Height * 3 / 100; //Get 3% of pixels
            Process = Process.GetProcessesByName("chrome")[0];
            Graph = Graphics.FromImage(screenCatch as Image);
            CameraDistance = 750;
        }

        // Implements dino Logic and movment
        public void DinoLogic()
        {
            while (DinoBotState)
            {
                Graph.CopyFromScreen(CameraDistance, 220, 0, 0, screenCatch.Size);
                for (int x = 0; x < screenCatch.Width; x++)
                {
                    for (int y = 0; y < screenCatch.Height; y++)
                    {
                        Color pixel = screenCatch.GetPixel(y, x);
                        if (x >= 37)
                            if (pixel.R < 127 | pixel.G < 127 | pixel.B < 127)
                                ++halfBlackPixels;
                            else
                                ++halfWhitePixels;
                        if (pixel.R < 127 | pixel.G < 127 | pixel.B < 127)
                            ++blackPixels;
                        else
                            ++whitePixels;
                    }
                }
                if (whitePixels > blackPixels)
                {
                    DinoControl(blackPixels, halfBlackPixels);
                    level += innerLevelState & level < 120 ? 10 : 0;
                    innerLevelState = false;

                }
                else
                {
                    DinoControl(whitePixels, halfWhitePixels);
                    level += !innerLevelState & level < 120 ? 10 : 0;
                    innerLevelState = true;
                }
                blackPixels = 0;
                halfBlackPixels = 0;
                whitePixels = 0;
                halfWhitePixels = 0;
            }
        }

        //implements Dino movments
        public void DinoControl(int pixels, int halfPixels)
        {
            if (halfPixels <= tree_percent && (pixels - halfPixels) >= five_percent)
            {
                PostMessage(Process.MainWindowHandle, WM_KEYDOWN, WM_ARROWDOWN, 0);
                System.Threading.Thread.Sleep(300);
                PostMessage(Process.MainWindowHandle, WM_KEYUP, WM_ARROWDOWN, 0);
            }
            else if (pixels >= seven_percent)
            {
                JumpDelay = pixels >= ten_percent ? 240 - level : pixels >= seven_percent ? 220 - level : 0;
                PostMessage(Process.MainWindowHandle, WM_KEYDOWN, WM_SPACE, 0);
                PostMessage(Process.MainWindowHandle, WM_KEYUP, WM_SPACE, 0);
                System.Threading.Thread.Sleep(JumpDelay);
                PostMessage(Process.MainWindowHandle, WM_KEYDOWN, WM_ARROWDOWN, 0);
                PostMessage(Process.MainWindowHandle, WM_KEYUP, WM_ARROWDOWN, 0);
            }
        }

        //Starts dino bot work
        public void Start()
        {
            DinoBotState = true;
            DinoLogic();
        }
    }
}
