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
        public int CurrentLevelIndex { get; set; }

        public GameWorld(Player player)
        {
            Player = player;
            Levels = new List<Level>();
            CurrentLevelIndex = 0;
        }

        public void AddLevel(Level level)
        {
            Levels.Add(level);
        }

        public void StartGame()
        {
            Level level1 = new Level(1);
            Level level2 = new Level(2);

            AddLevel(level1);
            AddLevel(level2);

            CurrentLevelIndex = 0;

            LoadCurrentLevel();
        }

          private void GenerateDynamicEnemies(Level level)
            {
            // Random number of enemies between 4 and 5
            Random random = new Random();
            int enemyCount = random.Next(4, 12);

            // Enemy types to choose from
            string[] enemyTypes = {
                "Goomba", "Koopa", "Piranha", "Hammer Bro", "Bowser Jr"
            };

            for (int i = 0; i < enemyCount; i++)
            {
                // Randomly select enemy type
                string enemyType = enemyTypes[random.Next(enemyTypes.Length)];

                // Dynamic enemy attributes
                int x = random.Next(200, 3000);  // Random x position
                int health = random.Next(20, 51);  // Random health between 20-50
                int speed = random.Next(1, 4);  // Random speed between 1-3
                int damage = random.Next(10, 31);  // Random damage between 10-30
                int defaultRight = random.Next(2);
                int width = random.Next(32, 40);
                int height = random.Next(32, 40);

                // Create and add enemy to level
                Enemy enemy = new Enemy(
                    $"{enemyType}{i + 1}",
                    x: x,
                    y: 690,
                    health: health,
                    speed: speed,
                    damage: damage,
                    defaultRight: defaultRight == 1,
                    width: width,
                    height: height
                );

                level.AddEnemy(enemy);
            }
        }

        private void GenerateDynamicTraps(Level level)
        {
            // Random number of traps between 2 and 4
            Random random = new Random();
            int trapCount = random.Next(2, 5);

            // Trap types to choose from (for example, spike traps, fire traps, etc.)
            string[] trapTypes = {
                "Spikes", "Fire", "Pitfall"
            };

            for (int i = 0; i < trapCount; i++)
            {
                // Randomly select trap type
                string trapType = trapTypes[random.Next(trapTypes.Length)];

                // Dynamic trap attributes
                int x = random.Next(300, 2000); 
                        int damage = random.Next(10, 50);
                        int width = random.Next(50, 150);  
                        int height = 20;

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
            int wallCount = random.Next(5, 15);

            const int MIN_WALL_SPACING = 200;
            int lastWallX = 300;

            for (int i = 0; i < wallCount; i++)
            {
               
                int width = random.Next(30, 60);
                int height = random.Next(50, 100);

              
                int x = lastWallX + MIN_WALL_SPACING + random.Next(100);
                int y = 690 - height + 10 ;

                // Randomly choose wall type
                WallType wallType = (WallType)random.Next(Enum.GetValues(typeof(WallType)).Length);

                Wall wall = new Wall(
                    $"Wall{i + 1}",
                    x: x,
                    y: y,
                    width: width,
                    height: height,
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

            GenerateDynamicEnemies(currentLevel);
            GenerateDynamicTraps(currentLevel);
            GenerateRandomWalls(currentLevel);
        }

        public void CompleteCurrentLevel()
        {
            Levels[CurrentLevelIndex].CompleteLevel();
            CurrentLevelIndex++;

            // Load next level if available
            if (CurrentLevelIndex < Levels.Count)
            {
                LoadCurrentLevel();
            }
        }


        public void Update()
        {
            var currentLevel = GetCurrentLevel();

            // Check for collisions and interactions
            CheckEnemyCollisions(currentLevel);
            CheckItemCollections(currentLevel);
            CheckTrapCollisions(currentLevel);
            CheckWallCollisions(currentLevel);
            CheckEnemyWallCollisions(currentLevel);
            CheckTopCollisions(currentLevel);

            // Remove defeated enemies
            currentLevel.Enemies.RemoveAll(e => e.Health <= 0);
            currentLevel.Items.RemoveAll(i => i.IsCollected);
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

                    currentLevel.Enemies.Remove(enemy);
                }

            }
        }

        private bool IsTopCollision(GameObject player, GameObject enemy)
        {
            //Check if the player's bottom collides with the enemy's top
            // Check if the player's bottom overlaps with the enemy's top
            bool isVerticallyAligned = player.Y + player.Height >= enemy.Y &&
                                       player.Y + player.Height <= enemy.Y + enemy.Height / 2;

            // Check if the player and enemy horizontally overlap
            bool isHorizontallyAligned = player.X < enemy.X + enemy.Width &&
                                         player.X + player.Width > enemy.X;

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
            // Check if player is above the wall (platform collision)
            bool isAboveWall = Player.Y + Player.Height <= wall.Y + 10 &&
                              Player.Y + Player.Height >= wall.Y - 10 &&
                              Player.X + Player.Width > wall.X &&
                              Player.X < wall.X + wall.Width;

            if (isAboveWall)
            {
                // Place player on top of wall
                Player.Y = wall.Y - Player.Height;

                // Apply damage if it's a spiked wall
                if (wall.Type == WallType.Spiked)
                {
                    Player.TakeDamage(10);
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
    


    }

}
