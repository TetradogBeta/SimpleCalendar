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
		public static string pathTest;
		Color colorDia;
		int dia;
		List<ItemCalendario> items;
		public DiaCalendario()
		{
			ImageBrush img;
			InitializeComponent();
			items=new List<ItemCalendario>();
			Dia=1;
			colorDia=Colors.Black;
			img=new ImageBrush(new System.Drawing.Bitmap(pathTest).ToImage().Source);
			img.Stretch=Stretch.Uniform;
			gDiaCalendario.Background=img;
		

		}

		public Color ColorDia {
			get {
				return colorDia;
			}
			set {
				colorDia = value;
			}
		}

		public int Dia {
			get {
				return dia;
			}
			set {
				dia = value;
				txtDia.Text=dia+"";
				//poner la animación...
				if(items.Count>0)
				{
				gDiaCalendario.Background=Brushes.LightCoral;
				}
			}
		}

		public List<ItemCalendario> Items {
			get {
				return items;
			}
		}

		public void EstaEnElMesActual(bool elDiaEstaEnNegrita)
		{
			if(elDiaEstaEnNegrita)
				txtDia.Foreground=Brushes.Black;
			else txtDia.Foreground=Brushes.Gray;

		}

		public void SetDia(IList<ItemCalendario> items,int dia)
		{
			Items.Clear();
			if(items!=null)
				Items.AddRange(items);
			Dia=dia;
			
		}
		public void SetDia(int posicionPrimerDiaMes,int diaFinMesActual,int diaFinMesAnterior,int posicion,IList<ItemCalendario> items)
		{
			//Dias mes anterior,mesActual,mesSiguiente->con la posicion actual y la informacion que tengo saco el dia
			
			int dia=posicion-posicionPrimerDiaMes+2;//lunes=1,martes=2
			
			if(dia<1)
			{
				//mes anterior
				dia=diaFinMesAnterior+dia;
				
			}else if(dia>diaFinMesActual)
			{
				//mes siguiente
				dia=dia-diaFinMesActual;
			}
			
			SetDia(items,dia);
			
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
	}
}