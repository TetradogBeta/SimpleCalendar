/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 21/01/2018
 * Hora: 4:10
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
	/// Description of Calendario.
	/// </summary>
	public class Calendario:ElementoBinario
	{
		public static Formato Formato;
		LlistaOrdenada<Dia> diasConItems;
		static Calendario()
		{
			Formato=new Formato();
			Formato.ElementosArchivo.Add(new ElementoIEnumerableBinario(new Dia()));
		}
		public Calendario()
		{
			diasConItems=new LlistaOrdenada<Dia>();
		}

		public LlistaOrdenada<Dia> DiasConItems {
			get {
				return diasConItems;
			}
		}

		
		public IList<Dia> GetDias(int mes)
		{
			List<Dia> dias=new List<Dia>();
			Dia aux;
			for(int i=0;i<diasConItems.Count;i++){
				aux=diasConItems.GetValueAt(i);
				
				if(aux.Fecha.Month==mes)
					dias.Add(aux);
			}
			return dias;
			
		}
		
		public void CambioFechaItem(ItemCalendario item,DateTime antiguaFecha,DateTime nuevaFecha)
		{
			if(item==null)
				throw new ArgumentNullException();
			
			Dia fechaNueva=new Dia(nuevaFecha);
			
			//si esta lo quito del dia que esta
			EliminarItem(item,antiguaFecha);
			
			//ahora lo pongo en su nueva fecha
			if(diasConItems.ContainsKey(fechaNueva))
				fechaNueva=diasConItems.GetValue(fechaNueva);
			else diasConItems.Add(fechaNueva);
			
			fechaNueva.Items.Add(item);
			
			
		}

		public void EliminarItem(ItemCalendario item,DateTime fecha)
		{
			if(item==null)
				throw new ArgumentNullException();
			Dia aux=new Dia(fecha);
			if(diasConItems.ContainsKey(aux))
				diasConItems.GetValue(aux).Items.Remove(item);
		}
		public void AñadirItem(ItemCalendario item,DateTime fecha)
		{
			if(item==null)
				throw new ArgumentNullException();
			Dia aux=new Dia(fecha);
			if(!diasConItems.ContainsKey(aux))
				diasConItems.Add(aux);
			
			diasConItems.GetValue(fecha).Items.Add(item);
		}
		public void AñadirItem(IList<ItemCalendario> items,DateTime fecha)
		{
			if(items==null)
				throw new ArgumentNullException();
			
			for(int i=0;i<items.Count;i++)
				AñadirItem(items[i],fecha);
		}
		public Dia AñadirItems(DateTime fecha,IList<string> pathItems)
		{
			Dia diaFecha;
			Dia aux=new Dia(fecha);
			if(!diasConItems.ContainsKey(aux))
				diasConItems.Add(aux);
			
			diaFecha=diasConItems.GetValue(aux);
			
			for(int i=0;i<pathItems.Count;i++)
			{
				diaFecha.Items.Add(new ItemCalendario(new System.IO.FileInfo(pathItems[i])));
			}
			return diaFecha;
		}
		#region implemented abstract members of ElementoBinario

		public override byte[] GetBytes(object obj=null)
		{
			if(obj==null)obj=this;
			Calendario calendario=(Calendario)obj;
			return Calendario.Formato.GetBytes(calendario.diasConItems.Values);
		}

		public override object GetObject(System.IO.MemoryStream bytes)
		{
			Calendario calendario=new Calendario();
			object[] partes=Calendario.Formato.GetPartsOfObject(bytes);
			calendario.diasConItems.AddRange(new List<Dia>((IEnumerable<Dia>)partes[0]));
			return calendario;
		}

		#endregion
	}
}
