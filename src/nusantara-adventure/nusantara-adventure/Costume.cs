using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Costume : GameObject
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Costume(string name, string description, int x, int y, int width, int height) : base("FinishLine", x, y, width, height)
        {
            Name = name;
            Description = description;
        }

        public void Wear()
        {
            // Logic for wearing the costume
        }

        public void Remove()
        {
            // Logic for removing the costume
        }
    }
}
