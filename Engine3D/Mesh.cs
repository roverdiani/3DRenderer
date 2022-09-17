using System.Globalization;
using Engine3D.MathUtil;

namespace Engine3D;

public static class Mesh
{
    public static List<Vector3> Vertices { get; } = new();
    public static List<Face> Faces { get; } = new();
    public static Vector3 Rotation = new(0, 0, 0);
    public static Vector3 Scale = new(1, 1, 1);
    public static Vector3 Translation = new(0, 0, 0);

    private static readonly Vector3[] CubeVertices =
    {
        new(-1, -1, -1),
        new(-1, 1, -1),
        new(1, 1, -1),
        new(1, -1, -1),
        new(1, 1, 1),
        new(1, -1, 1),
        new(-1, 1, 1),
        new(-1, -1, 1)
    };

    private static readonly Face[] CubeFaces =
    {
        // FRONT
        new(1, 2, 3),
        new(1, 3, 4),
        // RIGHT
        new(4, 3, 5),
        new(4, 5, 6),
        // BACK
        new(6, 5, 7),
        new(6, 7, 8),
        // LEFT
        new(8, 7, 2),
        new(8, 2, 1),
        // TOP
        new(2, 7, 5),
        new(2, 5, 3),
        // BOTTOM
        new(6, 8, 1),
        new(6, 1, 4)
    };

    public static void LoadCubeData()
    {
        foreach (Vector3 vertex in CubeVertices)
            Vertices.Add(vertex);
        
        foreach (Face face in CubeFaces)
            Faces.Add(face);
    }

    public static void LoadObjFileData(string filename)
    {
        using FileStream fileStream = File.OpenRead(filename);
        using StreamReader streamReader = new StreamReader(fileStream);
        string? line;
        while ((line = streamReader.ReadLine()) != null)
        {
            // Vertex Information
            if (line.StartsWith("v "))
            {
                string[] vertexData = line.Split(' ').Skip(1).ToArray();
                Vector3 vertex = new()
                {
                    X = float.Parse(vertexData[0], CultureInfo.InvariantCulture.NumberFormat),
                    Y = float.Parse(vertexData[1], CultureInfo.InvariantCulture.NumberFormat),
                    Z = float.Parse(vertexData[2], CultureInfo.InvariantCulture.NumberFormat)
                };
                Vertices.Add(vertex);
            }
            
            // Face Information
            if (line.StartsWith("f "))
            {
                string[] faceData = line.Split(' ').Skip(1).ToArray();
                int[] vertexIndices = new int[3];
                int[] textureIndices = new int[3];
                int[] normalIndices = new int[3];

                for (int i = 0; i < 3; i++)
                {
                    string[] data = faceData[i].Split('/');
                    vertexIndices[i] = int.Parse(data[0]);
                    textureIndices[i] = int.Parse(data[1]);
                    normalIndices[i] = int.Parse(data[2]);
                }

                Face face = new()
                {
                    A = vertexIndices[0],
                    B = vertexIndices[1],
                    C = vertexIndices[2]
                };
                Faces.Add(face);
            }
        }
    }
}