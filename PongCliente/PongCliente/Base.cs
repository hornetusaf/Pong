using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace PongCliente
{
    class Base
    {
        private Rectangle rec = new Rectangle();

        public Rectangle rectangulo
        {
            get { return rec; }
        }

        public int X
        {
            get { return rec.X; }
            set { rec.X = value; }
        }

        public int Y
        {
            get { return rec.Y; }
            set { rec.Y = value; }
        }

        public Size tamaño
        {
            set { rec.Size = value; }
        }
    }
}
