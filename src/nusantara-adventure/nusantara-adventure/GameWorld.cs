using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            CurrentLevelIndex = 0;
            Levels[CurrentLevelIndex].StartLevel();
        }

        public void StartLevel(int index)
        {
            CurrentLevelIndex = index;
            Levels[CurrentLevelIndex].StartLevel();
        }

        public void CompleteCurrentLevel()
        {
            Levels[CurrentLevelIndex].CompleteLevel();
            CurrentLevelIndex++;
        }
    }

}
