using System;
using System.Collections.Generic;
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
using GeneticAlgorithm.CirclesChromosome;
using GeneticAlgorithm.Presentation.Services;

namespace GeneticAlgorithm.Presentation.Pages
{
    /// <summary>
    /// Interaction logic for PlacedCircles.xaml
    /// </summary>
    public partial class PlacedCircles : Page
    {
        public double Scale = 1;
        private const float _mainCircleX = 1200;
        private const float _mainCircleY = 500;
        public PlacedCircles()
        {
            InitializeComponent();
            CirclePlacingService.drawCircle += DrawCircle;
            CirclePlacingService.writeMessage += Message;
            CirclePlacingService.replaceEllipse += RemoveEllipse;
            RunAsync();
        }

        private void Message(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                messageInfo.Content = message;
            });
        }

        private void RemoveEllipse(Ellipse ellipse)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                placingCirclesCanvas.Children.Remove(ellipse);
            });
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

                Canvas.SetTop(eliplse, (_mainCircleY / 4) + (circle.Y - circle.Radius) * Scale);
                Canvas.SetLeft(eliplse, (_mainCircleX / 4) + (circle.X - circle.Radius) * Scale);
                Canvas.SetBottom(eliplse, (_mainCircleY / 4) + (circle.Y - circle.Radius) * Scale);
                Canvas.SetRight(eliplse, (_mainCircleX / 4) + (circle.X - circle.Radius) * Scale);

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

                Canvas.SetLeft(eliplse, _mainCircleX / 4 - 10);
                Canvas.SetTop(eliplse, _mainCircleY / 4 + 20);

                Application.Current.Dispatcher.Invoke(() =>
                {
                    placingCirclesCanvas.Children.Add(eliplse);
                });

            });
            return eliplse;
        }

        private async void RunAsync()
        {
            var circles = CirclesChromosome.CirclesChromosome.Resource;
            if (CirclesChromosome.CirclesChromosome.Resource == null)
            {
                circles = CirclesDataService.GetRandomCircles(100, 10, 50);
            }
            DrawMainCircle(new Circle()
            {
                Radius = 320
            }, Brushes.Black);
            await CirclePlacingService.CirclesPlacing(circles, _mainCircleY, _mainCircleX);
        }
    }
}
