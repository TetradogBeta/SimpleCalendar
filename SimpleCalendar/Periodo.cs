/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
using Gabriel.Cat.Binaris;
namespace SimpleCalendar
{
	public class Periodo:ElementoBinario
	{
		public static readonly Formato Formato;
		
		
		DateTime inicio;

		TimeSpan tiempoPeriodo;

		DateTime añoFinRecordatorio;

		static Periodo()
		{
			Formato=new Formato();
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.DateTime));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.TimeSpan));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.DateTime));
		}
		public Periodo()
		{}
		public Periodo(DateTime inicio,TimeSpan tiempoPeriodo,DateTime añoFinRecordatorio)
		{
			Inicio=inicio;
			TiempoPeriodo=tiempoPeriodo;
			AñoFinRecordatorio=añoFinRecordatorio;
		}
		public DateTime AñoFinRecordatorio {
			get {
				return añoFinRecordatorio;
			}
			set {
				if (value < Inicio)
					value = Inicio;
				añoFinRecordatorio = value;
			}
		}

		public DateTime Inicio {
			get {
				return inicio;
			}
			set {
				inicio = value;
			}
		}

		public TimeSpan TiempoPeriodo {
			get {
				return tiempoPeriodo;
			}
			set {
				tiempoPeriodo = value;
			}
		}

		public DateTime Fin {
			get {
				return inicio + tiempoPeriodo;
			}
		}

		public bool EstaDentro(DateTime fecha)
		{
			return Inicio<=fecha&&fecha<=Fin;
		}
		#region implemented abstract members of ElementoBinario

		public override byte[] GetBytes(object obj)
		{
			Periodo peridodoASerializar=(Periodo)obj;
			return Periodo.Formato.GetBytes(new object[]{peridodoASerializar.Inicio,peridodoASerializar.TiempoPeriodo,peridodoASerializar.AñoFinRecordatorio});
		}

		public override object GetObject(System.IO.MemoryStream bytes)
		{
			object[] partesPeriodo=Periodo.Formato.GetPartsOfObject(bytes);
			return new Periodo((DateTime)partesPeriodo[0],(TimeSpan)partesPeriodo[1],(DateTime)partesPeriodo[2]);
		}

		#endregion
	}
}


