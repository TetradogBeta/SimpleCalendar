using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace KawaiCalendar
{
    /// <summary>
    /// Lógica de interacción para PicViewer.xaml
    /// </summary>
    public partial class PicViewer : Window
    {
        public PicViewer()
        {
            InitializeComponent();
        }
        public PicViewer(object objFilePath):this() => img.SetImage(new Bitmap(objFilePath + ""));
    }
}
