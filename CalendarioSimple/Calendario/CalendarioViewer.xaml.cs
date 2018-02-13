/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 13/02/2018
 * Hora: 18:08
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
using System.IO;
using Gabriel.Cat.Extension;

namespace CalendarioSimple
{
	/// <summary>
	/// Interaction logic for CalendarioViewer.xaml
	/// </summary>
	public partial class CalendarioViewer : UserControl
	{
		public static readonly string Path=System.IO.Path.Combine(Environment.CurrentDirectory,"calendario.data");
		public static readonly string PathAnterior=Path+".old";
		const int DIASAVER=7*6;
		Calendario calendario;
		int añoActual;
		int mesActual;
		public CalendarioViewer()
		{
			InitializeComponent();
			añoActual=DateTime.Now.Year;
			mesActual=DateTime.Now.Month;
			if(File.Exists(PathAnterior))
				File.Move(PathAnterior,Path);//si no se a guardado bien restauro los datos anteriores
			CargarCalendario();
			if(calendario!=null){
				for(int i=0;i<DIASAVER;i++)
					ugDiasMes.Children.Add(new DiaViewer(calendario));
				PonMes();
				
			}
		}

		public Calendario Calendario {
			get {
				return calendario;
			}
		}
		public void GuardarCalendario()
		{
			if(calendario.Dias.Count>0){
				if(File.Exists(Path))
				{
					if(File.Exists(PathAnterior))
						File.Delete(PathAnterior);
					File.Move(Path,PathAnterior);
				}
				File.WriteAllBytes(Path,calendario.GetBytes());
				if(File.Exists(PathAnterior))
					File.Delete(PathAnterior);
			}else if(File.Exists(Path)) File.Delete(Path);
		}
		void CargarCalendario()
		{
			if(File.Exists(Path))
			{
				try{
					calendario=Calendario.Formato.GetObject(File.ReadAllBytes(Path)) as Calendario;
				}catch{
					calendario=null;
				}
			}else calendario=new Calendario();
		}
		void PonMes()
		{
			List<DiaCalendario> dias=calendario.GetMonth(this.mesActual,this.añoActual,DIASAVER);
			DiaViewer diaCalendario;
			DiaCalendario aux,diaAPoner;
			int pos=0;
			DateTime mesActual=new DateTime(this.añoActual,this.mesActual,1);
			DateTime mesAnterior=mesActual.GetMesAnterior();
			DateTime mesSiguiente=mesActual.GetMesSiguiente();
			int diaDeLaSemana=(int)mesActual.DayOfWeek;//si el dia 1 es Lunes no pongo ninguno
			int mesAnteriorDiasAPoner=mesAnterior.GetDiaFinMes()-diaDeLaSemana+2;
			int diasMesActual=mesActual.GetDiaFinMes();
			
			for(int i=1;i<diaDeLaSemana;i++,mesAnteriorDiasAPoner++)
			{
				diaAPoner= GetDia(mesAnterior,mesAnteriorDiasAPoner,dias);
				diaCalendario=((DiaViewer)ugDiasMes.Children[pos]);
				if(diaAPoner!=null){
					diaCalendario.SetDia(mesAnterior.Year,mesActual.Month,diaAPoner);
				}
				else diaCalendario.SetDia(mesAnterior.Year,mesActual.Month,new DiaCalendario(mesAnteriorDiasAPoner,mesAnterior.Month));
				pos++;
			}
			for(int i=1,f=diasMesActual;i<=f;i++)
			{
				diaAPoner= GetDia(mesActual,i,dias);
				diaCalendario=((DiaViewer)ugDiasMes.Children[pos]);
				if(diaAPoner!=null)
					diaCalendario.SetDia(mesActual.Year,mesActual.Month,diaAPoner);
				else diaCalendario.SetDia(mesActual.Year,mesActual.Month,new DiaCalendario(i,mesActual.Month));
				pos++;
			}
			for(int i=1;pos<DIASAVER;i++)
			{
				diaAPoner= GetDia(mesSiguiente,i,dias);
				diaCalendario=((DiaViewer)ugDiasMes.Children[pos]);
				if(diaAPoner!=null)
					diaCalendario.SetDia(mesSiguiente.Year,mesActual.Month,diaAPoner);
				else diaCalendario.SetDia(mesSiguiente.Year,mesActual.Month,new DiaCalendario(i,mesSiguiente.Month));
				pos++;
			}
			txtAñoActual.Text=añoActual+"";
			txtMesActual.Text=mesActual.NombreMes();
			txtMesActual.Text=char.ToUpper(txtMesActual.Text[0])+txtMesActual.Text.Substring(1);
		}
		DiaCalendario GetDia(DateTime mes,int dia,List<DiaCalendario> dias)
		{
			DiaCalendario	diaAPoner = new DiaCalendario(dia, mes.Month);
			return dias.Busca(diaAPoner);
			
		}
		void TxtAñoActual_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if(e.Delta<0)
				añoActual--;
			else añoActual++;
			PonMes();
		}
		void TxtMesActual_MouseWheel(object sender, MouseWheelEventArgs e)
		{
			if(e.Delta<0)
				mesActual--;
			else mesActual++;
			
			if(mesActual==13){
				añoActual++;
				mesActual=1;
			}
			else if(mesActual==0){
				mesActual=12;
				añoActual--;
			}
			PonMes();
		}
	}
}