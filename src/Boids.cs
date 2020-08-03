using System;
using System.Collections.Generic;
using System.Text;
using Altseed2;

namespace Altseed2.Boids
{
    static class Vector2FExt
    {
        public static readonly Vector2F Zero = new Vector2F(0.0f, 0.0f);
        public static readonly Vector2F One = new Vector2F(1.0f, 1.0f);

        public static bool IsInside(this Vector2F v, Vector2F min, Vector2F max)
        {
            return (min.X < v.X && min.Y < v.Y && v.X < max.X && v.Y < max.Y);
        }
    }

    sealed class BoidsParameter
    {
        public float Speed { get; set; } 
        public float Separation { get; set; }
        public float Alignment { get; set; }
        public float Cohesion { get; set; }
        public float Wall { get; set; }
        public float Mouse { get; set; }
    }

    sealed class Boids : Node
    {
        private static readonly Vector2F Area = new Vector2F(600.0f, 600.0f);
        private const int BoidsCount = 256;
        private readonly BoidsParameter boidsParameter;

        private readonly Vector2F[] positions;
        private readonly Vector2F[] directions;
        private Vector2F gravityPoint;

        private readonly CircleNode[] boidDisplays;

        private readonly CircleNode mouseCircle;

        public Boids(BoidsParameter boidsParameter)
        {
            this.boidsParameter = boidsParameter;

            boidDisplays = new CircleNode[BoidsCount];
            positions = new Vector2F[BoidsCount];
            directions = new Vector2F[BoidsCount];

            // Background
            AddChildNode(new RectangleNode { Color = new Color(10, 10, 10), RectangleSize = Area });

            // Initialize Parameters
            var rand = new Random();
            for(int i = 0; i < BoidsCount; i++)
            {
                boidDisplays[i] = new CircleNode { VertNum = 20, Radius = 5.0f };
                AddChildNode(boidDisplays[i]);

                positions[i] = new Vector2F(Area.X * (float)rand.NextDouble(), Area.Y * (float)rand.NextDouble());
                directions[i] = new Vector2F((float)rand.NextDouble() * 2.0f - 1.0f, (float)rand.NextDouble() * 2.0f - 1.0f);
                if(directions[i].SquaredLength != 0.0f) { directions[i] = directions[i].Normal; }
                gravityPoint += positions[i];
            }
            gravityPoint /= BoidsCount;

            mouseCircle  = new CircleNode { VertNum = 20, Radius = 5.0f, Color = new Color(255, 50, 50) };
            AddChildNode(mouseCircle);
        }

        protected override void OnUpdate()
        {
            var mousePos = Engine.Mouse.Position;
            var isMouseInside = mousePos.IsInside(Vector2FExt.Zero, Area);

            var gravity = new Vector2F(0.0f, 0.0f);
            for(int i = 0; i < BoidsCount; i++)
            {
                var direction = Vector2FExt.Zero;

                for(int j = 0; j < BoidsCount; j++)
                {
                    if (i == j) continue;

                    var diff = positions[i] - positions[j];
                    var sl = diff.SquaredLength;

                    if (sl == 0.0f) continue;

                    // Separation
                    direction += boidsParameter.Separation * diff / sl * 1.0f / BoidsCount;
                    // Alignment
                    direction += boidsParameter.Alignment * directions[j] / sl * 1.0f / BoidsCount;
                }

                // Cohesion
                direction += boidsParameter.Cohesion * (gravityPoint - positions[i]).Normal;

                // Mouse
                if(isMouseInside)
                {
                    direction += boidsParameter.Mouse * (positions[i] - mousePos).Normal;
                }

                // Wall
                {
                    var v = positions[i] / Area;
                    if (v.X * (1.0f - v.X) * v.Y * (1.0f - v.Y) != 0.0f)
                    {
                        var rv = (Vector2FExt.One - v);
                        direction += boidsParameter.Wall * (Vector2FExt.One / v / v - Vector2FExt.One / rv / rv);

                    }
                }

                directions[i] = (directions[i] + direction);
                if(directions[i].Length > 0.0f) { directions[i] = directions[i].Normal; }
                positions[i] += directions[i] * boidsParameter.Speed * Engine.DeltaSecond;

                positions[i].X = MathHelper.Clamp(positions[i].X, Area.X, 0.0f);
                positions[i].Y = MathHelper.Clamp(positions[i].Y, Area.Y, 0.0f);

                gravity += positions[i];
            }

            gravityPoint = gravity / BoidsCount;

            for (int i = 0; i < BoidsCount; i++)
            {
                boidDisplays[i].Position = positions[i];
            }

            if(isMouseInside)
            {
                mouseCircle.IsDrawn = true;
                mouseCircle.Position = mousePos;
            }
            else
            {
                mouseCircle.IsDrawn = false;
            }
        }

    }
}
