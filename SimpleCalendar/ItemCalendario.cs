/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
using System.Drawing;
using System.IO;
using Gabriel.Cat.Extension;
namespace SimpleCalendar
{
	/// <summary>
	/// Description of ItemCalendario.
	/// </summary>
	public class ItemCalendario:IComparable,IComparable<ItemCalendario>
	{
		DateTime fechaInicio;
		string titulo;
		string descripcion;
		FileInfo item;
		string hash;
		int posicion;
		Bitmap bmpMiniatura;
		Recordatorio recordatorio;
		
		public ItemCalendario(FileInfo file)
		{
			Item=file;
			recordatorio=new Recordatorio();
		}

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
					compareTo=Hash.CompareTo(other.Hash);
				}
			}else compareTo=(int)Gabriel.Cat.CompareTo.Inferior;
			
			return compareTo;
		}

		#endregion
	}
}
