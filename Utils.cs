using System;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;

namespace PathFinder
{
    public struct Position
    {
        public Position(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; set; }
        public int Y { get; set; }
    }
    public enum NodeType
    {
        Empty = 0,
        Block = 1,
        StartPoint = 2,
        EndPoint = 3,
        Opened = 4,
        Closed = 5,
        Path = 6,
    }


    public static class Utils
    {
        //public static Grid grid;
        public static WriteableBitmap surface;

        public static readonly Dictionary<NodeType, Color> ColorByType = new Dictionary<NodeType, Color>
        {
            { NodeType.Empty, Color.FromRgb(100, 100, 100) },
            { NodeType.Block, Color.FromRgb(50, 50, 50) },
            { NodeType.StartPoint, Colors.Green },
            { NodeType.EndPoint, Colors.Red },
            { NodeType.Opened, Colors.Yellow },
            { NodeType.Closed, Colors.GreenYellow },
            { NodeType.Path, Colors.Cyan },
        };

        public static Color ConvertTypeToColor(this NodeType type)
        {
            if (ColorByType.ContainsKey( type ))
            {
                return ColorByType[type];
            }

            return Colors.Blue;
        }

        // Extend existing class 'Color'
        public static Color Lerp(this Color self, Color target, float by)
        {
            return self * (1 - by) + target * by;
        }

        // Extend existing class 'Point'
        public static Point Clamp(this Point p, int w, int h)
        {
            p.X = Math.Clamp(p.X, 0, w);
            p.Y = Math.Clamp(p.Y, 0, h);
            return p;
        }
    }
}
