using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Trap : GameObject
    {
        public int Damage { get; set; }

        public Trap(string name, int x, int y, int damage)
            : base(name, x, y)
        {
            Damage = damage;
        }

        public void Activate(Player player)
        {
            player.TakeDamage(Damage);
        }
    }
}
