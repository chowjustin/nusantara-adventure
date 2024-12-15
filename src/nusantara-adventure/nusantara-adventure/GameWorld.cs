using System;
using System.Collections.Generic;
using System.Linq;
using System.Media;
using static nusantara_adventure.Wall;

namespace nusantara_adventure;

internal class GameWorld
{
    public Player Player { get; set; }
    public List<Level> Levels { get; set; }
    public GameObject FinishLine { get; set; }

    private SoundPlayer _eatSound;
    private Random random = new Random();
    public static int CurrentLevelIndex { get; set; }

    private const int TOTAL_LEVEL = 5;
    private const int START_BUFFER_ZONE = 300;

    public GameWorld(Player player)
    {
        Player = player;
        Levels = new List<Level>();
        CurrentLevelIndex = 0;
        FinishLine = new GameObject("Finish Line", 3000, 0, 50, 800);

        var soundStream = new System.IO.MemoryStream(Resource.eat);
        _eatSound = new SoundPlayer(soundStream);
        _eatSound.Load();
    }

    public void AddLevel(Level level)
    {
        Levels.Add(level);
    }

    public void StartGame(int selectedLevel)
    {
        Levels.Clear();

        for (int i = 0; i < TOTAL_LEVEL; i++)
        {
            Level level = new Level(i + 1);
            Levels.Add(level);
        }

        CurrentLevelIndex = selectedLevel;
        LoadCurrentLevel();
    }

    private void GenerateDynamicItems(Level currentLevel)
    {
        int itemCount = currentLevel.LevelNumber;

        for (int i = 0; i < itemCount; i++)
        {
            int x = random.Next(START_BUFFER_ZONE, 1500 * (currentLevel.LevelNumber));
            int value = random.Next(1, 3);
            int healthBoost = random.Next(20, 40);  
            int speedBoost = random.Next(5, 7); 
            int width = random.Next(50, 60);

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

            currentLevel.AddItem(item);
        }
    }

    private void GenerateDynamicEnemies(Level currentLevel)
    {
        int enemyCount = 5 * (currentLevel.LevelNumber); 

        string[] enemyTypes = {
            "enemy1", "enemy2", "enemy3"
        };

        for (int i = 0; i < enemyCount; i++)
        {
            string enemyType = enemyTypes[random.Next(enemyTypes.Length)];

            int x = random.Next(START_BUFFER_ZONE, 1500 * (currentLevel.LevelNumber));
            int health = random.Next(20, 51); 
            int speed = random.Next(1, 4);  
            int damage = random.Next(10, 31);  
            int defaultRight = random.Next(2);

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

            currentLevel.AddEnemy(enemy);
        }
    }

    private void GenerateDynamicTraps(Level currentLevel)
    {
        int trapCount = 2 + currentLevel.LevelNumber;

        string[] trapTypes = {"Spikes", "Fire"};

        for (int i = 0; i < trapCount; i++)
        {
            string trapType = trapTypes[random.Next(trapTypes.Length)];
            int x, damage, width, height;
            bool isValidPosition;

            do
            {
                x = random.Next(START_BUFFER_ZONE, 1500 * currentLevel.LevelNumber);
                damage = random.Next(10, 20);
                width = random.Next(50, 100);
                height = 20;
                isValidPosition = true;

                foreach (var wall in currentLevel.Walls)
                {
                    if (x < wall.X + wall.Width && x + width > wall.X)
                    {
                        isValidPosition = false;
                        break;
                    }
                }

                foreach (var existingTrap in currentLevel.Traps)
                {
                    if (x < existingTrap.X + existingTrap.Width && x + width > existingTrap.X)
                    {
                        isValidPosition = false;
                        break;
                    }
                }
            } while (!isValidPosition);

            Trap trap = new Trap(
                $"{trapType}{i + 1}",
                x: x,
                y: 690,
                damage: damage,
                width: width,
                height: height
            );

            currentLevel.AddTrap(trap);
        }
    }

