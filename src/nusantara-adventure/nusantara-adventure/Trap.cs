using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace nusantara_adventure
{
    internal class Trap : GameObject
    {
        public int Damage { get; set; }
        private Image spikeImage;

        public Trap(string name, int x, int y, int damage, int width, int height)
            : base(name, x, y, width, height)
        {
            Damage = damage;

            using (MemoryStream ms = new MemoryStream(Resource.spike))
            {
                spikeImage = Image.FromStream(ms);
            }
        }

        public void Activate(Player player)
        {
            player.TakeDamage(Damage);
        }

        public void Draw(Graphics g, int worldOffset)
        {
            // Draw the current frame at the player's position
            g.DrawImage(
                spikeImage,
                new Rectangle(X - worldOffset, Y-8, Width, Height)
            );
        }
    }
}
