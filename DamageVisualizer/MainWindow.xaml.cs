using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
using TelemetryEngine;

namespace DamageVisualizer
{
    /// <summary>
    /// Логика работы основного окна
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path = "";        
        Engine engine;                
        public MainWindow()
        {
            InitializeComponent();
            path = Directory.GetCurrentDirectory() + "/JsonData";
        }

        /// <summary>
        /// Генерация исходных данных
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async ()=> await GenerateDataAsync());            
        }

        /// <summary>
        /// Расчет всех возможных комбинаций
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnCalc_Click(object sender, RoutedEventArgs e)
        {
            engine = new Engine(path);
            //ListBoxCombinations.ItemsSource = engine.GetFilteredCombinations().Select(x=>x.Name);            
            DataGridCombinations.ItemsSource = engine.GetFilteredCombinations();
            DataGridCombinations.Columns[0].Header = "№";
            DataGridCombinations.Columns[1].Header = "Каналы";
            DataGridCombinations.Columns[0].CanUserResize = false;
            DataGridCombinations.Columns[1].CanUserResize = false;
        }

        /// <summary>
        /// Генерация исходных матриц "из коробки"
        /// </summary>
        /// <returns></returns>
        private async Task GenerateDataAsync()
        {
            await MatrixDataInitializer.GenerateAsync();
        }

        /// <summary>
        /// Действие на изменение комбинации матриц
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridCombinations_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var val = (DataGridCombinations.SelectedItem as Items).Name;
            if (engine != null & val != null)
            {
                RawDataMatrix result = engine.PerformCombination(val.ToString());
                UnitedFramesComponent comp = new UnitedFramesComponent(myCanvas, 10, 10, 30);

                List<MismatchesCoordList> allCoords = engine.GetMatrixDifference();
                MismatchesCoordList mainCoords = result.GetMismatches(engine.Etalon);
                allCoords.Add(mainCoords);
                comp.DrawData(allCoords);
            }
        }

        private void DataGridCombinations_MouseMove(object sender, MouseEventArgs e)
        {
            //TODO: Подсветка подсказки
            /*
            DataGrid dt = sender as DataGrid;
            //if (dt.CurrentCell != null)
            if(e.Source!=null)
            {
                dgTooltip.Content = "Новые данные";
            }
            var val = (DataGridCombinations.SelectedItem as Items).Name;
            */
        }
    }
}
