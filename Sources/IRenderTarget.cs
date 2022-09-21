namespace Renderer;

public interface IRenderTarget
{
    Color this[int x, int y] { get; set; }
    double GetDepth(int x, int y);
    void SetDepth(int x, int y, double z);
    int Width { get; }
    int Height { get; }
}
