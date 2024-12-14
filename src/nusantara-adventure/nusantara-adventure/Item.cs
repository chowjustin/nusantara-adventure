using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure
{
    internal class Item : GameObject
    {
        public int Value { get; set; }
        public int HealthBoost { get; set; }
        public bool IsCollected { get; set; }

        private Image foodImage;
        private int frameWidth;
        private int frameHeight;
        private int foodFrameX;
        private int foodFrameY;

        public Item(string name, int x, int y, int value, int healthBoost, int speedBoost, int width, int height)
            : base(name, x, y, width, height)
        {
            Value = value;
            HealthBoost = healthBoost;

            using (MemoryStream ms = new MemoryStream(Resource.foods))
            {
                foodImage = Image.FromStream(ms);
            }

            // Select random frame when item is created
            frameWidth = foodImage.Width / 3;
            frameHeight = foodImage.Height / 4;
            Random random = new Random();
            foodFrameX = random.Next(0, 3) * frameWidth;
            foodFrameY = random.Next(0, 4) * frameHeight;
        }

      

        public void Draw(Graphics g, int worldOffset)
        {
            // Draw the current frame at the player's position
            g.DrawImage(
                foodImage,
                new Rectangle(X - worldOffset, Y - 18, 50, 50), 
                new Rectangle(
                    foodFrameX,
                    foodFrameY,
                    frameWidth,
                    frameHeight
                ),                                      
                GraphicsUnit.Pixel
            );
        }
    }
}
