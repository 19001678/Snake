using System;
using System.Collections.Generic;
using System.IO;
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

namespace Snake
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly Dictionary<GridValue, ImageSource> gridValToImage = new()
        {
            { GridValue.Empty, Images.Empty },
            { GridValue.Snake, Images.Body },
            { GridValue.Food, Images.Food },
        };

        private readonly Dictionary<Direction, int> dirToRotation = new()
        {
            {Direction.Up, 0},
            {Direction.Right, 90},
            {Direction.Down, 180},
            {Direction.Left, 270},

            {Direction.UpRight, 45},
            {Direction.DownRight, 135},
            {Direction.DownLeft, 225},
            {Direction.UpLeft, 315},
        };

        private readonly int rows = 15;
        private readonly int cols = 15;
        private readonly Image[,] gridImages;
        private GameState gameState;
        private bool gameRunning;
        private int highScore = 0;
        private GameTimer gameTimer;

        public MainWindow()
        {
            InitializeComponent();

            gameTimer = new GameTimer(15);
            gameTimer.Start();

            gridImages = SetupGrid();
            gameState = new GameState(rows, cols, gameTimer);
            string fileName = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "highscore.txt");
            if (File.Exists(fileName))
            {
                StreamReader sr = new StreamReader(fileName);
                highScore = int.Parse(sr.ReadLine());
                sr.Close();
                HighScoreText.Text = $"High Score: {highScore}";
            }
            else
            {
                StreamWriter sw = new StreamWriter(fileName);
                sw.WriteLine(highScore);
                sw.Close();
            }
        }

        private async Task RunGame()
        {
            Draw();
            await ShowCountDown();
            Overlay.Visibility = Visibility.Hidden;
            await GameLoop();
            await ShowGameOver();
            gameState = new GameState(rows,cols,gameTimer);
        }
        
        private async void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Overlay.Visibility == Visibility.Visible)
            {
                e.Handled = true;
            }

            if (!gameRunning)
            {
                gameRunning = true;
                await RunGame();
                gameRunning = false;
            }
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if (gameState.GameOver)
            {
                return;
            }
            switch (e.Key)
            {
                case Key.Left:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.Right:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.Up:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.Down:
                    gameState.ChangeDirection(Direction.Down);
                    break;

                case Key.A:
                    gameState.ChangeDirection(Direction.Left);
                    break;
                case Key.D:
                    gameState.ChangeDirection(Direction.Right);
                    break;
                case Key.W:
                    gameState.ChangeDirection(Direction.Up);
                    break;
                case Key.S:
                    gameState.ChangeDirection(Direction.Down);
                    break;
                case Key.E:
                    gameState.ChangeDirection(Direction.UpRight);
                    break;
                case Key.C:
                    gameState.ChangeDirection(Direction.DownRight);
                    break;
                case Key.Z:
                    gameState.ChangeDirection(Direction.DownLeft);
                    break;
                case Key.Q:
                    gameState.ChangeDirection(Direction.UpLeft);
                    break;
            }
        }

        private async Task GameLoop()
        {
            while (!gameState.GameOver)
            {
                if (gameTimer.TimeInSeconds <= 0)
                {
                    gameState.GameOver = true;
                } 
                
                await Task.Delay(100);
                gameState.Move();
                Draw();
            }
        }

        private Image[,] SetupGrid()
        {
            Image[,] images = new Image[rows, cols];
            GameGrid.Rows = rows;
            GameGrid.Columns = cols;
            GameGrid.Width = GameGrid.Height * (cols / (double)rows);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Image image = new Image
                    {
                        Source = Images.Empty,
                        RenderTransformOrigin = new Point(0.5, 0.5)
                    };

                    images[r, c] = image;
                    GameGrid.Children.Add(image);
                }
            }

            return images;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Draw();
            await GameLoop();
        }

        private void Draw()
        {
            DrawGrid();
            DrawSnakeHead();
            TimeText.Text = $"TIME {gameTimer.TimeInSeconds}";
            //ScoreText.Text = $"SCORE {gameTimer.Score}";
        }



        private void DrawGrid()
        {
            for (int r=0; r< rows; r++)
            {
                for (int c=0; c< cols; c++)
                {
                    GridValue gridVal = gameState.Grid[r, c];
                    gridImages[r, c].Source = gridValToImage[gridVal];
                    gridImages[r, c].RenderTransform = Transform.Identity;
                }
            }
        }

        private void DrawSnakeHead()
        {
            Position headPos = gameState.HeadPosition();
            Image image = gridImages[headPos.Row, headPos.Col];
            image.Source = Images.Head;

            int rotation = dirToRotation[gameState.Dir];
            image.RenderTransform = new RotateTransform(rotation);
        }

        private async Task DrawDeadSnake()
        {
            List<Position> positions = new List<Position>(gameState.SnakePositions());

            for (int i=0; i<positions.Count; i++)
            {
                Position pos = positions[i];
                ImageSource source = (i == 0) ? Images.DeadHead : Images.DeadBody;
                gridImages[pos.Row, pos.Col].Source = source;
                await Task.Delay(50);
            }
        }

        private async Task ShowCountDown()
        {
            for (int i=3; i>=1; i--)
            {
                OverlayText.Text = i.ToString();
                await Task.Delay(500);
            }
        }

        private async Task ShowGameOver()
        {
            Audio.GameOver.Play();
            if (gameState.Score > highScore)
            {
                highScore = gameState.Score;
                StreamWriter sw = new StreamWriter(Directory.GetCurrentDirectory() + "\\highscore.txt");
                sw.WriteLine(highScore);
                sw.Close();
            }

            HighScoreText.Text = $"High Score: {highScore}";

            await DrawDeadSnake();
            await Task.Delay(1000);
            OverlayText.Text = "All wars are civil wars, because all men are brothers. - Francois Fenelon";
            Overlay.Visibility = Visibility.Visible;
        }
    }
}
