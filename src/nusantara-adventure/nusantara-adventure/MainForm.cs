namespace nusantara_adventure
{
    public partial class MainForm : Form
    {
        private GameWorld gameWorld;
        private System.Windows.Forms.Timer gameTimer;
        private int moveX = 0, moveY = 0;
        private bool jumpRequested = false;
        private Button restartButton;

        public MainForm()
        {
            InitializeComponent();
            InitializeGame();
            InitializeRestartButton();
        }
        private void InitializeRestartButton()
        {
            restartButton = new Button
            {
                Text = "Restart Game",
                Visible = false,
                Location = new Point(this.ClientSize.Width / 2 - 50, this.ClientSize.Height / 2 + 50),
                Size = new Size(100, 30)
            };
            restartButton.Click += RestartGame;
            this.Controls.Add(restartButton);
        }

       private void RestartGame(object sender, EventArgs e)
       {
            this.Controls.Clear();

            InitializeGame();
            InitializeRestartButton();

            Invalidate();
       }


        private void InitializeGame()
        {
            Player player = new Player("Justin", 0, 690, 100, 5);
            // Add some initial items or costumes if needed
            player.AddCostume(new Costume("Default", "Starting costume"));

            gameWorld = new GameWorld(player);

            gameWorld.StartGame();

            gameTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            // Check for game over condition
            if (gameWorld.Player.Health <= 0)
            {
                GameOver();
                return;
            }

            int verticalInput = 0;
            if (jumpRequested)
            {
                verticalInput = -1;
                jumpRequested = false;
            }

            gameWorld.Player.Move(moveX, verticalInput);
            gameWorld.Player.Update(); // Important: call Update for gravity

            Level currentLevel = gameWorld.GetCurrentLevel();
            foreach (var enemy in currentLevel.Enemies)
            {
                enemy.Update();
            }

            gameWorld.Update();
            Invalidate();
        }


        private void GameOver()
        {
            // Stop the game timer
            gameTimer.Stop();

            // Show restart button
            restartButton.Visible = true;

            // Trigger a repaint to show final state
            Invalidate();
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Space:
                    jumpRequested = true;
                    break;
                case Keys.S: moveY = 1; break;
                case Keys.A: moveX = -1; break;
                case Keys.D: moveX = 1; break;
            }
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.W:
                case Keys.Space:
                    jumpRequested = false;
                    break;
                case Keys.S:
                    moveY = 0;
                    break;
                case Keys.A:
                case Keys.D:
                    moveX = 0;
                    break;
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            var g = e.Graphics;
            var player = gameWorld.Player;

            // Draw Player
            g.FillRectangle(Brushes.Blue, player.X, player.Y, player.Width, player.Height);

            var currentLevel = gameWorld.GetCurrentLevel();
            foreach (var enemy in currentLevel.Enemies)
            {
                g.FillRectangle(Brushes.Red, enemy.X, enemy.Y, enemy.Width, enemy.Height);
                g.DrawString(enemy.Name, new Font("Arial", 10), Brushes.White, enemy.X, enemy.Y - 15);
            }

            // Draw HUD
            g.DrawString($"Health: {player.Health}", new Font("Arial", 12), Brushes.White, 10, 10);
            g.DrawString($"Score: {player.Score}", new Font("Arial", 12), Brushes.White, 10, 30);
            g.DrawString($"Costume: {player.CurrentCostume?.Name ?? "None"}", new Font("Arial", 12), Brushes.White, 10, 50);

            // Game Over text if player health is 0
            if (player.Health <= 0)
            {
                string gameOverText = "GAME OVER";
                SizeF textSize = g.MeasureString(gameOverText, new Font("Arial", 24, FontStyle.Bold));
                g.DrawString(
                    gameOverText,
                    new Font("Arial", 24, FontStyle.Bold),
                    Brushes.Red,
                    (this.ClientSize.Width - textSize.Width) / 2,
                    this.ClientSize.Height / 2
                );
            }
        }


        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
