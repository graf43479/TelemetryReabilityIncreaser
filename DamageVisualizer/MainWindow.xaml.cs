﻿using System;
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
using TelemetryEngine.Interfaces;

namespace DamageVisualizer
{
    /// <summary>
    /// Логика работы основного окна
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path = "";        
        IEngine engine;                
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
            try
            {
                engine = new Engine(path);
                //ListBoxCombinations.ItemsSource = engine.GetFilteredCombinations().Select(x=>x.Name);            
                DataGridCombinations.ItemsSource = engine.GetFilteredCombinations();
                DataGridCombinations.Columns[0].Header = "№";
                DataGridCombinations.Columns[1].Header = "Каналы";
                DataGridCombinations.Columns[0].CanUserResize = false;
                DataGridCombinations.Columns[1].CanUserResize = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }            
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
                Task<List<MismatchesCoordList>> task = new Task<List<MismatchesCoordList>>(() =>
                {
                    List<MismatchesCoordList> allCoords = new List<MismatchesCoordList>();
                    RawDataMatrix result = engine.PerformCombination(val.ToString());
                    allCoords = engine.GetMatrixDifference();
                    MismatchesCoordList mainCoords = result.GetMismatches(engine.Etalon);
                    mainCoords.Gamma = engine.Gamma;
                    allCoords.Add(mainCoords);
                    return allCoords;
                });

                task.Start();

                UnitedFramesComponent comp = new UnitedFramesComponent(myCanvas, 10, 38, 38);
                comp.DrawData(task.Result);
            }
        }

        private void DataGridCombinations_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                HitTestResult hitTestResult = VisualTreeHelper.HitTest(DataGridCombinations, e.GetPosition(DataGridCombinations));
                DataGridRow dataGridRow = hitTestResult.VisualHit.GetParentOfType<DataGridRow>();
                if (dataGridRow != null)
                {
                    int index = dataGridRow.GetIndex();
                    string combination = ((Items)DataGridCombinations.Items[index]).Name;
                    int len = combination.Length;
                    string message = "";
                    for (int i = 1; i <= len; i++)
                    {
                        char ch = combination[i - 1];                        
                        string tmp = $"Канал №{i}. Интенс. помех: {ch}\t";
                        message += tmp;
                    }
                    txtBlockChannelInfo.Text = message.Substring(0, message.Length - 1);
                    //dgTooltip.Content = message.Substring(0, message.Length - 1);
                }
            }
            catch (Exception)
            {
            }
         }
    }

    public static class DataExtensions
    {
        public static T GetParentOfType<T>(this DependencyObject element) where T : DependencyObject
        {
            Type type = typeof(T);
            if (element == null) return null;
            DependencyObject parent = VisualTreeHelper.GetParent(element);
            if (parent == null && ((FrameworkElement)element).Parent is DependencyObject) parent = ((FrameworkElement)element).Parent;
            if (parent == null) return null;
            else if (parent.GetType() == type || parent.GetType().IsSubclassOf(type)) return parent as T;
            return GetParentOfType<T>(parent);
        }
    }

    
}
