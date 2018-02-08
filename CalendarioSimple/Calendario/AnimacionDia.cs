/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 04/02/2018
 * Hora: 18:29
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using Gabriel.Cat.Extension;

namespace CalendarioSimple.Calendario
{
	/// <summary>
	/// Description of AnimacionDia.
	/// </summary>
	public class AnimacionDia
	{
		public const int INTERVALO=2*1000;
		int año;
		DiaCalendario dia;
		int posicion;
		int posRecordatorio;
		bool mostrarRecordatorios;
		IList<ItemDia> recordatorios;
		
		Temporizador temporizadorAnimacion;
		
		
		public event EventHandler<AnimacionEventArgs> NuevoFotograma;
		
		public AnimacionDia(DiaCalendario dia,int año)
		{
			this.dia=dia;
			Posicion=0;
			Año=año;
			temporizadorAnimacion=new Temporizador();
			temporizadorAnimacion.Interval=INTERVALO;
			temporizadorAnimacion.Elapsed+=ImagenNueva;
			mostrarRecordatorios=false;
			posRecordatorio=0;
		}

		public int Posicion {
			get {
				return posicion;
			}
			set {
				posicion = value;
			}
		}
		public int Año {
			get {
				return año;
			}
			set {
				año = value;
			}
		}

		public DiaCalendario Dia {
			get {
				return dia;
			}
			set{
				dia=value;
			}
		}

		public IList<ItemDia> Recordatorios {
			get {
				return recordatorios;
			}
			set {
				recordatorios = value;
			}
		}
		public bool IsEnabled
		{
			get{return temporizadorAnimacion.EstaOn;}
			set{
				if(value)
					temporizadorAnimacion.Start();
				else temporizadorAnimacion.Stop();
			}
		}
		void ImagenNueva(Temporizador temporizador)
		{
			ItemDia item;	
			
			if(posicion==int.MaxValue)
				posicion=0;

			item=dia.GetAt(posicion++);
			
			if(item!=null&&NuevoFotograma!=null)
				NuevoFotograma(this,new AnimacionEventArgs(item.Miniatura));

			if(recordatorios!=null&&mostrarRecordatorios&&recordatorios.Count>0&&NuevoFotograma!=null)
			{
				System.Threading.Thread.Sleep(INTERVALO);
				NuevoFotograma(this,new AnimacionEventArgs(recordatorios[posRecordatorio].Miniatura));
				posRecordatorio=(posRecordatorio+1)%recordatorios.Count;
				mostrarRecordatorios=!mostrarRecordatorios;
			}
		}
	}
	public class AnimacionEventArgs:EventArgs
	{
		Bitmap bmp;
		public AnimacionEventArgs(Bitmap bmp)
		{
			this.bmp=bmp;
		}
		public Bitmap Fotograma
		{
			get{
				return bmp;
			}
		}
	}
}
