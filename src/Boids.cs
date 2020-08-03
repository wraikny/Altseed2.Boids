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
    }

    sealed class BoidsParameter
    {
        public float Speed { get; set; } 
        public float Separation { get; set; }
        public float Alignment { get; set; }
        public float Cohesion { get; set; }
        public float Wall { get; set; }
    }

    sealed class Boids : TransformNode
    {
        private static readonly Vector2F Area = new Vector2F(600.0f, 600.0f);
        private const int BoidsCount = 256;
        private readonly BoidsParameter boidsParameter;

        private readonly Vector2F[] positions;
        private readonly Vector2F[] directions;
        private Vector2F gravityPoint;

        private readonly CircleNode[] boidDisplays;


        public Boids(BoidsParameter boidsParameter)
        {
            this.boidsParameter = boidsParameter;

            boidDisplays = new CircleNode[BoidsCount];
            positions = new Vector2F[BoidsCount];
            directions = new Vector2F[BoidsCount];

            var rand = new Random();

            for(int i = 0; i < BoidsCount; i++)
            {
                positions[i] = new Vector2F(Area.X * (float)rand.NextDouble(), Area.Y * (float)rand.NextDouble());
                gravityPoint += positions[i];
            }

            gravityPoint /= BoidsCount;
        }


        protected override void OnAdded()
        {
            AddChildNode(new RectangleNode { Color = new Color(10, 10, 10), RectangleSize = Area });

            for(int i = 0; i < BoidsCount; i++)
            {
                boidDisplays[i] = new CircleNode { VertNum = 20, Radius = 5.0f };
                AddChildNode(boidDisplays[i]);
            }
        }

        protected override void OnUpdate()
        {
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

                // Wall
                {
                    var v = positions[i] / Area;
                    var l = v.SquaredLength;
                    if (v.X * (1.0f - v.X) * v.Y * (1.0f - v.Y) != 0.0f)
                    {
                        var rv = (Vector2FExt.One - v);

                        direction += boidsParameter.Wall * (Vector2FExt.One / v / v - Vector2FExt.One / rv / rv);

                    }
                }

                directions[i] = (directions[i] + direction);
                if(directions[i].Length > 0.0f) { directions[i].Normalize(); }
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
        }

    }
}
