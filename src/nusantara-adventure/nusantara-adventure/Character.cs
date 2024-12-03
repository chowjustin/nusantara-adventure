using System;
using System.Collections.Generic;
namespace nusantara_adventure
{
    internal class Character : GameObject
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public Level CurrentLevel { get; set; }
        public float VerticalVelocity { get; set; }
        public bool CharIsGrounded { get; set; }

        protected const float GRAVITY = 0.5f;
        protected const float JUMP_STRENGTH = -10f;
        protected const int GROUND_LEVEL = 700; // Adjust based on form height

        public Character(string name, int x, int y, int health, int speed, int width, int height)
            : base(name, x, y, width, height)
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

        protected virtual void ApplyGravity()
        {
            // Apply vertical velocity
            Y += (int)VerticalVelocity;
            // Increase downward velocity
            VerticalVelocity += GRAVITY;
            // Check ground collision
            if (Y + Height >= GROUND_LEVEL)
            {
                Y = GROUND_LEVEL - Height;
                VerticalVelocity = 0;
                CharIsGrounded = true;
            }
  
        }

        protected virtual void Jump()
        {
            if (CharIsGrounded)
            {
                VerticalVelocity = JUMP_STRENGTH;
                CharIsGrounded = false;
            }
        }

        public override void Update()
        {
        
            ApplyGravity();
            
        }
    }
}