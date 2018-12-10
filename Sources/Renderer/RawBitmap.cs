using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace Renderer
{
    /// <summary>
    /// Класс, предоставляющий быстрый доступ к пикселям изображения.
    /// </summary>
    public unsafe sealed class RawBitmap : IDisposable, IRenderTarget
    {
        /// <summary>
        /// Пиксель.
        /// </summary>
        public struct Pixel
        {
            // Цвета пикселя.
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;

            public Pixel(byte red, byte green, byte blue, byte alpha)
            {
                Red = red;
                Green = green;
                Blue = blue;
                Alpha = alpha;
            }

            public Pixel(byte red, byte green, byte blue)
            {
                Red = red;
                Green = green;
                Blue = blue;
                Alpha = 255;
            }
        }

        private readonly BitmapData _data;
        readonly byte* _pBase = null;
        private readonly double[,] _depth;

        public RawBitmap(Bitmap bmp)
        {
            InnerBitmap = bmp;
            Width = bmp.Width;
            Height = bmp.Height;

            _data = bmp.LockBits(
                new Rectangle(0, 0, bmp.Width, bmp.Height),
                ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);

            _pBase = (byte*)_data.Scan0;

            _depth = new double[Width, Height];
            for (int j = 0; j < Height; ++j)
                for (int i = 0; i < Width; ++i)
                    SetDepth(i, j, double.MinValue);
        }

        /// <summary>
        /// Освобождает ресурсы (разблокирует битмап).
        /// </summary>
        public void Dispose()
        {
            InnerBitmap.UnlockBits(_data);
            InnerBitmap = null;
        }

        /// <summary>
        /// Возвращает глубину текущего пикселя.
        /// </summary>
        public double GetDepth(int x, int y)
        {
            return _depth[x, y];
        }

        /// <summary>
        /// Устанавливает глубину текущего пикселя.
        /// </summary>
        public void SetDepth(int x, int y, double z)
        {
            _depth[x, y] = z;
        }

        /// <summary>
        /// Ширина изображения.
        /// </summary>
        public int Width
        {
            get;
            private set;
        }

        /// <summary>
        /// Высота изображения.
        /// </summary>
        public int Height
        {
            get;
            private set;
        }

        /// <summary>
        /// Ссылка на объект изображения.
        /// </summary>
        public Bitmap InnerBitmap
        {
            get;
            private set;
        }

        /// <summary>
        /// Заливает всё изображение заданным цветом.
        /// </summary>
        public void Clear(Color color)
        {
            for (int j = 0; j < Height; ++j)
                for (int i = 0; i < Width; ++i)
                {
                    this[i, j] = color;
                    SetDepth(i, j, double.MinValue);
                }
        }

        /// <summary>
        /// Доступ к пикселям изображения.
        /// </summary>
        public Color this[int x, int y]
        {
            get
            {
                var pixel = (Pixel*)(_pBase + y * _data.Stride + x * 4);

                return Color.FromArgb(
                    (*pixel).Alpha,
                    (*pixel).Red,
                    (*pixel).Green,
                    (*pixel).Blue);
            }

            set
            {
                var pixel = (Pixel*)(_pBase + y * _data.Stride + x * 4);
                *pixel = new Pixel(value.R, value.G, value.B, value.A);
            }
        }
    }
}