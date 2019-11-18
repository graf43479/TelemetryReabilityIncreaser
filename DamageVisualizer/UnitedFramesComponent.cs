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

        public void DrawData(IEnumerable<MismatchesCoordList> coords)
        {
            canvas.Children.Clear();            
            int xOffset = 10;
            int yOffset = 20;
            int pixelSize = 10;

            int blockSizeX = 32 * pixelSize + mainXOffset;
            foreach (MismatchesCoordList mDif in coords)
            {                
                DrawCustomRectangle(mDif.Coords, new Coord(yOffset, xOffset), mDif.Name);               

                if ((canvas.Width - xOffset-blockSizeX) < blockSizeX) //xOfFset
                {
                    yOffset += 20 * pixelSize + mainYOffset;
                    xOffset = 10;
                }
                else
                {
                    xOffset += 32 * pixelSize + mainXOffset;
                }
            }
        }

        void DrawCustomRectangle(List<Coord> coords, Coord offset, string text)
        {
            TextBlock textBlock = new TextBlock();
            textBlock.Text = text;
            textBlock.Foreground = new SolidColorBrush(Colors.Coral);
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.FontSize = 24;
            Canvas.SetLeft(textBlock, offset.Y+pixelSize*32/2);
            // Canvas.SetTop(textBlock, offset.X - pixelSize * 20 - mainYOffset-30);
            Canvas.SetTop(textBlock, offset.X -32);

            canvas.Children.Add(textBlock);

          foreach (Coord coord in coords)
          {
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

            
            
            /*for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Rectangle grid = new Rectangle
                    {
                        Height = pixelSize,
                        Width = pixelSize,
                        Stroke = new SolidColorBrush(Colors.Black),
                        StrokeThickness = 1
                    };

                    canvas.Children.Add(grid);
                    Canvas.SetTop(grid, j*pixelSize+offset.X);
                    Canvas.SetLeft(grid, i * pixelSize + offset.Y);
                }
            }
            */
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
