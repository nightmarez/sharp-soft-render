namespace Renderer;

public sealed class Vertex
{
    public Vector Coords;
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

    public Vertex Mult(Matrix matrix)
    {
        return new Vertex(
            Coords * matrix,
            (Normal * matrix).Normalize());
    }

    public static Vertex operator* (Vertex vx, Matrix matrix)
    {
        return vx.Mult(matrix);
    }
}
