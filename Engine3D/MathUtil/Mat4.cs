namespace Engine3D.MathUtil;

public class Mat4
{
    public float[,] M { get; init; } = new float[4, 4];

    public static Vector4 operator *(Mat4 m, Vector4 v)
    {
        return new Vector4(
            m.M[0, 0] * v.X + m.M[0, 1] * v.Y + m.M[0, 2] * v.Z + m.M[0, 3] * v.W,
            m.M[1, 0] * v.X + m.M[1, 1] * v.Y + m.M[1, 2] * v.Z + m.M[1, 3] * v.W,
            m.M[2, 0] * v.X + m.M[2, 1] * v.Y + m.M[2, 2] * v.Z + m.M[2, 3] * v.W,
            m.M[3, 0] * v.X + m.M[3, 1] * v.Y + m.M[3, 2] * v.Z + m.M[3, 3] * v.W
        );
    }
    
    public static Mat4 operator *(Mat4 a, Mat4 b)
    {
        Mat4 m = new Mat4();
        for (int i = 0; i < 4; i++)
        for (int j = 0; j < 4; j++)
            m.M[i, j] = a.M[i, 0] * b.M[0, j] + a.M[i, 1] * b.M[1, j] + a.M[i, 2] * b.M[2, j] + a.M[i, 3] * b.M[3, j];

        return m;
    }

    public static Mat4 Identity()
    {
        return new Mat4
        {
            M = new float[,]
            {
                { 1, 0, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 0, 1 }
            }
        };
    }

    public static Mat4 Scale(float sX, float sY, float sZ)
    {
        Mat4 m = Identity();
        m.M[0, 0] = sX;
        m.M[1, 1] = sY;
        m.M[2, 2] = sZ;

        return m;
    }
    
    public static Mat4 Scale(Vector3 v)
    {
        return Scale(v.X, v.Y, v.Z);
    }

    public static Mat4 Translate(float tX, float tY, float tZ)
    {
        Mat4 m = Identity();
        m.M[0, 3] = tX;
        m.M[1, 3] = tY;
        m.M[2, 3] = tZ;

        return m;
    }
    
    public static Mat4 Translate(Vector3 v)
    {
        return Translate(v.X, v.Y, v.Z);
    }

    public static Mat4 RotateX(float angle)
    {
        float c = MathF.Cos(angle);
        float s = MathF.Sin(angle);

        Mat4 m = Identity();
        m.M[1, 1] = c;
        m.M[1, 2] = -s;
        m.M[2, 1] = s;
        m.M[2, 2] = c;

        return m;
    }
    
    public static Mat4 RotateY(float angle)
    {
        float c = MathF.Cos(angle);
        float s = MathF.Sin(angle);

        Mat4 m = Identity();
        m.M[0, 0] = c;
        m.M[0, 2] = s;
        m.M[2, 0] = -s;
        m.M[2, 2] = c;

        return m;
    }
    
    public static Mat4 RotateZ(float angle)
    {
        float c = MathF.Cos(angle);
        float s = MathF.Sin(angle);

        Mat4 m = Identity();
        m.M[0, 0] = c;
        m.M[0, 1] = -s;
        m.M[1, 0] = s;
        m.M[1, 1] = c;

        return m;
    }

    public static Mat4 Perspective(float fov, float aspect, float zNear, float zFar)
    {
        Mat4 m = Identity();
        m.M[0, 0] = aspect * (1.0f / MathF.Tan(fov / 2.0f));
        m.M[1, 1] = 1.0f / MathF.Tan(fov / 2.0f);
        m.M[2, 2] = zFar / (zFar - zNear);
        m.M[2, 3] = -zFar * zNear / (zFar - zNear);
        m.M[3, 2] = 1.0f;

        return m;
    }

    public static Vector4 Project(Mat4 projectionMatrix, Vector4 v)
    {
        // Multiply the projection matrix by the original vector
        Vector4 result = projectionMatrix * v;
        
        // Perform perspective divide with original z-value that is now stored in w
        if (result.W != 0.0f)
        {
            result.X /= result.W;
            result.Y /= result.W;
            result.Z /= result.Z;
        }

        return result;
    }
}