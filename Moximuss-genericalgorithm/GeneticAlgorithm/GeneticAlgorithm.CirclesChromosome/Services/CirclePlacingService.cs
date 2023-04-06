using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneticAlgorithm.CirclesChromosome.Services
{
    public static class CirclePlacingService
    {
        public static async Task<double> PlaceCircles(List<Circle> circles, int[] sequence, float topBorder, float leftBorder = 0, float bottomBorder = 0)
        {
            if (sequence.Length != circles.Count)
                throw new ArgumentException();
            if (sequence.Length == 0)
                return 0;
            var placed = new List<Circle>();
            circles[sequence[0]].X = circles[sequence[0]].Radius;
            circles[sequence[0]].Y = circles[sequence[0]].Radius;
            placed.Add(circles[sequence[0]]);
            
            var currentLenth = placed.First().X + placed.First().Radius;
            for (int k = 2; k < circles.Count; k++)
            {
                var possiblePositions = new List<XYZPoint>();
                for (int i = 0; i < placed.Count; i++)
                {
                    for (int j = i + 1; j < placed.Count; j++)
                    {
                        possiblePositions.AddRange(PlacementForTwoLaps(placed[i], placed[j], circles[sequence[k]].Radius));
                    }
                    possiblePositions.AddRange(PlacementRelativeCircleAndBottomBorder(placed[i], circles[sequence[k]].Radius, bottomBorder));
                    possiblePositions.AddRange(PlacementRelativeCircleAndLeftBorder(placed[i], circles[sequence[k]].Radius, leftBorder));
                    possiblePositions.AddRange(PlacementRelativeCircleAndTopBorder(placed[i], circles[sequence[k]].Radius, topBorder));
                }
                await FilterPossiblePos(placed, possiblePositions, circles[sequence[k]].Radius, topBorder);
                var tmp = SearchBestPos(placed, possiblePositions, circles[sequence[k]].Radius, currentLenth);
                if (tmp == null)
                    continue;
                circles[sequence[k]].X = tmp.X;
                circles[sequence[k]].Y = tmp.Y;
                placed.Add(circles[sequence[k]]);
                currentLenth = placed.Max(c => c.X + c.Radius);
            }
            return currentLenth;
        }

        private static Task FilterPossiblePos(List<Circle> placed, List<XYZPoint> possiblePos, double radiusOfPlacingCircle, double topBorder)
        {
         
            return Task.Run(() =>
            {
                for (int i = 0; i < possiblePos.Count; i++)
                {
                    if ((possiblePos[i].X - radiusOfPlacingCircle < 0)
                        || (possiblePos[i].Y - radiusOfPlacingCircle < 0)
                        || (possiblePos[i].Y + radiusOfPlacingCircle > topBorder))
                    {
                        possiblePos.RemoveAt(i);
                        i--;
                        continue;
                    }
                    foreach (var c in placed)
                    {
                        double sumRadius = c.Radius + radiusOfPlacingCircle;
                        double distanceBetweenPoints = Math.Sqrt(Math.Pow(c.X - possiblePos[i].X, 2) + Math.Pow(c.Y - possiblePos[i].Y, 2));
                        if (sumRadius > distanceBetweenPoints + 0.0001)
                        {
                            possiblePos.RemoveAt(i);
                            i--;
                            break;
                        }
                    }
                }
            });
        }

        private static bool IsInCircle(double radius, double x, double y)
        {
            for (int i = 0; i < 360; i++)
            {
                double cirlceX = Math.Sin(i) * radius;
                double circleY = Math.Cos(i) * radius;
                double distance = Math.Sqrt(Math.Pow((cirlceX - x), 2) + Math.Pow((circleY - y), 2));
                if (distance > radius)
                {
                    return false;
                }
            }
            return true;
        }

        private static XYZPoint SearchBestPos(List<Circle> placed, List<XYZPoint> possiblePos, double rad, double currentLength)
        {
            double bestLength = currentLength + (2 * rad);
            XYZPoint best = null;
            foreach (var p in possiblePos)
            {
                var afterPlacing = p.X + rad > currentLength ? p.X + rad : currentLength;

                if (afterPlacing == currentLength)
                {
                    return p;
                }
                if (afterPlacing <= bestLength)
                {
                    best = p;
                    bestLength = afterPlacing;
                }
            }
            return best;
        }

        public static IEnumerable<XYZPoint> PlacementRelativeCircleAndLeftBorder(Circle other, float radius, float leftBorder)
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
            var P1 = new XYZPoint();
            P1.X = other.X - a;
            P1.Y = (float)(other.Y + c);
            var P2 = new XYZPoint();
            P2.X = other.X - a;
            P2.Y = (float)(other.Y - c);
            var result = new List<XYZPoint>();
            result.Add(P1);
            if (a != b)
                result.Add(P2);
            return result;
        }

        public static IEnumerable<XYZPoint> PlacementRelativeCircleAndTopBorder(Circle other, float radius, float topBorder)
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
            var P1 = new XYZPoint();
            P1.X = (float)(other.X + c);
            P1.Y = other.Y + a;
            var P2 = new XYZPoint();
            P2.X = (float)(other.X - c);
            P2.Y = other.Y + a;
            var result = new List<XYZPoint>();
            result.Add(P1);
            if (a != b)
                result.Add(P2);
            return result;
        }
        public static IEnumerable<XYZPoint> PlacementRelativeCircleAndBottomBorder(Circle other, float radius, float bottomBorder)
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
            var P1 = new XYZPoint();
            P1.X = (float)(other.X + c);
            P1.Y = (float)(other.Y - a);
            var P2 = new XYZPoint();
            P2.X = (float)(other.X - c);
            P2.Y = other.Y - a;
            var result = new List<XYZPoint>();
            result.Add(P1);
            if (a != b)
                result.Add(P2);
            return result;
        }
        public static IEnumerable<XYZPoint> PlacementForTwoLaps(Circle first, Circle second, float thirdRadius)
        {
            float r1r2 = (float)Math.Sqrt(Math.Pow(first.X - second.X, 2) + Math.Pow(first.Y - second.Y, 2));
            if (r1r2 >= first.Radius + second.Radius + 2 * thirdRadius)
            {
                return new List<XYZPoint>() { new XYZPoint() { X = first.X, Y = first.Y } };
            }

            float r1r3 = first.Radius + thirdRadius;
            float r2r3 = second.Radius + thirdRadius;
            var p = (r1r2 + r1r3 + r2r3) / 2.0;

            float hr3 = (float)(2 * Math.Sqrt(p * (p - r1r2) * (p - r2r3) * (p - r1r3)) / r1r2);
            float hr2 = (float)Math.Sqrt(Math.Pow(r2r3, 2) - Math.Pow(hr3, 2));

            var H = new XYZPoint();
            float lambda = (float)((r1r2 - hr2) / hr2);

            H.X = (float)((first.X + lambda * second.X) / (1 + lambda));
            H.Y = (float)((first.Y + lambda * second.Y) / (1 + lambda));

            var K = new XYZPoint() { X = H.X + (second.Y - H.Y), Y = H.Y - (second.X - H.X) };
            var hk = Math.Sqrt(Math.Pow(H.X - K.X, 2) + Math.Pow(H.Y - K.Y, 2));

            lambda = (float)(hr3 / hk);

            var vectorHK = new XYZPoint() { X = K.X - H.X, Y = K.Y - H.Y };
            vectorHK.X *= lambda;
            vectorHK.Y *= lambda;
            var P3 = new XYZPoint();
            P3.X = vectorHK.X + H.X;
            P3.Y = vectorHK.Y + H.Y;
            var P4 = new XYZPoint();
            vectorHK.X *= -1;
            vectorHK.Y *= -1;
            P4.X = vectorHK.X + H.X;
            P4.Y = vectorHK.Y + H.Y;
            var result = new List<XYZPoint>();
            result.Add(P3);
            result.Add(P4);

            return result;
        }
    }
}
