using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using TelemetryEngine;

namespace DamageVisualizer
{
    public class UnitedFramesComponent
    {
        static Canvas canvas;        
        int pixelSize = 10;
        readonly int mainXOffset = 0;
        readonly int mainYOffset = 0;

        public UnitedFramesComponent(Canvas area, int pixelSize, int? offsetX, int? offsetY)
        {
            canvas = area;
            this.pixelSize = pixelSize;
            mainXOffset = offsetX ?? mainXOffset;
            mainYOffset = offsetY ?? mainYOffset;
        }

        public void DrawData(IEnumerable<List<Coord>> coords)
        {
            canvas.Children.Clear();            
            int count = 0;
            int xOffset = 0;
            int yOffset = 0;
            int pixelSize = 10;

            int blockSizeX = 32 * pixelSize + mainXOffset;
            foreach (List<Coord> mDif in coords)
            {                
                DrawCustomRectangle(mDif, new Coord(yOffset, xOffset));               

                if ((canvas.Width - xOffset-blockSizeX) < blockSizeX) //xOfFset
                {
                    yOffset += 20 * pixelSize + mainYOffset;
                    xOffset = 0;
                    count = 0;
                }
                else
                {
                    xOffset += 32 * pixelSize + mainXOffset;
                    ++count;
                }
            }
        }

          void DrawCustomRectangle(List<Coord> coords, Coord offset)
        {

            int i = 0;
            foreach (Coord coord in coords)
            {
                i++;
                Rectangle r = new Rectangle
                {
                    Height = pixelSize,
                    Width = pixelSize,
                    Fill = new SolidColorBrush(Colors.Coral),
                    Stroke = new SolidColorBrush(Colors.Black),
                    StrokeThickness = 1
                };

                r.SetValue(Grid.RowProperty, coord.X + offset.X);
                r.SetValue(Grid.ColumnProperty, coord.Y + offset.Y);
                canvas.Children.Add(r);
                Canvas.SetTop(r, coord.X * pixelSize + offset.X);
                Canvas.SetLeft(r, coord.Y * pixelSize + offset.Y);
            }

           // Pen pen = new Pen(new SolidColorBrush(Color.FromRgb(200, 10, 20)), 2);
            Rectangle border = new Rectangle
            {
                Height = pixelSize * 20,
                Width = pixelSize * 32,
                Stroke = new SolidColorBrush(Colors.Orange)
            };

            canvas.Children.Add(border);
            Canvas.SetTop(border, offset.X);
            Canvas.SetLeft(border, offset.Y);
        }
    }
}
