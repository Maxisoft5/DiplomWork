using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using GeneticAlgorithm.CirclesChromosome;
using GeneticAlgorithm.CirclesChromosome.Models;
using NumSharp;

namespace GeneticAlgorithm.Presentation.Services
{
    public delegate Ellipse DrawingDelegate(Circle circle, Brush brush = null);
    public delegate void MessageDelegate(string message);
    public delegate void ReplaceEllipse(Ellipse ellipse);

    public class CirclePlacingService
    {
        public static event DrawingDelegate drawCircle;
        public static event MessageDelegate writeMessage;
        public static event ReplaceEllipse replaceEllipse;
        private const float _radius = 380;
        private const float _mainCircleX = 1200 / 4;
        private const float _mainCircleY = 500 / 4;

        public static async Task CirclesPlacing(List<Circle> circles, double topBorder, double rightBorder)
        {
            var placed = new List<Circle>();
            circles[0].X = _mainCircleX;
            circles[0].Y = _mainCircleY;
            circles[0].Radius = 40;
            placed.Add(circles[0]);
            drawCircle(circles[0]);

            for (int k = 1; k < circles.Count; k++)
            {
                var currentLenth = placed.Max(c => c.X + c.Radius);
                var possiblePositions = new List<XYPoint>();

                for (int i = 0; i < placed.Count; i++)
                {
                    for (int j = i + 1; j < placed.Count; j++)
                    {
                        possiblePositions.AddRange(PlacementForTwoLaps(placed[i], placed[j], circles[k].Radius));
                    }
                    possiblePositions.AddRange(PlacementRelativeCircleAndBottomBorder(placed[i], circles[k].Radius, 0));
                    possiblePositions.AddRange(PlacementRelativeCircleAndLeftBorder(placed[i], circles[k].Radius, 0));
                    possiblePositions.AddRange(PlacementRelativeCircleAndTopBorder(placed[i], circles[k].Radius, topBorder));
                }

                await FilterPossiblePos(placed, possiblePositions, circles[k].Radius, topBorder, rightBorder);

                var tmp = SearchBestPos(possiblePositions, circles[k].Radius, currentLenth);
                if (tmp == null)
                    throw new ArgumentException();
                circles[k].X = tmp.X;
                circles[k].Y = tmp.Y;
                drawCircle(circles[k]);
                placed.Add(circles[k]);
            }
        }

        public static XYPoint SearchBestPos(List<XYPoint> possiblePos, double rad, double currentLength)
        {
            double bestLength = currentLength + (2 * rad);
            XYPoint best = null;
            foreach (var p in possiblePos)
            {
                var afterPlacing = p.X + rad > currentLength ? p.X + rad : currentLength;

                if (afterPlacing <= bestLength)
                {
                    best = p;
                    bestLength = afterPlacing;
                }

                if (afterPlacing == currentLength)
                {
                    return p;
                }
            }
            return best;
        }

