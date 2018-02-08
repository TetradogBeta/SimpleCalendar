/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 08/02/2018
 * Hora: 1:19
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using Gabriel.Cat;
using Gabriel.Cat.Binaris;
using Gabriel.Cat.Extension;

namespace CalendarioSimple.Calendario
{
	/// <summary>
	/// Description of Calendario.
	/// </summary>
	public class Calendario
	{
		public class Serializar:ElementoComplejoBinario
		{
			public Serializar()
			{
				Partes.Add(new ElementoIListBinario<DiaCalendario>(DiaCalendario.Formato,LongitudBinaria.UShort));
			}

			#region implemented abstract members of ElementoBinarioNullable

			protected override object IGetObject(System.IO.MemoryStream bytes)
			{
				return new Calendario((DiaCalendario[])GetPartsObject(bytes)[0]);
			}

			#endregion

			#region implemented abstract members of ElementoComplejoBinarioNullable

			protected override System.Collections.IList GetPartsObject(object obj)
			{
				Calendario calendario=obj as Calendario;
				if(calendario==null)
					throw new ArgumentException(String.Format("El objeto a serializar no es {0}",new Calendario().GetType().FullName));
				return calendario.dias.ToTaula(); 
			}

			#endregion
		}
		
		public static readonly Serializar Formato=new Serializar();
		Llista<DiaCalendario> dias;
		public Calendario(IEnumerable<DiaCalendario> dias=null)
		{
			this.dias=new Llista<DiaCalendario>();
			if(dias!=null)
				this.dias.AddRange(dias);
		}
	}
}
