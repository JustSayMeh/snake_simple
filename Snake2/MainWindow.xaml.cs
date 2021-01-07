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
        List<Bonus> bonuses = new List<Bonus>();
        List<Direction> directions = new List<Direction>();
        Snake snake;
        Quad apple;
        CustomCanvas canvas;
        int bestscore = 0;
        int animation_ticker = 0, delimiter = 0;
        System.Windows.Threading.DispatcherTimer gameTickTimer;
        static int tick_time = 50;
        public MainWindow()
        {
            InitializeComponent();

            string bestScoreText = Environment.GetEnvironmentVariable(best_score_variable_name, EnvironmentVariableTarget.User);
            BestScore.Text = bestScoreText;
            bestscore = int.Parse(bestScoreText);

            canvas = new CustomCanvas(GameArea);
            gameTickTimer = new System.Windows.Threading.DispatcherTimer();
            gameTickTimer.Tick += GameTickTimer_Tick;
            gameTickTimer.Interval = TimeSpan.FromMilliseconds(tick_time);
            
            
            
        }
        private Point randomPoint(int t, int b, int l, int r)
        {
            Random rand = new Random();
            int x = rand.Next(l, r);
            int y = rand.Next(t, b);
            return new Point(x * block_size, y * block_size);
        }
        private Direction randomDirection()
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
        private void setGameScore(int score)
        {
            this.Title = $"Score: {score}";
        }
        private void Game_start()
        {
            gameTickTimer.IsEnabled = false;
            bonuses.Clear();
            canvas.resetCanvas();
            
            GameArea.Background = Brushes.Yellow;
            width = GameArea.Width;
            height = GameArea.Height;

            h_count = (int)height / block_size;
            w_count = (int)width / block_size;
            snake = new Snake(randomPoint(5, h_count - 5, 5, w_count - 5), randomDirection(), block_size);
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
            } while (!validateGenerator(apple));
            canvas.Add(apple);
            canvas.setPosition(apple);
            gameTickTimer.IsEnabled = true;
        }
        private void GameTickTimer_Tick(object sender, EventArgs e)
        {
            
            if (delimiter == 1)
            {
                game_step(snake);
                addBonus();
            }else if (snake.getEffect() == Snake2.Effect.DOUBLESPEED)
                game_step(snake);
            
            delimiter += 1;
            if (delimiter == 2)
                delimiter = 0;
        }

        private void game_step(Snake snake)
        {
            if (!gameTickTimer.IsEnabled)
                return;

            if (directions.Count > 0)
            {
                snake.setDirection(directions.ElementAt(0));
                directions.RemoveAt(0);
            }

            snake.move(canvas);
            for (int i = 0; i < snake.parts.Count; i++)
            {
                canvas.setPosition(snake.parts.ElementAt(i));
            }
            if (hasBorderCollision(snake.parts.Last()))
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
            if (!validateHeadMove(snake.parts.Last()) && snake.getEffect() != Snake2.Effect.NOCLIP)
            {
                Game_end();
            }
            if (hasColision(apple, snake.parts.Last()))
            {
                snake.setEat();
                canvas.Remove(apple);
                setGameScore(snake.parts.Count - 1);
                do
                {
                    apple = generateApple();
                } while (!validateGenerator(apple));
                canvas.Add(apple);
                canvas.setPosition(apple);
            }

            for (int i = 0; i < bonuses.Count; i++)
            { 

                if (hasColision(bonuses.ElementAt(i), snake.parts.Last()))
                { 
                    snake.setBonus(bonuses.ElementAt(i));
                    canvas.Remove(bonuses.ElementAt(i).GetRectangle());
                    bonuses.RemoveAt(i);
                }else if(bonuses.ElementAt(i).decrementAge() == 0)
                {
                    canvas.Remove(bonuses.ElementAt(i).GetRectangle());
                    bonuses.RemoveAt(i);
                }
            }
           
        }
        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            if (!gameTickTimer.IsEnabled)
                return;
            switch (e.Key)
            {
                case Key.W:
                    directions.Add( Direction.UP);
                    break;
                case Key.S:
                    directions.Add(Direction.DOWN);
                    break;
                case Key.D:
                    directions.Add(Direction.RIGHT);
                    break;
                case Key.A:
                    directions.Add(Direction.LEFT);
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
        private bool hasBorderCollision(Quad q)
        {
            Point pos = q.getPosition();
            return pos.X < block_size || pos.X >= width - block_size || pos.Y < block_size || pos.Y > height - 2 * block_size; 
        }

        private bool hasColision(Quad a, Quad b)
        {
            if (a.position.X == b.position.X && a.position.Y == b.position.Y)
                return true;
            return false;
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

        private bool validateGenerator(Quad q)
        {
            for (int i = 0; i < snake.parts.Count; i++)
            {
                Quad th = snake.parts.ElementAt(i);
                if (hasColision(q, th))
                    return false;
            }
            for (int i = 0; i < bonuses.Count; i++)
            {
                Quad th = bonuses.ElementAt(i);
                if (hasColision(q, th))
                    return false;
            }
            return true;
        }

        private bool validateHeadMove(Quad head)
        {
            for (int i = 0; i < snake.parts.Count - 1; i++)
            {
                Quad th = snake.parts.ElementAt(i);
                if (hasColision(head, th))
                    return false;
            }
            return true;
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
                    bonus = generateBonus();
                } while (!validateGenerator(bonus) || hasColision(apple, bonus));
                bonuses.Add(bonus);
                canvas.Add(bonus);
                canvas.setPosition(bonus);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable(best_score_variable_name, "0", EnvironmentVariableTarget.User);
            BestScore.Text = "0";
        }

        private void RadioButton_Click(object sender, RoutedEventArgs e)
        {
            block_size = 20;
        }

        private void RadioButton_Click_1(object sender, RoutedEventArgs e)
        {
            block_size = 10;
        }

        private Bonus generateBonus()
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

        public void Game_end()
        {
            canvas.resetCanvas();
            gameTickTimer.IsEnabled = false;
            startMenu.Visibility = Visibility.Visible;
            this.Title = "Snake";
            int curcount = snake.parts.Count - 2;
            if (curcount > bestscore)
            {
                Environment.SetEnvironmentVariable(best_score_variable_name, curcount.ToString(), EnvironmentVariableTarget.User);
                BestScore.Text = curcount.ToString();
            }
        }
    }
}
