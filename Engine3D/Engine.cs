using Engine3D.MathUtil;
using SFML.Graphics;
using SFML.Window;

namespace Engine3D;
public class Engine
{
    public bool IsRunning { get; private set; }

    private readonly Vector3 _cameraPosition = new(0, 0, 0);
    private Mat4 _projectionMatrix = Mat4.Identity();
    
    private readonly List<Triangle> _trianglesToRender = new();

    public bool Init()
    {
        // Initialize SFML and create a Window
        if (!Display.InitializeWindow(60))
            return false;

        // Setup the SFML events
        SetupEvents();
        
        IsRunning = true;

        return true;
    }

    public void Setup()
    {
        // Initialize the perspective projection matrix
        float fieldOfView = MathF.PI / 3.0f; // 60 deg in radians
        float aspect = (float)Display.WindowHeight / Display.WindowWidth;
        float zNear = 0.1f;
        float zFar = 100.0f;
        _projectionMatrix = Mat4.Perspective(fieldOfView, aspect, zNear, zFar);
        
        Mesh.LoadCubeData();
    }

    public void ProcessInput()
    {
        Display.ProcessEvents();
    }

    public void Update()
    {
        Mesh.Rotation.X += 0.01f;
        Mesh.Rotation.Y += 0.01f;
        Mesh.Rotation.Z += 0.01f;

        //Mesh.Scale.X += 0.002f;
        //Mesh.Scale.Y += 0.002f;
        //Mesh.Scale.Z += 0.002f;

        //Mesh.Translation.X += 0.02f;
        
        // Translate the vertex away from the camera
        Mesh.Translation.Z = 5.0f;
    }

