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

        // New properties to support game state
        public List<Enemy> Enemies { get; private set; }
        public List<Item> Items { get; private set; }
        public List<Trap> Traps { get; private set; }

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
            CurrentLevelIndex = 0;
            LoadCurrentLevel();
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
            // Clear existing game objects
            Enemies.Clear();
            Items.Clear();
            Traps.Clear();

            // Load current level's objects
            var currentLevel = Levels[CurrentLevelIndex];
            Enemies.AddRange(currentLevel.Enemies);
            Items.AddRange(currentLevel.Items);
            Traps.AddRange(currentLevel.Traps);
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

            // Remove defeated enemies
            currentLevel.Enemies.RemoveAll(e => e.Health <= 0);
            currentLevel.Items.RemoveAll(i => i.IsCollected);
        }

        private void CheckEnemyCollisions(Level currentLevel)
        {
            foreach (var enemy in currentLevel.Enemies.ToList())
            {
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
    }
}