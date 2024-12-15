using System;
using System.Collections.Generic;
namespace nusantara_adventure
{
    internal abstract class Character : GameObject
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public Level CurrentLevel { get; set; }
        public float VerticalVelocity { get; set; }
        public bool CharIsGrounded { get; set; }

        protected const float GRAVITY = 0.5f;
        protected const float JUMP_STRENGTH = -10f;
        protected const int GROUND_LEVEL = 700;

        public Character(string name, int x, int y, int health, int speed, int width, int height)
            : base(name, x, y, width, height)
        {
            Health = health;
            Speed = speed;
            
        }

        public void TakeDamage(int damage)
        {
            Health -= damage;
        }

        private void EnterLevel(Level level)
        {
            CurrentLevel = level;
        }

        private void CompleteLevel()
        {
            CurrentLevel.IsCompleted = true;
        }

        protected virtual void ApplyGravity()
        {
            Y += (int)VerticalVelocity;
            VerticalVelocity += GRAVITY;

            if (Y + Height >= GROUND_LEVEL)
            {
                Y = GROUND_LEVEL - Height;
                VerticalVelocity = 0;
                CharIsGrounded = true;
            }
  
        }

        protected void Jump()
        {
            if (CharIsGrounded)
            {
                VerticalVelocity = JUMP_STRENGTH;
                CharIsGrounded = false;
            }
        }

        public abstract void Update();
    }
}