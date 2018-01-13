/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 01/01/2018
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
using Gabriel.Cat;
using Gabriel.Cat.Extension;

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for Calendario.xaml
	/// </summary>
	public partial class Calendario : UserControl
	{
		public enum Idioma{
			Castellano,Catalan,Ingles
				
		}
		public const int TOTALDIAS=31;
		public const int MINMES=1;
		public const int MAXMES=12;
		LlistaOrdenada<int,Llista<ItemCalendario>> itemsMes;
		LlistaOrdenada<int,Llista<ItemCalendario>> itemsDiaPosicionAnual;
		Llista<DiaCalendario> dias;
		int añoActual;
		int mesActual;
		
		Idioma idioma;
		public event EventHandler<DiaSeleccionadoEventArgs> DiaSeleccionado;
		public Calendario(int año,int mes,IList<ItemCalendario> items,Idioma idioma=Idioma.Castellano)
		{
			int diaInicioMes=DiaCalendario.GetDiaInicioMes(año,mes);
			int diaFinMesActual=DiaCalendario.GetDiaFinMes(año,mes);
			int dieFinMesAnterior=DiaCalendario.GetDiaFinMes(año,mes-1<1?MAXMES:mes-1);
			int diaInicioMesPosicionAño;
			DiaCalendario dia;
			this.idioma=idioma;
			InitializeComponent();
			
			AñoActual=año;
			MesActual=mes;
			diaInicioMesPosicionAño=DiaInicioMesPosicionAño;
			itemsMes=new LlistaOrdenada<int, Llista<ItemCalendario>>();
			itemsDiaPosicionAnual=new LlistaOrdenada<int, Llista<ItemCalendario>>();
			itemsDiaPosicionAnual.Removed+=QuitarItem;
			itemsMes.Removed+=QuitarItem;
			itemsDiaPosicionAnual.Added+=AñadirItem;
			itemsMes.Added+=AñadirItem;
			
			dias=new Llista<DiaCalendario>();
			PonItems(items);
			
			for(int i=0;i<TOTALDIAS;i++){
				dia=new DiaCalendario();
				dia.SetDia(diaInicioMes,diaFinMesActual,dieFinMesAnterior,i,itemsDiaPosicionAnual.GetValue(diaInicioMesPosicionAño+i));
				dia.MouseLeftButtonDown+=ClickDia;
				ugCalendarioTest.Children.Add(dia);
				dias.Add(dia);
			}
		}
		public Calendario(DateTime fecha,IList<ItemCalendario> items):this(fecha.Year,fecha.Month,items)
		{
		}

		public int AñoActual {
			get {
				return añoActual;
			}
			set {
				añoActual = value;
				txtAño.Text=añoActual+"";
			}
		}

		public int MesActual {
			get {
				return mesActual;
			}
			set {
				if(value-1>MAXMES)
				{
					AñoActual+=value/MAXMES;
					value=value%MAXMES;
				}
				else if(value<MINMES)
				{
					value=-value;
					AñoActual-=value/MAXMES;
					value=MAXMES-(value%MAXMES);
				}
				mesActual = value;
				PonNombreMes();
				PonDias();
			}
		}

		public Idioma IdiomaMes {
			get {
				return idioma;
			}
			set {
				idioma = value;
				PonNombreMes();
			}
		}
		public int DiaInicioMesPosicionAño
		{
			get{
				int posicion=1;
				for(int i=1;i<MesActual;i++)
				{
					posicion+=DiaCalendario.GetDiaFinMes(AñoActual,i);
				}
				return posicion;
				
			}
		}

		void ClickDia(object sender, MouseButtonEventArgs e)
		{
			DiaCalendario dia=sender as DiaCalendario;
			int mes=MesActual;
			int año=AñoActual;
			if(DiaSeleccionado!=null)
			{
				
				if(dia.Dia>20&&dias.IndexOf(dia)<10){
					mes=MesActual-1==0?MAXMES:MesActual-1;
					if(mes==MAXMES)
						año=AñoActual-1;
					
				}
				
				DiaSeleccionado(this,new DiaSeleccionadoEventArgs(new DateTime(año,mes,dia.Dia),dia.Items));
			}
		}

		void PonDias()
		{
			int diaInicioMes=DiaCalendario.GetDiaInicioMes(AñoActual,MesActual);
			int diaFinMesActual=DiaCalendario.GetDiaFinMes(AñoActual,MesActual);
			int dieFinMesAnterior=DiaCalendario.GetDiaFinMes(AñoActual,MesActual-1<1?MAXMES:MesActual-1);
			int diaInicioMesPosicionAño=DiaInicioMesPosicionAño;
			
			for(int i=0;i<TOTALDIAS;i++)
			{
				dias[i].SetDia(diaInicioMes,diaFinMesActual,dieFinMesAnterior,i,itemsDiaPosicionAnual.GetValue(diaInicioMesPosicionAño+i));
			}
		}

		void PonNombreMes()
		{
			string idiomaCultural=null;
			switch (IdiomaMes) {
				case Idioma.Castellano:
					idiomaCultural="es-ES";
					break;
				case Idioma.Catalan:
					idiomaCultural="ca-ES";
					break;
				case Idioma.Ingles:
					idiomaCultural="en-US";
					break;

			}
			txtMes.Text=new DateTime(añoActual,mesActual,1).NombreMes(idiomaCultural);
		}

		void PonItems(IList<ItemCalendario> items)
		{
			//los pone en las listas itemsMes y itemsDiaPosicionAnual
			//solo añadir a una lista luego los eventos se encargan de eso :D a no ser que se relentice...
		}
		

		
		void QuitarItem(object sender, DicEventArgs<int, Llista<ItemCalendario>> e)
		{
			throw new NotImplementedException();
		}

		void AñadirItem(object sender, DicEventArgs<int, Llista<ItemCalendario>> e)
		{
			throw new NotImplementedException();
		}
		
	
	}

	public class DiaSeleccionadoEventArgs:EventArgs
	{
		Llista<ItemCalendario> items;
		DateTime diaSeleccionado;
		public DiaSeleccionadoEventArgs(DateTime diaSeleccionado,IList<ItemCalendario> items)
		{
			this.diaSeleccionado=diaSeleccionado;
			this.items=new Llista<ItemCalendario>(items);
		}

		public Llista<ItemCalendario> Items {
			get {
				return items;
			}
		}

		public DateTime DiaSeleccionado {
			get {
				return diaSeleccionado;
			}
		}
	}
}