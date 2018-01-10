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

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for DiaCalendario.xaml
	/// </summary>
	public partial class DiaCalendario : UserControl
	{
		Color colorDia;
		int dia;
		List<ItemCalendario> items;
		public DiaCalendario()
		{
			InitializeComponent();
			items=new List<ItemCalendario>();
			Dia=1;
			colorDia=Colors.Black;
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
				
			}
		}

		public List<ItemCalendario> Items {
			get {
				return items;
			}
		}
		public void SetDia(IList<ItemCalendario> items,int dia)
		{
			Items.Clear();
			if(items!=null)
				Items.AddRange(items);
			Dia=dia;
			
		}
		public void SetDia(int diaInicioMes,int diaFinMesActual,int diaFinMesAnterior,int posicion,IList<ItemCalendario> items,bool posicionBase0O1=true)
		{
			int dia=posicion-diaInicioMes-(posicionBase0O1?1:0);
			
			if(dia<=0)
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
		public static int GetDiaInicioMes(int año,int mes)
		{
			const int DIAINICIO=1;
			return (int)new DateTime(año,mes,DIAINICIO).DayOfWeek;
			
		}
		public static int GetDiaFinMes(int año,int mes)
		{
			const int DIAMESIMPAR=31;
			const int DIAFINMESPAR=30;
			const int DIAFINFEBRERO=28;
			const int AÑOVISIESTOFEBRERO=29;
			const int FEBRERO=2;
			int diaFin;
			switch(mes)
			{
				case FEBRERO:
					//si no es visiesto es 28 sino es 29
					if(año % 4 == 0 && (año % 100 != 0 || año % 400 == 0))
						diaFin=AÑOVISIESTOFEBRERO;
					else diaFin=DIAFINFEBRERO;
					
					break;
				default:
					if(mes%2==0)
					{
						diaFin=DIAFINMESPAR;
					}else{
						diaFin=DIAMESIMPAR;
					}
					break;
			}
			return diaFin;
		}
	}
}