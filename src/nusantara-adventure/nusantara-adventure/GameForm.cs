using System;
using System.Media;
using nusantara_adventure;
using static nusantara_adventure.Wall;

namespace nusantara_adventure
{
    public partial class GameForm : Form
    {
        private int selectedLevel;
        private GameWorld gameWorld;
        private System.Windows.Forms.Timer gameTimer;
        private int moveX = 0, moveY = 0;
        private bool jumpRequested = false;
        private Button restartButton;
        private Button mainMenuButton;
        private int worldOffset = 0;
        private const int SCROLL_THRESHOLD = 600;
        private Image platformImage;
        private bool gameCompleted = false;
        private SoundPlayer _deadSound;
        public SoundPlayer _jumpSound;
        public GameForm(int level)
        {
            selectedLevel = level;
            InitializeComponent();
            InitializeGame();
            InitializeRestartButton();
            InitializeMainMenuButton();

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
            //this.DialogResult = DialogResult.OK; 
            //this.Hide();
            mainMenuButton.Click += ReturnToMainMenu;
            this.Controls.Add(mainMenuButton);
        }

        private void ReturnToMainMenu(object sender, EventArgs e)
        {
            // Create and show the main form
            MainForm mainForm = new MainForm();
            mainForm.Show();

            // Close the current game form
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
            this.MaximumSize = this.Size; // Prevent resizing
            this.MinimumSize = this.Size;


            Player player = new Player("Justin", 0, 690, 100, 5, 32, 32);
            // Add some initial items or costumes if needed
            //player.AddCostume(new Costume("Default", "Starting costume"));

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
            // Check for game over condition
            if (gameWorld.Player.Health <= 0)
            {
                _deadSound.Play();
                GameOver();
                return;
            }
            var currentLevel = gameWorld.GetCurrentLevel();
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

            // Prevent moving left of the world offset
            if (moveX < 0 && gameWorld.Player.X <= minAllowedX)
            {
                moveX = 0;
                gameWorld.Player.X = minAllowedX;
            }

            int playerScreenPosition = gameWorld.Player.X - worldOffset;
            if (playerScreenPosition > SCROLL_THRESHOLD)
            {
                // Scroll world to the left
                worldOffset += playerScreenPosition - SCROLL_THRESHOLD;
                gameWorld.Player.X = SCROLL_THRESHOLD + worldOffset;
            }

            //gameWorld.Player.Move(moveX, verticalInput);
            //gameWorld.Player.Update();
            gameWorld.Player.Move(moveX, verticalInput);
            gameWorld.Player.Animate();  // Add this line to update animation
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
            // Stop the game timer
            gameTimer.Stop();

            // Show restart button
            restartButton.Visible = true;

            // Trigger a repaint to show final state
            Invalidate();
        }

        public void GameCompleted()
        {
            gameTimer.Stop();

            // Show restart button
            mainMenuButton.Visible = true;
            gameCompleted = true;

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
            var g = e.Graphics;
            var currentLevel = gameWorld.GetCurrentLevel();
            var resourceName = $"bg{currentLevel.LevelNumber}";
 

            using (MemoryStream ms = new MemoryStream((byte[])Resource.ResourceManager.GetObject(resourceName)))
            {
                Image backgroundImage = Image.FromStream(ms);
                int backgroundWidth = backgroundImage.Width;
                int numTiles = (this.ClientSize.Width / backgroundWidth) + 2; // +2 to ensure full coverage

                for (int i = 0; i < numTiles; i++)
                {
                    // Calculate the x position, accounting for world offset
                    int xPos = i * backgroundWidth - (worldOffset % backgroundWidth);

                    g.DrawImage(backgroundImage,
                        new Rectangle(xPos, 0, backgroundWidth, this.ClientSize.Height)
                    );
                }
            }

            base.OnPaint(e);
       
            var player = gameWorld.Player;
            var finishLine = gameWorld.FinishLine;

            // Draw Player
            //g.FillRectangle(Brushes.Blue, player.X - worldOffset, player.Y, player.Width, player.Height);
            player.Draw(g, worldOffset);


            g.FillRectangle(Brushes.Black, (1500 * currentLevel.LevelNumber) - worldOffset, 670, finishLine.Width, 50);
            g.DrawString("FINISH", new Font("Arial", 10), Brushes.Black, (1500*currentLevel.LevelNumber) - worldOffset, 655);

            foreach (var enemy in currentLevel.Enemies)
            {
                enemy.Draw(g, worldOffset);
            }

            foreach (var trap in currentLevel.Traps)
            {
                trap.Draw(g, worldOffset);
                //// Draw traps relative to world offset
                //g.FillRectangle(Brushes.Yellow, trap.X - worldOffset, trap.Y, trap.Width, trap.Height);
                //g.DrawString(trap.Name, new Font("Arial", 10), Brushes.White, trap.X - worldOffset, trap.Y - 15);
            }

            foreach (var item in currentLevel.Items)
            {
                item.Draw(g, worldOffset);
               
            }

            foreach (var wall in currentLevel.Walls)
            {
                wall.Draw(g, worldOffset);
            }

            // Draw HUD
            g.DrawString($"Health: {player.Health}", new Font("Arial", 12), Brushes.White, 10, 10);
            g.DrawString($"Score: {player.Score}", new Font("Arial", 12), Brushes.White, 10, 30);
            g.DrawString($"Level: {currentLevel.LevelNumber}", new Font("Arial", 12), Brushes.White, 10, 50);
            //g.DrawString($"Duration: {player.boostEndTime}", new Font("Arial", 12), Brushes.White, 10, 70);
            g.DrawString($"Speed: {player.isBoosted}", new Font("Arial", 12), Brushes.White, 10, 90);
            //g.DrawString($"playerScreenPosition: {playerScreenPosition}", new Font("Arial", 12), Brushes.White, 10, 110);

            //g.DrawImage(
            //    platformImage,
            //    new Rectangle(0, 700, 1200, 100)
            //);


            int platformWidth = platformImage.Width;
            int numPlatformTiles = (this.ClientSize.Width / platformWidth) + 2;

            // Adjust the parallax speed (you can tune this value)
            // Dividing worldOffset by a larger number will make it move slower than the background
            int platformOffset = worldOffset / 2;

            for (int i = 0; i < numPlatformTiles; i++)
            {
                int xPos = i * platformWidth - (worldOffset % platformWidth);

                g.DrawImage(
                    platformImage,
                    new Rectangle(xPos, 700, platformWidth, 100)
                );
            }

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

        }



        private void GameForm_Load(object sender, EventArgs e)
        {
        }

       
    }
}
