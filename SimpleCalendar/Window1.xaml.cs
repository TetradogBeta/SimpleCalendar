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
using Microsoft.Win32;

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		public Window1()
		{
			OpenFileDialog opnFile=new OpenFileDialog();
			opnFile.ShowDialog();
			DiaCalendario.pathTest=opnFile.FileName;
			InitializeComponent();
			wMain.ResizeMode=ResizeMode.CanMinimize|ResizeMode.NoResize;
	
		}

	
	}
}