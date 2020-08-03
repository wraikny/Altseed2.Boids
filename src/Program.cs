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

            Engine.Initialize("Boids", 800, 600, config);

            var param = new BoidsParameter
            {
                Speed = 100.0f,
                Separation = 20.0f,
                Alignment = 30.0f,
                Cohesion = 0.2f,
                Wall = 0.001f,
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

                    Engine.Tool.Text($"FPS:{Engine.CurrentFPS}");
                    if (Engine.Tool.InputFloat("Speed", ref speed)) { param.Speed = speed; }
                    if (Engine.Tool.InputFloat("Separation", ref separation)) { param.Separation = separation; }
                    if (Engine.Tool.InputFloat("Alignment", ref alignment)) { param.Alignment = alignment; }
                    if (Engine.Tool.InputFloat("Cohesion", ref cohesion)) { param.Cohesion = cohesion; }
                    if (Engine.Tool.InputFloat("Wall", ref wall)) { param.Wall = wall; }
                    Engine.Tool.End();
                }

                Engine.Update();
            }

            Engine.Terminate();
        }
    }
}
