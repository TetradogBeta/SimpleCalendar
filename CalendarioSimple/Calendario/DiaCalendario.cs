/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 04/02/2018
 * Hora: 18:11
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Linq;
using Gabriel.Cat;
using Gabriel.Cat.Binaris;
namespace CalendarioSimple.Calendario
{
	/// <summary>
	/// Description of DiaCalendario.
	/// </summary>
	public class DiaCalendario:IComparable,IComparable<DiaCalendario>,IClauUnicaPerObjecte
	{
		public class Serializar:ElementoComplejoBinario
		{
			class SerializarAño:ElementoComplejoBinario
			{
				public SerializarAño()
				{
					Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Int));
					Partes.Add(new ElementoIListBinario<ItemDia>(ItemDia.Formato));
					
				}

				#region implemented abstract members of ElementoBinarioNullable

				protected override object IGetObject(System.IO.MemoryStream bytes)
				{
					const int AÑO = 0;
					const int ITEMS = AÑO + 1;
					
					object[] partes=GetPartsObject(bytes);
					
					return new KeyValuePair<int,ItemDia[]>((int)partes[AÑO],(ItemDia[])partes[ITEMS]);
				}

				#endregion

				#region implemented abstract members of ElementoComplejoBinarioNullable

				protected override System.Collections.IList GetPartsObject(object obj)
				{
					KeyValuePair<int,ItemDia[]> año=(KeyValuePair<int,ItemDia[]>)obj;
					
					return new object[]{año.Key,año.Value};
				}

				#endregion
			}
			static readonly SerializarAño FormatoAño=new SerializarAño();
			public Serializar()
			{
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Int));
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Int));
				Partes.Add(new ElementoIListBinario<KeyValuePair<int,ItemDia[]>>(FormatoAño));
				
			}
			#region implemented abstract members of ElementoBinarioNullable
			protected override object IGetObject(System.IO.MemoryStream bytes)
			{
				const int DIA = 0;
				const int MES = DIA + 1;
				const int ITEMSAÑO = MES + 1;
				
				object[] partes=GetPartsObject(bytes);
				DiaCalendario dia=new DiaCalendario((int)partes[DIA],(int)partes[MES]);
				KeyValuePair<int,ItemDia[]>[] años=(KeyValuePair<int,ItemDia[]>[])partes[ITEMSAÑO];
				for(int i=0;i<años.Length;i++)
				{
					dia.itemsPorAño.Add(años[i].Key,new Llista<ItemDia>(años[i].Value));
				}
				return dia;
				
			}
			#endregion
			#region implemented abstract members of ElementoComplejoBinarioNullable
			protected override System.Collections.IList GetPartsObject(object obj)
			{
				DiaCalendario dia=obj as DiaCalendario;
				KeyValuePair<int,ItemDia[]>[] itemsAño;
				KeyValuePair<int, Llista<ItemDia>> aux;
				if(dia==null)
				{
					throw new ArgumentException(String.Format("El objeto a serializar no es {0}",new DiaCalendario(1,1).GetType().FullName));
				}
				itemsAño=new KeyValuePair<int, ItemDia[]>[dia.itemsPorAño.Count];
				for(int i=0;i<itemsAño.Length;i++)
				{
					aux=dia.itemsPorAño[i];
					itemsAño[i]=new KeyValuePair<int, ItemDia[]>(aux.Key,aux.Value.ToArray());
				}
				
				return new object[]{dia.dia,dia.mes,itemsAño};
			}
			#endregion
			
		}
		
		public static readonly Serializar Formato=new Serializar();
		
		int dia;
		int mes;
		
		LlistaOrdenada<int,Llista<ItemDia>> itemsPorAño;
		
		public DiaCalendario(DateTime fecha):this(fecha.Day,fecha.Month)
		{}
		public DiaCalendario(int dia,int mes)
		{
			this.dia=dia;
			this.mes=mes;
			itemsPorAño=new LlistaOrdenada<int, Llista<ItemDia>>();
		}

		public int Dia {
			get {
				return dia;
			}
		}

		public int Mes {
			get {
				return mes;
			}
		}
		/// <summary>
		/// Devuelve la lista de items del año pasado por parametro
		/// </summary>
		public Llista<ItemDia> this[int año]
		{
			get{
				Llista<ItemDia> items;

				if(!itemsPorAño.ContainsKey(año))
					itemsPorAño.Add(año,new Llista<ItemDia>());
				
				items=itemsPorAño.GetValue(año);
				
				return items;
			}
		}

		#region IClauUnicaPerObjecte implementation
		IComparable IClauUnicaPerObjecte.Clau {
			get {
				return this;
			}
		}
		#endregion
		public ItemDia GetAt(int posicion)
		{
			Llista<ItemDia> itemsAño;
			
			ItemDia item=null;
			for(int i=0;i<itemsPorAño.Count&&item==null;i++)
			{
				itemsAño=itemsPorAño.GetValueAt(i);
				
				if(posicion>itemsAño.Count)
					posicion-=itemsAño.Count;
				else item=itemsAño[posicion];
				
			}
			return item;
		}
		public void RemoveAt(int posicion)
		{
			bool encontrado=false;
			Llista<ItemDia> itemsAño;
			
			for(int i=0;i<itemsPorAño.Count&!encontrado;i++)
			{
				itemsAño=itemsPorAño.GetValueAt(i);
				
				if(posicion>itemsAño.Count)
					posicion-=itemsAño.Count;
				else{
					itemsAño.RemoveAt(posicion);
					encontrado=true;
				}
				
			}
		}
		public void ClearEmptyYears()
		{
			for(int i=0,f=itemsPorAño.Count,k=0;i<f;i++)
			{
				if(itemsPorAño.GetValueAt(k).Count==0)
				{
					itemsPorAño.RemoveAt(k);
				}else k++;
			}
		}
		public bool HayItems(int año)
		{
			return itemsPorAño.ContainsKey(año)&&itemsPorAño.GetValue(año).Count>0;
		}

		#region IComparable implementation

		int IComparable.CompareTo(object obj)
		{
			return Compara(obj as DiaCalendario);
		}

		#endregion

		#region IComparable implementation

		int IComparable<DiaCalendario>.CompareTo(DiaCalendario other)
		{
			return Compara(other);
		}

		#endregion

		int Compara(DiaCalendario diaCalendario)
		{
			int compareTo;
			if(diaCalendario!=null)
			{
				compareTo=mes.CompareTo(diaCalendario.mes);
				if(compareTo==(int)Gabriel.Cat.CompareTo.Iguales)
					compareTo=dia.CompareTo(diaCalendario.dia);
				
				
			}else compareTo=(int)Gabriel.Cat.CompareTo.Inferior;
			return compareTo;
		}
	}
}
