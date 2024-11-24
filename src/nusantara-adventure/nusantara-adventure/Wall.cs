using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Wall : GameObject
    {
        public WallType Type { get; private set; }

        public Wall(string name, int x, int y, int width, int height, WallType type)
            : base(name, x, y, width, height)
        {
            Type = type;
        }

        public enum WallType
        {
            Regular,
            Spiked
        }
    }
}
