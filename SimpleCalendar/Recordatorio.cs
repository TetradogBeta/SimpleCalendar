﻿/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
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
			Formato.ElementosArchivo.Add(new ElementoIEnumerableBinario(new Periodo(),ElementoIEnumerableBinario.LongitudBinaria.Byte));
		}
		public Recordatorio(IEnumerable<Periodo> periodos=null)
		{
			lstPeriodos=new List<Periodo>();
			if(periodos!=null)
				lstPeriodos.AddRange(periodos);
		}

		#region implemented abstract members of ElementoBinario

		public override byte[] GetBytes(object obj)
		{
			Recordatorio recordatorioASerializar=(Recordatorio)obj;
			return Recordatorio.Formato.GetBytes(lstPeriodos);
		}

		public override object GetObject(System.IO.MemoryStream bytes)
		{
			return new Recordatorio(Recordatorio.Formato.GetPartsOfObject(bytes).Casting<Periodo>()); 
		}

		#endregion
	}
	
}
