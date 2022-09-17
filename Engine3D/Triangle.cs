using Engine3D.MathUtil;
using SFML.Graphics;

namespace Engine3D;

public class Triangle
{
    public Vector2[] Points { get; } = new Vector2[3];
    public float AverageDepth { get; set; }
    
    public Color Color { get; set; } = Color.Magenta;
}

public class Face
{
    public int A { get; init; }
    public int B { get; init; }
    public int C { get; init; }

    public Color Color { get; set; } = Color.Magenta;

    public Face()
    {
    }

    public Face(int a, int b, int c)
    {
        A = a;
        B = b;
        C = c;
    }
}