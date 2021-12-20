using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PathFinder
{
    static class Heuristic
    {
        private static readonly float SQRT2 = MathF.Sqrt(2);
        public static float Manhattan( int dx, int dy )
        {
            return dx + dy;
        }

        public static float Euclidean(int dx, int dy)
        {
            return MathF.Sqrt(dx^2 + dy^2);
        }

        public static float Octile( int dx, int dy)
        {
            var F = SQRT2 - 1;
            return (dx < dy) ? F * dx + dy : F * dy + dx;
        }
    }
}
