/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Gabriel.Cat.Binaris;
using Gabriel.Cat.Extension;

namespace CalendarioSimple.Calendario
{
	/// <summary>
	/// Description of Recordatorio.
	/// </summary>
	public class Recordatorio
	{
		public class Serializar:ElementoComplejoBinario
		{
			public Serializar()
			{
				Partes.Add(new ElementoIListBinario<Periodo>(Periodo.Formato,LongitudBinaria.Byte));
			}
			
			#region implemented abstract members of ElementoBinarioNullable
			protected override object IGetObject(MemoryStream bytes)
			{
				return new Recordatorio((Periodo[])(GetPartsObject(bytes)[0]));
			}
			#endregion
			#region implemented abstract members of ElementoComplejoBinarioNullable
			protected override IList GetPartsObject(object obj)
			{
				Recordatorio recordatorio=obj as Recordatorio;
				if(recordatorio==null)
					throw new ArgumentException(String.Format("El objeto a serializar no es {0}",new Recordatorio().GetType().FullName));
				return recordatorio.lstPeriodos;
			}
			#endregion
		}
		
		public static readonly Serializar Formato=new Serializar();
		
		List<Periodo> lstPeriodos;
		

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

		public  byte[] GetBytes()
		{
			return Formato.GetBytes(this);
		}



	}
	
}
