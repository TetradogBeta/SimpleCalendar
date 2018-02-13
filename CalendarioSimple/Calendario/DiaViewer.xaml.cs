/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 13/02/2018
 * Hora: 17:19
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
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
using Gabriel.Cat.Extension;
using Microsoft.Win32;

namespace CalendarioSimple
{
	/// <summary>
	/// Interaction logic for DiaViewer.xaml
	/// </summary>
	public partial class DiaViewer : UserControl
	{
		Calendario calendario;
		
		AnimacionDia animacion;
		DiaCalendario dia;
		int año;

		bool isMesActual;
		
		public DiaViewer(Calendario calendario)
		{
			Calendario=calendario;
			InitializeComponent();
			animacion=new AnimacionDia();
			animacion.NuevoFotograma+=PonImagen;
		}

		public Calendario Calendario {
			get {
				return calendario;
			}
			private set{
				calendario=value;
			}
		}

		public DiaCalendario Dia {
			get {
				return dia;
			}
			set {
				System.Drawing.Brush color;
				dia = value;
				animacion.Dia=dia;
				Año=año;
				
				if(!dia.HayItems(año)&&Animacion.Recordatorios.Count==0)
				{
					color=isMesActual?System.Drawing.Brushes.Black:System.Drawing.Brushes.Gray;
					PonImagen(ConvertTextToImage(dia.Dia+"",color));
				}
				else if(!Animacion.IsEnabled)
					Animacion.IsEnabled=true;
			}
		}

		public int Año {
			get{return año;}
			set{
				año=value;
				if(dia!=null){
					Animacion.Año=año;
					Animacion.Recordatorios=calendario.GetRecordatoriosDia(dia.Dia,dia.Mes,Año);
					if(Animacion.Recordatorios.Count>0)
						Animacion.IsEnabled=true;
				}
				
			}
		}

		AnimacionDia Animacion {
			get {
				return animacion;
			}
		}
		public void SetDia(int año,int mesActual,DiaCalendario dia)
		{
			Año=año;
			isMesActual=mesActual==dia.Mes;
			Dia=dia;
			
		}

		void PonImagen(object sender, AnimacionEventArgs e)
		{
			PonImagen(e.Fotograma);
		}
		void PonImagen(Bitmap img)
		{
			Action act=()=>{
				if(animacion.IsEnabled||dia.GetTotal(Año)==0&&animacion.Recordatorios.Count==0)
					imgDia.SetImage(img);
				
			};
			Dispatcher.BeginInvoke(act);
		}

		void ImgDia_MouseEnter(object sender, MouseEventArgs e)
		{
			animacion.IsEnabled=false;
		}
		void ImgDia_MouseLeave(object sender, MouseEventArgs e)
		{
			animacion.IsEnabled=true;
		}
		void ImgDia_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if(e.Delta>0)
				animacion.Posicion++;
			else animacion.Posicion--;
		
		}
		void ImgDia_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			OpenFileDialog opnFile;
			if(Keyboard.IsKeyDown(Key.LeftCtrl|Key.RightCtrl))
			{
				opnFile=new OpenFileDialog();
				if(opnFile.ShowDialog().GetValueOrDefault())
				{
					AddItems(opnFile.FileNames);
					
				}
			}else{
				if(animacion.ItemActual!=null)
					animacion.ItemActual.Item.Abrir();
			}
		}

		void AddItems(string[] fileNames)
		{
				dia.AddItems(Año,fileNames);
					if(!animacion.IsEnabled)
						animacion.IsEnabled=true;
					calendario.AddDay(dia);
		}

		void ImgDia_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			if(Keyboard.IsKeyDown(Key.LeftCtrl|Key.RightCtrl))
			{
				//elimino el item
				calendario.RemoveItem(animacion.ItemActual);
				dia.ClearEmptyYears();
				if(animacion.Recordatorios.Count>0&&dia.HayItems(año))
					animacion.IsEnabled=true;
				else{
					animacion.IsEnabled=false;
					calendario.RemoveDay(dia);
				}
			}
		}
		void ImgDia_Drop(object sender, DragEventArgs e)
		{
			AddItems((string[])e.Data.GetData(DataFormats.FileDrop));
		}
		static Bitmap ConvertTextToImage(string txt,System.Drawing.Brush color ,string fontname="Arial", int fontsize=20)
		{
			//source:https://www.codeproject.com/tips/184102/convert-text-to-image
			//creating bitmap image
			Bitmap bmp = new Bitmap(1, 1);
			
			//FromImage method creates a new Graphics from the specified Image.
			Graphics graphics = Graphics.FromImage(bmp);
			// Create the Font object for the image text drawing.
			Font font = new Font(fontname, fontsize);
			// Instantiating object of Bitmap image again with the correct size for the text and font.
			SizeF stringSize = graphics.MeasureString(txt, font);
			bmp = new Bitmap(bmp,(int)stringSize.Width,(int)stringSize.Height);
			graphics = Graphics.FromImage(bmp);

			/* It can also be a way
          bmp = new Bitmap(bmp, new Size((int)graphics.MeasureString(txt, font).Width, (int)graphics.MeasureString(txt, font).Height));*/

			//Draw Specified text with specified format
			graphics.DrawString(txt, font,color, 0, 0);
			font.Dispose();
			graphics.Flush();
			graphics.Dispose();
			return bmp;     //return Bitmap Image
		}
		
	}
}