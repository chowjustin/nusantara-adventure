using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Level
    {
        public int LevelNumber { get; set; }
        public List<Enemy> Enemies { get; set; }
        public List<Item> Items { get; set; }
        public List<Trap> Traps { get; set; }
        public bool IsCompleted { get; set; }

        public Level(int levelNumber)
        {
            LevelNumber = levelNumber;
            Enemies = new List<Enemy>();
            Items = new List<Item>();
            Traps = new List<Trap>();
        }

        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public void AddItem(Item item)
        {
            Items.Add(item);
        }

        public void AddTrap(Trap trap)
        {
            Traps.Add(trap);
        }

        public void StartLevel()
        {
            IsCompleted = false;
            // Initialize level-specific logic
        }

        public void CompleteLevel()
        {
            IsCompleted = true;
        }
    }

}
