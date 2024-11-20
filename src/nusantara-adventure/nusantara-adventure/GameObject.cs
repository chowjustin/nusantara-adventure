using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class GameObject
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }

        public GameObject(string name, int x, int y)
        {
            Name = name;
            X = x;
            Y = y;
        }

        public virtual void Update()
        {
            // Update logic for general game objects
        }
    }
}
