namespace Engine3D;

internal class Program
{
    static void Main(string[] args)
    {
        Engine engine = new Engine();
        if (!engine.Init())
            return;

        engine.Setup();

        while (engine.IsRunning)
        {
            engine.ProcessInput();
            engine.Update();
            engine.Render();
        }
    }
}