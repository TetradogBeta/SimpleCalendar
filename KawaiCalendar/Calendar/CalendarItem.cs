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
        public const string EXTENSION = ".jpeg";
        public const int MAX = 250;
        public static readonly System.Drawing.Imaging.ImageFormat Formato = System.Drawing.Imaging.ImageFormat.Jpeg;

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
            string path;
            string idRapidoActual;

            if (Equals(img, default))
            {
                if (Equals(Hash, default))
                {
                    if (File.Exists(FilePic))
                    {
                        bmp = new Bitmap(FilePic);

                        Hash = bmp.GetBytes().Hash();
                        IdRapido = new FileInfo(FilePic).IdUnicoRapido();

                        img = bmp.SetMaxHeight(MAX);
                        path = System.IO.Path.Combine(Directory, IdRapido + EXTENSION);
                        if (!File.Exists(path))
                            img.Save(path, Formato);
                    }
                }
                else
                {
                    path = System.IO.Path.Combine(Directory, IdRapido + EXTENSION);
                    if (File.Exists(path))
                    {
                        img = new Bitmap(path);
                    }
                    else if (File.Exists(FilePic))
                    {
                        Hash = default;
                        LoadImg();
                    }
                }
            }

            idRapidoActual = new FileInfo(FilePic).IdUnicoRapido();
            if (!Equals(idRapidoActual, IdRapido))
            {//así pueden modificar la imagen y no pasa nada :D
                try
                {
                    img = default;
                    imgInvertida = default;
                    img = Img;
                    path = $"{IdRapido}.jpg";
                    if (File.Exists(path))
                        File.Delete(path);
                   
                }
                catch
                {
                    //si por lo que sea no se ha podido eliminar pues se queda así
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
