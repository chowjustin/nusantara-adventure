using System;
using System.Collections.Generic;
using System.Linq;
using static nusantara_adventure.Wall;

namespace nusantara_adventure
{

    internal class GameWorld
    {
        public List<Level> Levels { get; set; }
        public Player Player { get; set; }
        public Costume FinishLine { get; set; }
        public int CurrentLevelIndex { get; set; }

        public GameWorld(Player player)
        {
            Player = player;
            Levels = new List<Level>();
            CurrentLevelIndex = 0;
            FinishLine = new Costume("level1", "level 1 finish", 3000, 0, 50, 800);
        }

        public void AddLevel(Level level)
        {
            Levels.Add(level);
        }

        public void StartGame(int selectedLevel)
        {
            Levels.Clear();

            // Create levels with increasing difficulty
            for (int i = 0; i < 5; i++)
            {
                Level level = new Level(i + 1);
                Levels.Add(level);
            }

            CurrentLevelIndex = selectedLevel;
            LoadCurrentLevel();
        }

        private void GenerateDynamicItems(Level level)
        {
            Random random = new Random();
            var currentLevel = GetCurrentLevel();

            int itemCount = currentLevel.LevelNumber;

            const int START_BUFFER_ZONE = 300; // Minimum distance from the start of the level

            for (int i = 0; i < itemCount; i++)
            {
                // Dynamic enemy attributes
                int x = random.Next(START_BUFFER_ZONE, 1500 * (currentLevel.LevelNumber));  // Random x position
                int value = random.Next(1, 3);  // Random health between 20-50
                int healthBoost = random.Next(20, 40);  // Random speed between 1-3
                int speedBoost = random.Next(5, 7);  // Random damage between 10-30
                int width = random.Next(50, 60);

                // Create and add enemy to level
                Item item = new Item(
                    "Power UP",
                    x: x,
                    y: 550,
                    value: value,
                    healthBoost: healthBoost,
                    speedBoost: speedBoost,
                    width: width,
                    height: width
                );

                level.AddItem(item);
            }
        }

        private void GenerateDynamicEnemies(Level level)
        {
            // Random number of enemies between 4 and 5
            Random random = new Random();
            var currentLevel = GetCurrentLevel();

            int enemyCount = 5 * (currentLevel.LevelNumber); // More enemies in higher levels

            const int START_BUFFER_ZONE = 300; // Minimum distance from the start of the level

            // Enemy types to choose from
            string[] enemyTypes = {
                "enemy1", "enemy2", "enemy3"
            };

            for (int i = 0; i < enemyCount; i++)
            {
                // Randomly select enemy type
                string enemyType = enemyTypes[random.Next(enemyTypes.Length)];

                // Dynamic enemy attributes
                int x = random.Next(START_BUFFER_ZONE, 1500 * (currentLevel.LevelNumber));  // Random x position
                int health = random.Next(20, 51);  // Random health between 20-50
                int speed = random.Next(1, 4);  // Random speed between 1-3
                int damage = random.Next(10, 31);  // Random damage between 10-30
                int defaultRight = random.Next(2);

                // Create and add enemy to level
                Enemy enemy = new Enemy(
                    $"{enemyType}{i + 1}",
                    x: x,
                    y: 690,
                    health: health,
                    speed: speed,
                    damage: damage,
                    defaultRight: defaultRight == 1,
                    width: 32,
                    height: 32
                );

                level.AddEnemy(enemy);
            }
        }

