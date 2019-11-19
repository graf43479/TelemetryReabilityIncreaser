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
                DrawCustomRectangle(mDif.Coords, new Coord(yOffset, xOffset), mDif.ToString());               

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
            textBlock.Foreground = new SolidColorBrush(Colors.DarkCyan);
            textBlock.HorizontalAlignment = HorizontalAlignment.Left;
            textBlock.FontSize = 16;
            //Canvas.SetLeft(textBlock, offset.Y+pixelSize*32/2);
            Canvas.SetLeft(textBlock, offset.Y);
            Canvas.SetTop(textBlock, offset.X -20);

            canvas.Children.Add(textBlock);

            //закраска области матрицы
            DrawSimpleRectangle(offset, pixelSize * 32, pixelSize * 20, Colors.DarkCyan, Colors.Orange);           

            //закраска поврежденных блоков
            foreach (Coord coord in coords)
            {
                Coord tmpCoord = new Coord(coord.X * pixelSize + offset.X, coord.Y * pixelSize + offset.Y);
                DrawSimpleRectangle(tmpCoord, pixelSize, pixelSize, Colors.Coral, Colors.Black);                               
            }
            
            //закраска сетки
           /* for (int i = 0; i < 32; i++)
            {
                for (int j = 0; j < 20; j++)
                {
                    Coord tmpCoord = new Coord(j * pixelSize + offset.X, i * pixelSize + offset.Y);
                    DrawSimpleRectangle(tmpCoord, pixelSize, pixelSize, null, Colors.Black);                   
                }
            }*/

            //закраска всей области c отступами 
            DrawSimpleRectangle(new Coord(offset.X-1, offset.Y-1), pixelSize * 32+2, pixelSize * 20+2, null, Colors.Orange);
        }

        void DrawSimpleRectangle(Coord coord, int width, int height, Color? fillColor, Color? borderColor)
        {
            Rectangle border = new Rectangle
            {
                Height = height,
                Width = width,
                Stroke = borderColor == null ? null : new SolidColorBrush((Color)borderColor),                
                StrokeThickness = 1,
                Fill = fillColor == null ? null : new SolidColorBrush((Color)fillColor),
            };

            canvas.Children.Add(border);
            Canvas.SetTop(border, coord.X);
            Canvas.SetLeft(border, coord.Y);
        }
    }
}
