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

namespace CalendarioSimple
{
	/// <summary>
	/// Description of Calendario.
	/// </summary>
	public class Calendario
	{
		#region Serializar
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
		#endregion
		
		Llista<DiaCalendario> dias;
		public Calendario(IEnumerable<DiaCalendario> dias=null)
		{
			this.dias=new Llista<DiaCalendario>();
			if(dias!=null)
				this.dias.AddRange(dias);
		}
		public Llista<DiaCalendario> Dias {
			get {
				return dias;
			}
		}
		
		public List<DiaCalendario> GetMonth(int month,int year,int daysToView=7*6)
		{
			List<DiaCalendario> dias=new List<DiaCalendario>();
			DiaCalendario diaAPoner=null;
			
			DateTime mesActual=new DateTime(year,month,1);
			DateTime mesAnterior=mesActual.GetMesAnterior();
			DateTime mesSiguiente=mesActual.GetMesSiguiente();
			int diaDeLaSemana=(int)mesActual.DayOfWeek;//si el dia 1 es Lunes no pongo ninguno
			int mesAnteriorDiasAPoner=mesAnterior.GetDiaFinMes()-(diaDeLaSemana)+2;
			int diasMesActual=mesActual.GetDiaFinMes();
			
			for(int i=1;i<diaDeLaSemana;i++,mesAnteriorDiasAPoner++)
			{
				diaAPoner= GetDia(mesAnterior,mesAnteriorDiasAPoner);
				if(diaAPoner!=null)
					dias.Add(diaAPoner);
			}
			for(int i=1,f=diasMesActual;i<=f;i++)
			{
				diaAPoner= GetDia(mesActual,i);
				if(diaAPoner!=null)
					dias.Add(diaAPoner);
			}
			for(int i=1,f=daysToView-(diaDeLaSemana-1)-diasMesActual;i<=f;i++)
			{
				diaAPoner= GetDia(mesSiguiente,i);
				if(diaAPoner!=null)
					dias.Add(diaAPoner);
			}
			return dias;
		}

		DiaCalendario GetDia(DateTime mes,int dia)
		{
			DiaCalendario	diaAPoner = new DiaCalendario(dia, mes.Month);
			return dias.Busca(diaAPoner);
			
		}
		public List<ItemDia> GetRecordatoriosDia(int day,int month,int year)
		{
			DateTime fecha=new DateTime(year,month,day);
			List<ItemDia> recordatorios=new List<ItemDia>();
			IList<ItemDia> recordatoriosDia;
			for(int i=0;i<dias.Count;i++)
			{
				if(dias[i].HayItems(year)){
					recordatoriosDia=dias[i][year];
					recordatorios.AddRange(recordatoriosDia.Filtra((item)=>item.Recordatorio.EstaDentro(fecha)));
				}
			}
			return recordatorios;
		}

		public void AddDay(DiaCalendario dia)
		{
			if(dias.Busca(dia)==null)
				dias.Add(dia);
		}

		public void RemoveDay(DiaCalendario dia)
		{
			//si no tiene items lo elimino
			if(dia.GetAt(0)==null)
				dias.Remove(dia);
		}

		public void RemoveItem(ItemDia item)
		{
			bool encontrado=false;
			if(item!=null)
			{
				for(int i=0;i<dias.Count&&!encontrado;i++)
				{
					
					encontrado=	dias[i].RemoveItem(item);
					
				}
				
			}
		}

		public byte[] GetBytes()
		{
			return Formato.GetBytes(this);
		}
	}
}
