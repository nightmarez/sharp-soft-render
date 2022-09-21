using System.Drawing.Imaging;

namespace Renderer;

public sealed unsafe class RawBitmap : IDisposable, IRenderTarget
{
    public struct Pixel
    {
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
    }

    private readonly BitmapData _data;
    readonly byte* _pBase;
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
        {
            for (int i = 0; i < Width; ++i)
            {
                SetDepth(i, j, double.MinValue);
            }
        }
    }

    public void Dispose()
    {
        InnerBitmap.UnlockBits(_data);
        InnerBitmap = null;
    }

    public double GetDepth(int x, int y)
    {
        return _depth[x, y];
    }

    public void SetDepth(int x, int y, double z)
    {
        _depth[x, y] = z;
    }

    public int Width
    {
        get;
    }

    public int Height
    {
        get;
    }

    public Bitmap InnerBitmap
    {
        get;
        private set;
    }

    public void Clear(Color color)
    {
        for (int j = 0; j < Height; ++j)
        {
            for (int i = 0; i < Width; ++i)
            {
                this[i, j] = color;
                SetDepth(i, j, double.MinValue);
            }
        }
    }

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
