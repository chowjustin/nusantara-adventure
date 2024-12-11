using System;

namespace nusantara_adventure
{
    internal class Enemy : Character
    {
        public int Damage { get; set; }
        private int initialX;
        private bool isMovingRight = true;  // Direction flag for horizontal movement

        private Image spriteSheet;
        private int currentFrame;
        private int currentRow;
        private bool isMoving;
        private Rectangle spriteRect;
        private const int TOTAL_FRAMES = 6;
        private const int SPRITE_ROWS = 2;

        public Enemy(string name, int x, int y, int health, int speed, int damage, bool defaultRight, int width, int height)
            : base(name, x, y, health, speed, width, height)
        {
            Damage = damage;
            initialX = x; // Store the original X position
            isMovingRight = defaultRight;

            InitializeSprites();
            if (isMovingRight)
            {
                currentRow = 0;
            }
            else
            {
                currentRow = 1;
            }
            currentFrame = 0;

            UpdateSpriteRect();
        }

        public void Attack(Player player)
        {
            player.TakeDamage(Damage);
        }

        public override void Die()
        {
            // Custom death logic for enemy
        }

        private void InitializeSprites()
        {
            using (MemoryStream ms = new MemoryStream(Resource.enemy1))
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
                    new Rectangle(X - worldOffset, Y - 18, 50, 50),  // Destination rectangle
                    spriteRect,                                        // Source rectangle
                    GraphicsUnit.Pixel
                );
            }
        }

        // Method to automatically move the enemy
        public void AutoMove()
        {
            // Set moving state based on horizontal movement
            bool wasMoving = isMoving;
            isMoving = true;

            if (isMovingRight)
            {
                // Move to the right
                X += Speed;
                currentRow = 0;  // Right-facing row

                // If the enemy has moved 100 units from the starting position, stop moving
                if (X >= initialX + 10000)
                {
                    isMovingRight = false;
                    currentRow = 1;  // Left-facing row
                }
            }
            else
            {
                X -= Speed;
                currentRow = 1;  // Left-facing row

                if (X <= initialX - 10000)
                {
                    isMovingRight = true;
                    currentRow = 0;  // Right-facing row
                }
            }

            // Animate - similar to Player class logic
            if (isMoving)
            {
                currentFrame = (currentFrame + 1) % TOTAL_FRAMES;
            }
            else
            {
                currentFrame = 0;
            }

            UpdateSpriteRect();
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
