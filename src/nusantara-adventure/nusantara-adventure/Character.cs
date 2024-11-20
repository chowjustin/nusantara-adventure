using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Character : GameObject
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public Level CurrentLevel { get; set; }

        public Character(string name, int x, int y, int health, int speed)
            : base(name, x, y)
        {
            Health = health;
            Speed = speed;
        }

        public void Move(int deltaX, int deltaY)
        {
            X += deltaX * Speed;
            Y += deltaY * Speed;
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
            if (Health <= 0) Die();
        }

        public virtual void Die()
        {
            // Logic for character death
        }

        public void EnterLevel(Level level)
        {
            CurrentLevel = level;
        }

        public void CompleteLevel()
        {
            CurrentLevel.IsCompleted = true;
        }

        public override void Update()
        {
            // Update character-specific logic
        }
    }
}
