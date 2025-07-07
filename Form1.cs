using System;
using System.Drawing;
using System.Windows.Forms;
using Timer = System.Windows.Forms.Timer;
namespace PongGame
{
    public partial class Form1 : Form
    {
        // UI state
        private enum GameState { Menu, Playing }
        private GameState currentState = GameState.Menu;
        private Button? btnStartPong;

        // Game objects
        private Rectangle playerPaddle;
        private Rectangle pitchRect;
        private Rectangle aiPaddle;
        private Rectangle ball;
        private int playerScore = 0;
        private int aiScore = 0;

        // Game settings
        private int paddleWidth = 10;
        private int paddleHeight = 60;
        private int ballSize = 12;
        private int paddleSpeed = 6;
        private int ballSpeedX = 5;
        private int ballSpeedY = 5;
        private System.Windows.Forms.Timer? gameTimer;
        private bool upPressed = false;
        private bool downPressed = false;

        public Form1()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
            this.Width = 800;
            this.Height = 450;
            this.Text = "Pong Game";
            ShowMenu();
        }

        private void ShowMenu()
        {
            currentState = GameState.Menu;
            this.Controls.Clear();
            btnStartPong = new Button();
            btnStartPong.Text = "Play Pong";
            btnStartPong.Font = new Font("Arial", 24);
            btnStartPong.Size = new Size(250, 80);
            btnStartPong.Location = new Point((this.ClientSize.Width - btnStartPong.Width) / 2, (this.ClientSize.Height - btnStartPong.Height) / 2);
            btnStartPong.Click += BtnStartPong_Click;
            this.Controls.Add(btnStartPong);
            this.Paint -= Form1_Paint;
            this.KeyDown -= Form1_KeyDown;
            this.KeyUp -= Form1_KeyUp;
        }

        private void BtnStartPong_Click(object? sender, EventArgs e)
        {
            this.Controls.Clear();
            StartGame();
        }

        private void StartGame()
        {
            currentState = GameState.Playing;
            pitchRect = new Rectangle(0, 0, this.ClientSize.Width, this.ClientSize.Height);

            playerPaddle = new Rectangle(30, (this.ClientSize.Height - paddleHeight) / 2, paddleWidth, paddleHeight);
            aiPaddle = new Rectangle(this.ClientSize.Width - 40, (this.ClientSize.Height - paddleHeight) / 2, paddleWidth, paddleHeight);
            ball = new Rectangle((this.ClientSize.Width - ballSize) / 2, (this.ClientSize.Height - ballSize) / 2, ballSize, ballSize);
            ballSpeedX = 5;
            ballSpeedY = 5;
            gameTimer = new Timer();
            gameTimer.Interval = 16; // ~60 FPS
            gameTimer.Tick += GameTimer_Tick;
            gameTimer.Start();
            this.KeyDown += Form1_KeyDown;
            this.KeyUp += Form1_KeyUp;
            this.Paint += Form1_Paint;
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (currentState != GameState.Playing) return;
            // Player movement
            if (upPressed && playerPaddle.Top > 0)
                playerPaddle.Y -= paddleSpeed;
            if (downPressed && playerPaddle.Bottom < this.ClientSize.Height)
                playerPaddle.Y += paddleSpeed;

            // AI movement (simple follow)
            if (ball.Y + ball.Height / 2 < aiPaddle.Y + paddleHeight / 2 && aiPaddle.Top > 0)
                aiPaddle.Y -= paddleSpeed;
            else if (ball.Y + ball.Height / 2 > aiPaddle.Y + paddleHeight / 2 && aiPaddle.Bottom < this.ClientSize.Height)
                aiPaddle.Y += paddleSpeed;

            // Ball movement
            ball.X += ballSpeedX;
            ball.Y += ballSpeedY;

            // Collision with top/bottom
            if (ball.Top <= 0 || ball.Bottom >= this.ClientSize.Height)
                ballSpeedY = -ballSpeedY;

            // Collision with paddles
            if (ball.IntersectsWith(playerPaddle) && ballSpeedX < 0)
                ballSpeedX = -ballSpeedX;
            if (ball.IntersectsWith(aiPaddle) && ballSpeedX > 0)
                ballSpeedX = -ballSpeedX;

            // Scoring
            if (ball.Left <= 0)
            {
                aiScore++;
                ResetBall();
            }
            else if (ball.Right >= this.ClientSize.Width)
            {
                playerScore++;
                ResetBall();
            }

            this.Invalidate();
        }

        private void ResetBall()
        {
            ball.X = (this.ClientSize.Width - ballSize) / 2;
            ball.Y = (this.ClientSize.Height - ballSize) / 2;
            ballSpeedX = -ballSpeedX;
        }

        private void Form1_Paint(object? sender, PaintEventArgs e)
        {
            if (currentState != GameState.Playing) return;
            Graphics g = e.Graphics;
            // Draw paddles and ball
            g.FillRectangle(Brushes.Black, pitchRect);  
            g.FillRectangle(Brushes.White, playerPaddle);
            g.FillRectangle(Brushes.White, aiPaddle);
            g.FillEllipse(Brushes.White, ball);
            // Draw scores
            using (Font font = new Font("Arial", 24))
            {
                g.DrawString($"{playerScore}", font, Brushes.White, 300, 20);
                g.DrawString($"{aiScore}", font, Brushes.White, 470, 20);
            }
        }

        private void Form1_KeyDown(object? sender, KeyEventArgs e)
        {
            if (currentState != GameState.Playing) return;
            if (e.KeyCode == Keys.W) upPressed = true;
            if (e.KeyCode == Keys.S) downPressed = true;
        }

        private void Form1_KeyUp(object? sender, KeyEventArgs e)
        {
            if (currentState != GameState.Playing) return;
            if (e.KeyCode == Keys.W) upPressed = false;
            if (e.KeyCode == Keys.S) downPressed = false;
        }
    }
}
