namespace Engine3D.MathUtil;

public class Vector2
{
    public float X { get; set; }
    public float Y { get; set; }

    public float Length => MathF.Sqrt(X * X + Y * Y);

    public Vector2()
    {
    }

    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }

    public Vector2(Vector3 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public Vector2(Vector4 v)
    {
        X = v.X;
        Y = v.Y;
    }

    public static Vector2 operator +(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X + b.X, a.Y + b.Y);
    }

    public static Vector2 operator -(Vector2 a, Vector2 b)
    {
        return new Vector2(a.X - b.X, a.Y - b.Y);
    }

    public static Vector2 operator *(Vector2 v, float f)
    {
        return new Vector2(v.X * f, v.Y * f);
    }

    public static Vector2 operator /(Vector2 v, float f)
    {
        if (f == 0)
            throw new DivideByZeroException();
        
        return new Vector2(v.X / f, v.Y / f);
    }

    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }

    public static void Normalize(ref Vector2 v)
    {
        float length = v.Length;
        v.X /= length;
        v.Y /= length;
    }
}