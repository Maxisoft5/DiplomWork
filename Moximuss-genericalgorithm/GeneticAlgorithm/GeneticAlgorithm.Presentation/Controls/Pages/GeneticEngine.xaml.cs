using GeneticAlgorithm.CirclesChromosome;
using GeneticAlgorithm.Interfaces;
using GeneticAlgorithm.Presentation.Controls;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace GeneticAlgorithm.Presentation.Pages
{
    public partial class GeneticEngine : Page
    {
        private GeneticEngineBuilder<CirclesChromosome.CirclesChromosome> geneticCircleBuilder = new GeneticEngineBuilder<CirclesChromosome.CirclesChromosome>();

        private CancellationTokenSource metricCancellation = new CancellationTokenSource();
        public GeneticEngine()
        {
            InitializeComponent();

            var allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(asm => asm.GetTypes());

            var gentypes = typeof(GeneticEngine<>).Assembly.GetTypes();

            var selectors = SelectWhereImplementInterface(gentypes, typeof(ISelector<>));
            var crossovers = SelectWhereImplementInterface(gentypes, typeof(ICrossover<>));
            var mutators = SelectWhereImplementInterface(gentypes, typeof(IMutator<>));
            var parentSelectors = SelectWhereImplementInterface(gentypes, typeof(IParentsSelector<>));

            selectorDropdown.DisplayMemberPath = "Value";
            foreach (var item in selectors)
                selectorDropdown.Items.Add(item);

            crossoverDropdown.DisplayMemberPath = "Value";
            foreach (var item in crossovers)
                crossoverDropdown.Items.Add(item);

            mutatorDropdown.DisplayMemberPath = "Value";
            foreach (var item in mutators)
                mutatorDropdown.Items.Add(item);

            parentSelectorDropdown.DisplayMemberPath = "Value";
            foreach (var item in parentSelectors)
                parentSelectorDropdown.Items.Add(item);

            ChangeParamGridsEnabledState(true);
        }
        private Dictionary<Type, string> SelectWhereImplementInterface(IEnumerable<Type> source, Type type)
        {
            return source.Where(t => t.IsClass
                    && t.IsGenericType
                    && t.GetGenericTypeDefinition().GetInterfaces()
            .Where(i => i.IsGenericType
                    && i.GetGenericTypeDefinition() == type.GetGenericTypeDefinition()).Count() > 0)
                .ToDictionary(t => t, t => TranslateName(t.Name));
        }

        private void SelectorDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            UpdatePropertiesWrapper(comboBox, selectorParamsGrid, selectorParamsWrapper);

            var type = ((KeyValuePair<Type, string>)comboBox.SelectedItem).Key;
            var readyCircleType = type.MakeGenericType(typeof(CirclesChromosome.CirclesChromosome));
            geneticCircleBuilder.UseSelector((ISelector<CirclesChromosome.CirclesChromosome>)Activator.CreateInstance(readyCircleType));
        }
        private void CrossoverDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            UpdatePropertiesWrapper(comboBox, crossoverParamsGrid, crossoverParamsWrapper);

            var type = ((KeyValuePair<Type, string>)comboBox.SelectedItem).Key;
            var readyCircleType = type.MakeGenericType(typeof(GeneticAlgorithm.CirclesChromosome.CirclesChromosome));
            geneticCircleBuilder.UseCrossover((ICrossover<CirclesChromosome.CirclesChromosome>)Activator.CreateInstance(readyCircleType));
        }
        private void ParentSelectorDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            UpdatePropertiesWrapper(comboBox, parentSelectorParamsGrid, parentSelectorParamsWrapper);
            var type = ((KeyValuePair<Type, string>)comboBox.SelectedItem).Key;
            var readyCircleType = type.MakeGenericType(typeof(CirclesChromosome.CirclesChromosome));

            geneticCircleBuilder.UseParentsSelector((IParentsSelector<CirclesChromosome.CirclesChromosome>)Activator.CreateInstance(readyCircleType));
        }
        private void MutatorDropdown_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = sender as ComboBox;

            UpdatePropertiesWrapper(comboBox, mutatorParamsGrid, mutatorParamsWrapper);
            var type = ((KeyValuePair<Type, string>)comboBox.SelectedItem).Key;
            var readyCircleType = type.MakeGenericType(typeof(CirclesChromosome.CirclesChromosome));

            geneticCircleBuilder.UseMutator((IMutator<CirclesChromosome.CirclesChromosome>)Activator.CreateInstance(readyCircleType));
        }
        private void UpdatePropertiesWrapper(ComboBox comboBox, Grid grid, WrapPanel wrap)
        {
            if (comboBox == null || comboBox.SelectedItem == null)
            {
                return;
            }

            Type type = ((KeyValuePair<Type, string>)comboBox.SelectedItem).Key ?? null;

            if (type == null)
            {
                return;
            }
            grid.Visibility = Visibility.Collapsed;

            PropertyInfo[] properties = type.GetProperties();

            if (properties.Length > 0)
                grid.Visibility = Visibility.Visible;

            wrap.Children.Clear();
            foreach (var prop in properties)
            {
                wrap.Children.Add(new ParameterBox() { Title = prop.Name });
            }
        }
        private string TranslateName(string className)
        {
            int index = className.IndexOf('`');
            if (index != -1)
            {
                className = className.Remove(index);
            }

            for (int i = 1; i < className.Length; i++)
            {
                if (char.IsUpper(className[i]))
                {
                    className = className.Replace(className[i].ToString(), $" {char.ToLower(className[i])}");
                }
            }

            return className;
        }
        private void SetPropertyValue(object obj, string propertyName, object value)
        {
            try
            {
                var propInfo = obj.GetType().GetProperty(propertyName);
                propInfo.SetValue(obj, Convert.ChangeType(value, propInfo.PropertyType));
            }
            catch (Exception ex)
            {
                AppShared.Log($"Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            }
        }
        private void ResetOperatorParameters(UIElementCollection collection)
        {
            try
            {
                foreach (var element in collection)
                {
                    var param = element as ParameterBox;
                    if (!(param is null))
                    {
                        param.ParameterValue = "";
                    }
                }
            }
            catch (Exception ex)
            {
                AppShared.Log($"ResetOperatorParameters. Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            }
        }
        private void SelectorParamsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var child in selectorParamsWrapper.Children)
                {
                    var param = child as ParameterBox;

                    SetPropertyValue(geneticCircleBuilder.Selector, param.Title, param.ParameterValue);
                }
            }
            catch (Exception ex)
            {
                AppShared.Log($"SelectorParamsApply. Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            }
        }
        private void ParentsSelectorParamsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var child in parentSelectorParamsWrapper.Children)
                {
                    var param = child as ParameterBox;
                    SetPropertyValue(geneticCircleBuilder.ParentsSelector, param.Title, param.ParameterValue);
                }
            }
            catch (Exception ex)
            {
                AppShared.Log($"ParentsSelectorParamsApply. Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            }
        }
        private void CrossoverParamsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var child in crossoverParamsWrapper.Children)
                {
                    var param = child as ParameterBox;
                    SetPropertyValue(geneticCircleBuilder.ParentsSelector, param.Title, param.ParameterValue);
                }
            }
            catch (Exception ex)
            {
                AppShared.Log($"CrossoverParamsApply. Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            }
        }
        private void MutatorParamsApplyButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                foreach (var child in crossoverParamsWrapper.Children)
                {
                    var param = child as ParameterBox;
                    SetPropertyValue(geneticCircleBuilder.ParentsSelector, param.Title, param.ParameterValue);
                }
            }
            catch (Exception ex)
            {
                AppShared.Log($"MutatorParamsApply. Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            }
        }
        private void SelectorParamsResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetOperatorParameters(selectorParamsWrapper.Children);
        }
        private void ParentsSelectorParamsResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetOperatorParameters(parentSelectorParamsWrapper.Children);
        }
        private void CrossoverParamsResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetOperatorParameters(crossoverParamsWrapper.Children);
        }
        private void MutatorParamsResetButton_Click(object sender, RoutedEventArgs e)
        {
            ResetOperatorParameters(mutatorParamsWrapper.Children);
        }
        private void EngineParamsResetButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                populationSizeTextbox.Text = "";
                remainCountTextbox.Text = "";
                countOfIterationsTextbox.Text = "";
                mutationPercentSlider.Value = 0;
                crossoverDropdown.SelectedIndex = -1;
                mutatorDropdown.SelectedIndex = -1;
                parentSelectorDropdown.SelectedIndex = -1;
                selectorDropdown.SelectedIndex = -1;

                AppShared.Log("Engine params reset.");
            }
            catch (Exception ex)
            {
                AppShared.Log($"Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Exception", MessageBoxButton.OK);
            }
        }
        private void ChangeParamGridsEnabledState(bool isEnabled)
        {
            engineParamsGrid.IsEnabled = isEnabled;
            selectorParamsGrid.IsEnabled = isEnabled;
            parentSelectorParamsGrid.IsEnabled = isEnabled;
            crossoverParamsGrid.IsEnabled = isEnabled;
            mutatorParamsGrid.IsEnabled = isEnabled;
            chromosomesParameters.IsEnabled = isEnabled;
        }
        private void RunButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
           
        }
        private void Rectagnels_Apply(object sender, RoutedEventArgs e)
        {
          

        }
        private void EngineParamsApplyButton_Click(object sender, RoutedEventArgs e)
        {

        }
        private async void ChromosomesFromFileButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = null;

                if (ShowFileDialog(out path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string tmp = await reader.ReadToEndAsync();
                        var spl = tmp.Split(':');
                        if (spl.Length > 1)
                        {
                        }
                        else
                        {
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppShared.Log($"Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }
        private void ChromosomesParamsSaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = null;

                if (ShowFileDialog(out path))
                {
                    using (StreamWriter writer = new StreamWriter(path))
                    {
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }
        private bool ShowFileDialog(out string filePath)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.FileName = string.Empty;
            dlg.DefaultExt = ".txt";
            dlg.Filter = "Text documents (.txt)|*.txt";

            bool? result = dlg.ShowDialog();

            if (result != null && result == true)
            {
                filePath = dlg.FileName;
                return result.Value;
            }

            filePath = string.Empty;
            return false;
        }
        private void RectHeightApply_Click(object sender, RoutedEventArgs e)
        {
        }

        private void mutationPercentSlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            mutationPercentLabel.Content = $"Mutation: {(int)e.NewValue}%";
        }

        private void Сircles_Apply(object sender, RoutedEventArgs e)
        {
            try
            {
                string[] circles = chromosomesSourceTextBox2.Text
                        .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                        .Where(x => !string.IsNullOrWhiteSpace(x))
                        .ToArray();

                CirclesChromosome.CirclesChromosome.Resource = new List<Circle>();
                foreach (string radius in circles)
                {
                    if (string.IsNullOrEmpty(radius.Trim()))
                    {
                        continue;
                    }
                    CirclesChromosome.CirclesChromosome.Resource.Add(new Circle()
                    {
                        Radius = float.Parse(radius)
                    });
                }

                AppShared.Log($"Message: Circle's source applied. Count: {CirclesChromosome.CirclesChromosome.Resource.Count}.");
                MessageBox.Show("Source applied.", "Success", MessageBoxButton.OK);
            }
            catch (Exception ex)
            {
                AppShared.Log($"Source Parsing Exception: {ex.Message}");
                MessageBox.Show($"Wrong data format.", "Parsing error", MessageBoxButton.OK);
            }
        }

        private async void ReadFromFileCircles(object sender, RoutedEventArgs e)
        {
            try
            {
                string path = null;

                if (ShowFileDialog(out path))
                {
                    using (StreamReader reader = new StreamReader(path))
                    {
                        string tmp = await reader.ReadToEndAsync();
                        var spl = tmp.Split(':');
                        if (spl.Length > 1)
                        {
                            chromosomesSourceTextBox2.Text = spl[1].Trim();
                        }
                        else
                        {
                            chromosomesSourceTextBox2.Text = spl[0];
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppShared.Log($"Exception: {ex.Message}");
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK);
            }
        }
    }
}

