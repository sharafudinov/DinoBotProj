using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace DinoBotTester
{
    class DinoBot
    {
        // Использование API
        [DllImport("user32.dll")]
        static extern bool PostMessage(IntPtr hWnd, UInt32 Msg, int wParam, int lParam);

        //Использование 16-ричных кодов  
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

        // Задание свойств для управления ботом 
        public Process Process { get; set; } // текущий процесс chrome
        public Graphics Graph { get; set; } // Графическая реализация раастрового изображения экрана
        public int JumpDelay { get; set; } // Задание свойств для JumpDelay
        public int CameraDistance { get; set; } // Дистанционная камера слева направо
        public bool DinoBotState { get; set; } // Состояние работы дино бота 

        public DinoBot()
        {
            ten_percent = screenCatch.Width * screenCatch.Height * 10 / 100; //Получение 10% пикселей 
            seven_percent = screenCatch.Width * screenCatch.Height * 7 / 100; //Получение 7% пикселей 
            five_percent = screenCatch.Width * screenCatch.Height * 5 / 100; //Получение 5% пикселей 
            tree_percent = screenCatch.Width * screenCatch.Height * 3 / 100; //Получение 3% пикселей 
            Process = Process.GetProcessesByName("chrome")[0];
            Graph = Graphics.FromImage(screenCatch as Image);
            CameraDistance = 750;//750;
        }

        // Реализация дино логики движения
        public void DinoLogic()
        {
            while (DinoBotState)
            {
                Graph.CopyFromScreen(CameraDistance, 180, 0, 0, screenCatch.Size);
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

        //Движение динозавра относительно объектов
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
                JumpDelay = pixels >= ten_percent ? 277/*240 was updated to 200*/ - level : pixels >= seven_percent ? 257 - level : 0;
                PostMessage(Process.MainWindowHandle, WM_KEYDOWN, WM_SPACE, 0);
                PostMessage(Process.MainWindowHandle, WM_KEYUP, WM_SPACE, 0);
                System.Threading.Thread.Sleep(JumpDelay);
                PostMessage(Process.MainWindowHandle, WM_KEYDOWN, WM_ARROWDOWN, 0);
                PostMessage(Process.MainWindowHandle, WM_KEYUP, WM_ARROWDOWN, 0);
            }
        }

        //Состояние работы дино бота 
        public void Start()
        {
            DinoBotState = true;
            DinoLogic();
        }
    }
}