    private void GenerateRandomWalls(Level currentLevel)
    {
        int wallCount = 5 * (currentLevel.LevelNumber);

        const int MIN_WALL_SPACING = 200;

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

            bool isValidPosition = true;
            foreach (var trap in currentLevel.Traps)
            {
                if (x < trap.X + trap.Width && x + width > trap.X)
                {
                    isValidPosition = false;
                    break;
                }
            }

            if (!isValidPosition)
            {
                continue;
            }

            WallType wallType = (WallType)random.Next(Enum.GetValues(typeof(WallType)).Length);

            Wall wall = new Wall(
                $"Wall{i + 1}",
                x: x,
                y: wallType == WallType.Spiked ? 670 : y,
                width: width,
                height: wallType == WallType.Spiked ? spikedHeight : height,
                type: wallType
            );

            currentLevel.AddWall(wall);
            lastWallX = x;
        }
    }

    private void StartLevel(int index)
    {
        CurrentLevelIndex = index;
        LoadCurrentLevel();
    }

    public Level GetCurrentLevel()
    {
        return Levels[CurrentLevelIndex];
    }

    private void LoadCurrentLevel()
    {
        var currentLevel = Levels[CurrentLevelIndex];

        Player.X = 0;
        Player.Y = 690;
        Player.VerticalVelocity = 0;
        Player.Speed = 5;

        GenerateDynamicEnemies(currentLevel);
        GenerateRandomWalls(currentLevel);
        GenerateDynamicTraps(currentLevel);
        GenerateDynamicItems(currentLevel);
        
        FinishLine.X = (currentLevel.LevelNumber * 1500)+200;
    }

    private void CompleteCurrentLevel()
    {
        Levels[CurrentLevelIndex].CompleteLevel();
        CurrentLevelIndex++;

        if (CurrentLevelIndex >= Levels.Count)
        {
            CurrentLevelIndex = Levels.Count - 1; 
            return;
        }

        if (CurrentLevelIndex < Levels.Count)
        {
            Player.ResetEffect();
            LoadCurrentLevel();
        }
    }

    public void Update()
    {
        var currentLevel = GetCurrentLevel();

        Player.IsGrounded = (Player.Y + Player.Height >= 100) || IsOnAnyWall(currentLevel);

        CheckEnemyCollisions(currentLevel);
        CheckItemCollections(currentLevel);
        CheckTrapCollisions(currentLevel);
        CheckWallCollisions(currentLevel);
        CheckTopCollisions(currentLevel);
        CheckFinishLineCollision(currentLevel);

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



    private void HandleEnemyWallCollision(Enemy enemy, Wall wall)
    {

        if (enemy.X + enemy.Width > wall.X && enemy.X < wall.X)
        {
            enemy.X = wall.X - enemy.Width;
            enemy.ReverseDirection();
        }
        else if (enemy.X < wall.X + wall.Width && enemy.X + enemy.Width > wall.X + wall.Width)
        {
            enemy.X = wall.X + wall.Width;
            enemy.ReverseDirection();
        }
        
    }

    private void CheckItemCollections(Level currentLevel)
    {
        foreach (var item in currentLevel.Items.ToList())
        {
            if (IsColliding(Player, item) && !item.IsCollected)
            {
                _eatSound.Play();
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
        bool isVerticallyAligned =
            obj1.Y + obj1.Height >= obj2.Y &&
            obj1.Y + obj1.Height <= obj2.Y + obj2.Height / 2;

        bool isHorizontallyAligned =
            obj1.X < obj2.X + obj2.Width &&
            obj1.X + obj1.Width > obj2.X;

        if (obj1 is Player player)
        {
            isVerticallyAligned &= player.VerticalVelocity >= 0;
        }

        return isVerticallyAligned && isHorizontallyAligned;
    }
    private void CheckWallCollisions(Level currentLevel)
    {
        foreach (var enemy in currentLevel.Enemies)
        {
            foreach (var wall in currentLevel.Walls)
            {
                if (IsColliding(enemy, wall))
                {
                    HandleEnemyWallCollision(enemy, wall);
                }
                if (IsColliding(Player, wall))
                {
                    HandleWallCollision(wall);
                }
            }
        }
    }
    private void HandleWallCollision(Wall wall)
    {
        bool isTopCollision = IsTopCollision(Player, wall);

        if (isTopCollision)
        {
            bool isFullyOverWall =
                Player.X + Player.Width > wall.X &&
                Player.X < wall.X + wall.Width;

            if (isFullyOverWall)
            {
                Player.Y = wall.Y - Player.Height;
                Player.CharIsGrounded = true;
                Player.IsGrounded = true;
                Player.VerticalVelocity = -0.5f;

                if (wall.Type == WallType.Spiked)
                {
                    Player.TakeDamage(10);
                }
            }
        }
        else
        {
            if (Player.X + Player.Width > wall.X && Player.X < wall.X)
            {
                Player.X = wall.X - Player.Width;
            }
            else if (Player.X < wall.X + wall.Width && Player.X + Player.Width > wall.X + wall.Width)
            {
                Player.X = wall.X + wall.Width;
            }
        }
    }

    private bool IsOnAnyWall(Level currentLevel)
    {
        const float tolerance = 0f;

        foreach (var wall in currentLevel.Walls)
        {
            bool horizontalOverlap =
                Player.X + Player.Width > wall.X &&
                Player.X < wall.X + wall.Width;

            bool verticalCollision =
                Math.Abs((Player.Y + Player.Height) - wall.Y) <= tolerance &&
                Player.VerticalVelocity >= 0; 

            if (horizontalOverlap && verticalCollision)
            {
                return true;
            }
        }

        return false;
    }
}