        private void GenerateDynamicTraps(Level level)
        {
            Random random = new Random();
            var currentLevel = GetCurrentLevel();

            int trapCount = 2 + currentLevel.LevelNumber;

            const int START_BUFFER_ZONE = 300; // Minimum distance from the start of the level

            string[] trapTypes = {
        "Spikes", "Fire", "Pitfall"
    };

            for (int i = 0; i < trapCount; i++)
            {
                string trapType = trapTypes[random.Next(trapTypes.Length)];
                int x, damage, width, height;
                bool isValidPosition;

                do
                {
                    // Generate trap attributes
                    x = random.Next(START_BUFFER_ZONE, 1500 * currentLevel.LevelNumber);
                    damage = random.Next(10, 20);
                    width = random.Next(50, 150);
                    height = 20;

                    // Check for overlap with walls
                    isValidPosition = true;
                    foreach (var wall in level.Walls)
                    {
                        if ((x > wall.X && x < wall.X + wall.Width) ||
                            (x + width < wall.X + wall.Width && x + width > wall.X))
                        {
                            isValidPosition = false;
                            break;
                        }
                    }
                } while (!isValidPosition); // Retry if the position is invalid

                Trap trap = new Trap(
                    $"{trapType}{i + 1}",
                    x: x,
                    y: 690,
                    damage: damage,
                    width: width,
                    height: height
                );

                level.AddTrap(trap);
            }
        }
        private void GenerateRandomWalls(Level level)
        {
            Random random = new Random();
            var currentLevel = GetCurrentLevel();

            int wallCount = 5 * (currentLevel.LevelNumber);

            const int MIN_WALL_SPACING = 200;
            const int START_BUFFER_ZONE = 300; // Minimum distance from the start of the level

            int lastWallX = START_BUFFER_ZONE;

            for (int i = 0; i < wallCount; i++)
            {
                int width = random.Next(30, 60);
                int height = random.Next(50, 100);

                int spikedHeight = random.Next(40, 50);

                int x = lastWallX + MIN_WALL_SPACING + random.Next(100);
                int y = 690 - height + 10;

                if (x > currentLevel.LevelNumber * 1500 - 200)
                {
                    break;
                }

                // Randomly choose wall type
                WallType wallType = (WallType)random.Next(Enum.GetValues(typeof(WallType)).Length);

                Wall wall = new Wall(
                    $"Wall{i + 1}",
                    x: x,
                    y: wallType == WallType.Spiked ? 670 : y,
                    width: width,
                    height: wallType == WallType.Spiked ? spikedHeight : height,
                    type: wallType
                );

                level.AddWall(wall);
                lastWallX = x;
            }
        }

        public void StartLevel(int index)
        {
            CurrentLevelIndex = index;

            LoadCurrentLevel();
        }

        public Level GetCurrentLevel()
        {
            return Levels[CurrentLevelIndex];
        }

        public void LoadCurrentLevel()
        {
            var currentLevel = Levels[CurrentLevelIndex];

            Player.X = 0;
            Player.Y = 690;
            Player.VerticalVelocity = 0;

            GenerateDynamicEnemies(currentLevel);
            GenerateDynamicTraps(currentLevel);
            GenerateRandomWalls(currentLevel);
            GenerateDynamicItems(currentLevel);
            
            FinishLine.X = currentLevel.LevelNumber * 1500;
        }

        public void CompleteCurrentLevel()
        {
            Levels[CurrentLevelIndex].CompleteLevel();
            CurrentLevelIndex++;

            if (CurrentLevelIndex >= Levels.Count)
            {
                // Reset or handle game completion
                CurrentLevelIndex = Levels.Count - 1; // Stay on last level
                return;
            }

            var currentLevel = GetCurrentLevel();
            // Load next level if available
            if (CurrentLevelIndex < Levels.Count)
            {
                Player.ResetEffect();
                LoadCurrentLevel();
            }
        }

        public void Update()
        {
            var currentLevel = GetCurrentLevel();

            // Simplified grounded check
            Player.IsGrounded = (Player.Y + Player.Height >= 100) || IsOnAnyWall(currentLevel);

            // Rest of the update method remains the same
            CheckEnemyCollisions(currentLevel);
            CheckItemCollections(currentLevel);
            CheckTrapCollisions(currentLevel);
            CheckWallCollisions(currentLevel);
            CheckEnemyWallCollisions(currentLevel);
            CheckTopCollisions(currentLevel);
            CheckFinishLineCollision(currentLevel);

            // Remove defeated enemies
            currentLevel.Enemies.RemoveAll(e => e.Health <= 0);
            currentLevel.Items.RemoveAll(i => i.IsCollected);
        }

        private void CheckFinishLineCollision(Level currentLevel)
        {
            if (IsColliding(Player, FinishLine))
            {
                CompleteCurrentLevel();
            }
        }
        private void CheckEnemyCollisions(Level currentLevel)
        {
            foreach (var enemy in currentLevel.Enemies.ToList())
            {
                if (IsTopCollision(Player, enemy))
                {
                    continue;
                }

                if (IsColliding(Player, enemy))
                {
                    enemy.Attack(Player);
                }
            }
        }

        private void CheckEnemyWallCollisions(Level currentLevel)
        {
            foreach (var enemy in currentLevel.Enemies)
            {
                foreach (var wall in currentLevel.Walls)
                {
                    if (IsColliding(enemy, wall))
                    {
                        HandleEnemyWallCollision(enemy, wall);
                    }
                }
            }
        }

