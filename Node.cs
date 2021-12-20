using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace PathFinder
{
    class Node
    {
        public Position screenPosition { get; set; }
        public Position gridPosition { get; set; }
        public int size { get; set; }
        public int index { get; set; }

        public NodeType type { get; set; } = NodeType.Empty;

        private Color currentColor;
        private Color animateToColor;
        private bool shouldHandleAnimation = false;

        public void Tick(float msElapsed) // Handle animation
        {
            if (!shouldHandleAnimation) return;

            if (Color.AreClose(currentColor, animateToColor))
            {
                currentColor = animateToColor;
                shouldHandleAnimation = false;
                Draw();
                return;
            }

            currentColor = currentColor.Lerp(animateToColor, msElapsed * 0.02f);

            Draw();
        }

        public void SetType(NodeType newType, bool skipAnimation = false)
        {
            if (newType == type) return;

            type = newType;
            ResetColor(skipAnimation);
        }

        public void ResetColor(bool skipAnimation = false)
        {
            Color correctColor = type.ConvertTypeToColor( );
            if(skipAnimation)
            {
                currentColor = correctColor;
                shouldHandleAnimation = false;
                Draw();
                return;
            }

            if(correctColor != currentColor)
            {
                animateToColor = correctColor;
                shouldHandleAnimation = true;
            }
        }

        public void Draw()
        {
            Grid.Instance.surface.FillRectangle(screenPosition.X, screenPosition.Y, screenPosition.X + size, screenPosition.Y + size, currentColor);
        }
    }
}
