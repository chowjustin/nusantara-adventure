using System;

namespace nusantara_adventure
{
    internal class Enemy : Character, IDrawable
    {
        public int Damage { get; set; }
        private int initialX;
        private bool isMovingRight = true;

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
            initialX = x; 
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

                g.DrawImage(
                    spriteSheet,
                    new Rectangle(X - worldOffset, Y - 18, 50, 50), 
                    spriteRect,                       
                    GraphicsUnit.Pixel
                );
            }
        }

        private void AutoMove()
        {
            bool wasMoving = isMoving;
            isMoving = true;

            if (isMovingRight)
            {
                X += Speed;
                currentRow = 0; 

                if (X >= initialX + 10000)
                {
                    isMovingRight = false;
                    currentRow = 1;
                }
            }
            else
            {
                X -= Speed;
                currentRow = 1;

                if (X <= initialX - 10000)
                {
                    isMovingRight = true;
                    currentRow = 0; 
                }
            }

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

        public override void Update()
        {
            ApplyGravity();
            AutoMove();  
        }
    }
}
