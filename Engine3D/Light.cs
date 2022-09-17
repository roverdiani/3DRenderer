using Engine3D.MathUtil;
using SFML.Graphics;

namespace Engine3D;

public static class Light
{
    public static Vector3 Direction { get; } = new(0f, 0f, 1f);

    public static Color ApplyIntensity(Color color, float factor)
    {
        if (factor < 0)
            factor = 0;
        if (factor > 1)
            factor = 1;
        
        byte r = (byte)(color.R * factor);
        byte g = (byte)(color.G * factor);
        byte b = (byte)(color.B * factor);
        byte a = color.A;

        return new Color(r, g, b, a);
    }
}