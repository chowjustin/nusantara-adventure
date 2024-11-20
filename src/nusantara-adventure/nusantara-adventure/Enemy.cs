using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Enemy : Character
{
    public int Damage { get; set; }

    public Enemy(string name, int x, int y, int health, int speed, int damage)
        : base(name, x, y, health, speed)
    {
        Damage = damage;
    }

    public void Attack(Player player)
    {
        player.TakeDamage(Damage);
    }

    public override void Die()
    {
        // Custom death logic for enemy
    }
}
}
