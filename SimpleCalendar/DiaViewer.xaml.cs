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
	public delegate Dia DameDiaEvent(ItemCalendario item);
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
		DateTime fecha;
		Llista<Dia> dias;
		Llista<ItemCalendario> recordatorios;
		Temporizador tempAnimacion;
		int posAnimacion;
		bool mouseHover;
		public event DameDiaEvent DameDia;
		public event EventHandler<ItemsEventArgs> ItemsAñadidos;
		public event EventHandler<RemoveItemEventArgs> ItemEliminado;
		public DiaViewer()
		{
			InitializeComponent();
			dias=new Llista<Dia>();
			dias.Added+=PonDescripcionYAnimacion;
			dias.Removed+=QuitarDescripcionYAnimacion;
			recordatorios=new Llista<ItemCalendario>();
			recordatorios.Added+=(s,e)=>PonDescripcionYAnimacion();
			recordatorios.Removed+=(s,e)=>QuitarDescripcionYAnimacion();
			tempAnimacion=new Temporizador(TIMEIMGANIMACION);
			posAnimacion=0;
			tempAnimacion.Interval=TIEMPOSINMIRAR;
			tempAnimacion.Elapsed+=PonImagen;
			mouseHover=false;
			
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

		public void EliminarRecordatorios(IList<ItemCalendario> items)
		{
			recordatorios.RemoveRange(items);
		}
		void QuitarDescripcionYAnimacion(object sender=null, ListEventArgs<Dia> e=null)
		{
			if(dias.Count==0&&recordatorios.Count==0&&ToolTip!=null)
			{
				ToolTip=null;
				tempAnimacion.StopAndAbort();
			}
		}
		void PonDescripcionYAnimacion(object sender=null, ListEventArgs<Dia> e=null)
		{
			if(dias.Count>0||recordatorios.Count>0){
				if(ToolTip==null)
					ToolTip=new Descripcion(this);
				if(!tempAnimacion.EstaOn)
					tempAnimacion.Stop();
			}
		}
		void PonImagen(Temporizador temporizador)
		{
			int itemsActuales;
			if(dias.Count==0&&recordatorios.Count==0)
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
			Background=Brushes.White;
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
			mouseHover=true;
		}
		void Grid_MouseLeave(object sender, MouseEventArgs e)
		{
			mouseHover=false;
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
			int total;
			if(dias.Count!=0||recordatorios.Count!=0){
				
				total=GetTotalItems();
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
					if(ItemsAñadidos!=null)
						ItemsAñadidos(this,new ItemsEventArgs(Fecha,opnFiles.FileNames));
				}
			}
			else{
				try{
					GetItem(posAnimacion).Item.Abrir();
				}catch{}
			}
		}
		void Grid_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
		{
			Action act;
			int itemsActuales;
			if(Keyboard.Modifiers==ModifierKeys.Control)
			{
				itemsActuales=GetTotalItems();
				//elimino el elemento actual
				if(itemsActuales>0)
				{
					if(ItemEliminado!=null)
						ItemEliminado(this,new RemoveItemEventArgs(Fecha,GetItem(posAnimacion)));
					itemsActuales--;
					tempAnimacion.StopAndAbort();
					
					if(itemsActuales>0){
						
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
			Dia diaItem;
			DateTime fechaRelativa;
			try{
				if(DameDia!=null)
				{
					diaItem=DameDia(GetItem(posAnimacion));
					fechaRelativa=new DateTime(Fecha.Year,diaItem.Fecha.Month,diaItem.Fecha.Day);
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
					else if(diaItem.Fecha.Year==Fecha.Year){
						if(Fecha.Year==DateTime.Now.Year)
							txt="Hoy empieza";
						else txt="Hoy empiezará";
					}
					else if(diaItem.Fecha.Year>Fecha.Year)
					{
						if(Fecha.Year==DateTime.Now.Year)
							txt="Falta";
						else txt="Faltará";
						
						txt+=(diaItem.Fecha.Year-Fecha.Year>1?"n ":" ")+(diaItem.Fecha.Year-Fecha.Year)+" año"+(diaItem.Fecha.Year-Fecha.Year>1?"s":"");
						
					}else{

						if(Fecha.Year==DateTime.Now.Year)
							txt="Hace ";
						else txt="Hará ";
						
						txt+=(Fecha.Year-diaItem.Fecha.Year)+" año"+(Fecha.Year-diaItem.Fecha.Year>1?"s":"");
					}
				}
			}catch{}
			return txt;
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
	public class RemoveItemEventArgs:EventArgs
	{
		DateTime fecha;
		IList<ItemCalendario> items;
		public RemoveItemEventArgs(DateTime fecha,params ItemCalendario[] items)
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

		public IList<ItemCalendario> Items {
			get {
				return items;
			}
			private set {
				items = value;
			}
		}
	}
}