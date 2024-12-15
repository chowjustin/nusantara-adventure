using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nusantara_adventure;

public interface IDrawable
{
    public void Draw(Graphics g, int worldOffset);
}
