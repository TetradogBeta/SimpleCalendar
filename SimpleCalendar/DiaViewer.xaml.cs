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
using Microsoft.Win32;

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for DiaViewer.xaml
	/// </summary>
	public partial class DiaViewer : UserControl
	{
		class Descripcion
		{
			DiaViewer dia;
			public Descripcion(DiaViewer dia)
			{
				this.dia=dia;
			}
			public override string ToString()
			{
				return dia.TooltTipDespcripcion();
			}
		}
		public const int TIMEIMGANIMACION=1500;
		public const int TIEMPOSINMIRAR=5*1000;
		public const int TIEMPOESPERAHOVER=200;
		
		short añoAMostrar;
		DiaCalendario dia;
		Calendario calendario;
		Llista<ItemCalendario> recordatorios;
		Temporizador tempAnimacion;
		int posAnimacion;
		bool mouseHover;

		public DiaViewer(Calendario calendario=null)
		{
			this.calendario=calendario;
			InitializeComponent();

			recordatorios=new Llista<ItemCalendario>();
			tempAnimacion=new Temporizador(TIMEIMGANIMACION);
			posAnimacion=0;
			tempAnimacion.Interval=TIEMPOSINMIRAR;
			tempAnimacion.Elapsed+=PonImagen;
			mouseHover=false;
			
		}

		public DateTime Fecha {
			get {
				return new DateTime(añoAMostrar, dia.Mes,dia.Dia);
			}
		}

		public Calendario Calendario {
			get {
				return calendario;
			}
			set {
				calendario = value;
			}
		}

		public DiaCalendario Dia {
			get {
				return dia;
			}
			set{
				dia=value;
				txtDia.Text=dia.Dia+"";
			}
		}

		public Llista<ItemCalendario> Recordatorios {
			get {
				return recordatorios;
			}
		}


		
		void PonImagen(Temporizador temporizador)
		{
			int itemsActuales;
			if(dia.Count==0&&recordatorios.Count==0)
				temporizador.Interval=TIEMPOSINMIRAR;
			else {
				
				temporizador.Interval=TIMEIMGANIMACION;
				PonImagen();
				itemsActuales=GetTotalItems();
				if(itemsActuales>0)
					posAnimacion=(posAnimacion+1)%itemsActuales;
				System.Threading.Thread.Sleep(TIMEIMGANIMACION);
				while(mouseHover)
					System.Threading.Thread.Sleep(TIEMPOESPERAHOVER);
				
			}
		}
		void PonImagen()
		{
			Action act=()=>{
				ImageBrush img;
				
				try{
					img=new ImageBrush(GetItem(posAnimacion).Miniatura.ToImage().Source);
					Background=img;
				}catch{}
				
			};
			
			Dispatcher.BeginInvoke(act);
			
		}
		

		ItemCalendario GetItem(int posAnimacion)
		{
			//tener en cuenta los recordatorios!!
			ItemCalendario item=dia.GetItemAt(posAnimacion);
			
			if(item==null)
			{
				item=recordatorios[posAnimacion-dia.Count];
			}
			return item;
		}
		public void Clear()
		{
			recordatorios.Clear();
			//paro la animacion
			tempAnimacion.StopAndAbort();
			Background=Brushes.White;
		}
		public void PonFecha(DiaCalendario dia,bool esMesActual=true)
		{
			Dia=dia;
			if(esMesActual)
				txtDia.Foreground=Brushes.Black;
			else txtDia.Foreground=Brushes.Gray;
			//pongo la animacion
			tempAnimacion.Start();
		}
		void Grid_Drop(object sender, DragEventArgs e)
		{
			dia.Add(añoAMostrar,(string[])e.Data.GetData(DataFormats.FileDrop));
		}
		void Grid_MouseEnter(object sender, MouseEventArgs e)
		{
			mouseHover=true;
		}
		void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			mouseHover=false;
		}

		int GetTotalItems()
		{
			return dia.Count+recordatorios.Count;
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
			if(total>0){
				posAnimacion+=e.Delta/WHEEL;
				
				if(posAnimacion<0)
					posAnimacion=total-1;
				else if(posAnimacion>=total)
					posAnimacion=0;
				
				PonImagen();
			}
		}
		void Grid_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
		{
			OpenFileDialog opnFiles;
			if(Keyboard.Modifiers==ModifierKeys.Control)
			{
				opnFiles=new OpenFileDialog();
				opnFiles.Multiselect=true;
				if(opnFiles.ShowDialog().GetValueOrDefault())
				{
					dia.Add(añoAMostrar,opnFiles.FileNames);
				}
			}
			else{
				try{
					GetItem(posAnimacion).Item.Abrir();
				}catch{MessageBox.Show("Hay problemas para abrir el archivo");}
			}
		}
		void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Action act;
			int itemsActuales;
			ItemCalendario item;
			if(Keyboard.Modifiers==ModifierKeys.Control)
			{
				itemsActuales=GetTotalItems();
				//elimino el elemento actual
				if(itemsActuales>0)
				{
					
					item=GetItem(posAnimacion);
					dia.Remove(añoAMostrar,item);
					recordatorios.Remove(item);
					tempAnimacion.StopAndAbort();
					itemsActuales--;
					if(itemsActuales>0){
						posAnimacion=posAnimacion%itemsActuales;
						PonImagen();
						tempAnimacion.Start();
					}else{
						
						act=()=>{
							Background=Brushes.Transparent;
							ToolTip=null;
						};
						
						Dispatcher.BeginInvoke(act);
						
						
					}
				}
				
				
			}
		}
		private  string TooltTipDespcripcion()
		{
			string txt="";
			int añoItem;
			DiaCalendario diaItem;
			DateTime fechaRelativa;
			ItemCalendario item=GetItem(posAnimacion);
			
			if(item!=null){
				diaItem=calendario.GetDia(item);
				añoItem=diaItem.GetAño(item);
				
				
				if(diaItem!=null)
				{
					fechaRelativa=new DateTime(Fecha.Year,diaItem.Mes,diaItem.Dia);
					txt=(fechaRelativa-Fecha).TotalDays+"";
					if(fechaRelativa<Fecha){
						if(Fecha.Year==DateTime.Now.Year)
							txt="Faltan "+txt;
						else txt="Faltarian "+txt;
						
					}
					else if(fechaRelativa>Fecha){
						if(Fecha.Year==DateTime.Now.Year)
							txt="Hace "+txt;
						else txt="Hará "+txt;
					}
					else if(añoItem==Fecha.Year){
						if(Fecha.Year==DateTime.Now.Year)
							txt="Hoy empieza";
						else txt="Hoy empiezará";
					}
					else if(añoItem>Fecha.Year)
					{
						if(Fecha.Year==DateTime.Now.Year)
							txt="Falta";
						else txt="Faltará";
						
						txt+=(añoItem-Fecha.Year>1?"n ":" ")+(añoItem-Fecha.Year)+" año"+(añoItem-Fecha.Year>1?"s":"");
						
					}else{

						if(Fecha.Year==DateTime.Now.Year)
							txt="Hace ";
						else txt="Hará ";
						
						txt+=(Fecha.Year-añoItem)+" año"+(Fecha.Year-añoItem>1?"s":"");
					}
					
				}
			}
			return txt;
		}
		
	}


}