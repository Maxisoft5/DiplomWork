using System.Windows;
using System.Windows.Controls;

namespace GeneticAlgorithm.Presentation.Controls
{
    /// <summary>
    /// Логика взаимодействия для ParameterBox.xaml
    /// </summary>
    public partial class ParameterBox : UserControl
    {
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register("ParameterTitle", typeof(string), typeof(ParameterBox));
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register("ParameterValue", typeof(string), typeof(ParameterBox));

        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }
        
        public string ParameterValue
        {
            get { return (string)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public ParameterBox()
        {
            InitializeComponent();
            this.DataContext = this;
        }
    }
}
