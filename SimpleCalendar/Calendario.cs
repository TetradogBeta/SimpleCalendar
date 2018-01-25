/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 21/01/2018
 * Hora: 4:10
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
	/// Description of Calendario.
	/// </summary>
	public class Calendario:ElementoBinario
	{
		public static Formato Formato;
		LlistaOrdenada<DiaCalendario> diasConItems;
		static Calendario()
		{
			Formato=new Formato();
			Formato.ElementosArchivo.Add(new ElementoIListBinario(new DiaCalendario()));
		}
		public Calendario()
		{
			diasConItems=new LlistaOrdenada<DiaCalendario>();
		}

		public LlistaOrdenada<DiaCalendario> DiasConItems {
			get {
				return diasConItems;
			}
		}

		
		public List<DiaCalendario> GetDias(int mes,int año)
		{
			byte bMes=(byte)mes;
			DiaCalendario dia;
			List<DiaCalendario> dias=new List<DiaCalendario>();
			for(byte i=0,f=(byte)new DateTime(año,mes,1).GetDiaFinMes();i<f;i++)
			{
				dia=new DiaCalendario(i,bMes);
				if(diasConItems.ContainsKey(dia))
					dias.Add(diasConItems.GetValue(dia));
			}
			return dias;
			
		}

		public DiaCalendario GetDia(int diaAPoner, int month)
		{
			DiaCalendario dia=new DiaCalendario((byte)diaAPoner,(byte)month);
			
			if(diasConItems.ContainsKey(dia))
				dia=diasConItems.GetValue(dia);
			else dia=null;
			
			return dia;
		}
		public DiaCalendario GetDia(ItemCalendario item)
		{
			DiaCalendario dia=null,aux;
			for(int i=0;i<diasConItems.Count&&dia==null;i++){
				aux=diasConItems.GetValueAt(i);
				if(aux.Contains(item))
					dia=aux;
			}
			
			return dia;
		}
		public void CambioFechaItem(ItemCalendario item,DateTime antiguaFecha,DateTime nuevaFecha)
		{
			if(item==null)
				throw new ArgumentNullException();
			
			DiaCalendario fechaNueva=new DiaCalendario(nuevaFecha);
			
			//si esta lo quito del dia que esta
			EliminarItem(item,antiguaFecha);
			
			//ahora lo pongo en su nueva fecha
			if(diasConItems.ContainsKey(fechaNueva))
				fechaNueva=diasConItems.GetValue(fechaNueva);
			else diasConItems.Add(fechaNueva);
			
			fechaNueva.Add(nuevaFecha.Year,item);
			
			
		}

		public DiaCalendario GetDiaItem(ItemCalendario item)
		{
			if(item==null)
				throw new ArgumentNullException();
			
			DiaCalendario diaItem=null;
			DiaCalendario aux;
			for(int i=0;i<diasConItems.Count&&diaItem==null;i++)
			{
				aux=diasConItems.GetValueAt(i);
				if(aux.Contains(item))
					diaItem=aux;
			}
			return diaItem;
		}
		public void EliminarItem(ItemCalendario item,DateTime fecha)
		{
			if(item==null)
				throw new ArgumentNullException();
			DiaCalendario aux=new DiaCalendario(fecha);
			if(diasConItems.ContainsKey(aux))
				diasConItems.GetValue(aux).Remove(fecha.Year,item);
		}
		public void AñadirItem(ItemCalendario item,DateTime fecha)
		{
			if(item==null)
				throw new ArgumentNullException();
			DiaCalendario aux=new DiaCalendario(fecha);
			if(!diasConItems.ContainsKey(aux))
				diasConItems.Add(aux);
			
			diasConItems.GetValue(fecha).Add(fecha.Year,item);
		}
		public void AñadirItem(IList<ItemCalendario> items,DateTime fecha)
		{
			if(items==null)
				throw new ArgumentNullException();
			
			for(int i=0;i<items.Count;i++)
				AñadirItem(items[i],fecha);
		}
		public DiaCalendario AñadirItems(DateTime fecha,IList<string> pathItems)
		{
			DiaCalendario diaFecha;
			DiaCalendario aux=new DiaCalendario(fecha);
			if(!diasConItems.ContainsKey(aux))
				diasConItems.Add(aux);
			
			diaFecha=diasConItems.GetValue(aux);
			
			for(int i=0;i<pathItems.Count;i++)
			{
				diaFecha.Add(fecha.Year,new ItemCalendario(new System.IO.FileInfo(pathItems[i])));
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
			calendario.diasConItems.AddRange(new List<DiaCalendario>(partes[0] as IEnumerable));
			return calendario;
		}

		#endregion
	}
}
