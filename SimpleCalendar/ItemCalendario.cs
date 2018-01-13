/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Gabriel.Cat.Binaris;
using Gabriel.Cat.Extension;
namespace SimpleCalendar
{
	/// <summary>
	/// Description of ItemCalendario.
	/// </summary>
	public class ItemCalendario:ElementoBinario,IComparable,IComparable<ItemCalendario>
	{
		public static readonly Formato Formato;
		DateTime fechaInicio;
		string titulo;
		string descripcion;
		FileInfo item;
		string hash;
		int posicion;
		Bitmap bmpMiniatura;
		Recordatorio recordatorio;
		static ItemCalendario()
		{
			Formato=new Formato();
			
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.DateTime));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.String));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.String));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.String));//FileInfo
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.String));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Int));
			Formato.ElementosArchivo.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Bitmap));
			Formato.ElementosArchivo.Add(new Recordatorio());
		}
		public ItemCalendario(FileInfo file)
		{
			Item=file;
			recordatorio=new Recordatorio();
		}
		internal ItemCalendario()
		{}

		public int Posicion {
			get {
				return posicion;
			}
			set {
				posicion = value;
			}
		}
		public DateTime FechaInicio {
			get {
				return fechaInicio;
			}
			set {
				fechaInicio = value;
			}
		}

		public string Descripcion {
			get {
				return descripcion;
			}
			set {
				descripcion = value;
			}
		}
		public string Titulo {
			get {
				return titulo;
			}
			set {
				titulo = value;
			}
		}

		public FileInfo Item {
			get {
				return item;
			}
			set {
				if(value==null)
					throw new NullReferenceException();
				
				item = value;
				bmpMiniatura=null;
				hash=null;
			}
		}

		public string Hash {
			get {
				if(hash==null)
					hash=item.Sha256();
				return hash;
			}
		}
		public Bitmap Miniatura
		{
			get{

				if(bmpMiniatura==null)
					bmpMiniatura=item.Miniatura();
				
				return bmpMiniatura;
				
			}
		}
		
		public Recordatorio Recordatorio {
			get {
				return recordatorio;
			}
		}

		#region IComparable implementation

		public int CompareTo(object obj)
		{
			return CompareTo(obj as ItemCalendario);
		}

		#endregion

		#region IComparable implementation

		public int CompareTo(ItemCalendario other)
		{
			int compareTo;
			if(other!=null)
			{
				compareTo=fechaInicio.CompareTo(other.fechaInicio);
				if(compareTo==(int)Gabriel.Cat.CompareTo.Iguales)
				{
					compareTo=Posicion.CompareTo(other.Posicion);
					if(compareTo==(int)Gabriel.Cat.CompareTo.Iguales)
					{
						compareTo=Hash.CompareTo(other.Hash);
					}
				}
			}else compareTo=(int)Gabriel.Cat.CompareTo.Inferior;
			
			return compareTo;
		}

		#endregion

		#region implemented abstract members of ElementoBinario

		public override byte[] GetBytes(object obj)
		{
			ItemCalendario itemCalendario=(ItemCalendario)obj;
			return ItemCalendario.Formato.GetBytes(new object[]{FechaInicio,Titulo,Descripcion,Item.FullName,Hash,Posicion,Miniatura,Recordatorio});
			
		}

		public override object GetObject(MemoryStream bytes)
		{
			object[] partes=ItemCalendario.Formato.GetPartsOfObject(bytes);
			ItemCalendario item=new ItemCalendario();
			item.fechaInicio=(DateTime)partes[0];
			item.titulo=(string)partes[1];
			item.descripcion=(string)partes[2];
			item.item=new FileInfo((string)partes[3]);
			item.hash=(string)partes[4];
			item.posicion=(int)partes[5];
			item.bmpMiniatura=(Bitmap)partes[6];
			item.recordatorio=(Recordatorio)partes[7];
			
			return item;
		}
		

		#endregion
	}
	public class ItemsCalendario:ElementoBinario
	{
		public static readonly Formato Formato;
		
		List<ItemCalendario> items;
		static ItemsCalendario()
		{
			Formato=new Formato();
			Formato.ElementosArchivo.Add(new ElementoIEnumerableBinario(new ItemCalendario()));
		}
		public ItemsCalendario()
		{
			items=new List<ItemCalendario>();
		}

		public List<ItemCalendario> Items {
			get {
				return items;
			}
		}

		#region implemented abstract members of ElementoBinario

		public override byte[] GetBytes(object obj)
		{
			ItemsCalendario items=(ItemsCalendario)obj;
			return ItemsCalendario.Formato.GetBytes(items.Items);
		}

		public override object GetObject(MemoryStream bytes)
		{
			object[] partes=ItemsCalendario.Formato.GetPartsOfObject(bytes);
			ItemsCalendario items=new ItemsCalendario();
			items.Items.AddRange(partes.Casting<ItemCalendario>());
			return items;
		}

		#endregion

		
	}
}

