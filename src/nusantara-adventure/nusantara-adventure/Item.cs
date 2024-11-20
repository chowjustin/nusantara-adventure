using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Item : GameObject
    {
        public int Value { get; set; }
        public int HealthBoost { get; set; }
        public int SpeedBoost { get; set; }
        public bool IsCollected { get; set; }

        public Item(string name, int x, int y, int value, int healthBoost, int speedBoost, int width, int height)
            : base(name, x, y, width, height)
        {
            Value = value;
            HealthBoost = healthBoost;
            SpeedBoost = speedBoost;
        }

        public void ApplyEffect(Player player)
        {
            player.Health += HealthBoost;
            player.Speed += SpeedBoost;
        }
    }
}
