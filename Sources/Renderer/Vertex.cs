namespace Renderer
{
    /// <summary>
    /// Вершина.
    /// </summary>
    public sealed class Vertex
    {
        /// <summary>
        /// Координаты.
        /// </summary>
        public Vector Coords;

        /// <summary>
        /// Вектор нормали.
        /// </summary>
        public Vector Normal;

        public Vertex(Vector coords, Vector normal)
        {
            Coords = coords;
            Normal = normal;
        }

        public Vertex(Vector coords)
        {
            Coords = coords;
        }

        /// <summary>
        /// Умножает вершину на матрицу.
        /// </summary>
        public Vertex Mult(Matrix matrix)
        {
            return new Vertex(
                Coords * matrix,
                (Normal * matrix).Normalize());
        }

        /// <summary>
        /// Умножает вершину на матрицу.
        /// </summary>
        public static Vertex operator* (Vertex vx, Matrix matrix)
        {
            return vx.Mult(matrix);
        }
    }
}
