namespace Engine3D.MathUtil;

public class Vector4
{
    public float X { get; set; }
    public float Y { get; set; }
    public float Z { get; set; }
    public float W { get; set; }
    
    public float Length => MathF.Sqrt(X * X + Y * Y + Z * Z + W * W);
    
    public Vector4()
    {
    }

    public Vector4(float x, float y, float z, float w)
    {
        X = x;
        Y = y;
        Z = z;
        W = w;
    }

    public Vector4(Vector3 v)
    {
        X = v.X;
        Y = v.Y;
        Z = v.Z;
        W = 1.0f;
    }
}