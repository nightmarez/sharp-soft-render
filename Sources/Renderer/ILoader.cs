namespace Renderer
{
    public interface ILoader
    {
        bool CanLoad(string ext);
        VertexBuffer Load(string fileName);
    }
}
