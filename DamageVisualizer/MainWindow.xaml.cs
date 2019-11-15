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

        private void ListBoxCombinations_SelectionChanged(object sender, RoutedEventArgs e)
        {            
            var val = ListBoxCombinations.SelectedItem;
            if (engine != null & val!=null)
            {
                EraseAllPixels();
                myCanvas.Children.Clear();
                //DrawCustomRectangle();
                RawDataMatrix result = engine.PerformCombination(val.ToString());
                int count = 0;
                int xOffset = 1;
                int yOffset = 1;
                int pixelSize = 10;
                foreach (var mList in engine.GetMatrixDifference())
                {                    
                    //TODO: вывести в отдельный класс с высчитыванием офсета 
                     DrawCustomRectangle(mList, pixelSize, new Coord(yOffset, xOffset));
                    //отрисовка пикселей для каждой матрицы
                    foreach (Coord coord in mList)
                    {
                        DrawPixel(new Coord(coord.X, coord.Y + count));
                    }
                    count += 50;

                    if ((myCanvas.Width - xOffset) < xOffset) //1047
                    {
                        yOffset += 20 * pixelSize;
                        xOffset = 1;
                    }
                    else
                    {
                        xOffset += 32 * pixelSize;     
                    }
                }

                foreach (Coord coord in result.GetMismatches(engine.Etalon))
                {
                    DrawPixel(new Coord(coord.X, coord.Y + count));
                }
                DrawCustomRectangle(result.GetMismatches(engine.Etalon).ToList(), pixelSize, new Coord(yOffset, xOffset));
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

        static void ErasePixels(IEnumerable<Coord> coords)
        {
            byte[] ColorData = { 0, 0, 0, 0 }; // B G R

            foreach (Coord coord in coords)
            {
                Int32Rect rect = new Int32Rect(
                    coord.X,
                    coord.Y,
                    1,
                    1);
                writeableBitmap.WritePixels(rect, ColorData, 4, 0);
            }
        }

        void EraseAllPixels()
        {
            List<Coord> allArea = new List<Coord>();
            for (int i = 0; i < (int)img.Width; i++)
            {
                for (int j = 0; j < (int)img.Height; j++)
                {
                    allArea.Add(new Coord(i, j));
                }
            }
            ErasePixels(allArea);
        }

        void DrawCustomRectangle(List<Coord> coords, int size, Coord offset)
        {
            Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
            Rectangle border = new Rectangle
            {
                Height = size * 20 ,
                Width = size * 32 ,
                Stroke = new SolidColorBrush(Colors.Orange)
            };

            myCanvas.Children.Add(border);
            Canvas.SetTop(border, offset.X);
            Canvas.SetLeft(border, offset.Y);

            int i = 0;
            foreach (Coord coord in coords)
            {
                i++;
                Rectangle r = new Rectangle
                {
                    Height = size,
                    Width = size,
                    Fill = new SolidColorBrush(Colors.Coral),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };

                r.SetValue(Grid.RowProperty, coord.X + offset.X );
                r.SetValue(Grid.ColumnProperty, coord.Y + offset.Y);
                myCanvas.Children.Add(r);
                Canvas.SetTop(r,  coord.X * size + offset.X);
                Canvas.SetLeft(r, coord.Y * size + offset.Y);
            }           
        }
    }
}
