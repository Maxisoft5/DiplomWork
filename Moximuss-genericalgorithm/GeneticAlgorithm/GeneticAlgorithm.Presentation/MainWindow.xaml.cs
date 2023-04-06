using GeneticAlgorithm.Presentation.Controls.Pages;
using GeneticAlgorithm.Presentation.Pages;
using GeneticAlgorithm.Presentation.Services;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace GeneticAlgorithm.Presentation
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool isMinimizedMenu = false;
        private Button activeMenuItem = null;
        Dictionary<Type, Page> pages = new Dictionary<Type, Page>();

        public MainWindow()
        {
            InitializeComponent();

            foreach (UIElement ui in menuBar.Children)
            {
                var asBtn = ui as Button;
                if (asBtn != null)
                    asBtn.Click += MenuItemBtn_Click;
            }
            menuCollapseBtn.Click -= MenuItemBtn_Click;
            MenuItemBtn_Click(settingsBtn, null);
            SettingsBtn_Click(settingsBtn, null);
        }

        private void MenuCollapseButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            menuIcon.RenderTransformOrigin = new Point(0.5, 0.5);

            var duration = TimeSpan.FromSeconds(0.2);

            double angle = 180;
            double trFrom = 45;
            double trTo = 165;

            if (!isMinimizedMenu)
            {
                trFrom = 165;
                trTo = 45;
                angle = 0;
            }

            RotateTransform transform = new RotateTransform()
            {
                CenterX = 0.5,
                CenterY = 0.5,
                Angle = angle + 180,
            };
            menuIcon.RenderTransform = transform;   

            DoubleAnimation animation = new DoubleAnimation()
            {
                From = angle,
                To = angle + 180,
                Duration = duration
            };

            DoubleAnimation menuAmination = new DoubleAnimation()
            {
                From = trFrom,
                To = trTo,
                Duration = duration
            };

            ThicknessAnimation thicknessAnimation = new ThicknessAnimation()
            {
                From = new Thickness() { Left = trFrom },
                To = new Thickness() { Left = trTo },
                Duration = duration
            };

            menuBar.BeginAnimation(StackPanel.WidthProperty, menuAmination);
            pageContainer.BeginAnimation(StackPanel.MarginProperty, thicknessAnimation);
            transform.BeginAnimation(RotateTransform.AngleProperty, animation);

            isMinimizedMenu = !isMinimizedMenu;
        }

        private void MenuItemBtn_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;

            if (btn != null)
            {
                if (activeMenuItem != null)
                    activeMenuItem.Background = (new BrushConverter().ConvertFrom("#cbd3ee") as Brush);

                btn.Background = new BrushConverter().ConvertFrom("#cbd3ee") as Brush;
                activeMenuItem = btn;
            }
        }

        private void PlacingRectangleResultBtn_Click(object sender, RoutedEventArgs e)
        {
            Title = "Placing minimize result";
            ChangePage<MinimizeCircles>();
        }

        private void PlacingCircleResultBtn_Click(object sender, RoutedEventArgs e)
        {
            Title = "Placing circle result";
            ChangePage<PlacedCircles>();
        }

        private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        {
            Title = "Genetic algorithm settings";
            ChangePage<GeneticEngine>();
        }

        private void TriangulationCirclesBtn_Click(object sender, RoutedEventArgs e)
        {
            Title = "Triangulation circles algorithm";
            ChangePage<CirclesTriangulation>();
        }

        private void EngineTraceLog_Click(object sender, RoutedEventArgs e)
        {
            Title = "Trace log";
            ChangePage<EngineTraceLog>();
        }

        private void ChangePage<T>() 
            where T : Page, new()
        {
            if (!pages.TryGetValue(typeof(T), out Page ui))
            {
                ui = new T();
                pages.Add(typeof(T), ui);
            }
            pageContainer.Content = ui;
        }

        private void pageContainer_Navigated(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void pageContainer_Navigated_1(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }
    }
}
