using System;
using System.Collections.Generic;
using EloBuddy.SDK;
using EloBuddy;
using ClipperLib;
using SharpDX;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace Aka_s_Vayne.Logic
{
    public static class AJSGeometry
    {
        private const int CircleLineSegmentN = 22;

        public static Polygon ToPolygon(this Path v)
        {
            var polygon = new Polygon();
            foreach (var point in v)
            {
                polygon.Add(new Vector2(point.X, point.Y));
            }
            return polygon;
        }

        public static Paths ClipPolygons(List<Polygon> polygons)
        {
            var subj = new Paths(polygons.Count);
            var clip = new Paths(polygons.Count);

            foreach (var polygon in polygons)
            {
                subj.Add(polygon.ToClipperPath());
                clip.Add(polygon.ToClipperPath());
            }

            var solution = new Paths();
            var c = new Clipper();
            c.AddPaths(subj, PolyType.ptSubject, true);
            c.AddPaths(clip, PolyType.ptClip, true);
            c.Execute(ClipType.ctUnion, solution, PolyFillType.pftPositive, PolyFillType.pftEvenOdd);

            return solution;
        }

        public class Polygon
        {
            public List<Vector2> Points = new List<Vector2>();

            public void Add(Vector2 point)
            {
                Points.Add(point);
            }

            public Path ToClipperPath()
            {
                var result = new Path(Points.Count);

                foreach (var point in Points)
                {
                    result.Add(new IntPoint(point.X, point.Y));
                }

                return result;
            }

            public bool IsOutside(Vector2 point)
            {
                var p = new IntPoint(point.X, point.Y);
                return Clipper.PointInPolygon(p, ToClipperPath()) != 1;
            }

            public void Draw(System.Drawing.Color color, int width = 1)
            {
                for (var i = 0; i <= Points.Count - 1; i++)
                {
                    var nextIndex = (Points.Count - 1 == i) ? 0 : (i + 1);
                    DrawLineInWorld(Points[i].To3D(), Points[nextIndex].To3D(), width, color);
                }
            }
            public static void DrawLineInWorld(Vector3 start, Vector3 end, int width, System.Drawing.Color color)
            {
                var from = Drawing.WorldToScreen(start);
                var to = Drawing.WorldToScreen(end);
                Drawing.DrawLine(from[0], from[1], to[0], to[1], width, color);
            }
        }

        public class Circle
        {
            public Vector2 Center;
            public float Radius;

            public Circle(Vector2 center, float radius)
            {
                Center = center;
                Radius = radius;
            }

            public Polygon ToPolygon(int offset = 0, float overrideWidth = -1)
            {
                var result = new Polygon();
                var outRadius = (overrideWidth > 0
                    ? overrideWidth
                    : (offset + Radius) / (float)Math.Cos(2 * Math.PI / CircleLineSegmentN));

                for (var i = 1; i <= CircleLineSegmentN; i++)
                {
                    var angle = i * 2 * Math.PI / CircleLineSegmentN;
                    var point = new Vector2(
                        Center.X + outRadius * (float)Math.Cos(angle), Center.Y + outRadius * (float)Math.Sin(angle));
                    result.Add(point);
                }

                return result;
            }
        }



        public class AJSPolygon
        {
            public List<Vector2> Points;

            public AJSPolygon(List<Vector2> p)
            {
                Points = p;
            }

            public void Add(Vector2 vec)
            {
                Points.Add(vec);
            }

            public int Count()
            {
                return Points.Count;
            }

            public bool Contains(Vector2 point)
            {
                var result = false;
                var j = Count() - 1;
                for (var i = 0; i < Count(); i++)
                {
                    if (Points[i].Y < point.Y && Points[j].Y >= point.Y || Points[j].Y < point.Y && Points[i].Y >= point.Y)
                    {
                        if (Points[i].X +
                            (point.Y - Points[i].Y) / (Points[j].Y - Points[i].Y) * (Points[j].X - Points[i].X) < point.X)
                        {
                            result = !result;
                        }
                    }
                    j = i;
                }
                return result;
            }
            public static List<Vector2> Rectangle(Vector2 startVector2, Vector2 endVector2, float radius)
            {
                var points = new List<Vector2>();

                var v1 = endVector2 - startVector2;
                var to1Side = Vector2.Normalize(v1).Perpendicular() * radius;

                points.Add(startVector2 + to1Side);
                points.Add(startVector2 - to1Side);
                points.Add(endVector2 - to1Side);
                points.Add(endVector2 + to1Side);
                return points;
            }
        }
    }
}
