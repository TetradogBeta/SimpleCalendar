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

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for DiaViewer.xaml
	/// </summary>
	public partial class DiaViewer : UserControl
	{
		Llista<Dia> dias;
		Llista<ItemCalendario> recordatorios;
		public DiaViewer()
		{
			InitializeComponent();
			dias=new Llista<Dia>();
			recordatorios=new Llista<ItemCalendario>();
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
		public void Clear()
		{
			dias.Clear();
			recordatorios.Clear();
		}
		public void PonFecha(int dia,bool esMesActual=true)
		{
			txtDia.Text=dia+"";
			if(esMesActual)
				txtDia.Foreground=Brushes.Black;
			else txtDia.Foreground=Brushes.Gray;
		}
	}
}