using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Snake2
{
    class Tools
    {
        public static Bonus generateBonus(int block_size, int w_count, int h_count)
        {
            Random rand = new Random();
            double r = rand.NextDouble();
            int x = rand.Next(1, w_count - 1);
            int y = rand.Next(1, h_count - 1);
            Point pos = new Point(block_size * x, block_size * y);
            if (r < 0.1)
            {
                return new NoClipBonus(pos, block_size);
            }
            else
            {
                return new DoubleSpeedBonus(pos, block_size);
            }

        }

        public static bool validateHeadMove(Snake snake)
        {
            Quad head = snake.parts.Last();
            for (int i = 0; i < snake.parts.Count - 1; i++)
            {
                Quad th = snake.parts.ElementAt(i);
                if (hasColision(head, th))
                    return false;
            }
            return true;
        }


        public static bool hasColision(Quad a, Quad b)
        {
            if (a.position.X == b.position.X && a.position.Y == b.position.Y)
                return true;
            return false;
        }

        public static bool hasBorderCollision(Snake snake, int block_size, double width, double height)
        {
            Quad q = snake.parts.Last();
            Point pos = q.getPosition();
            return pos.X < block_size || pos.X > width - 1.5 * block_size || pos.Y < block_size || pos.Y > height - 1.5 * block_size;
        }


        public static Point randomPoint(int block_size, int t, int b, int l, int r)
        {
            Random rand = new Random();
            int x = rand.Next(l, r);
            int y = rand.Next(t, b);
            return new Point(x * block_size, y * block_size);
        }


        public static Direction randomDirection()
        {
            Random rand = new Random();
            int f = rand.Next(1, 4);
            switch (f)
            {
                case 1:
                    return Direction.UP;

                case 2:
                    return Direction.DOWN;

                case 3:
                    return Direction.LEFT;

                case 4:
                    return Direction.RIGHT;

                default:
                    return Direction.UP;
            }
        }

        public static bool validateGenerator(Quad q, CustomCanvas canvas)
        {
            for (int i = 0; i < canvas.getQuads().Count; i++)
            {
                Quad th = canvas.getQuads().ElementAt(i);
                if (Tools.hasColision(q, th))
                    return false;
            }
            return true;
        }

    }
}
