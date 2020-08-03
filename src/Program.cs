using System;
using Altseed2;

namespace Altseed2.Boids
{
    class Program
    {
        static void Main(string[] args)
        {
            var config = new Configuration();
            config.ConsoleLoggingEnabled = true;
            config.ToolEnabled = true;
            config.WaitVSync = false;

            Engine.Initialize("Boids", 800, 600, config);

            Engine.TargetFPS = 2000.0f;

            var param = new BoidsParameter
            {
                Speed = 100.0f,
                Separation = 20.0f,
                Alignment = 30.0f,
                Cohesion = 0.2f,
                Wall = 0.001f,
                Mouse = 1.0f,
            };

            Engine.AddNode(new Boids(param));

            while (Engine.DoEvents())
            {
                if (Engine.Tool.Begin("Boids", ToolWindowFlags.None))
                {
                    float speed = param.Speed;
                    float separation = param.Separation;
                    float alignment = param.Alignment;
                    float cohesion = param.Cohesion;
                    float wall = param.Wall;
                    float mouse = param.Mouse;

                    Engine.Tool.Text($"FPS:{Engine.CurrentFPS}");
                    if (Engine.Tool.InputFloat("Speed", ref speed)) { param.Speed = speed; }
                    if (Engine.Tool.InputFloat("Separation", ref separation)) { param.Separation = separation; }
                    if (Engine.Tool.InputFloat("Alignment", ref alignment)) { param.Alignment = alignment; }
                    if (Engine.Tool.InputFloat("Cohesion", ref cohesion)) { param.Cohesion = cohesion; }
                    if (Engine.Tool.InputFloat("Wall", ref wall)) { param.Wall = wall; }
                    if (Engine.Tool.InputFloat("Mouse", ref mouse)) { param.Mouse = mouse; }
                    Engine.Tool.End();
                }

                Engine.Update();
            }

            Engine.Terminate();
        }
    }
}
