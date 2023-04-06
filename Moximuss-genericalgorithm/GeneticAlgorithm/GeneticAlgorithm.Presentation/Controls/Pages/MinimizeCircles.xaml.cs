using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CenterSpace.NMath.Analysis;
using CenterSpace.NMath.Core;
using GeneticAlgorithm.CirclesChromosome;
using NumSharp;

namespace GeneticAlgorithm.Presentation.Controls.Pages
{
    /// <summary>
    /// Interaction logic for MinimizeCircles.xaml
    /// </summary>
    public partial class MinimizeCircles : Page
    {
        public double Scale = 1;
        public MinimizeCircles()
        {
            InitializeComponent();
            DrawMainCircle(new Circle()
            {
                Radius = 320,
                X = (float)(placingCirclesCanvas.Width / 2),
                Y = (float)(placingCirclesCanvas.Height / 2)
            });
            StartCirclesInitialize();
        }

        private void StartCirclesInitialize()
        {
            var xyz1 = new List<XYZPoint>()
            {
                new XYZPoint()
                {
                    X = 300,
                    Y = 200
                },
                new XYZPoint() 
                {
                    X = 190,
                    Y = 200
                },
                new XYZPoint()
                {
                    X = 220,
                    Y = 200
                },
                new XYZPoint()
                {
                    X = 180,
                    Y = 300
                },
                new XYZPoint()
                {
                    X = 230,
                    Y = 300
                },
                new XYZPoint()
                {
                    X = 250,
                    Y = 300
                },
                new XYZPoint()
                {
                    X = 290,
                    Y = 400
                },
                new XYZPoint()
                {
                    X = 50,
                    Y = 400
                },
                new XYZPoint()
                {
                    X = 150,
                    Y = 400
                },
                new XYZPoint()
                {
                    X = 120,
                    Y = 450
                },
                new XYZPoint()
                {
                    X = 90,
                    Y = 450
                },
                new XYZPoint()
                {
                    X = 60,
                    Y = 450
                },
            };

            var circles = CirclesChromosome.CirclesChromosome.Resource;

            if (circles.Count == 0)
            {
                MessageBox.Show("Radiuses list is emty");
            }

            for (int i = 0; i < circles.Count; i++)
            {
                DrawCircle(new Circle() { Radius = circles[i].Radius, X = xyz1[i].X, Y = xyz1[i].Y }, Brushes.Black);
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            var radiuses = CirclesChromosome.CirclesChromosome.Resource;
            var minRadius = radiuses.OrderBy(x => x.Radius).First();
            placingCirclesCanvas.Children.Clear();

            DrawMainCircle(new Circle()
            {
                Radius = 320,
            });

            var xyz2 = new List<XYZPoint>();

            if (minRadius.Radius > 40)
            {
                xyz2.AddRange(new List<XYZPoint>() {
                    new XYZPoint()
                    {
                        X = 190,
                        Y = 200
                    },
                    new XYZPoint()
                    {
                        X = 190,
                        Y = 280
                    },
                    new XYZPoint()
                    {
                        X = 190,
                        Y = 340
                    },
                    new XYZPoint()
                    {
                        X = 300,
                        Y = 200
                    },
                    new XYZPoint()
                    {
                        X = 300,
                        Y = 280
                    },
                    new XYZPoint()
                    {
                        X = 300,
                        Y = 370
                    },
                    new XYZPoint()
                    {
                        X = 400,
                        Y = 200
                    },
                    new XYZPoint()
                    {
                        X = 400,
                        Y = 280
                    },
                    new XYZPoint()
                    {
                        X = 400,
                        Y = 370
                    },
                    new XYZPoint()
                    {
                        X = 600,
                        Y = 200
                    },
                    new XYZPoint()
                    {
                        X = 600,
                        Y = 280
                    },
                    new XYZPoint()
                    {
                        X = 630,
                        Y = 390
                    } });
            }
            else
            {
                xyz2.AddRange(new List<XYZPoint>() {
                  new XYZPoint()
                    {
                        X = 250,
                        Y = 250
                    },
                    new XYZPoint()
                    {
                        X = 250,
                        Y = 260
                    },
                    new XYZPoint()
                    {
                        X = 250,
                        Y = 320
                    },
                    new XYZPoint()
                    {
                        X = 300,
                        Y = 220
                    },
                    new XYZPoint()
                    {
                        X = 300,
                        Y = 300
                    },
                    new XYZPoint()
                    {
                        X = 300,
                        Y = 320
                    },
                    new XYZPoint()
                    {
                        X = 350,
                        Y = 220
                    },
                    new XYZPoint()
                    {
                        X = 350,
                        Y = 300
                    },
                    new XYZPoint()
                    {
                        X = 350,
                        Y = 340
                    } 
                });
                  
            }

            var points = new List<float>();
            var radius = new List<float>();

            for (int i = 0; i < xyz2.Count; i++)
            {
                if (i % 2 == 0)
                {
                    points.Add(xyz2[i].X);
                }
                else
                {
                    points.Add(xyz2[i].Y);
                }
            }

            for(int i = 0; i < radiuses.Count; i++)
            {
                radius.Add(radiuses[i].Radius);
            }

            Minimize(xyz2, radiuses);
        }

        private double CalculateSquare(double radius1, double radius2, XYZPoint point1, XYZPoint point2)
        {
            double distance = Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));

