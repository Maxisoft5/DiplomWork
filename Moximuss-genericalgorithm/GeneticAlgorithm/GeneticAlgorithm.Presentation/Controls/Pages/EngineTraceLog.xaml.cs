using System.Windows;
using System.Windows.Controls;

namespace GeneticAlgorithm.Presentation.Pages
{
    public partial class EngineTraceLog : Page
    {
        public EngineTraceLog()
        {
            InitializeComponent();
            
            AppShared.TextBoxLog.Margin = new Thickness() { Bottom = 10, Left = 10, Top = 10, Right = 10};
            gridEngineTraceLog.Children.Add(AppShared.TextBoxLog);
        }
    }
}
