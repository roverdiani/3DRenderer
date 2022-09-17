namespace Engine3D.MathUtil;

public class Vector3
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    
    public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z);

    public Vector3()
    {
    }

    public Vector3(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public Vector3(Vector4 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
    }
    
    public static Vector3 operator +(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
    }

    public static Vector3 operator -(Vector3 a, Vector3 b)
    {
        return new Vector3(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
    }
    
    public static Vector3 operator *(Vector3 v, float f)
    {
        return new Vector3(v.X * f, v.Y * f, v.Z * f);
    }

    public static Vector3 operator /(Vector3 v, float f)
    {
        if (f == 0)
            throw new DivideByZeroException();
        
        return new Vector3(v.X / f, v.Y / f, v.Z / f);
    }

    public static Vector3 Cross(Vector3 a, Vector3 b)
    {
        return new Vector3(
            a.Y * b.Z - a.Z * b.Y,
            a.Z * b.X - a.X * b.Z,
            a.X * b.Y - a.Y * b.X
        );
    }
    
    public static float Dot(Vector3 a, Vector3 b)
    {
        return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
    }
    
    public static void Normalize(ref Vector3 v)
    {
        float length = v.Length;
        v.X /= length;
        v.Y /= length;
        v.Z /= length;
    }
}