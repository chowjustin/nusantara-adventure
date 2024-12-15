using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure;

internal class Wall : GameObject, IDrawable
{
    public WallType Type { get; private set; }
    private Image wallImage;

    public Wall(string name, int x, int y, int width, int height, WallType type)
        : base(name, x, y, width, height)
    {
        Type = type;
    }

    public void Draw(Graphics g, int worldOffset)
    {
        using (MemoryStream ms = new MemoryStream(Resource.wall))
        {
            wallImage = Image.FromStream(ms);
        }

        Brush wallBrush = Type switch
        {
            WallType.Regular => Brushes.Gray,
            WallType.Spiked => Brushes.DarkRed,
            _ => Brushes.Gray
        };

        g.DrawImage(
          wallImage,
          new Rectangle(X - worldOffset, Y, Width, Height)
        );

        if (Type == WallType.Spiked)
        {
            using (Pen spikePen = new Pen(Color.Red, 2))
            {
                int spikeCount = Width / 10;
                for (int i = 0; i < spikeCount; i++)
                {
                    int spikeX = X - worldOffset + (i * 10);
                    g.DrawLine(spikePen,
                        spikeX, Y,
                        spikeX + 5, Y - 5
                    );
                    g.DrawLine(spikePen,
                        spikeX + 5, Y - 5,
                        spikeX + 10, Y
                    );
                }
            }
        }
    }
    public enum WallType
    {
        Regular,
        Spiked
    }
}
