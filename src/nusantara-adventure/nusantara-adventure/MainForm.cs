namespace nusantara_adventure
{
    public partial class MainForm : Form
    {
        private GameWorld gameWorld;
        private System.Windows.Forms.Timer gameTimer;
        private int moveX = 0, moveY = 0;
        private bool jumpRequested = false;

        public MainForm()
        {
            InitializeComponent();
            InitializeGame();
        }

        private void InitializeGame()
        {
            Level level1 = new Level(1);
            Level level2 = new Level(2);

            Player player = new Player("Justin", 0, 0, 100, 5);

            // Add some initial items or costumes if needed
            player.AddCostume(new Costume("Default", "Starting costume"));

            gameWorld = new GameWorld(player);

            if (level1.LevelNumber == 1)
            {
                GenerateDynamicEnemies(level1);
            }

            gameWorld.AddLevel(level1);
            gameWorld.AddLevel(level2);

            gameTimer = new System.Windows.Forms.Timer { Interval = 16 }; // ~60 FPS
            gameTimer.Tick += GameLoop;
            gameTimer.Start();

            this.KeyDown += OnKeyDown;
            this.KeyUp += OnKeyUp;
        }

        private void GameLoop(object sender, EventArgs e)
        {
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

        private void GenerateDynamicEnemies(Level level)
        {
            // Random number of enemies between 4 and 5
            Random random = new Random();
            int enemyCount = random.Next(4, 6);

            // Enemy types to choose from
            string[] enemyTypes = {
        "Goomba", "Koopa", "Piranha", "Hammer Bro", "Bowser Jr"
    };

            for (int i = 0; i < enemyCount; i++)
            {
                // Randomly select enemy type
                string enemyType = enemyTypes[random.Next(enemyTypes.Length)];

                // Dynamic enemy attributes
                int x = random.Next(200, 1000);  // Random x position
                int health = random.Next(20, 51);  // Random health between 20-50
                int speed = random.Next(1, 4);  // Random speed between 1-3
                int damage = random.Next(10, 31);  // Random damage between 10-30
                int defaultRight = random.Next(2);

                // Create and add enemy to level
                Enemy enemy = new Enemy(
                    $"{enemyType}{i + 1}",
                    x: x,
                    y: 0,
                    health: health,
                    speed: speed,
                    damage: damage,
                    defaultRight: defaultRight == 1
                );

                level.AddEnemy(enemy);
            }
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
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
        }
    }
}
