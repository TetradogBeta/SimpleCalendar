/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 21/01/2018
 * Hora: 5:34
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
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
using Gabriel.Cat;
using Gabriel.Cat.Extension;

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for DiaViewer.xaml
	/// </summary>
	public partial class DiaViewer : UserControl
	{
		public const int TIMEIMGANIMACION=1500;
		public const int TIEMPOSINMIRAR=5*1000;
		DateTime fecha;
		Llista<Dia> dias;
		Llista<ItemCalendario> recordatorios;
		Temporizador tempAnimacion;
		int posAnimacion;
		
		public event EventHandler<ItemsEventArgs> ItemsAñadidos;
		public DiaViewer()
		{
			InitializeComponent();
			dias=new Llista<Dia>();
			recordatorios=new Llista<ItemCalendario>();
			tempAnimacion=new Temporizador(TIMEIMGANIMACION);
			posAnimacion=0;
			tempAnimacion.Elapsed+=PonImagen;
		}

		public DateTime Fecha {
			get {
				return fecha;
			}
		}
		public Llista<Dia> Dias {
			get {
				return dias;
			}
		}

		public Llista<ItemCalendario> Recordatorios {
			get {
				return recordatorios;
			}
		}

		void PonImagen(Temporizador temporizador)
		{
			if(dias.Count==0&&recordatorios.Count==0)
				temporizador.Interval=TIEMPOSINMIRAR;
			else {
				
				temporizador.Interval=TIMEIMGANIMACION;
				PonImagen();
				posAnimacion=(posAnimacion+1)%GetTotalItems();
			}
		}
		void PonImagen()
		{
			Action act=()=>{
				ImageBrush img=new ImageBrush(GetItem(posAnimacion).Miniatura.ToImage().Source);
				Background=img;
			};
			
			Dispatcher.BeginInvoke(act);
			
		}
		

		ItemCalendario GetItem(int posAnimacion)
		{
			//tener en cuenta los recordatorios!!
			ItemCalendario item=null;
			for(int i=0;i<dias.Count&&item==null;i++)
			{
				if((posAnimacion-dias[i].Items.Count)>=0){
					posAnimacion-=dias[i].Items.Count;
					if(posAnimacion==0)
						item=dias[i].Items[0];
				}
				else item=dias[i].Items[posAnimacion];
			}
			if(item==null)
			{
				item=recordatorios[posAnimacion];
			}
			return item;
		}
		public void Clear()
		{
			dias.Clear();
			recordatorios.Clear();
			//paro la animacion
			tempAnimacion.StopAndAbort();
		}
		public void PonFecha(int dia,DateTime fechaMesAño,bool esMesActual=true)
		{
			this.fecha=new DateTime(fechaMesAño.Year,fechaMesAño.Month,dia);
			
			txtDia.Text=this.fecha.Day+"";
			
			if(esMesActual)
				txtDia.Foreground=Brushes.Black;
			else txtDia.Foreground=Brushes.Gray;
			//pongo la animacion
			tempAnimacion.Start();
		}
		void Grid_Drop(object sender, DragEventArgs e)
		{
			if(ItemsAñadidos!=null)
				ItemsAñadidos(this,new ItemsEventArgs(Fecha, (string[])e.Data.GetData(DataFormats.FileDrop)));
		}
		void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			tempAnimacion.StopAndAbort();
		}
		void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			tempAnimacion.Start();
		}

		int GetTotalItems()
		{
			int total=0;
			for(int i=0;i<dias.Count;i++)
				total+=dias[i].Items.Count;
			total+=recordatorios.Count;
			return total;
		}
		public void StartAnimation()
		{
			try{
				tempAnimacion.StopAndAbort();
				posAnimacion=0;
				tempAnimacion.Start();
				PonImagen();
			}catch{}
		}
		void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			
			const int WHEEL=120;
			int total=GetTotalItems();
			posAnimacion+=e.Delta/WHEEL;
			
			if(posAnimacion<0)
				posAnimacion=total-1;
			else if(posAnimacion>=total)
				posAnimacion=0;
			
			PonImagen();
		}
		void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			try{
				GetItem(posAnimacion).Item.Abrir();
			}catch{}
		}
	}
	public class ItemsEventArgs:EventArgs
	{
		DateTime fecha;
		IList<string> items;
		public ItemsEventArgs(DateTime fecha,IList<string> items)
		{
			Fecha=fecha;
			Items=items;
		}
		public DateTime Fecha {
			get {
				return fecha;
			}
			private set{fecha=value;}
		}

		public IList<string> Items {
			get {
				return items;
			}
			private set {
				items = value;
			}
		}
	}
	
}