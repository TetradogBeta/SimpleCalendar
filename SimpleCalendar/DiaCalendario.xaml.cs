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
		IList<ItemCalendario> items;
		public DiaCalendario()
		{
			InitializeComponent();
			Items=new ItemCalendario[0];
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
				
			}
		}

		public IList<ItemCalendario> Items {
			get {
				return items;
			}
			set {
				items = value;
			}
		}
	}
}