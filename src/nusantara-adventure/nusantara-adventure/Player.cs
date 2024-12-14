using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace nusantara_adventure
{
    internal class Player : Character
    {
        // Sprite animation properties
        private const int TOTAL_FRAMES = 7;
        private const int SPRITE_ROWS = 4;
        private Image spriteSheet;
        private int currentFrame;
        private int currentRow;
        private bool isMoving;
        private Rectangle spriteRect;

        // Platform state tracking
        private bool wasOnPlatform;
        private int lastPlatformY;

        public bool IsGrounded;
        public bool isBoosted = false;
        public int Score { get; set; }
        public Costume CurrentCostume { get; set; }
        public List<Costume> OwnedCostumes { get; set; }

        private System.Threading.Timer boostTimer;
        public DateTime boostEndTime;

        public Player(string name, int x, int y, int health, int speed, int width, int height)
            : base(name, x, y, health, speed, width, height)
        {
            OwnedCostumes = new List<Costume>();

            InitializeSprites();
            currentFrame = 0;
            currentRow = 0; // Default facing down
            isMoving = false;
            IsGrounded = true;

            wasOnPlatform = false;
            UpdateSpriteRect();
        }
        public override void Update()
        {
            // If we were on a platform but aren't anymore, start falling
            if (wasOnPlatform && !IsGrounded)
            {
                VerticalVelocity = 0.5f; // Start with no vertical velocity
                IsGrounded = false;
            }

            base.Update(); // Apply normal gravity

            // Update platform state
            wasOnPlatform = IsGrounded;
            if (CharIsGrounded)
            {
                lastPlatformY = Y + Height;
            }
        }

        private void InitializeSprites()
        {
            using (MemoryStream ms = new MemoryStream(Resource.batik_sprite))
            {
                spriteSheet = Image.FromStream(ms);
            }
        }

        private void UpdateSpriteRect()
        {
            int frameWidth = spriteSheet.Width / TOTAL_FRAMES;
            int frameHeight = spriteSheet.Height / SPRITE_ROWS;
            spriteRect = new Rectangle(
                currentFrame * frameWidth,
                currentRow * frameHeight,
                frameWidth,
                frameHeight
            );
        }

        public void Draw(Graphics g, int worldOffset)
        {
            if (spriteSheet != null)
            {
                int frameWidth = spriteSheet.Width / TOTAL_FRAMES;
                int frameHeight = spriteSheet.Height / SPRITE_ROWS;

                // Draw the current frame at the player's position
                g.DrawImage(
                    spriteSheet,
                    new Rectangle(X - worldOffset, Y-28 , 50, 60),  // Destination rectangle
                    spriteRect,                                        // Source rectangle
                    GraphicsUnit.Pixel
                );
            }

            if (isBoosted)
            {
                int remainingTime = (int)Math.Max((boostEndTime - DateTime.Now).TotalSeconds, 0); // Ensure non-negative
                g.DrawString($"Boost Time: {remainingTime}s", new Font("Arial", 12), Brushes.White, 10, 70);
            }
        }

        public void Animate()
        {
            if (isMoving)
            {
                currentFrame = (currentFrame + 1) % TOTAL_FRAMES;
                UpdateSpriteRect();
            }
            else
            {
                currentFrame = 0;
                UpdateSpriteRect();
            }
        }

        public void Move(int horizontalInput, int verticalInput)
        {
            isMoving = horizontalInput != 0 || verticalInput != 0;

            // Update sprite direction based on movement
            if (horizontalInput > 0)
                currentRow = 1;      // Right
            else if (horizontalInput < 0)
                currentRow = 2;      // Left
            else if (horizontalInput > 0 && !IsGrounded)
                currentRow = 0;      // Up
            else if (horizontalInput < 0 && !IsGrounded)
                currentRow = 3;      // Down

            // Horizontal movement
            X += horizontalInput * Speed;

            // Jump logic
            if (verticalInput < 0 && IsGrounded)
            {
                Jump();
            }

            // Update animation frame if moving
            if (isMoving)
            {
                UpdateSpriteRect();
            }
        }

        public void CollectItem(Item item)
        {
            Score += item.Value;
            if (!isBoosted)
            {
                Speed += 5;
                isBoosted = true;
            }

            if (boostEndTime > DateTime.Now)
            {
                // If there is remaining time, add 2 seconds to the current boostEndTime
                boostEndTime = boostEndTime.AddMilliseconds(2000);
            }
            else
            {
                // If the boost has already expired, start a fresh 2 seconds from now
                boostEndTime = DateTime.Now.AddMilliseconds(2000);
            }

            // Reset or start the timer
            if (boostTimer == null)
            {
                boostTimer = new System.Threading.Timer(_ =>
                {
                    // Check if the current time is past the boost end time
                    if (DateTime.Now >= boostEndTime)
                    {
                        Speed = 5;
                        isBoosted = false;

                        // Dispose the timer
                        boostTimer.Dispose();
                        boostTimer = null;
                    }
                }, null, 0, 100); // Check every 100 ms
            }
        }

        public void AddCostume(Costume costume)
        {
            OwnedCostumes.Add(costume);
        }

        public void ChangeCostume(Costume costume)
        {
            if (OwnedCostumes.Contains(costume))
            {
                CurrentCostume = costume;
            }
        }

        public void ResetEffect()
        {
            Health = 100;
            Speed = 5;
        }

        public void RemoveCostume(Costume costume)
        {
            OwnedCostumes.Remove(costume);
            if (CurrentCostume == costume)
            {
                CurrentCostume = null;
            }
        }

        public void Attack(Enemy enemy)
        {
            // Logic for player attacking an enemy
        }
    }
}