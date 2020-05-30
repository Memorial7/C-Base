using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//添加以下两个引用
using System.Drawing;//因为要使用Bitmap
using System.Windows.Forms;//因为要使用Application.StartupPath

//7.3
namespace Minesweeper.BaseClass
{
    class LoadBitmap
    {
        public static Bitmap LoadBmp(string bmpFileName)
        {
            
            return new Bitmap(Application.StartupPath + "\\GamePictures\\" + bmpFileName + ".bmp");
        }

    }
}
