using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Snake2
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        const string best_score_variable_name = "SnakeScore";
        static int block_size = 20;
        double width = 0;
        double height = 0;
        int h_count, w_count;
        LinkedList<Bonus> bonuses = new LinkedList<Bonus>(), l_bonuses;
        LinkedList<Direction> directions = new LinkedList<Direction>();
        Snake snake;
        Quad apple;
        CustomCanvas canvas;
        IntSystemVariable bestscore;
        int animation_ticker = 0, delimiter = 0;
        Timer timer;
        static int tick_time = 60;
        public MainWindow()
        {
            InitializeComponent();
            bestscore = new IntSystemVariable(best_score_variable_name, EnvironmentVariableTarget.User);
            BestScore.Text = bestscore.get().ToString();
            canvas = new CustomCanvas(GameArea);

        }
     

        private void setGameScore(int score)
        {
            this.Title = $"Score: {score}";
        }
        private void Game_start()
        {
            timer = new Timer(tick_time, timer_tick);
            timer.stop();
            bonuses.Clear();
            canvas.resetCanvas();
            
            GameArea.Background = Brushes.Yellow;
            width = GameArea.Width;
            height = GameArea.Height;

            h_count = (int)height / block_size;
            w_count = (int)width / block_size;
            snake = new Snake(Tools.randomPoint(block_size, 5, h_count - 5, 5, w_count - 5), Tools.randomDirection(), block_size);
            snake.setEat();
            setGameScore(0);
            for (int i = 0; i < h_count; i++)
            {
                Quad q1 = new Quad(new Point(0, i * block_size), block_size, Brushes.Green);
                Quad q2 = new Quad(new Point((w_count - 1) * block_size, i * block_size), block_size, Brushes.Green);
                canvas.setPosition(q1);
                canvas.setPosition(q2);
                canvas.Add(q1);
                canvas.Add(q2);
            }


            for (int i = 0; i < w_count; i++)
            {
                Quad q1 = new Quad(new Point(i * block_size, (h_count - 1) * block_size), block_size, Brushes.Green);
                Quad q2 = new Quad(new Point(i * block_size, 0), block_size, Brushes.Green);
                canvas.setPosition(q1);
                canvas.setPosition(q2);
                canvas.Add(q1);
                canvas.Add(q2);
            }
       
            for (int i = 0; i < snake.parts.Count; i++)
            {
                Quad q = snake.parts.ElementAt(i);
                canvas.Add(q);
                canvas.setPosition(q);
            }
            do
            {
                apple = generateApple();
            } while (!Tools.validateGenerator(apple, canvas));
            canvas.Add(apple);
            canvas.setPosition(apple);
            timer.start();
        }
        private void timer_tick(object sender, EventArgs e)
        {
            
            if (delimiter == 1)
            {
                game_step(snake);
                animation_step();
                addBonus();
            }else if (snake.getEffect() == Snake2.Effect.DOUBLESPEED)
                game_step(snake);
            
            delimiter += 1;
            if (delimiter == 2)
                delimiter = 0;
        }

        private void game_step(Snake snake)
        {
            if (!timer.isRunning())
                return;

            if (directions.Count > 0)
            {
                snake.setDirection(directions.ElementAt(0));
                directions.RemoveFirst();
            }

            snake.move(canvas);
            for (int i = 0; i < snake.parts.Count; i++)
            {
                canvas.setPosition(snake.parts.ElementAt(i));
            }
            if (Tools.hasBorderCollision(snake, block_size, width, height))
            {
                if (snake.getEffect() == Snake2.Effect.NOCLIP)
                {
                    Quad q = snake.parts.Last();
                    Point pos = q.getPosition();
                    if (pos.X <= block_size)
                        q.position.X = width - 2 * block_size;
                    else if (pos.X >= width - block_size)
                        q.position.X = block_size;
                    else if (pos.Y <= block_size)
                        q.position.Y = height - 2 * block_size;
                    else if (pos.Y >= height - block_size)
                        q.position.Y = block_size;
                }
                else
                    Game_end();

                    
              
            }
            if (!Tools.validateHeadMove(snake) && snake.getEffect() != Snake2.Effect.NOCLIP)
            {
                Game_end();
            }
            if (Tools.hasColision(apple, snake.parts.Last()))
            {
                snake.setEat();
                canvas.Remove(apple);
                setGameScore(snake.parts.Count - 1);
                do
                {
                    apple = generateApple();
                } while (!Tools.validateGenerator(apple, canvas));
                canvas.Add(apple);
                canvas.setPosition(apple);
            }
        }

        private void animation_step()
        {
            l_bonuses = new LinkedList<Bonus>(bonuses);
            foreach (Bonus th in bonuses)
            {
                int age = th.decrementAge();
                if (Tools.hasColision(th, snake.parts.Last()))
                {
                    snake.setBonus(th);
                    canvas.Remove(th);
                    l_bonuses.Remove(th);
                }
                else if (age == 0)
                {
                    canvas.Remove(th);
                    l_bonuses.Remove(th);
                }
                else if (age < 5)
                {
                    th.GetRectangle().Opacity = 0.3;
                }
                else if (age < 10)
                {
                    th.GetRectangle().Opacity = 0.5;
                }
                else if (age < 15)
                {
                    th.GetRectangle().Opacity = 0.7;
                }
                else if (age < 20)
                {
                    th.GetRectangle().Opacity = 0.9;
                }
            }

            bonuses = l_bonuses;
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (!timer.isRunning())
                return;
            switch (e.Key)
            {
                case Key.W:
                    directions.AddLast(Direction.UP);
             
                    break;
                case Key.S:
                    directions.AddLast(Direction.DOWN);
                    break;
                case Key.D:
                    directions.AddLast(Direction.RIGHT);
                    break;
                case Key.A:
                    directions.AddLast(Direction.LEFT);
                    break;
                case Key.E:
                    snake.setEat();
                    break;
                case Key.Escape:
                    Game_end();
                    break;
            }
            //move_snake();
        }



        private Quad generateApple()
        {
            Random rand = new Random();
            int x = rand.Next(1, w_count - 1);
            int y = rand.Next(1, h_count - 1);
            Point pos = new Point(block_size * x, block_size * y);
            Quad quad = new Quad(pos, block_size, Brushes.Brown);
            return quad;
        }



      

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            startMenu.Visibility = Visibility.Collapsed;
            Game_start();
        }
        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            canvas.resetCanvas();
            Guide.Visibility = Visibility.Visible;
            startMenu.Visibility = Visibility.Collapsed;
            
        }
        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            Guide.Visibility = Visibility.Collapsed;
            startMenu.Visibility = Visibility.Visible;
        }
        private void addBonus()
        {
            Random rand = new Random();
            double f = rand.NextDouble();
            if (f < 0.3 && bonuses.Count < 3)
            {
                Bonus bonus;
                do
                {
                    bonus = Tools.generateBonus(block_size, w_count, h_count);
                } while (!Tools.validateGenerator(bonus, canvas) || Tools.hasColision(apple, bonus));
                bonuses.AddLast(bonus);
                canvas.Add(bonus);
                canvas.setPosition(bonus);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            bestscore.set(0);
            BestScore.Text = "0";
        }

        private void RadioButton_Click_2(object sender, RoutedEventArgs e)
        {
            tick_time = 60;
            timer = new Timer(tick_time, timer_tick);
        }

        private void RadioButton_Click_3(object sender, RoutedEventArgs e)
        {
            tick_time = 40;
            timer = new Timer(tick_time, timer_tick);
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            block_size = 20;
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            block_size = 10;
        }

        public void Game_end()
        {
            canvas.resetCanvas();
            timer.stop();
            startMenu.Visibility = Visibility.Visible;
            this.Title = "Snake";
            int curcount = snake.parts.Count - 2;
            if (curcount > bestscore.get())
            {
                bestscore.set(curcount);
                BestScore.Text = curcount.ToString();
            }
        }
    }
}