            if (distance != 0)
            {
                double x1 = Math.Pow(radius1, 2) - Math.Pow(radius2, 2) + Math.Pow(distance, 2);
                double y1 = 2 * radius1 * distance;
                double x2 = Math.Pow(radius2, 2) - Math.Pow(radius1, 2) + Math.Pow(distance, 2);
                double y2 = 2 * radius2 * distance;
                double intersectionPoint1 = 2 * Math.Abs(Math.Acos(x1 / y1));
                double intersectionPoint2 = 2 * Math.Acos(x2 / y2);
                if (intersectionPoint1 != 0 && intersectionPoint2 != 0)
                {
                    double elipseSquare1 = (Math.Pow(radius1, 2) * (intersectionPoint1 - Math.Sin(intersectionPoint1))) / 2;
                    double elipseSquare2 = (Math.Pow(radius2, 2) * (intersectionPoint2 - Math.Sin(intersectionPoint2))) / 2;
                    double intersectionSquare = elipseSquare1 + elipseSquare2;
                    return intersectionSquare;
                }
                else
                {
                    return 0;
                }
            }
            else
            {
                return 0;
            }
        }

        private void Minimize(List<XYZPoint> xyzPoints, List<Circle> circles)
        {
            float r_out = 3.5f;
            float size = r_out + 1;

            // Create a multi-variable function.
            var function = new MultiVariableFunction(new Func<DoubleVector, double>(f));

            var arr = new List<double>();
            var arr2 = new List<double>();
            var arr3 = new List<double>();

            for (int i = 0; i < xyzPoints.Count; i++)
            {
                arr.Add(xyzPoints[i].X);
                arr2.Add(xyzPoints[i].Y);
            }

            for (int i = 0; i < arr.Count; i++)
            {
                arr3.Add(arr2[i]);
                arr3.Add(arr3[i]);
            }

          //  var radiuses = new DoubleVector(circles.Select(x => (double)x.Radius).ToArray());
         //   var xyz = new DoubleVector(arr3.ToArray());

            // Create a DownhillSimplexMinimizer with the default error tolerance and maximum
            // iterations.
            var simplex = new DownhillSimplexMinimizer();

            // Minimize the function.
         //   DoubleVector min = simplex.Minimize(function, xyz);

            // Create an array of partial derivatives
            var partials = new MultiVariableFunction[4];
            partials[0] = new MultiVariableFunction(new Func<DoubleVector, double>(df0));
            partials[1] = new MultiVariableFunction(new Func<DoubleVector, double>(df1));
            partials[2] = new MultiVariableFunction(new Func<DoubleVector, double>(df2));
            partials[3] = new MultiVariableFunction(new Func<DoubleVector, double>(df3));

            // Create the minimizer with default tolerance and default maximum iterations.
            var cg = new ConjugateGradientMinimizer();
            var minCircles = new List<Circle>();
         //   var minXyz = cg.Minimize(function, partials, xyz);
//            var minRadiuses = cg.Minimize(function, partials, radiuses);

            //for (int i = 0; i < minXyz.Length / 2; i++)
            //{
            //    var circle = new Circle();
            //    if (i % 2 == 0)
            //    {
            //        circle.X = (float)minXyz[i];
            //    }
            //    else
            //    {
            //        circle.Y = (float)minXyz[i];
            //    }
            //    minCircles.Add(circle);
            //}

            for (int i = 0; i < circles.Count; i++)
            {
                DrawCircle(new Circle()
                {
                    Radius = circles[i].Radius,
                    X = xyzPoints[i].X,
                    Y = xyzPoints[i].Y
                });
            }

        }
        private static double df0(DoubleVector x)
        {
            return 1.6 * Math.Pow(x[0], 3);
        }

        private static double df1(DoubleVector x)
        {
            return 0.1 * Math.Cos(x[1]);
        }

        private static double df2(DoubleVector x)
        {
            return 10 * x[2];
        }

        private static double df3(DoubleVector x)
        {
            return 16 * Math.Pow(x[3], 3);
        }

        private static double f(DoubleVector x)
        {
            return 0.4 * Math.Pow(x[0], 4) + 0.1 * Math.Sin(x[1]) + 5 * x[2] * x[2] + 4 * Math.Pow(x[3], 4);
        }

        private Ellipse DrawCircle(Circle circle, Brush brush = null)
        {
            Ellipse eliplse = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (brush == null)
                    brush = Brushes.Black;
                eliplse = new Ellipse();
                eliplse.Stroke = brush;
                eliplse.Height = circle.Radius * 2.0 * Scale;
                eliplse.Width = eliplse.Height;

                Canvas.SetTop(eliplse, (circle.Y - circle.Radius) * Scale);
                Canvas.SetLeft(eliplse, (circle.X - circle.Radius) * Scale);
                Canvas.SetBottom(eliplse, (circle.Y - circle.Radius) * Scale);
                Canvas.SetRight(eliplse, (circle.X - circle.Radius) * Scale);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    placingCirclesCanvas.Children.Add(eliplse);
                });
            });
            return eliplse;
        }

        private Ellipse DrawMainCircle(Circle circle, Brush brush = null)
        {
            Ellipse eliplse = null;
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (brush == null)
                    brush = Brushes.Black;
                eliplse = new Ellipse();
                eliplse.Stroke = brush;
                eliplse.Height = circle.Radius;
                eliplse.Width = eliplse.Height;

                Canvas.SetLeft(eliplse, placingCirclesCanvas.Width / 4);
                Canvas.SetTop(eliplse, placingCirclesCanvas.Height / 4);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    placingCirclesCanvas.Children.Add(eliplse);
                });

            });
            return eliplse;
        }
    }
}
