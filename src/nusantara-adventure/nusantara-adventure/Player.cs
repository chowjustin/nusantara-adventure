using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Numerics;

namespace nusantara_adventure
{
    internal class Player : Character, IDrawable
    {
        private const int TOTAL_FRAMES = 7;
        private const int SPRITE_ROWS = 4;
        private Image spriteSheet;
        private int currentFrame;
        private int currentRow;
        private bool isMoving;
        private Rectangle spriteRect;

        private bool wasOnPlatform;
        private int lastPlatformY;

        public bool IsGrounded;
        public bool isBoosted = false;
        public int Score { get; set; }

        private System.Threading.Timer boostTimer;
        public DateTime boostEndTime;

        public Player(string name, int x, int y, int health, int speed, int width, int height)
            : base(name, x, y, health, speed, width, height)
        {

            InitializeSprites();
            currentFrame = 0;
            currentRow = 0;
            isMoving = false;
            IsGrounded = true;

            wasOnPlatform = false;
            UpdateSpriteRect();
        }

        public Player() : this("Player1", 0, 690, 100, 5, 32, 32)
        {
        }

        public override void Update()
        {
            if (wasOnPlatform && !IsGrounded)
            {
                VerticalVelocity = 0.5f; 
                IsGrounded = false;
            }

            ApplyGravity();

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

                g.DrawImage(
                    spriteSheet,
                    new Rectangle(X - worldOffset, Y-28 , 50, 60),  
                    spriteRect,                                       
                    GraphicsUnit.Pixel
                );
            }

            if (isBoosted)
            {
                int remainingTime = (int)Math.Max((boostEndTime - DateTime.Now).TotalSeconds, 0);
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

            if (horizontalInput > 0)
                currentRow = 1;     
            else if (horizontalInput < 0)
                currentRow = 2;   
            else if (horizontalInput > 0 && !IsGrounded)
                currentRow = 0;      
            else if (horizontalInput < 0 && !IsGrounded)
                currentRow = 3;    

            X += horizontalInput * Speed;

            if (verticalInput < 0 && IsGrounded)
            {
                Jump();
            }

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
                boostEndTime = boostEndTime.AddMilliseconds(2000);
            }
            else
            {
                boostEndTime = DateTime.Now.AddMilliseconds(2000);
            }

            if (boostTimer == null)
            {
                boostTimer = new System.Threading.Timer(_ =>
                {
                    if (DateTime.Now >= boostEndTime)
                    {
                        Speed = 5;
                        isBoosted = false;

                        boostTimer.Dispose();
                        boostTimer = null;
                    }
                }, null, 0, 100); 
            }
        }

        public void ResetEffect()
        {
            Health = 100;
            Speed = 5;
        }
    }
}