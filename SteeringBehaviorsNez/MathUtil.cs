using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace SteeringBehaviorsNez
{
    public static class MathUtil
    {
        public static Vector2 Truncate(Vector2 vec, float max)
        {
            var i = max / vec.Length();
            i = i < 1.0f ? i : 1.0f;

            return vec * i;
        }
        
        public static float TurnToFace(Vector2 position, Vector2 faceThis, float currentAngle, float turnSpeed)
        {
            float x = faceThis.X - position.X;
            float y = faceThis.Y - position.Y;

            float desiredAngle = (float)Math.Atan2(y, x);

            float difference = MathHelper.WrapAngle(desiredAngle - currentAngle);

            difference = MathHelper.Clamp(difference, -turnSpeed, turnSpeed);

            return MathHelper.WrapAngle(currentAngle + difference);
        }

        public static Vector2 Normalized(this Vector2 vec)
        {
            var v = vec;
            v.Normalize();
            return v;
        }
        
        public static float NextFloat(this System.Random rand, float minValue, float maxValue)
        {
            return (float)rand.NextDouble() * (maxValue - minValue) + minValue;
        }
        
        public static Vector2 NextVector2(this System.Random rand, float minLength, float maxLength)
        {
            double theta = rand.NextDouble() * 2 * Math.PI;
            float length = rand.NextFloat(minLength, maxLength);
            return new Vector2(length * (float)Math.Cos(theta), length * (float)Math.Sin(theta));
        }

        public static int Preference(Vector2 goal, Vector2 hex)
        {
            return (0xFFFF & Math.Abs((int) CrossScalar(goal, hex)));
        }

        public static float CrossScalar(Vector2 a, Vector2 b)
        {
            return (a.X * b.Y) - (a.Y * b.X);
        }

        public static IEnumerable<Point> GetPointsOnLine(Point a, Point b) =>
            GetPointsOnLine(a.X, a.Y, b.X, b.Y);
        
        public static IEnumerable<Point> GetPointsOnLine(int x0, int y0, int x1, int y1)
        {
            bool steep = Math.Abs(y1 - y0) > Math.Abs(x1 - x0);
            if (steep)
            {
                int t;
                t = x0; // swap x0 and y0
                x0 = y0;
                y0 = t;
                t = x1; // swap x1 and y1
                x1 = y1;
                y1 = t;
            }
            if (x0 > x1)
            {
                int t;
                t = x0; // swap x0 and x1
                x0 = x1;
                x1 = t;
                t = y0; // swap y0 and y1
                y0 = y1;
                y1 = t;
            }
            int dx = x1 - x0;
            int dy = Math.Abs(y1 - y0);
            int error = dx / 2;
            int ystep = (y0 < y1) ? 1 : -1;
            int y = y0;
            for (int x = x0; x <= x1; x++)
            {
                yield return new Point((steep ? y : x), (steep ? x : y));
                error = error - dy;
                if (error < 0)
                {
                    y += ystep;
                    error += dx;
                }
            }
            yield break;
        }
    }
}