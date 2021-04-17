using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gabriel.Cat.S.Extension
{
    public static class BitmapExtension
    {
        public static Bitmap SetMaxHeight(this Bitmap bmp,float max)
        {
            Bitmap bmpResult = bmp;
            if (bmp.Height > max)
            {
                bmpResult = bmp.Escala(max / bmp.Height);
            }
            return bmpResult;
        }
    }
}
