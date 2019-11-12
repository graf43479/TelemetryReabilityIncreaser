using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private string path = "";
        Engine engine;
        static WriteableBitmap writeableBitmap;
        static Image i;
        public MainWindow()
        {
            InitializeComponent();
            InitializeBitmapOptions();
            path = Directory.GetCurrentDirectory() + "/JsonData";

        }

        private void BtnGenerate_Click(object sender, RoutedEventArgs e)
        {
            Task.Run(async ()=> await GenerateDataAsync());
            
        }

        private void BtnCalc_Click(object sender, RoutedEventArgs e)
        {
            engine = new Engine(path);
             IEnumerable<string> combinations = engine.GetFilteredCombinations();
            ListBoxCombinations.ItemsSource = combinations;
        }


        private async Task GenerateDataAsync()
        {
            await MatrixDataInitializer.GenerateAsync();
        }

        private void ListBoxCombinations_GotFocus(object sender, RoutedEventArgs e)
        {
            
            var val = ListBoxCombinations.SelectedItem;
            if (engine != null & val!=null)
            {
                RawDataMatrix result = engine.PerformCombination(val.ToString());
                int count = 0;
                foreach(var mList in engine.GetMatrixDifference())
                {
                    
                    //отрисовка пикселей для каждой матрицы
                    //myCanvas.ClearValue();
                    foreach (Coord coord in mList)
                    {
                        DrawPixel(new Coord(coord.X, coord.Y + count));
                    }
                    count += 50;
                }

                foreach (Coord coord in result.GetMismatches(engine.Etalon))
                {
                    DrawPixel(new Coord(coord.X, coord.Y + count));
                }
                
            }
            //MessageBox.Show("Hello");
        }

        private void InitializeBitmapOptions()
        {
            i = img;
            RenderOptions.SetBitmapScalingMode(i, BitmapScalingMode.NearestNeighbor);
            RenderOptions.SetEdgeMode(i, EdgeMode.Aliased);
            img.DataContext = i;

            writeableBitmap = new WriteableBitmap(
             //   32,
             //   20,
            (int)img.Width,
            (int)img.Height,
            96,
            96,
            PixelFormats.Bgr32,
            null);

            i.Source = writeableBitmap;

            i.Stretch = Stretch.None;
            i.HorizontalAlignment = HorizontalAlignment.Left;
            i.VerticalAlignment = VerticalAlignment.Top;

        }

        static void DrawPixel(Coord coord)
        {
            int column = coord.Y;
            int row = coord.X;

            try
            {
                // Reserve the back buffer for updates.
                writeableBitmap.Lock();

                unsafe
                {
                    // Get a pointer to the back buffer.
                    IntPtr pBackBuffer = writeableBitmap.BackBuffer;

                    // Find the address of the pixel to draw.
                    pBackBuffer += row * writeableBitmap.BackBufferStride;
                    pBackBuffer += column * 4;

                    // Compute the pixel's color.
                    int color_data = 255 << 16; // R
                    color_data |= 128 << 8;   // G
                    color_data |= 255 << 0;   // B

                    // Assign the color data to the pixel.
                    *((int*)pBackBuffer) = color_data;
                }

                // Specify the area of the bitmap that changed.
                writeableBitmap.AddDirtyRect(new Int32Rect(column, row, 1, 1));
            }
            finally
            {
                // Release the back buffer and make it available for display.
                writeableBitmap.Unlock();
            }
        }

        static void ErasePixel(MouseEventArgs e)
        {
            byte[] ColorData = { 0, 0, 0, 0 }; // B G R

            Int32Rect rect = new Int32Rect(
                    (int)(e.GetPosition(i).X),
                    (int)(e.GetPosition(i).Y),
                    1,
                    1);

            writeableBitmap.WritePixels(rect, ColorData, 4, 0);
        }


    }
    // MessageBox.Show((string)sender);
}
