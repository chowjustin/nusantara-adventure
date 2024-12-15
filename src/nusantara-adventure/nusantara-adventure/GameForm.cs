using System.Media;

namespace nusantara_adventure
{
    public partial class GameForm : Form
    {
        private GameWorld gameWorld;
        private Button restartButton;
        private Button mainMenuButton;
        private SoundPlayer _deadSound;
        private SoundPlayer _jumpSound;
        private Image platformImage;
        private System.Windows.Forms.Timer gameTimer;

        private int selectedLevel;
        private bool jumpRequested = false;
        private bool gameCompleted = false;

        private int worldOffset = 0;
        private int moveX = 0, moveY = 0;
        private const int SCROLL_THRESHOLD = 600;

        public GameForm(int level)
        {
            selectedLevel = level;
            InitializeComponent();
            InitializeGame();
            InitializeRestartButton();
            InitializeMainMenuButton();
            InitializeResources();
        }

        private void InitializeResources()
        {
            using (MemoryStream ms = new MemoryStream(Resource.soil))
            {
                platformImage = Image.FromStream(ms);
            }

            var soundStream = new System.IO.MemoryStream(Resource.dead);
            _deadSound = new SoundPlayer(soundStream);
            var jumpSound = new System.IO.MemoryStream(Resource.jump);
            _jumpSound = new SoundPlayer(jumpSound);

            _deadSound.Load();
            _jumpSound.Load();

            gameWorld.Player.SetJumpSound(_jumpSound);

            this.SetStyle(ControlStyles.DoubleBuffer | ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint, true);
            this.UpdateStyles();
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

        private void InitializeMainMenuButton()
        {
            mainMenuButton = new Button
            {
                Text = "Main Menu",
                Visible = false,
                Location = new Point(this.ClientSize.Width / 2 - 50, this.ClientSize.Height / 2 + 50),
                Size = new Size(100, 30)
            };
            mainMenuButton.Click += ReturnToMainMenu;
            this.Controls.Add(mainMenuButton);
        }

        private void ReturnToMainMenu(object sender, EventArgs e)
        {
            MainForm mainForm = new MainForm();
            mainForm.Show();
            this.Close();
        }
        private void RestartGame(object sender, EventArgs e)
        {
            int currentLevelNumber = gameWorld.GetCurrentLevel().LevelNumber;

            this.Controls.Clear();
            worldOffset = 0;
            selectedLevel = currentLevelNumber - 1;

            InitializeGame();
            InitializeRestartButton();
            InitializeMainMenuButton();
            gameCompleted = false;
            Invalidate();
        }

        private void InitializeGame()
        {
            this.Size = new Size(1200, 800);
            this.MaximumSize = this.Size; 
            this.MinimumSize = this.Size;

            Player player = new Player();

            gameWorld = new GameWorld(player);
            gameWorld.StartGame(selectedLevel);

            gameTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
        }

        private void GameLoop(object sender, EventArgs e)
        {
            var currentLevel = gameWorld.GetCurrentLevel();

            if (gameWorld.Player.Health <= 0)
            {
                _deadSound.Play();
                GameOver();
                return;
            }

            if (currentLevel.LevelNumber == 5 && gameWorld.Player.X >= gameWorld.FinishLine.X)
            {
                GameCompleted();
                return;
            }

            if (gameWorld.Player.X == 0)
            {
                worldOffset = 0;
            }

            int verticalInput = 0;
            if (jumpRequested)
            {
                verticalInput = -1;
                jumpRequested = false;
            }

            int minAllowedX = worldOffset;
            if (moveX < 0 && gameWorld.Player.X <= minAllowedX)
            {
                moveX = 0;
                gameWorld.Player.X = minAllowedX;
            }

            int playerScreenPosition = gameWorld.Player.X - worldOffset;
            if (playerScreenPosition > SCROLL_THRESHOLD)
            {
                worldOffset += playerScreenPosition - SCROLL_THRESHOLD;
                gameWorld.Player.X = SCROLL_THRESHOLD + worldOffset;
            }

            gameWorld.Player.Move(moveX, verticalInput);
            gameWorld.Player.Animate(); 
            gameWorld.Player.Update();

            foreach (var enemy in currentLevel.Enemies)
            {
                enemy.Update();
            }

            gameWorld.Update();
            Invalidate();
        }


        private void GameOver()
        {
            gameTimer.Stop();
            restartButton.Visible = true;

            Invalidate();
        }

        public void GameCompleted()
        {
            gameTimer.Stop();

            mainMenuButton.Visible = true;
            gameCompleted = true;

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
            var g = e.Graphics;
            var player = gameWorld.Player;
            var currentLevel = gameWorld.GetCurrentLevel();

            PaintBackground(currentLevel, e);
            PaintFinishLine(currentLevel, e);

            player.Draw(g, worldOffset);

            foreach (var enemy in currentLevel.Enemies)
            {
                enemy.Draw(g, worldOffset);
            }

            foreach (var trap in currentLevel.Traps)
            {
                trap.Draw(g, worldOffset);
            }

            foreach (var item in currentLevel.Items)
            {
                item.Draw(g, worldOffset);
            }

            foreach (var wall in currentLevel.Walls)
            {
                wall.Draw(g, worldOffset);
            }

            BackgroundParallax(e);

            g.DrawString($"Health: {(player.Health >= 0 ? player.Health : 0)}", new Font("Arial", 12), Brushes.White, 10, 10);
            g.DrawString($"Level: {currentLevel.LevelNumber}", new Font("Arial", 12), Brushes.White, 10, 30);

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

            if (gameCompleted)
            {
                string gameCompletedText = "Game Completed!!";
                SizeF textSize = g.MeasureString(gameCompletedText, new Font("Arial", 24, FontStyle.Bold));
                g.DrawString(
                    gameCompletedText,
                    new Font("Arial", 24, FontStyle.Bold),
                    Brushes.Red,
                    (this.ClientSize.Width - textSize.Width) / 2,
                    this.ClientSize.Height / 2
                );
            }

            base.OnPaint(e);
        }

        private void PaintBackground(Level currentLevel, PaintEventArgs e)
        {
            var resourceName = $"bg{currentLevel.LevelNumber}";


            using (MemoryStream ms = new MemoryStream((byte[])Resource.ResourceManager.GetObject(resourceName)))
            {
                Image backgroundImage = Image.FromStream(ms);
                int backgroundWidth = backgroundImage.Width;
                int numTiles = (this.ClientSize.Width / backgroundWidth) + 2;

                for (int i = 0; i < numTiles; i++)
                {
                    int xPos = i * backgroundWidth - (worldOffset % backgroundWidth);

                    e.Graphics.DrawImage(backgroundImage,
                        new Rectangle(xPos, 0, backgroundWidth, this.ClientSize.Height)
                    );
                }
            }
        }

        private void PaintFinishLine(Level currentLevel, PaintEventArgs e)
        {
            var finishLine = gameWorld.FinishLine;
            int cellSize = 10;
            int rows = finishLine.Width / cellSize;
            int cols = this.ClientSize.Height / cellSize;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Brush brush = (i + j) % 2 == 0 ? Brushes.Black : Brushes.White;

                    e.Graphics.FillRectangle(
                        brush,
                        ((1500 * currentLevel.LevelNumber)+200) - worldOffset + (i * cellSize),
                        j * cellSize,
                        cellSize,
                        cellSize
                    );
                }
            }
        }

        private void BackgroundParallax(PaintEventArgs e)
        {
            int platformWidth = platformImage.Width;
            int numPlatformTiles = (this.ClientSize.Width / platformWidth) + 2;
            int platformOffset = worldOffset / 2;

            for (int i = 0; i < numPlatformTiles; i++)
            {
                int xPos = i * platformWidth - (worldOffset % platformWidth);

                e.Graphics.DrawImage(
                    platformImage,
                    new Rectangle(xPos, 700, platformWidth, 100)
                );
            }
        }


        private void GameForm_Load(object sender, EventArgs e) { }

    }
}
