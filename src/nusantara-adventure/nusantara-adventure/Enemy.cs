using System;

namespace nusantara_adventure
{
    internal class Enemy : Character
    {
        public int Damage { get; set; }
        private int initialX;
        private bool isMovingRight = true;  // Direction flag for horizontal movement

        public Enemy(string name, int x, int y, int health, int speed, int damage, bool defaultRight, int width, int height)
            : base(name, x, y, health, speed, width, height)
        {
            Damage = damage;
            initialX = x; // Store the original X position
            isMovingRight = defaultRight;
        }

        public void Attack(Player player)
        {
            player.TakeDamage(Damage);
        }

        public override void Die()
        {
            // Custom death logic for enemy
        }

        // Method to automatically move the enemy
        public void AutoMove()
        {
            if (isMovingRight)
            {
                // Move to the right
                X += Speed;

                // If the enemy has moved 100 units from the starting position, stop moving
                if (X >= initialX + 100)
                {
                    isMovingRight = false;
                }
            }
            else
            {
                X-= Speed;

                if (X <= initialX - 100)
                {
                    isMovingRight = true;
                }
            }
        }

        public void ReverseDirection()
        {
            isMovingRight = !isMovingRight;
            
        }

        // Override the update method if you have one to call AutoMove
        public void Update()
        {
            ApplyGravity();
            AutoMove();  // This should be called in the main game loop or update cycle
        }
    }
}
