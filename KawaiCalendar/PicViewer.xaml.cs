using Gabriel.Cat.S.Extension;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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

        public PicViewer(object objFilePath) : this() {
            FilePic =new FileInfo( objFilePath + "");
            img.SetImage(new Bitmap(FilePic.FullName));
            Title = $"Visualizando '{FilePic.Name}'";

        }
        public FileInfo FilePic { get; set; }

        private void Window_MouseLeftClick(object sender, MouseButtonEventArgs e)
        {


            Notifications.Wpf.Core.NotificationManager manager = new Notifications.Wpf.Core.NotificationManager();

            if (File.Exists(FilePic.FullName))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"\"{FilePic.FullName}\""
                });

            }
            else
            {
                manager.ShowAsync(new Notifications.Wpf.Core.NotificationContent()
                {
                    Title = "Archivo no encontrado",
                    Message = $"'{FilePic.Name}'",
                    Type = Notifications.Wpf.Core.NotificationType.Information

                });
            }
        }

        private void Window_MouseRightButtonDown(object sender=null, MouseButtonEventArgs e=null)
        {
            Notifications.Wpf.Core.NotificationManager manager;

            if (Directory.Exists(FilePic.Directory.FullName))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "explorer",
                    WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    Arguments = $"\"{FilePic.Directory.FullName}\""
                });
            }
            else
            {
                manager = new Notifications.Wpf.Core.NotificationManager();

                manager.ShowAsync(new Notifications.Wpf.Core.NotificationContent()
                    {
                        Title = "Carpeta no encontrada",
                        Message = $"'{FilePic.Directory.Name}'",
                        Type = Notifications.Wpf.Core.NotificationType.Information

                    });
                
            }
        }
    }
}
