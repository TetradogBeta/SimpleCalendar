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

		public const int MINMES=1;
		public const int MAXMES=12;
		LlistaOrdenada<int,Llista<ItemCalendario>> itemsMes;
		LlistaOrdenada<int,Llista<ItemCalendario>> itemsDia;
		int añoActual;
		int mesActual;
		
		Idioma idioma;

		public Calendario(int año,int mes)
		{
			InitializeComponent();
			itemsMes=new LlistaOrdenada<int, Llista<ItemCalendario>>();
			itemsDia=new LlistaOrdenada<int, Llista<ItemCalendario>>();
			itemsDia.Removed+=QuitarItem;
			itemsMes.Removed+=QuitarItem;
			itemsDia.Added+=AñadirItem;
			itemsMes.Added+=AñadirItem;
			AñoActual=año;
			MesActual=mes;
			
			for(int i=1;i<32;i++)
				ugCalendarioTest.Children.Add(new Border(){BorderThickness=new System.Windows.Thickness(2),BorderBrush=Brushes.Transparent, Child=new TextBlock(){Text=i+""},Height=Width,HorizontalAlignment=HorizontalAlignment.Center,VerticalAlignment=VerticalAlignment.Center});

		}
		public Calendario(DateTime fecha):this(fecha.Year,fecha.Month)
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
		void PonDias()
		{
			//poner el
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

		

		void QuitarItem(object sender, DicEventArgs<int, Llista<ItemCalendario>> e)
		{
			if(e.Items.Count>0){
				if(sender==itemsMes)
				{
					if(itemsDia.ContainsKey(e.Items[0].Key))
						itemsDia.RemoveRange(e.Items.KeysToArray());
				}else{
					if(itemsMes.ContainsKey(e.Items[0].Key))
						itemsMes.RemoveRange(e.Items.KeysToArray());
				}}
		}

		void AñadirItem(object sender, DicEventArgs<int, Llista<ItemCalendario>> e)
		{
			if(e.Items.Count>0){
				if(sender==itemsMes)
				{
					if(!itemsDia.ContainsKey(e.Items[0].Key))
						itemsDia.AddRange(e.Items);
				}else{
					if(!itemsMes.ContainsKey(e.Items[0].Key))
						itemsMes.AddRange(e.Items);
				}}
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