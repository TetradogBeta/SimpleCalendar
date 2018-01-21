/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 21/01/2018
 * Hora: 5:12
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
using Gabriel.Cat.Extension;

namespace SimpleCalendar
{
	/// <summary>
	/// Interaction logic for CalendarioViewer.xaml
	/// </summary>
	public partial class CalendarioViewer : UserControl
	{
		public static readonly string PathCalendario=System.IO.Path.Combine(Environment.CurrentDirectory,"calendario.data");
		public static readonly string PathBackUp=PathCalendario+".old";
		
		public const int TOTALDIAS=6*7;//seis semanas muestro :)
		Calendario calendario;
		public CalendarioViewer()
		{
		
			InitializeComponent();
			
			
			if(!System.IO.File.Exists(PathCalendario)&&System.IO.File.Exists(PathBackUp)){
				//Si ha habido problemas mientras se guardaba pues rescato lo anterior
				System.IO.File.Move(PathBackUp,PathCalendario);
			}
			
			if(System.IO.File.Exists(PathCalendario))
				calendario=new Calendario().GetObject(System.IO.File.ReadAllBytes(PathCalendario)) as Calendario; 
			else calendario=new Calendario();
			
			//pongo los dias;
			for(int i=0;i<TOTALDIAS;i++)
				ugDiasMes.Children.Add(new DiaViewer());
			
			PonFecha(DateTime.Now);
		}

		public void PonFecha(DateTime fecha)
		{
			IList<Dia> dias;
			DiaViewer dia;
			DateTime mesAnterior=fecha.GetMesAnterior();
			int diasMesAnterior=mesAnterior.GetDiaFinMes();
			DateTime mesSiguiente=fecha.GetMesSiguiente();
			int diasMesSiguiente=mesSiguiente.GetDiaFinMes();
			int diaAPoner;
			int diasMesActual=fecha.GetDiaFinMes();
			DayOfWeek diaInicio=fecha.GetDayOfWeekInicioMes();
			dias=calendario.GetDias(fecha.Month);
			
			txtAñoActual.Text=fecha.Year+"";
			txtMesActual.Text=fecha.NombreMes();
			txtMesActual.Text=char.ToUpper(txtMesActual.Text[0])+txtMesActual.Text.Substring(1);
			
			
			for(int i=0,f=(int)diaInicio-1,p;i<f;i++)
			{
				p=f-i;
				diaAPoner=diasMesAnterior-p;
				dia=(ugDiasMes.Children[p] as DiaViewer);
				
				dia.Clear();
				dia.Dias.AddRange(calendario.DiasConItems.FiltraValues((d)=>d.Fecha.Month==mesAnterior.Month&&d.Fecha.Day==diaAPoner));
				dia.Recordatorios.AddRange(Dia.GetRecordatorios(calendario.DiasConItems,new DateTime(mesAnterior.Year,mesAnterior.Month,diaAPoner)));
				dia.PonFecha(diaAPoner,false);
			}
			for(int i=(int)diaInicio-1,j=1,f=diasMesActual,p;j<=f;j++,i++)
			{
			
				diaAPoner=j;
				dia=(ugDiasMes.Children[i] as DiaViewer);
				
				dia.Clear();
				dia.Dias.AddRange(dias.Filtra((d)=>d.Fecha.Month==mesAnterior.Month&&d.Fecha.Day==diaAPoner));
				dia.Recordatorios.AddRange(Dia.GetRecordatorios(dias,new DateTime(mesAnterior.Year,mesAnterior.Month,diaAPoner)));  
				dia.PonFecha(diaAPoner);
			}
			for(int i=(int)diaInicio+diasMesActual-1,j=1;i<TOTALDIAS;i++,j++)
			{
			
				diaAPoner=j;
				dia=(ugDiasMes.Children[i] as DiaViewer);
				
				dia.Clear();
				dia.Dias.AddRange(dias.Filtra((d)=>d.Fecha.Month==mesSiguiente.Month&&d.Fecha.Day==diaAPoner));
				dia.Recordatorios.AddRange(Dia.GetRecordatorios(dias,new DateTime(mesSiguiente.Year,mesSiguiente.Month,diaAPoner)));  
				dia.PonFecha(diaAPoner,false);
			}
		}

		public void Save()
		{
		
			if(System.IO.File.Exists(PathCalendario))
				System.IO.File.Move(PathCalendario,PathBackUp);
			
			System.IO.File.WriteAllBytes(PathCalendario,calendario.GetBytes(calendario));
			
			if(System.IO.File.Exists(PathBackUp))
				System.IO.File.Delete(PathBackUp);
		}
	}
}