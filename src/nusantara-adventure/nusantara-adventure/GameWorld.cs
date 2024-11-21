using System;
using System.Collections.Generic;
using System.Linq;

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
        int x = random.Next(300, 2000);  // Random x position
                int damage = random.Next(10, 50);
                int width = random.Next(50, 150);  // Random width between 50-150
                int height = 20;


                // Create and add trap to level
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
                    // Player "jumps" after colliding with the top of an enemy
                    //Player.Jump();

                    // Enemy takes damage or dies
                    //enemy.TakeDamage(Player.Score); // Or apply a fixed damage value
                    //if (enemy.Health <= 0)
                    //{
                    //enemy.Die();
                    //}
                    enemy.TakeDamage(0);

                    currentLevel.Enemies.Remove(enemy);
                }

            }
        }

        private bool IsTopCollision(Player player, Enemy enemy)
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


    }

}
