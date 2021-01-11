using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Snake2
{
    enum Effect
    {
        NOPE,
        DOUBLESPEED,
        NOCLIP
    }
    abstract class Bonus : Quad
    {
        protected int lifecount = 0;
        protected int age = 0;
        protected Effect effect;
        public Bonus(Point pos, int block_size, SolidColorBrush color) : base(pos, block_size, color)
        {
            Random rand = new Random();
            age = rand.Next(25, 50);
        }

        public abstract void custEffect(Snake snake);
        public abstract void disableEffect(Snake snake);
        public Effect GetEffect()
        {
            return effect;
        }
        public int decrementAge()
        {
            return --age;
        }
    }
    class NoClipBonus : Bonus
    {
        static SolidColorBrush color = Brushes.BlueViolet;
        public NoClipBonus(Point pos, int block_size) : base(pos, block_size, color)
        {
            effect = Effect.NOCLIP;
            lifecount = 60;
        }
        public override void custEffect(Snake snake)
        {
            if (--lifecount == 0)
            {
                disableEffect(snake);
                return;
            }
            for (int i = 0; i < snake.parts.Count; i++)
            {
                Rectangle rect = snake.parts.ElementAt(i).GetRectangle();
                rect.Opacity = 0.5;
            }
        }
        public override void disableEffect(Snake snake)
        {
            for (int i = 0; i < snake.parts.Count; i++)
            {
                Rectangle rect = snake.parts.ElementAt(i).GetRectangle();
                rect.Opacity = 1;
            }
            snake.setBonus(null);
        }
    }


    class DoubleSpeedBonus : Bonus
    {
        static SolidColorBrush color = Brushes.Aquamarine;
        public DoubleSpeedBonus(Point pos, int block_size) : base(pos, block_size, color)
        {
            effect = Effect.DOUBLESPEED;
            lifecount = 90;
        }
        public override void custEffect(Snake snake)
        {
            if (--lifecount == 0)
            {
                disableEffect(snake);
                return;
            }
        }
        public override void disableEffect(Snake snake)
        {
            
            snake.setBonus(null);
        }
       
    }
}