    public void Render()
    {
        Display.StartRender();

        //Display.DrawGrid(new Color(105, 105, 105));

        Mat4 scaleMatrix = Mat4.Scale(Mesh.Scale);
        Mat4 translationMatrix = Mat4.Translate(Mesh.Translation);
        Mat4 rotationMatrixX = Mat4.RotateX(Mesh.Rotation.X);
        Mat4 rotationMatrixY = Mat4.RotateY(Mesh.Rotation.Y);
        Mat4 rotationMatrixZ = Mat4.RotateZ(Mesh.Rotation.Z);
        
        _trianglesToRender.Clear();
        
        // Loop all triangle faces of the mesh
        foreach (Face meshFace in Mesh.Faces)
        {
            Vector3[] faceVertices = new Vector3[3];
            faceVertices[0] = Mesh.Vertices[meshFace.A - 1];
            faceVertices[1] = Mesh.Vertices[meshFace.B - 1];
            faceVertices[2] = Mesh.Vertices[meshFace.C - 1];

            // Loop all three vertices of this current face and apply transformations
            Vector4[] transformedVertices = new Vector4[3];
            for (int i = 0; i < 3; i++)
            {
                Vector4 transformedVertex = new Vector4(faceVertices[i]);
                
                // Create a World Matrix combining scale, rotation and transformation matrices
                Mat4 worldMatrix = Mat4.Identity();
                worldMatrix = scaleMatrix * worldMatrix;
                worldMatrix = rotationMatrixX * worldMatrix;
                worldMatrix = rotationMatrixY * worldMatrix;
                worldMatrix = rotationMatrixZ * worldMatrix;
                worldMatrix = translationMatrix * worldMatrix;

                // Multiply the World Matrix by the original vector
                transformedVertex = worldMatrix * transformedVertex;

                // Save transformed vertex in the array of transformed vertices
                transformedVertices[i] = transformedVertex;
            }


            Vector3 vecA = new Vector3(transformedVertices[0]);
            Vector3 vecB = new Vector3(transformedVertices[1]);
            Vector3 vecC = new Vector3(transformedVertices[2]);

            // Get the vector subtraction of B-A and C-A
            Vector3 vecAb = vecB - vecA;
            Vector3 vecAc = vecC - vecA;
            Vector3.Normalize(ref vecAb);
            Vector3.Normalize(ref vecAc);

            // Compute the face normal
            Vector3 normal = Vector3.Cross(vecAb, vecAc);
            Vector3.Normalize(ref normal);

            // Find the vector between a point in the triangle and the camera origin
            Vector3 cameraRay = _cameraPosition - vecA;

            // Calculate how aligned the camera ray is with the face normal
            float dotNormalCamera = Vector3.Dot(normal, cameraRay);

            // Backface culling - test to see if the current face should be projected
            // Bypass the triangles that are looking away from the camera
            if (Display.CullingMethod == CullingMethod.CullBackface)
                if (dotNormalCamera < 0)
                    continue;

            // Calculate the shade intensity based on how aligned is the normal and the inverse of the light ray
            float lightIntensityFactor = -Vector3.Dot(normal, Light.Direction);
            
            // Calculate the triangle color based on the light angle
            Color triangleColor = Light.ApplyIntensity(meshFace.Color, lightIntensityFactor);

            // Loop all three vertices to perform projection
            Triangle projectedTriangle = new Triangle();
            // Holds the average depth for each face based on the vertices after transformation
            float averageDepth = 0;
            for (int i = 0; i < 3; i++)
            {
                // Project the current vertex
                Vector4 projectedPoint = Mat4.Project(_projectionMatrix, transformedVertices[i]);
                
                // Scale into view
                projectedPoint.X *= Display.WindowWidth / 2f;
                projectedPoint.Y *= Display.WindowHeight / 2f;
                
                // Invert the y values to account for flipped screen y coordinate
                projectedPoint.Y *= -1.0f;

                // Translate the project points to the middle of the screen
                projectedPoint.X += Display.WindowWidth / 2f;
                projectedPoint.Y += Display.WindowHeight / 2f;

                averageDepth += transformedVertices[i].Z;

                projectedTriangle.Points[i] = new Vector2(projectedPoint);
            }

            projectedTriangle.AverageDepth = averageDepth / 3.0f;
            
            projectedTriangle.Color = triangleColor;
            _trianglesToRender.Add(projectedTriangle);
        }

        // Sort the triangles to render by their average depth
        int numTriangles = _trianglesToRender.Count;
        for (int i = 0; i < numTriangles; i++)
        for (int j = i; j < numTriangles; j++)
            if (_trianglesToRender[i].AverageDepth < _trianglesToRender[j].AverageDepth)
                (_trianglesToRender[i], _trianglesToRender[j]) = (_trianglesToRender[j], _trianglesToRender[i]);

        // Loop all projected triangles and render them
        foreach (Triangle triangle in _trianglesToRender)
        {
            // Draw filled triangle
            if (Display.RenderMethod == RenderMethod.RenderFillTriangle ||
                Display.RenderMethod == RenderMethod.RenderFillTriangleWire)
                Display.DrawFilledTriangle(triangle, new Color(triangle.Color));

            // Draw triangle wireframe
            if (Display.RenderMethod == RenderMethod.RenderWire ||
                Display.RenderMethod == RenderMethod.RenderWireVertex ||
                Display.RenderMethod == RenderMethod.RenderFillTriangleWire)
                Display.DrawTriangle(triangle, Color.White);

            // Draw triangle vertex points
            if (Display.RenderMethod == RenderMethod.RenderWireVertex)
            {
                Display.DrawRect(triangle.Points[0].X - 3, triangle.Points[0].Y - 3, 6, 6,
                    new Color(0xFF0000FF));
                Display.DrawRect(triangle.Points[1].X - 3, triangle.Points[1].Y - 3, 6, 6,
                    new Color(0xFF0000FF));
                Display.DrawRect(triangle.Points[2].X - 3, triangle.Points[2].Y - 3, 6, 6,
                    new Color(0xFF0000FF));
            }
        }

        Display.EndRender();
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    //////////////////////////////////////////// Event Handlers ////////////////////////////////////////////////////
    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////
    
    private void SetupEvents()
    {
        Display.OnWindowClosed = WindowCloseRequested;
        Display.OnKeyPressed = WindowKeyPressed;
    }
    
    private void WindowCloseRequested(object? sender, EventArgs e)
    {
        IsRunning = false;
    }
    
    private void WindowKeyPressed(object? sender, KeyEventArgs e)
    {
        switch (e.Code)
        {
            case Keyboard.Key.Escape:
                IsRunning = false;
                break;
            case Keyboard.Key.Num1:
                Display.RenderMethod = RenderMethod.RenderWire;
                break;
            case Keyboard.Key.Num2:
                Display.RenderMethod = RenderMethod.RenderWireVertex;
                break;
            case Keyboard.Key.Num3:
                Display.RenderMethod = RenderMethod.RenderFillTriangle;
                break;
            case Keyboard.Key.Num4:
                Display.RenderMethod = RenderMethod.RenderFillTriangleWire;
                break;
            case Keyboard.Key.Num9:
                Display.CullingMethod = CullingMethod.CullBackface;
                break;
            case Keyboard.Key.Num0:
                Display.CullingMethod = CullingMethod.CullNone;
                break;
        }
    }
}