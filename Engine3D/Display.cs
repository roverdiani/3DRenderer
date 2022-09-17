using SFML.Graphics;
using SFML.Window;

namespace Engine3D;

public enum CullingMethod
{
    CullNone,
    CullBackface
}

public enum RenderMethod
{
    RenderWire,
    RenderWireVertex,
    RenderFillTriangle,
    RenderFillTriangleWire
}

public static class Display
{
    public const int WindowWidth = 800;
    public const int WindowHeight = 600;

    private static RenderWindow Window { get; set; }

    private static readonly Image PixelBuffer = new(WindowWidth, WindowHeight);
    private static readonly Texture PixelTexture = new(WindowWidth, WindowHeight);

    public static CullingMethod CullingMethod { get; set; } = CullingMethod.CullBackface;
    public static RenderMethod RenderMethod { get; set; } = RenderMethod.RenderFillTriangleWire;

    public static EventHandler OnWindowClosed
    {
        set => Window.Closed += value;
    }

    public static EventHandler<KeyEventArgs> OnKeyPressed
    {
        set => Window.KeyPressed += value;
    }

    public static bool InitializeWindow(uint targetFps)
    {
        Window = new RenderWindow(new VideoMode(WindowWidth, WindowHeight), "Engine3D");
        Window.SetFramerateLimit(targetFps);
        return Window.IsOpen;
    }

    public static void ProcessEvents()
    {
        Window.DispatchEvents();
    }

    public static void StartRender()
    {
        Window.Clear();
    }

    public static void EndRender()
    {
        RenderColorBuffer();
        ClearColorBuffer(Color.Black);
        Window.Display();
    }

    public static void DrawGrid(Color color)
    {
        for (int y = 10; y < WindowHeight; y += 10)
        for (int x = 10; x < WindowWidth; x += 10)
            DrawPixel(x, y, color);
    }

    public static void DrawRect(float x, float y, int width, int height, Color color)
    {
        for (int i = 0; i < width; i++)
        for (int j = 0; j < height; j++)
        {
            int currentX = (int)x + i;
            int currentY = (int)y + j;
            
            DrawPixel(currentX, currentY, color);
        }
    }

    public static void DrawLine(int x0, int y0, int x1, int y1, Color color)
    {
        int deltaX = x1 - x0;
        int deltaY = y1 - y0;
        int longestSideLength = Math.Abs(deltaX) >= Math.Abs(deltaY) ? Math.Abs(deltaX) : Math.Abs(deltaY);

        float xInc = deltaX / (float)longestSideLength;
        float yInc = deltaY / (float)longestSideLength;

        float currentX = x0;
        float currentY = y0;

        for (int i = 0; i <= longestSideLength; i++)
        {
            DrawPixel((int)Math.Round(currentX), (int)Math.Round(currentY), color);
            currentX += xInc;
            currentY += yInc;
        }
    }

    public static void DrawLineBresenham(int x0, int y0, int x1, int y1, Color color)
    {
        int dx = Math.Abs(x1 - x0), sx = x0 < x1 ? 1 : -1;
        int dy = Math.Abs(y1 - y0), sy = y0 < y1 ? 1 : -1;
        int err = (dx > dy ? dx : -dy) / 2;
        while (true)
        {
            DrawPixel(x0, y0, color);
            if (x0 == x1 && y0 == y1)
                break;
            int e2 = err;
            if (e2 > -dx)
            {
                err -= dy;
                x0 += sx;
            }

            if (e2 < dy)
            {
                err += dx;
                y0 += sy;
            }
        }
    }

    public static void DrawTriangle(Triangle triangle, Color color)
    {
        DrawLineBresenham((int)triangle.Points[0].X, (int)triangle.Points[0].Y, (int)triangle.Points[1].X,
            (int)triangle.Points[1].Y, color);
        DrawLineBresenham((int)triangle.Points[1].X, (int)triangle.Points[1].Y, (int)triangle.Points[2].X,
            (int)triangle.Points[2].Y, color);
        DrawLineBresenham((int)triangle.Points[2].X, (int)triangle.Points[2].Y, (int)triangle.Points[0].X,
            (int)triangle.Points[0].Y, color);
    }

    public static void DrawFilledTriangle(Triangle triangle, Color color)
    {
        // Need to sort the vertices by y-coordinate ascending (y0 < y1 < y2)
        int x0 = (int)triangle.Points[0].X;
        int y0 = (int)triangle.Points[0].Y;
        int x1 = (int)triangle.Points[1].X;
        int y1 = (int)triangle.Points[1].Y;
        int x2 = (int)triangle.Points[2].X;
        int y2 = (int)triangle.Points[2].Y;

        if (y0 > y1)
        {
            (y0, y1) = (y1, y0);
            (x0, x1) = (x1, x0);
        }

        if (y1 > y2)
        {
            (y1, y2) = (y2, y1);
            (x1, x2) = (x2, x1);
        }

        if (y0 > y1)
        {
            (y0, y1) = (y1, y0);
            (x0, x1) = (x1, x0);
        }

        if (y1 == y2)
            FillFlatBottomTriangle(x0, y0, x1, y1, x2, y2, color);
        else if (y0 == y1)
            FillFlatTopTriangle(x0, y0, x1, y1, x2, y2, color);
        else
        {
            // Calculate the new vertex (Mx, My) using triangle similarity
            int mY = y1;
            int mX = (x2 - x0) * (y1 - y0) / (y2 - y0) + x0;

            FillFlatBottomTriangle(x0, y0, x1, y1, mX, mY, color);
            FillFlatTopTriangle(x1, y1, mX, mY, x2, y2, color);
        }
    }

    private static void FillFlatBottomTriangle(int x0, int y0, int x1, int y1, int x2, int y2, Color color)
    {
        // Find the two slopes (two triangle legs)
        float invSlope1 = (float)(x1 - x0) / (y1 - y0);
        float invSlope2 = (float)(x2 - x0) / (y2 - y0);
        
        // Start xStart and xEnd from the top vertex (x0, y0)
        float xStart = x0;
        float xEnd = x0;
        
        // Loop all the scan-lines from top to bottom
        for (int y = y0; y <= y2; y++)
        {
            DrawLineBresenham((int)xStart, y, (int)xEnd, y, color);
            xStart += invSlope1;
            xEnd += invSlope2;
        }
    }

    private static void FillFlatTopTriangle(int x0, int y0, int x1, int y1, int x2, int y2, Color color)
    {
        // Find the two slopes (two triangle legs)
        float invSlope1 = (float)(x2 - x0) / (y2 - y0);
        float invSlope2 = (float)(x2 - x1) / (y2 - y1);
        
        // Start xStart and xEnd from the bottom vertex (x2, y2)
        float xStart = x2;
        float xEnd = x2;
        
        // Loop all the scan-lines from bottom to top
        for (int y = y2; y >= y0; y--)
        {
            DrawLineBresenham((int)xStart, y, (int)xEnd, y, color);
            xStart -= invSlope1;
            xEnd -= invSlope2;
        }
    }
    
    private static void RenderColorBuffer()
    {
        PixelTexture.Update(PixelBuffer);
        Window.Draw(new Sprite(PixelTexture));
    }

    private static void ClearColorBuffer(Color color)
    {
        for (int y = 0; y < WindowHeight; y++)
        for (int x = 0; x < WindowWidth; x++)
            DrawPixel(x, y, color);
    }

    private static void DrawPixel(int x, int y, Color color)
    {
        if (x >= 0 && x < WindowWidth && y >= 0 && y < WindowHeight)
            PixelBuffer.SetPixel((uint) x, (uint) y, color);
    }
}