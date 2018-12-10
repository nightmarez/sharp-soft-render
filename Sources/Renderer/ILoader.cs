namespace Renderer
{
    /// <summary>
    /// Интерфейс загрузчика 3D модели.
    /// </summary>
    public interface ILoader
    {
        /// <summary>
        /// Может ли данный загрузчик загрузить модель из указанного файла.
        /// </summary>
        bool CanLoad(string ext);

        /// <summary>
        /// Загружает 3D модель.
        /// </summary>
        VertexBuffer Load(string fileName);
    }
}