        public static Task FilterPossiblePos(List<Circle> placed, List<XYPoint> possiblePos, float radiusOfPlacingCircle,
            double topBorder, double rightBorder)
        {
            int pause = 10;
            return Task.Run(() =>
            {
                for (int i = 0; i < possiblePos.Count; i++)
                {
                    if ((possiblePos[i].X < 0)
                         || (possiblePos[i].Y < 0)
                         || (rightBorder - (possiblePos[i].X + radiusOfPlacingCircle) <= radiusOfPlacingCircle)
                         || (topBorder - (possiblePos[i].Y + radiusOfPlacingCircle) <= radiusOfPlacingCircle)
                         || IsInCircle(possiblePos[i].X, possiblePos[i].Y, radiusOfPlacingCircle) == false)
                    {
                        var tmpCircle = new Circle();
                        tmpCircle.Radius = radiusOfPlacingCircle;
                        tmpCircle.X = possiblePos[i].X;
                        tmpCircle.Y = possiblePos[i].Y;
                        var tmpEllipse = drawCircle(tmpCircle, Brushes.Red);
                        writeMessage("Пересекает границу или вне границ.");
                        Thread.Sleep(pause);
                        replaceEllipse(tmpEllipse);
                        possiblePos.RemoveAt(i);
                        i--;
                        continue;
                    }
                    bool removed = false;
                    foreach (var c in placed)
                    {
                        double sumRadius = c.Radius + radiusOfPlacingCircle;
                        double distanceBetweenPoints = Math.Sqrt(Math.Pow(c.X - possiblePos[i].X, 2) + Math.Pow(c.Y - possiblePos[i].Y, 2));

                        if (sumRadius > distanceBetweenPoints + 0.00001)
                        {
                            var tmpCircle = new Circle();
                            tmpCircle.Radius = radiusOfPlacingCircle;
                            tmpCircle.X = possiblePos[i].X;
                            tmpCircle.Y = possiblePos[i].Y;
                            var tmpEllipse = drawCircle(tmpCircle, Brushes.Red);
                            writeMessage("Пересекает другой круг.");
                            Thread.Sleep(pause);
                            replaceEllipse(tmpEllipse);
                            removed = true;
                            possiblePos.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                    if (!removed)
                    {
                        var tmpCircle = new Circle();
                        tmpCircle.Radius = radiusOfPlacingCircle;
                        tmpCircle.X = possiblePos[i].X;
                        tmpCircle.Y = possiblePos[i].Y;
                        var tmpEllips = drawCircle(tmpCircle, Brushes.Blue);
                        writeMessage("");
                        Thread.Sleep(pause);
                        replaceEllipse(tmpEllips);
                    }
                }
            });
        }

        private static bool IsInCircle(float x, float y, float r)
        {
            double distance = Math.Sqrt(Math.Pow(x - _mainCircleX, 2) + Math.Pow(y - _mainCircleY, 2));
            if (distance > _radius || distance < -_radius)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public static IEnumerable<XYPoint> PlacementRelativeCircleAndLeftBorder(Circle other, float radius, float leftBorder)
        {
            var a = other.X - (leftBorder + radius);
            var b = other.Radius + radius;
            var c = Math.Sqrt(Math.Pow(b, 2) - Math.Pow(a, 2));
            if (Math.Abs(a) > b)
            {
                var sign = Math.Sign(a);
                a = Math.Abs(b) * sign;
                c = 0;
            }
            var P1 = new XYPoint();
            P1.X = other.X - a;
            P1.Y = (float)(other.Y + c);
            var P2 = new XYPoint();
            P2.X = other.X - a;
            P2.Y = (float)(other.Y - c);
            var result = new List<XYPoint>();
            result.Add(P1);
            if (a != b)
                result.Add(P2);
            return result;
        }

        public static IEnumerable<XYPoint> PlacementRelativeCircleAndTopBorder(Circle other, double radius, double topBorder)
        {
            var a = topBorder - radius - other.Y;
            var b = other.Radius + radius;
            var c = Math.Sqrt(Math.Pow(b, 2) - Math.Pow(a, 2));
            if (Math.Abs(a) > b)
            {
                var sign = Math.Sign(a);
                a = Math.Abs(b) * sign;
                c = 0;
            }
            var P1 = new XYPoint();
            P1.X = (float)(other.X + c);
            P1.Y = (float)(other.Y + a);
            var P2 = new XYPoint();
            P2.X = (float)(other.X - c);
            P2.Y = (float)(other.Y + a);
            var result = new List<XYPoint>();
            result.Add(P1);
            if (a != b)
                result.Add(P2);
            return result;
        }

        public static IEnumerable<XYPoint> PlacementRelativeCircleAndBottomBorder(Circle other, float radius, float bottomBorder)
        {
            var a = other.Y - (bottomBorder + radius);
            var b = other.Radius + radius;
            var c = Math.Sqrt(Math.Pow(b, 2) - Math.Pow(a, 2));
            if (Math.Abs(a) > b)
            {
                var sign = Math.Sign(a);
                a = Math.Abs(b) * sign;
                c = 0;
            }
            var P1 = new XYPoint();
            P1.X = (float)(other.X + c);
            P1.Y = other.Y - a;
            var P2 = new XYPoint();
            P2.X = (float)(other.X - c);
            P2.Y = other.Y - a;
            var result = new List<XYPoint>();
            result.Add(P1);
            if (a != b)
                result.Add(P2);
            return result;
        }

        public static IEnumerable<XYPoint> PlacementForTwoLaps(Circle first, Circle second, float thirdRadius)
        {
            var distance = Math.Sqrt(Math.Pow(second.X - first.X, 2) + Math.Pow(second.Y - second.Y, 2));
            var result = new List<XYPoint>();

            if (distance >= first.Radius + second.Radius)
            {
                var R1 = new Circle();
                R1.Radius = first.Radius + thirdRadius;
                var R2 = new Circle() { X = second.X - first.X, Y = second.Y - first.Y, Radius = second.Radius + thirdRadius };

                var a = -2 * R2.X;
                var b = -2 * R2.Y;
                var c = Math.Pow(R2.X, 2) + Math.Pow(R2.Y, 2) + Math.Pow(R1.Radius, 2) - Math.Pow(R2.Radius, 2);
                // ax+by+c = 0 - line where passes though the points of intersection of circles

                var EPS = 0.0000001;
                var r = R1.Radius;
                float x0 = (float)(-a * c / (a * a + b * b)), y0 = (float)(-b * c / (a * a + b * b));
                if (c * c > r * r * (a * a + b * b) + EPS)
                    // no intersection points
                    return result;

                if (Math.Abs(Math.Pow(c, 2) - Math.Pow(r, 2) * (Math.Pow(a, 2) + Math.Pow(b, 2))) < EPS)
                {
                    // one intersection point
                    result.Add(new XYPoint() { X = x0 + first.X, Y = y0 + first.Y });
                    return result;
                }

                // two intersection points
                float d = (float)(Math.Pow(r, 2) - Math.Pow(c, 2) / (Math.Pow(a, 2) + Math.Pow(b, 2)));
                float mult = (float)Math.Sqrt(d / (Math.Pow(a, 2) + Math.Pow(b, 2)));

                result.Add(new XYPoint()
                {
                    X = x0 + b * mult + first.X,
                    Y = y0 - a * mult + first.Y,
                });
                result.Add(new XYPoint()
                {
                    X = x0 - b * mult + first.X,
                    Y = y0 + a * mult + first.Y,
                });
            }
            return result;
        }
    }
}