        private void HandleEnemyWallCollision(Enemy enemy, Wall wall)
        {
            // Check if enemy is above the wall (platform collision)
            bool isAboveWall = enemy.Y + enemy.Height <= wall.Y + 10 &&
                              enemy.Y + enemy.Height >= wall.Y - 10 &&
                              enemy.X + enemy.Width > wall.X &&
                              enemy.X < wall.X + wall.Width;

            if (isAboveWall)
            {
                // Place enemy on top of wall
                enemy.Y = wall.Y - enemy.Height;

                // If it's a spiked wall, damage the enemy
                if (wall.Type == WallType.Spiked)
                {
                    enemy.TakeDamage(10);
                }
            }
            else
            {

                if (enemy.X + enemy.Width > wall.X && enemy.X < wall.X)
                {

                    enemy.X = wall.X - enemy.Width;
                    enemy.ReverseDirection();
                }
                else if (enemy.X < wall.X + wall.Width && enemy.X + enemy.Width > wall.X + wall.Width)
                {
                    // Collision from right side
                    enemy.X = wall.X + wall.Width;
                    enemy.ReverseDirection();
                }
            }
        }

        private void CheckItemCollections(Level currentLevel)
        {
            foreach (var item in currentLevel.Items.ToList())
            {
                if (IsColliding(Player, item) && !item.IsCollected)
                {
                    Player.CollectItem(item);
                    item.IsCollected = true;
                }
            }
        }

        private void CheckTrapCollisions(Level currentLevel)
        {
            foreach (var trap in currentLevel.Traps.ToList())
            {
                if (IsColliding(Player, trap))
                {
                    trap.Activate(Player);
                }
            }
        }

        private bool IsColliding(GameObject obj1, GameObject obj2)
        {
            return obj1.X < obj2.X + obj2.Width &&
                   obj1.X + obj1.Width > obj2.X &&
                   obj1.Y < obj2.Y + obj2.Height &&
                   obj1.Y + obj1.Height > obj2.Y;
        }

        private void CheckTopCollisions(Level currentLevel)
        {
            foreach (var enemy in currentLevel.Enemies.ToList())
            {
                if (IsTopCollision(Player, enemy))
                {

                    enemy.TakeDamage(0);


                    Player.IsGrounded = false;
                    currentLevel.Enemies.Remove(enemy);
                }

            }
        }

        private bool IsTopCollision(GameObject obj1, GameObject obj2)
        {
            // More precise top collision check for player-specific vertical velocity
            bool isVerticallyAligned =
                obj1.Y + obj1.Height >= obj2.Y &&
                obj1.Y + obj1.Height <= obj2.Y + obj2.Height / 2;

            // Check if objects are horizontally overlapping
            bool isHorizontallyAligned =
                obj1.X < obj2.X + obj2.Width &&
                obj1.X + obj1.Width > obj2.X;

            // For enemies and other objects, remove vertical velocity check
            if (obj1 is Player player)
            {
                isVerticallyAligned &= player.VerticalVelocity >= 0;
            }

            return isVerticallyAligned && isHorizontallyAligned;
        }

        private void CheckWallCollisions(Level currentLevel)
        {
            foreach (var wall in currentLevel.Walls)
            {
                if (IsColliding(Player, wall))
                {
                    HandleWallCollision(wall);
                }
            }
        }

        private void HandleWallCollision(Wall wall)
        {
            // More precise top collision check
            bool isTopCollision = IsTopCollision(Player, wall);

            if (isTopCollision)
            {
                // Ensure player is fully or mostly over the wall
                bool isFullyOverWall =
                    Player.X + Player.Width > wall.X &&
                    Player.X < wall.X + wall.Width;

                if (isFullyOverWall)
                {
                    // Place player on top of wall
                    Player.Y = wall.Y - Player.Height;
                    Player.CharIsGrounded = true;
                    Player.IsGrounded = true;
                    Player.VerticalVelocity = -0.5f;

                    // Apply damage if it's a spiked wall
                    if (wall.Type == WallType.Spiked)
                    {
                        Player.TakeDamage(10);
                    }
                }
            }
            else
            {
                // Handle horizontal collisions
                if (Player.X + Player.Width > wall.X && Player.X < wall.X)
                {
                    // Collision from left side
                    Player.X = wall.X - Player.Width;
                }
                else if (Player.X < wall.X + wall.Width && Player.X + Player.Width > wall.X + wall.Width)
                {
                    // Collision from right side
                    Player.X = wall.X + wall.Width;
                }
            }
        }

        private bool IsOnAnyWall(Level currentLevel)
        {
            // Small tolerance to account for floating-point imprecision
            const float tolerance = 0f;

            foreach (var wall in currentLevel.Walls)
            {
                // Check if player is within horizontal bounds of the wall
                bool horizontalOverlap =
                    Player.X + Player.Width > wall.X &&
                    Player.X < wall.X + wall.Width;

                // Check if player is just touching or slightly above the wall surface
                bool verticalCollision =
                    Math.Abs((Player.Y + Player.Height) - wall.Y) <= tolerance &&
                    Player.VerticalVelocity >= 0; // Only consider grounded when falling or at rest

                if (horizontalOverlap && verticalCollision)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
