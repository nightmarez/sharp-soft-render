using System.Drawing;

namespace Renderer
{
    /// <summary>
    /// Интерфейс объекта для рендеринга.
    /// </summary>
    public interface IRenderTarget
    {
        /// <summary>
        /// Закрашивает все пиксели заданным цветом.
        /// </summary>
        void Clear(Color color);

        /// <summary>
        /// Доступ к пикселю.
        /// </summary>
        Color this[int x, int y] { get; set; }

        /// <summary>
        /// Возвращает глубину текущего пикселя.
        /// </summary>
        double GetDepth(int x, int y);

        /// <summary>
        /// Устанавливает глубину текущего пикселя.
        /// </summary>
        void SetDepth(int x, int y, double z);

        /// <summary>
        /// Ширина области для рендеринга.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Высота области для рендеринга.
        /// </summary>
        int Height { get; }
    }
}
