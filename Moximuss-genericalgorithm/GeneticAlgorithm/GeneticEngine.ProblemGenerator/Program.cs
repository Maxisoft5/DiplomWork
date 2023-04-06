using System;
using System.Collections.Generic;
using System.IO;

namespace GeneticEngine.ProblemGenerator
{
    class Rectangle
    {
        public double Height { get; set; }
        public double Width { get; set; }
    }

    class Program
    {
        public static List<Rectangle> rectangles = new List<Rectangle>();
        public static int limit = 0;
        public static double minHeight = 20;
        public static double minWidth = 20;
        private static readonly Random random = new Random();
        static void Main(string[] args)
        {
            Console.Write("Height: ");
            double height = double.Parse(Console.ReadLine());
            Console.Write("Width: ");
            double width = double.Parse(Console.ReadLine());
            Console.Write("Limit: ");
            limit = int.Parse(Console.ReadLine());
            limit--;

            int layer = 2;

            Rectangle mainRect = new Rectangle() { Height = height, Width = width };
 
            RndSeparateNext(mainRect, layer);

            while (rectangles.Count < limit)
            {
                int rndIndex = random.Next(0, rectangles.Count - 1);
                var tmp = rectangles[rndIndex];
                rectangles.RemoveAt(rndIndex);
                RndSeparateNext(tmp, 4);
            }
            using (StreamWriter writer = new StreamWriter("result.txt"))
            {
                foreach (var rect in rectangles)
                    writer.WriteLine($"{rect.Width} {rect.Height};");
            }
        }

        public static void RndSeparateNext(Rectangle rect, int layer)
        {
            bool tooSmall = false;
            if (rect.Height < minHeight || rect.Width < minWidth)
            {
                tooSmall = true;
            }
            if (!tooSmall && random.Next() % (int)(layer * 1.25) == 0 && rectangles.Count < limit)
            {
                int separateRnd = random.Next() % 3;
                if (limit - rectangles.Count == 1)
                {
                    separateRnd = 0;
                }
                if (limit - rectangles.Count == 2)
                {
                    separateRnd = 1;
                }
                switch (separateRnd)
                {
                    case 0: 
                        SeparateToTwoRect(rect, layer + 1); 
                        break;
                    case 1: 
                        SeparateToTheeRect(rect, layer + 1); 
                        break;
                    case 2: 
                        SeparateToFourRect(rect, layer + 1); 
                        break;
                }
            }
            else
            {
                rectangles.Add(rect);
            }
        }

        public static void SeparateToTwoRect(Rectangle rectangle, int layer)
        {
            RndHeightWidth(rectangle, out double rndHeight, out double rndWidth);

            bool isHorisontalRects = random.Next() % 2 == 0;

            Rectangle first = new Rectangle();
            Rectangle second = new Rectangle();
            if (isHorisontalRects)
            {
                first.Height = rndHeight;
                first.Width = rectangle.Width;

                second.Height = rectangle.Height - first.Height;
                second.Width = rectangle.Width;
            }
            else 
            {
                first.Width = rndWidth;
                first.Height = rectangle.Height;

                second.Width = rectangle.Width - first.Width;
                second.Height = rectangle.Height;
            }

            RndSeparateNext(first, layer);
            RndSeparateNext(second, layer);
        }
        
        public static void SeparateToTheeRect(Rectangle rectangle, int layer)
        {
            RndHeightWidth(rectangle, out double rndHeight, out double rndWidth);

            bool isHorisontal = random.Next() % 2 == 0;
            bool isTopOrRight = random.Next() % 2 == 0;

            Rectangle first = new Rectangle();
            Rectangle second = new Rectangle();
            Rectangle third = new Rectangle();

            if (isHorisontal && isTopOrRight)
            {
                first.Height = rndHeight;
                first.Width = rectangle.Width;

                second.Height = rectangle.Height - rndHeight;
                second.Width = rndWidth;

                third.Height = second.Height;
                third.Width = rectangle.Width - rndWidth;
            }
            if (isHorisontal && !isTopOrRight)
            {
                first.Height = rectangle.Height - rndHeight;
                first.Width = rectangle.Width;

                second.Height = rndHeight;
                second.Width = rndWidth;

                third.Height = second.Height;
                third.Width = rectangle.Width - rndWidth;
            }
            if (!isHorisontal && isTopOrRight)
            {
                first.Height = rectangle.Height;
                first.Width = rectangle.Width - rndWidth;

                second.Height = rndHeight;
                second.Width = rndWidth;

                third.Height = rectangle.Height - rndHeight;
                third.Width = rectangle.Width;
            }
            if (!isHorisontal && !isTopOrRight)
            {
                first.Height = rectangle.Height;
                first.Width = rndWidth;

                second.Height = rndHeight;
                second.Width = rectangle.Width - rndWidth;

                third.Height = rectangle.Height - rndHeight;
                third.Width = second.Width;
            }

            RndSeparateNext(first, layer);
            RndSeparateNext(second, layer);
            RndSeparateNext(third, layer);
        }

        public static void SeparateToFourRect(Rectangle rectangle, int layer)
        {
            RndHeightWidth(rectangle, out double rndHeight, out double rndWidth);

            Rectangle first = new Rectangle();
            Rectangle second = new Rectangle();
            Rectangle third = new Rectangle();
            Rectangle fourth = new Rectangle();

            first.Height = rndHeight;
            first.Width = rndWidth;

            second.Height = rndHeight;
            second.Width = rectangle.Width - rndWidth;

            third.Height = rectangle.Height - rndHeight;
            third.Width = rndWidth;

            fourth.Height = rectangle.Height - rndHeight;
            fourth.Width = rectangle.Width - rndWidth;

            RndSeparateNext(first, layer);
            RndSeparateNext(second, layer);
            RndSeparateNext(third, layer);
            RndSeparateNext(fourth, layer);
        }

        public static void RndHeightWidth(Rectangle rectangle, out double rndHeight, out double rndWidth)
        {
            rndWidth = rectangle.Width * Math.Abs(random.NextDouble() - 0.5);
            rndHeight = rectangle.Height * Math.Abs(random.NextDouble() - 0.5);
        }
    }
}
