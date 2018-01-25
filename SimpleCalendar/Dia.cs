/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 21/01/2018
 * Hora: 4:04
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections;
using System.Collections.Generic;
using Gabriel.Cat;
using Gabriel.Cat.Binaris;
using Gabriel.Cat.Extension;

namespace SimpleCalendar
{
	/// <summary>
	/// Description of Dia.
	/// </summary>
	public class DiaCalendario:ElementoBinario,IComparable<DiaCalendario>,IComparable
	{
		class ItemsAño:ElementoBinario
		{
			public static Formato Formato;
			
			short año;
			IList items;

			internal ItemsAño()
			{}
			public ItemsAño(KeyValuePair<short,List<ItemCalendario>> itemsAño):this(itemsAño.Key,(IList)itemsAño.Value)
			{
			}
			public ItemsAño(short año,IList items)
			{
				this.año=año;
				this.items=items;
			}

			public short Año {
				get {
					return año;
				}
			}

			public IList Items {
				get {
					return items;
				}
			}
			#region implemented abstract members of ElementoBinario

			public override byte[] GetBytes(object obj)
			{
				ItemsAño itemsAño=obj as ItemsAño;
				return ItemsAño.Formato.GetBytes(new object[]{itemsAño.año,itemsAño.items});
			}

			public override object GetObject(System.IO.MemoryStream bytes)
			{
				object[] partes=ItemsAño.Formato.GetPartsOfObject(bytes);
				return new ItemsAño((short)partes[0],(IList)partes[1]);
			}

			#endregion
		}
		public static readonly Formato Formato;
		
		byte dia;
		byte mes;
		
		LlistaOrdenada<short,Llista<ItemCalendario>> itemsAño;
		int count;
		static DiaCalendario()
		{
			Formato=new Formato();
			
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.Byte));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.Byte));
			Formato.ElementosArchivo.Add(new ElementoIListBinario(new ItemsAño()));
			
		}
		public DiaCalendario():this(DateTime.Now)
		{}
		public DiaCalendario(DateTime fecha):this((byte)fecha.Day,(byte)fecha.Month)
		{
			
		}
		public DiaCalendario(byte dia,byte mes)
		{
			this.dia=dia;
			this.mes=mes;
			count=0;
			itemsAño=new LlistaOrdenada<short,Llista<ItemCalendario>>();
		}

		public int Count {
			get{return count;}
			
		}
		public byte Dia {
			get {
				return dia;
			}
		}

		public byte Mes {
			get {
				return mes;
			}
		}
		public List<ItemCalendario> GetRecordatorios(DateTime fecha)
		{
			List<ItemCalendario> itemsRecordatorio=new List<ItemCalendario>();
			Llista<ItemCalendario> items;
			//miro los items uno a uno y miro sus recordatorios si estan dentro de esta fecha
			for(int i=0;i<itemsAño.Count;i++){
				items=itemsAño.GetValueAt(i);
				for(int j=0,fJ=items.Count;j<fJ;j++)
					if(items[j].Recordatorio.EstaDentro(fecha))
						itemsRecordatorio.Add(items[j]);
			}
			
			return itemsRecordatorio;
		}

		public int GetAño(ItemCalendario item)
		{
			KeyValuePair<short,Llista<ItemCalendario>> año=new KeyValuePair<short, Llista<ItemCalendario>>(-1,null);
			KeyValuePair<short,Llista<ItemCalendario>> aux;
			
			for(int i=0;i<itemsAño.Count&&año.Key<0;i++){
				aux=itemsAño[i];
				if(aux.Value.Contains(item))
					año=aux;
			}
			return año.Key;
		}
		public ItemCalendario GetItemAt(int posicion)
		{
			ItemCalendario item=null;
			Llista<ItemCalendario> items;
			for(int i=0;i<itemsAño.Count&&item==null&&posicion>0;i++)
			{
				items=itemsAño.GetValueAt(i);
				
				if(posicion>items.Count)
					posicion-=items.Count;
				else item=items[posicion];
			}
			return item;
		}

		#region IComparable implementatio
		public void Add(int año,IList<string> pathItems)
		{
			for(int i=0;i<pathItems.Count;i++)
				Add(año,new ItemCalendario(new System.IO.FileInfo(pathItems[i])));
		}
		public void Add(int año,ItemCalendario item)
		{
			short sAño=(short)año;
			
			if(!itemsAño.ContainsKey(sAño))
				itemsAño.Add(sAño,new Llista<ItemCalendario>());
			
			itemsAño[sAño].Add(item);
			count++;
		}
		public void Remove(int año,ItemCalendario item)
		{
			short sAño=(short)año;
			
			if(itemsAño.ContainsKey(sAño))
			{
				if(itemsAño[sAño].Remove(item))
					count--;
				
				if(itemsAño[sAño].Count==0)
					itemsAño.Remove(sAño);
			}
			
			
		}
		public bool Contains(ItemCalendario item)
		{
			bool contiene=false;
			for(int i=0;i<itemsAño.Count&&!contiene;i++)
				contiene=itemsAño.GetValueAt(i).Contains(item);
			return contiene;
		}
		public int CompareTo(DiaCalendario other)
		{
			int compareTo;
			
			if(other!=null){
				compareTo=dia.CompareTo(other.Dia);
				if(compareTo==(int)Gabriel.Cat.CompareTo.Iguales)
					compareTo=mes.CompareTo(other.Mes);
			}
			else compareTo=(int)Gabriel.Cat.CompareTo.Inferior;
			
			return compareTo;
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(object obj)
		{
			return CompareTo(obj as DiaCalendario);
		}

		#region implemented abstract members of ElementoBinario
		public override byte[] GetBytes(object obj)
		{
			KeyValuePair<short,Llista<ItemCalendario>> items;
			DiaCalendario dia=(DiaCalendario)obj;
			IList lstItems=new List<object>();
			for(int i=0;i<dia.itemsAño.Count;i++)
			{
				items=dia.itemsAño[i];
				lstItems.Add(new ItemsAño(items.Key,items.Value.ToTaula()));
			}
			return DiaCalendario.Formato.GetBytes(new object[]{dia.dia,dia.mes,lstItems});
		}
		public override object GetObject(System.IO.MemoryStream bytes)
		{
			object[] partes=DiaCalendario.Formato.GetPartsOfObject(bytes);
			DiaCalendario dia=new DiaCalendario((byte)partes[0],(byte)partes[1]);
			IList lst=(IList)partes[2];
			ItemsAño items;
			for(int i=0;i<lst.Count;i++)
			{
				items=(ItemsAño)lst[i];
				dia.itemsAño.Add(items.Año,new Llista<ItemCalendario>(new List<ItemCalendario>(items.Items)));
			}
			return dia;
		}
		#endregion
		#endregion
		public static IList<ItemCalendario> GetRecordatorios(LlistaOrdenada<DiaCalendario> dias,DateTime fechaRecordatorio)
		{
			List<ItemCalendario> recordatoriosFecha=new List<ItemCalendario>();
			for(int i=0;i<dias.Count;i++)
				recordatoriosFecha.AddRange(dias.GetValueAt(i).GetRecordatorios(fechaRecordatorio));
			return recordatoriosFecha;
		}
		public static IList<ItemCalendario> GetRecordatorios(IList<DiaCalendario> dias,DateTime fechaRecordatorio)
		{
			List<ItemCalendario> recordatoriosFecha=new List<ItemCalendario>();
			for(int i=0;i<dias.Count;i++)
				recordatoriosFecha.AddRange(dias[i].GetRecordatorios(fechaRecordatorio));
			return recordatoriosFecha;
		}
		
	}
}
