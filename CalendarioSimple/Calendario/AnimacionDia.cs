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

namespace CalendarioSimple
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
		ItemDia itemActual;
		Temporizador temporizadorAnimacion;
		
		
		public event EventHandler<AnimacionEventArgs> NuevoFotograma;

		public AnimacionDia()
		{
			Posicion=0;
			itemActual=null;
			temporizadorAnimacion=new Temporizador();
			temporizadorAnimacion.Interval=INTERVALO;
			temporizadorAnimacion.Elapsed+=ImagenNueva;
			mostrarRecordatorios=false;
			posRecordatorio=0;
		}
		public AnimacionDia(DiaCalendario dia,int año):this()
		{
			this.dia=dia;
			Año=año;
		}

		public int Posicion {
			get {
				return posicion;
			}
			set {
				posicion = value;
				ImagenNueva();
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
				itemActual=null;
				IsEnabled=false;
				posicion=0;
				posRecordatorio=0;
			}
		}

		public ItemDia ItemActual {
			get {
				return itemActual;
			}
		}
		public IList<ItemDia> Recordatorios {
			get {
				return recordatorios;
			}
			set {
				IsEnabled=false;
				recordatorios = value;
				itemActual=null;
				posicion=0;
				posRecordatorio=0;
			}
		}
		public bool IsEnabled
		{
			get{return temporizadorAnimacion.EstaOn;}
			set{
				if(value){
					temporizadorAnimacion.Start();
				}
				else temporizadorAnimacion.StopAndAbort();
			}
		}
		void ImagenNueva(Temporizador temporizador=null)
		{
			int total;
			if(Dia!=null&&año>0&&IsEnabled){

				total=dia.GetTotal(Año);
				if(total>0){
					itemActual=dia.GetAt(posicion++,Año);
					posicion=posicion%total;
					if(itemActual!=null&&NuevoFotograma!=null)
						NuevoFotograma(this,new AnimacionEventArgs(itemActual.Miniatura));
				}
				if(recordatorios!=null&&mostrarRecordatorios&&recordatorios.Count>0&&NuevoFotograma!=null)
				{
					System.Threading.Thread.Sleep(INTERVALO);
					itemActual=recordatorios[posRecordatorio];
					NuevoFotograma(this,new AnimacionEventArgs(itemActual.Miniatura));
					posRecordatorio=(posRecordatorio+1)%recordatorios.Count;
					mostrarRecordatorios=!mostrarRecordatorios;
				}
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
