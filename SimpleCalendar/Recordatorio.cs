/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Gabriel.Cat.Binaris;
using Gabriel.Cat.Extension;

namespace SimpleCalendar
{
	/// <summary>
	/// Description of Recordatorio.
	/// </summary>
	public class Recordatorio:ElementoBinario
	{
		public static readonly Formato Formato;
		List<Periodo> lstPeriodos;
		static Recordatorio()
		{
			Formato=new Formato();
			Formato.ElementosArchivo.Add(new ElementoIListBinario(new Periodo(),ElementoIListBinario.LongitudBinaria.Byte));
		}
		public Recordatorio(IEnumerable<Periodo> periodos=null)
		{
			lstPeriodos=new List<Periodo>();
			if(periodos!=null)
				lstPeriodos.AddRange(periodos);
		}

		public bool EstaDentro(DateTime fechaRecordatorio)
		{
			bool estaDentro=false;
			for(int i=0;i<lstPeriodos.Count&&!estaDentro;i++)
				estaDentro=lstPeriodos[i].EstaDentro(fechaRecordatorio);
			return estaDentro;
		}
		#region implemented abstract members of ElementoBinario

		public override byte[] GetBytes(object obj)
		{
			Recordatorio recordatorioASerializar=(Recordatorio)obj;
			return Recordatorio.Formato.GetBytes(recordatorioASerializar.lstPeriodos);
		}

		public override object GetObject(System.IO.MemoryStream bytes)
		{
			return new Recordatorio(new List<Periodo>(((IList)Recordatorio.Formato.GetPartsOfObject(bytes)[0])));
		}

		#endregion
	}
	
}
