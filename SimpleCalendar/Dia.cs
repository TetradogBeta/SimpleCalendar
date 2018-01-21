/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 21/01/2018
 * Hora: 4:04
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using Gabriel.Cat;
using Gabriel.Cat.Binaris;

namespace SimpleCalendar
{
	/// <summary>
	/// Description of Dia.
	/// </summary>
	public class Dia:ElementoBinario,IComparable<Dia>,IComparable,IComparable<DateTime>
	{
		public static readonly Formato Formato;
		DateTime fecha;
		Llista<ItemCalendario> items;
		
		static Dia()
		{
			Formato=new Formato();
			
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Serializar.TiposAceptados.DateTime));
			Formato.ElementosArchivo.Add(new ElementoIEnumerableBinario(new ItemsCalendario()));
			
		}
		public Dia():this(DateTime.Now)
		{}
		public Dia(DateTime fecha)
		{
			this.fecha=fecha;
			items=new Llista<ItemCalendario>();
		}

		public DateTime Fecha {
			get {
				return fecha;
			}
			set {
				fecha = value;
			}
		}

		public Llista<ItemCalendario> Items {
			get {
				return items;
			}
		}
		public bool EstaVacia
		{
			get{return items.Count==0;}
		}
		public IList<ItemCalendario> GetRecordatorios(DateTime fecha)
		{
			List<ItemCalendario> itemsRecordatorio=new List<ItemCalendario>();
			
			//miro los items uno a uno y miro sus recordatorios si estan dentro de esta fecha
			for(int i=0;i<items.Count;i++)
				if(items[i].Recordatorio.EstaDentro(fecha))
					itemsRecordatorio.Add(items[i]);
			
			return itemsRecordatorio;
		}
		#region IComparable implementation

		public int CompareTo(Dia other)
		{
			int compareTo;
			
			if(other!=null)
				compareTo=CompareTo(other.fecha);
			else compareTo=(int)Gabriel.Cat.CompareTo.Inferior;
			
			return compareTo;
		}
		public int CompareTo(DateTime other)
		{
			return DateTime.Compare(fecha,other);
		}
		#endregion

		#region IComparable implementation

		public int CompareTo(object obj)
		{
			return CompareTo(obj as Dia);
		}

		#region implemented abstract members of ElementoBinario
		public override byte[] GetBytes(object obj)
		{
			Dia dia=(Dia)obj;
			return Dia.Formato.GetBytes(new object[]{dia.Fecha,dia.Items});
		}
		public override object GetObject(System.IO.MemoryStream bytes)
		{
			object[] partes=Dia.Formato.GetPartsOfObject(bytes);
			Dia dia=new Dia((DateTime)partes[0]);
			dia.Items.AddRange((IEnumerable<ItemCalendario>)partes[1]);
			return dia;
		}
		#endregion
		#endregion
		public static IList<ItemCalendario> GetRecordatorios(IList<Dia> dias,DateTime fechaRecordatorio)
		{
			List<ItemCalendario> recordatoriosFecha=new List<ItemCalendario>();
			for(int i=0;i<dias.Count;i++)
				recordatoriosFecha.AddRange(dias[i].GetRecordatorios(fechaRecordatorio));
			return recordatoriosFecha;
		}
	}
}
