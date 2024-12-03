using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace nusantara_adventure
{
    internal class Player : Character
    {
        // Sprite animation properties
        private const int TOTAL_FRAMES = 8;
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
        public int Score { get; set; }
        public Costume CurrentCostume { get; set; }
        public List<Costume> OwnedCostumes { get; set; }

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
            using (MemoryStream ms = new MemoryStream(Resource.rpg_sprite_walk))
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
                    new Rectangle(X - worldOffset, Y-18 , 50, 50),  // Destination rectangle
                    spriteRect,                                        // Source rectangle
                    GraphicsUnit.Pixel
                );
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
                currentRow = 3;      // Right
            else if (horizontalInput < 0)
                currentRow = 2;      // Left
            else if (verticalInput < 0)
                currentRow = 1;      // Up
            else if (verticalInput > 0)
                currentRow = 0;      // Down

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
            item.ApplyEffect(this);
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