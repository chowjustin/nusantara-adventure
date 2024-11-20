using System;
using System.Collections.Generic;
namespace nusantara_adventure
{
    internal class Character : GameObject
    {
        public int Health { get; set; }
        public int Speed { get; set; }
        public Level CurrentLevel { get; set; }
        public float VerticalVelocity { get; protected set; }
        public bool IsGrounded { get; protected set; }
        protected const float GRAVITY = 0.5f;
        protected const float JUMP_STRENGTH = -10f;
        protected const int GROUND_LEVEL = 700; // Adjust based on form height

        public Character(string name, int x, int y, int health, int speed)
            : base(name, x, y)
        {
            Health = health;
            Speed = speed;
            IsGrounded = false;
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
                IsGrounded = true;
            }
            else
            {
                IsGrounded = false;
            }
        }

        protected virtual void Jump()
        {
            if (IsGrounded)
            {
                VerticalVelocity = JUMP_STRENGTH;
                IsGrounded = false;
            }
        }

        public override void Update()
        {
            ApplyGravity();
        }
    }
}