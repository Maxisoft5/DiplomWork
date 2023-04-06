using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GeneticAlgorithm.RectanglesGenotype.Services
{

    public class RectanglePlacingService
    {
        public static double PlaceRectangles(List<Rectangle> rectangles, int[] sequence, out List<XYPoint> positions, float topBorder, float leftBorder = 0, float bottomBorder = 0)
        {
            if (sequence is null || sequence.Length == 0)
            {
                throw new ArgumentException("Sequence is null or empty!");
            }
            if (rectangles is null || rectangles.Count == 0)
            {
                throw new ArgumentException("Rectangles collection is null or empty!");
            }
            if (sequence.Length != rectangles.Count)
            {
                throw new ArgumentException("Count of rectangles not equal to sequence length!");
            }

            var sortedSource = new List<Rectangle>();

            for (int i = 0; i < sequence.Length; i++)
            {
                sortedSource.Add(rectangles[sequence[i]]);
            }

            int placed = 1;
            positions = new List<XYPoint>();
            positions.Add(new XYPoint());

            var currentLength = sortedSource[0].Width;

            for (int i = 1; i < sortedSource.Count; i++)
            {
                var possiblePositions = new List<XYPoint>();

                for (int j = 0; j < placed; j++)
                {
                    for (int k = j + 1; k < placed; k++)
                    {   
                        possiblePositions.AddRange(PlacementRelativeTwoRects(i, j, k, sortedSource, positions));
                    }
                    possiblePositions.AddRange(PlacementRelativeRectAndBottomBorder(i, j, bottomBorder, sortedSource, positions));
                    possiblePositions.AddRange(PlacementRelativeRectAndLeftBorder(i, j, leftBorder, sortedSource, positions));
                }
                FilterPossiblePos(i, possiblePositions, positions, sortedSource, topBorder, bottomBorder, leftBorder, placed);
                var best = SearchBestPos(sortedSource, positions, possiblePositions, i, currentLength, bottomBorder);

                positions.Add(best);
                placed++;

                float max = 0;
                for (int l = 0; l < placed; l++)
                {
                    float tmp = sortedSource[l].Width + positions[l].X;
                    if (tmp > max)
                    {
                        max = tmp;
                    }
                }
                currentLength = max;
            }

            return currentLength;
        }

        private static void FilterPossiblePos(int toPlace, List<XYPoint> possiblePos, List<XYPoint> positions,  List<Rectangle> rects, float topBorder, float bottomBorder, float leftBorder, int placed)
        {
            for (int i = 0; i < possiblePos.Count; i++)
            {
                // если выходит за границы полубесконечной полосы
                if (
                    (possiblePos[i].X < leftBorder)
                 || (possiblePos[i].Y < bottomBorder)
                 || (possiblePos[i].Y + rects[toPlace].Height > topBorder)
                )
                {
                    possiblePos.RemoveAt(i);
                    i--;
                    continue;
                }

                for (int j = 0; j < placed; j++) //если пересекается с любым из размещенных прямоугольников
                {
                    if (IsRectanglesIntersect(rects[toPlace], rects[j], possiblePos[i], positions[j]))
                    {
                        possiblePos.RemoveAt(i);
                        i--;
                        break;
                    }
                }
            }
        }
        
        private static XYPoint SearchBestPos(List<Rectangle> rects, List<XYPoint> positions, List<XYPoint> possiblePos, int toPlace, float currentLength, float bottomBorder)
        {
            float bestLength = currentLength + rects[toPlace].Width;
            XYPoint best = new XYPoint() { X = currentLength, Y = bottomBorder };

            foreach (var p in possiblePos)
            {
                var afterPlacing = p.X + rects[toPlace].Width > currentLength ? p.X + rects[toPlace].Width : currentLength;

                if (afterPlacing == bestLength)
                {
                    return p;
                }
                if (afterPlacing < bestLength)
                {
                    best = p;
                    bestLength = afterPlacing;
                }
            }
            return best;
        }

        public static List<XYPoint> PlacementRelativeRectAndLeftBorder(int toPlace, int other, float leftBorder, List<Rectangle> rectangles, List<XYPoint> positions)
        {
            List<XYPoint> possible = new List<XYPoint>();

            if (positions[other].X < rectangles[toPlace].Width)
            {
                possible.Add(new XYPoint() { X = leftBorder, Y = positions[other].Y + rectangles[other].Height });
            }
            return possible;
        }
     
        public static List<XYPoint> PlacementRelativeRectAndBottomBorder(int toPlace, int other, float bottomBorder, List<Rectangle> rects, List<XYPoint> positions)
        {
            List<XYPoint> possible = new List<XYPoint>();
            if (positions[other].Y < rects[toPlace].Height)
            {
                possible.Add(new XYPoint() { X = positions[other].X + rects[other].Width, Y = bottomBorder });
            }
            return possible;
        }
        
        public static List<XYPoint> PlacementRelativeTwoRects(int toPlace, int first, int second, List<Rectangle> rects, List<XYPoint> positions)
        {
            List<XYPoint> possible = new List<XYPoint>();

            if (positions[first].X <= positions[second].X &&
                positions[second].X < positions[first].X + rects[first].Width + rects[toPlace].Width &&
                positions[second].Y <= positions[first].Y &&
                positions[first].Y < positions[second].Y + rects[second].Height + rects[toPlace].Height &&
                positions[second].X + rects[second].Width > positions[first].X + rects[first].Width
                )
            {
                possible.Add(new XYPoint() { X = positions[first].X + rects[first].Width, Y = positions[second].Y + rects[second].Height });
            }
            if (positions[second].X <= positions[first].X &&
                positions[first].X < positions[second].X + rects[second].Width + rects[toPlace].Width &&
                positions[first].Y <= positions[second].Y &&
                positions[second].Y < positions[first].Y + rects[first].Height + rects[toPlace].Height &&
                positions[second].X + rects[second].Width < positions[first].X + rects[first].Width
                )
            {
                possible.Add(new XYPoint() { X = positions[second].X + rects[second].Width, Y = positions[first].Y + rects[first].Height });
            }

            return possible;
        }
        
        public static bool IsRectanglesIntersect(Rectangle first, Rectangle second, XYPoint firstPos, XYPoint secondPos)
        {
            return !(firstPos.X >= secondPos.X + second.Width
                || firstPos.X + first.Width <= secondPos.X
                || firstPos.Y >= secondPos.Y + second.Height
                || firstPos.Y + first.Height <= secondPos.Y);
        }
    }
}
