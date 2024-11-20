using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Costume
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public Costume(string name, string description)
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
