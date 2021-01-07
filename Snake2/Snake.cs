using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake2
{
    enum Direction
    {
        UP, LEFT, RIGHT, DOWN
    }
    class Quad
    {
        public Point position;
        public Rectangle rect;
        public Quad(Point pos, int size, SolidColorBrush color)
        {
            position = pos;
            this.rect = new Rectangle
            {
                Width = size,
                Height = size,
                Fill = color
            };
        }
        public Point GetPoint()
        {
            return position;
        }

        public Rectangle GetRectangle()
        {
            return rect;
        }
        public void setColor(SolidColorBrush color)
        {
            rect.Fill = color;
        }
        public Point getPosition()
        {
            return position;
        }
        public void setOpacity(double opacity)
        {
            rect.Opacity = opacity;
        }

    }
    class Snake
    {
        bool isEat = false;
        public LinkedList<Quad> parts;
        SolidColorBrush head = Brushes.Red, tail = Brushes.DarkRed;
        int block_size;
        Direction direction = Direction.UP;
        private Bonus bonus = null;
        public Snake(Point start1, Direction d, int size)
        {
            parts = new LinkedList<Quad>();
       
            parts.AddLast(new Quad(start1, size, head));
            direction = d;
            block_size = size;
        }
        public void setDirection(Direction dir)
        {
            if (direction == Direction.UP && (dir == Direction.LEFT || dir == Direction.RIGHT))
                direction = dir;
            else if (direction == Direction.DOWN && (dir == Direction.LEFT || dir == Direction.RIGHT))
                direction = dir;
            else if (direction == Direction.RIGHT && (dir == Direction.UP || dir == Direction.DOWN))
                direction = dir;
            else if (direction == Direction.LEFT && (dir == Direction.UP || dir == Direction.DOWN))
                direction = dir;
        }
        public void move(CustomCanvas canvas)
        {
            Quad last = parts.Last();
            last.setColor(tail);
            switch (direction)
            {
                case Direction.UP:
                    parts.AddLast(new Quad(new Point(last.position.X, last.position.Y - block_size), block_size, head));
                    break;
                case Direction.DOWN:
                    parts.AddLast(new Quad(new Point(last.position.X, last.position.Y + block_size), block_size, head));
                    break;
                case Direction.RIGHT:
                    parts.AddLast(new Quad(new Point(last.position.X + block_size, last.position.Y), block_size, head));
                    break;
                case Direction.LEFT:
                    parts.AddLast(new Quad(new Point(last.position.X - block_size, last.position.Y), block_size, head));
                    break;
            }
            canvas.Add(parts.Last());
            if (!isEat)
            {
                canvas.Remove(parts.First());
       
                parts.RemoveFirst();
            }
            else
            {
                isEat = false;
            }
            if (bonus != null)
                bonus.custEffect(this);
        }

        public void setEat()
        {
            isEat = true;
        }
        public void setBonus(Bonus bonus)
        {
            if (bonus != null)
                bonus.disableEffect(this);
            this.bonus = bonus;
        }

        public Effect getEffect()
        {
            if (bonus == null)
                return Effect.NOPE;
            return bonus.GetEffect();
        }
    }
}
