﻿using System;
using System.Collections.Generic;
using System.Linq;
using Asteroids.Engine.Common;
using Asteroids.Engine.Components;
using Asteroids.Engine.Components.Interfaces;
using Asteroids.Engine.Interfaces;
using Asteroids.Game.Components;
using Asteroids.Game.Components.CommonComponents;
using Asteroids.Game.Components.EnemyComponents;
using Asteroids.Game.Components.PlayerComponents;
using Asteroids.Game.Utils;
using OpenTK;

namespace Asteroids.Game.Factories
{
    public static class AsteroidFactory
    {
        private static Random r = new Random();
        
        public static GameObject GetAsteroid(Vector3 coordinate, float scale, IGameState gameWorld, bool isFragment = true)
        {
            IComponent rendererComponent = GenerateAsteroidMesh(out Vector2 size);
            
            int rotations = r.Next(0, 360);
            
            int minVelocity = isFragment ? 80 : 130;
            int maxVelocity = isFragment ? 120 : 290;

            TransformComponent transform = new TransformComponent(coordinate,
                new Vector3(0.0f, 0.0f, rotations),
                new Vector3(scale, scale, 1.0f),
                Vector3.Zero);
            
            transform.Size = new Vector2(size.X * transform.Scale.X, size.Y * transform.Scale.Y);
            
            string tag = isFragment ? "Asteroid" : "Fragment";
            
            
            GameObject asteroid = new GameObject(tag, transform);
            
            if(Settings.RenderMode == RenderModes.Polygons) asteroid.AddComponent(rendererComponent);
            else asteroid.AddComponent(new SpriteRendererComponent(isFragment ? "asteroid-2.png": "asteroid-3.png"));
            
            asteroid.AddComponent(new PhysicsComponent());
            asteroid.AddComponent(new AsteroidAiComponent(minVelocity,maxVelocity));
            asteroid.AddComponent(new CoordinateComponent(gameWorld));
            return asteroid;
        }
        
        private static IComponent GenerateAsteroidMesh(out Vector2 size)
        {
            int minRadius = 30;
            int maxRadius = 50;
            int granularity = 21;
            int minVary = 25;
            int maxVary = 75;

            double tau = 2 * Math.PI;

            IList<float> points = new List<float>();
            IList<uint> indices = new List<uint>();

            points.Add(0.0f);
            points.Add(0.0f);
            points.Add(0.0f);

            uint prevIndex = 0;
            uint currentIndex = 1;

            float xMax = 0, yMax = 0, xMin = 0, yMin= 0;
            
            for (float angle = 0; angle < tau; angle += (float) (tau / granularity))
            {
                int angleVary = r.Next(minVary, maxVary);
                var angleVaryRadians = (tau / granularity) * angleVary / 100;
                var angleFinal = angle + angleVaryRadians - (Math.PI / granularity);
                float radius = r.Next(minRadius, maxRadius) / 100.0f;

                float x = (float) Math.Sin(angleFinal) * radius;
                float y = (float) -Math.Cos(angleFinal) * radius;

                if (x > xMax) xMax = x;
                if (y > yMax) yMax = y;
                if (x < xMin) xMin = y;
                if (y < yMin) yMin = y;
                
                points.Add(x);
                points.Add(y);
                points.Add(0.0f);

                if (currentIndex == 1)
                {
                    prevIndex = currentIndex;
                    currentIndex++;
                    continue;
                }

                indices.Add(0);
                indices.Add(prevIndex);
                indices.Add(currentIndex);

                prevIndex = currentIndex;
                currentIndex++;
            }

            indices.Add(0);
            indices.Add(currentIndex);
            indices.Add(1);

            IComponent mesh = new PolygonRenderComponent(points.ToArray(), indices.ToArray());

            size = new Vector2(Math.Abs(xMax)+Math.Abs(xMin),Math.Abs(yMax)+Math.Abs(yMin));
            
            return mesh;
        }
    }
}