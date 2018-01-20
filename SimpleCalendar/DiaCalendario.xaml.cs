/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using Gabriel.Cat.Extension;

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for DiaCalendario.xaml
	/// </summary>
	public partial class DiaCalendario : UserControl
	{
		Color colorDia;
		DateTime dia;
		List<ItemCalendario> items;
		Temporizador animador;
		int posicionAnimacion;
		public event EventHandler<DropItemsEventArgs> ItemsAñadidos;
		public DiaCalendario()
		{
			const int TIEMPOIMAGEN=1500;
			
			InitializeComponent();
			
			items=new List<ItemCalendario>();
			colorDia=Colors.Black;
			animador=new Temporizador(TIEMPOIMAGEN);
			animador.Elapsed+=CambiarImagen;
			
		}

		public Color ColorDia {
			get {
				return colorDia;
			}
			set {
				colorDia = value;
			}
		}

		public DateTime Dia {
			get {
				return dia;
			}
			set {
				dia = value;
				txtDia.Text=dia.Day+"";
				animador.StopAndAbort();
				posicionAnimacion=0;
				//poner la animación...
				
				animador.Start();
			}
		}

		public List<ItemCalendario> Items {
			get {
				return items;
			}
		}

		void CambiarImagen(Temporizador temporizador)
		{
			Action act=()=>{
			
				if(items.Count>0)
				{
					posicionAnimacion=++posicionAnimacion%items.Count;
					PonImagen();
					
				}else{ gDiaCalendario.Background=Brushes.Transparent;
					
				}
			};
			
			Dispatcher.BeginInvoke(act);
			if(items.Count==0)
				System.Threading.Thread.Sleep(5*1000);//si no hay lo pongo a dormir 5 segundos
		}

		void PonImagen()
		{
			ImageBrush img=new ImageBrush(items[posicionAnimacion].Miniatura.ToImage().Source);
			img.Stretch=Stretch.Uniform;
			gDiaCalendario.Background=img;
		}

		public void EstaEnElMesActual(bool elDiaEstaEnNegrita)
		{
			if(elDiaEstaEnNegrita)
				txtDia.Foreground=Brushes.Black;
			else txtDia.Foreground=Brushes.Gray;

		}

		public void SetDia(IList<ItemCalendario> items,DateTime dia)
		{
			Items.Clear();
			if(items!=null)
				Items.AddRange(items);
			Dia=dia;
			
		}
		public void SetDia(int posicionPrimerDiaMes/*,int diaFinMesActual,int diaFinMesAnterior*/,int posicion,DateTime inicioMes,IList<ItemCalendario> items)
		{
			//Dias mes anterior,mesActual,mesSiguiente->con la posicion actual y la informacion que tengo saco el dia
			
			int dia=posicion-posicionPrimerDiaMes+1;//lunes=1,martes=2
			
			/*if(dia<1)
			{
				//mes anterior
				dia=diaFinMesAnterior+dia;
				
			}else if(dia>diaFinMesActual)
			{
				//mes siguiente
				dia=dia-diaFinMesActual;
			}*/
			
			SetDia(items,inicioMes+new TimeSpan(dia,0,0,0));
			
		}
		public static DayOfWeek GetDiaInicioMes(int año,int mes)
		{
			const int DIAINICIO=1;
			return new DateTime(año,mes,DIAINICIO).DayOfWeek;
			
		}
		public static int GetDiaFinMes(int año,int mes)
		{
			if(mes==12)
				mes=0;
			return (new DateTime(año,mes+1,1)-new TimeSpan(1,0,0,0)).Day;
		}
		void GDiaCalendario_Drop(object sender, DragEventArgs e)
		{
			string[] files;
			ItemCalendario[] items;
			//añado los items :D
			if (e.Data.GetDataPresent(DataFormats.FileDrop))
			{
				// Note that you can have more than one file.
				files = (string[])e.Data.GetData(DataFormats.FileDrop);
				items=new ItemCalendario[files.Length];
				for(int i=0;i<files.Length;i++)
				{
					items[i]=new ItemCalendario(new System.IO.FileInfo(files[i]));
					items[i].FechaInicio=dia;
				}
				Items.AddRange(items);
				if(ItemsAñadidos!=null)
					ItemsAñadidos(this,new DropItemsEventArgs(this,items));
			}
		}
		void GDiaCalendario_MouseEnter(object sender, MouseEventArgs e)
		{
			animador.StopAndAbort();
		}
		void GDiaCalendario_MouseLeave(object sender, MouseEventArgs e)
		{
			animador.Start();
		}
		void GDiaCalendario_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			items[posicionAnimacion].Item.Abrir();
		}
		void GDiaCalendario_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			Action act=()=>PonImagen();
			posicionAnimacion=(posicionAnimacion+(e.Delta/120))%items.Count;
			if(posicionAnimacion<0)
				posicionAnimacion+=items.Count;
			Dispatcher.BeginInvoke(act);
		}
	}
	public class DropItemsEventArgs:EventArgs
	{
		IList<ItemCalendario> items;
		DiaCalendario dia;
		public DropItemsEventArgs(DiaCalendario dia,IList<ItemCalendario> items)
		{
			this.items=items;
			this.dia=dia;
		}

		public DiaCalendario Dia {
			get {
				return dia;
			}
		}
		public IList<ItemCalendario> Items {
			get {
				return items;
			}
		}
	}
}