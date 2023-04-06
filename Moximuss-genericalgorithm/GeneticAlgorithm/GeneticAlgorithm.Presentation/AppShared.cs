using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Documents;
using GeneticAlgorithm.CirclesChromosome;

namespace GeneticAlgorithm.Presentation
{
    public delegate void DrawRectangles();
    public delegate void DrawCircles();
    public static class AppShared
    {
        public static RichTextBox TextBoxLog { get; } = new RichTextBox();
        public static void Log(string text = "")
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (TextBoxLog != null)
                    TextBoxLog.Document.Blocks.Add(new Paragraph(new Run(text)) { LineHeight = 1 });
            });
        }
        
        private static List<Circle> _cirlces;
        public static GeneticEngine<CirclesChromosome.CirclesChromosome> CircleEngine { get; set; } = null;
        public static DrawRectangles DrawRectanglesOnCanvas { get; set; } = null;
        public static DrawCircles DrawCirclesOnCanvas { get; set; } = null;
        public static bool IsNeedToPlaceRectangles { get; set; } = false;
  
        public static List<Circle> Circles
        {
            get
            {
                return _cirlces;
            }
            set
            {
                _cirlces = value;
                if (_cirlces is null || DrawCirclesOnCanvas is null)
                    return;
                DrawCirclesOnCanvas.Invoke();
            }
        }
        public static CirclesChromosome.CirclesChromosome CircleResult { get; set; }
    }
}
