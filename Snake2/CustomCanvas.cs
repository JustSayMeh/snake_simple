using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Snake2
{
    class CustomCanvas
    {
        private Canvas GameArea;
        private List<Quad> quads = new List<Quad>();
        public CustomCanvas(Canvas area)
        {
            GameArea = area;
        }

        public void Add(Rectangle rect)
        {
            GameArea.Children.Add(rect);
        }
        public void Remove(Rectangle rect)
        {
            GameArea.Children.Remove(rect);
        }
        public void Add(Quad q)
        {
            quads.Add(q);
            GameArea.Children.Add(q.GetRectangle());
        }
        public void Remove(Quad q)
        {
            GameArea.Children.Remove(q.GetRectangle());
            quads.Remove(q);
        }

        public void resetCanvas()
        {
            for (int i = 0; i < quads.Count; i++)
            {
                Quad q = quads.ElementAt(i);
                GameArea.Children.Remove(q.GetRectangle());
            }
            quads.Clear();
        }
        public void setPosition(Quad q)
        {
            Canvas.SetLeft(q.GetRectangle(), q.getPosition().X);
            Canvas.SetTop(q.GetRectangle(), q.getPosition().Y);
        }
        public void setPosition(Rectangle rect, double x, double y)
        {
            Canvas.SetLeft(rect, x);
            Canvas.SetTop(rect, y);
        }
    }
}
