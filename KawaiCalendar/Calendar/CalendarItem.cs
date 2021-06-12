using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using Gabriel.Cat.S.Binaris;
using Gabriel.Cat.S.Extension;

namespace KawaiCalendar.Calendar
{
    public class CalendarItem : IElementoBinarioComplejo, ISaveAndLoad, IComparable, IComparable<CalendarItem>
    {
        public const string EXTENSION_DEFAULT = ".jpeg";
        public const string EXTENSION_TRANSPARENT = ".png";
        public const int MAX = 250;
        public static readonly System.Drawing.Imaging.ImageFormat FormatoDefault = System.Drawing.Imaging.ImageFormat.Jpeg;
        public static readonly System.Drawing.Imaging.ImageFormat FormatoTransparent = System.Drawing.Imaging.ImageFormat.Png;

        public static ElementoBinario Serializador = ElementoBinario.GetSerializador<CalendarItem>();
        public static string Directory;
        private Bitmap img;
        private Bitmap imgInvertida;

        static CalendarItem()
        {
            Directory = new DirectoryInfo("Thumbnails").FullName;
            if (!System.IO.Directory.Exists(Directory))
                System.IO.Directory.CreateDirectory(Directory);
        }

        public int Year { get; set; }
        public string FilePic { get; set; }
        public string FileName => System.IO.Path.GetFileName(FilePic);

        public string Hash { get; set; }
        public string IdRapido { get; set; }
        [IgnoreSerialitzer]
        public DateDay Date { get; set; }

        [IgnoreSerialitzer]
        public Bitmap ImgInvertida
        {
            get
            {
                if (Equals(imgInvertida, default))
                    imgInvertida = Img.ChangeColor(Gabriel.Cat.S.Drawing.FiltroImagen.Inverted);
                return imgInvertida;
            }
        }
        [IgnoreSerialitzer]
        public Bitmap Img
        {
            get
            {
                LoadImg();
                return img;
            }
            set
            {
                img = value;
            }
        }

        private void LoadImg()
        {
            Bitmap bmp;
            string idRapidoActual;
            FileInfo file;
            byte[] dataImg;
            string extension;
            Notifications.Wpf.Core.NotificationManager manager;

            string path = string.Empty;
            file = new FileInfo(FilePic);
            if (Equals(img, default))
            {
                if (Equals(Hash, default))
                {
                    if (File.Exists(FilePic))
                    {
                
                        dataImg = file.GetBytes();
                        bmp = (Bitmap)Bitmap.FromStream(new MemoryStream(dataImg));

                        Hash = dataImg.Hash();
                        IdRapido = file.IdUnicoRapido();

                        img = bmp.SetMaxHeight(MAX);
                        path = System.IO.Path.Combine(Directory, IdRapido);
                        if (bmp.IsArgb())
                        {
                            path += EXTENSION_TRANSPARENT;
                        }
                        else
                        {
                            path += EXTENSION_DEFAULT;
                        }
                        if (!File.Exists(path))
                        {
                            if (bmp.IsArgb())
                            {
                                img.Save(path, FormatoTransparent);
                            }
                            else
                            {
                                img.Save(path, FormatoDefault);
                            }
                      
                        }
                
                    }
                }
                else
                {
                    path = System.IO.Path.Combine(Directory, IdRapido);
                    if (IdRapido.Split(';')[0]==EXTENSION_TRANSPARENT)
                    {
                        path += EXTENSION_TRANSPARENT;
                    }
                    else
                    {
                        path += EXTENSION_DEFAULT;
                    }
                    if (File.Exists(path))
                    {
                        img = new Bitmap(path);
                    }
                    else if (file.Exists)
                    {
                        Hash = default;
                        LoadImg();
                    }
                }
            }

            if (file.Exists)
            {
                idRapidoActual = file.IdUnicoRapido();
                if (!Equals(idRapidoActual, IdRapido))
                {//así pueden modificar la imagen y no pasa nada :D
                    try
                    {
                        img = default;
                        imgInvertida = default;
                        Hash = default;
                     
                        extension = ((Bitmap)Bitmap.FromStream(file.GetStream())).IsArgb() ? EXTENSION_TRANSPARENT : EXTENSION_DEFAULT;
                        path = $"{IdRapido}{extension}";
                        if (File.Exists(path))
                            File.Delete(path);

                        img = Img;

                    }
                    catch(Exception ex)
                    {
                        manager = new Notifications.Wpf.Core.NotificationManager();
                        if (!Equals(path, default))
                        {
                            //si por lo que sea no se ha podido eliminar pues se queda así
                           
                            manager.ShowAsync(new Notifications.Wpf.Core.NotificationContent()
                            {
                                Title = "Problemas al actualizar el Thumbnail",
                                Message = $"No se ha podido eliminar el archivo '{path}' quizás se esté usando actualmente.",
                                Type = Notifications.Wpf.Core.NotificationType.Information,

                            });
                        }
                        else
                        {
                            manager.ShowAsync(new Notifications.Wpf.Core.NotificationContent()
                            {
                                Title = "Exception",
                                Message =ex.Message,
                                Type = Notifications.Wpf.Core.NotificationType.Error,

                            });
                        }
                    }

                }
            }
            
        }
        #region Serializador
        ElementoBinario IElementoBinarioComplejo.Serialitzer => Serializador;


        void ISaveAndLoad.Load()
        {

        }

        void ISaveAndLoad.Save()
        {
            LoadImg();
        }
        #endregion
        public Bitmap GetImgOrInvertida() => File.Exists(FilePic) ? Img : ImgInvertida;

        public int CompareTo(object obj)
        {
            return CompareTo(obj as CalendarItem);
        }

        public int CompareTo([AllowNull] CalendarItem other)
        {
            int compareTo = Equals(other, default) ? -1 : 0;
            if (compareTo == 0)
                compareTo = Year.CompareTo(other.Year);
            if (compareTo == 0 && !Equals(IdRapido, default))
                compareTo = IdRapido.CompareTo(other.IdRapido);
            return compareTo;
        }
        public override string ToString()
        {
            return FilePic;
        }

    }
}
