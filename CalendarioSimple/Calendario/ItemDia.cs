/*
 * Creado por SharpDevelop.
 * Usuario: pc
 * Fecha: 04/02/2018
 * Hora: 12:19
 * 
 * Para cambiar esta plantilla use Herramientas | Opciones | Codificación | Editar Encabezados Estándar
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using Gabriel.Cat.Extension;
using Gabriel.Cat;
using Gabriel.Cat.Binaris;

namespace CalendarioSimple.Calendario
{
	/// <summary>
	/// Description of ItemDia.
	/// </summary>
	public class ItemDia:IClauUnicaPerObjecte,IComparable,IComparable<ItemDia>
	{
		public class Serializar:ElementoComplejoBinario
		{


			public Serializar()
			{
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.String));
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.String));
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.Bitmap));
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.DateTime));
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.String));//fileInfo
				Partes.Add(Recordatorio.Formato);
				
			}
			
			public ItemDia GetItem(MemoryStream ms)
			{
				return GetObject(ms) as ItemDia;
			}


	

			
			
			#region implemented abstract members of ElementoBinarioNullable
			protected override object IGetObject(MemoryStream bytes)
			{
				const int IDRAPIDO = 0;
				const int HASH = IDRAPIDO + 1;
				const int MINIATURA = HASH + 1;
				const int FECHAMINIATURA = MINIATURA + 1;
				const int ITEM = FECHAMINIATURA + 1;
				const int RECORDATORIO=ITEM+1;
				
				object[] partes=GetPartsObject(bytes);
				ItemDia item=new ItemDia();
				
				item.idRapido=(string)partes[IDRAPIDO];
				item.hash=(string)partes[HASH];
				item.bmp=(Bitmap)partes[MINIATURA];
				item.fechaBmp=(DateTime)partes[FECHAMINIATURA];
				item.item=new FileInfo((string)partes[ITEM]);
				item.recordatorio=(Recordatorio)partes[RECORDATORIO];
				
				return item;
			}
			#endregion
			#region implemented abstract members of ElementoComplejoBinarioNullable
			protected override System.Collections.IList GetPartsObject(object obj)
			{
				ItemDia item=obj as ItemDia;
				if(item==null)
					throw new ArgumentException(String.Format("El objeto a serializar no es {0}",new ItemDia().GetType().FullName));
				
				return new object[]{item.idRapido,item.hash,item.bmp,item.fechaBmp,item.item.FullName,item.recordatorio};
			}
			#endregion
		}
		
		public static readonly Serializar Formato=new Serializar();
		static GeneradorInt GenId=new GeneradorInt();
		
		int id;
		
		string idRapido;
		string hash;
		Bitmap bmp;
		DateTime fechaBmp;
		FileInfo item;
		Recordatorio recordatorio;
		
		public ItemDia(string pathItem):this()
		{
			item=new FileInfo(pathItem);
			idRapido=item.IdUnicoRapido();
			
		}
		internal ItemDia()
		{
			id=GenId.Siguiente();
		}

		public FileInfo Item {
			get {
				return item;
			}
		}

		public string Hash
		{
			get{
				GenInfo();
				return hash;
			}
		}
		public Bitmap Miniatura
		{
			get{
				GenInfo();
				return bmp;
			}
		}

		public Recordatorio Recordatorio {
			get {
				return recordatorio;
			}
			set {
				if(value==null)
					value=new Recordatorio();
				recordatorio = value;
			}
		}
		#region IClauUnicaPerObjecte implementation


		IComparable IClauUnicaPerObjecte.Clau {
			get {
				return id;
			}
		}


		#endregion

		public byte[] GetBytes()
		{
			return Formato.GetBytes(this);
		}
		
		void GenInfo()
		{
			if(fechaBmp<item.LastWriteTime)
			{
				idRapido=item.IdUnicoRapido();
				hash=item.Sha256();
				bmp=item.Miniatura();
				fechaBmp=DateTime.Now;
			}
		}

		#region IComparable implementation
		int IComparable.CompareTo(object obj)
		{
			return Compara(obj as ItemDia);
		}
		int IComparable<ItemDia>.CompareTo(ItemDia other)
		{
			return	Compara(other);
			
		}
		int Compara(ItemDia other)
		{
			int compareTo;
			if (other != null)
				compareTo = id.CompareTo(other.id);
			else
				compareTo = (int)Gabriel.Cat.CompareTo.Inferior;
			return compareTo;
		}



		




		#endregion
		public static void BuscarItemsFaltantes(IList<ItemDia> items)
		{
			List<ItemDia> itemsFaltantes=new List<ItemDia>(items.Filtra((i)=>!i.Item.Exists));
			List<FileInfo> filesDisco;
			TwoKeysList<string,string,FileInfo> filesComprobados=new TwoKeysList<string, string, FileInfo>();
			string idRapidoAux;
			bool encontrado;
			if(itemsFaltantes.Count>0)
			{
				filesDisco=DiscoLogico.GetFiles();
				for(int i=0;i<itemsFaltantes.Count;i++)
				{
					encontrado=false;
					for(int j=0;j<filesDisco.Count&&!encontrado;j++)
					{
						if(!filesComprobados.ContainsKey1(filesDisco[j].FullName)){
							idRapidoAux=filesDisco[j].IdUnicoRapido();
							filesComprobados.Add(filesDisco[i].FullName,idRapidoAux,filesDisco[j]);
							
						}else idRapidoAux=filesComprobados.GetTkey2WhithTkey1(filesDisco[j].FullName);
						encontrado=itemsFaltantes[i].idRapido.Equals(idRapidoAux)&&itemsFaltantes[i].hash.Equals(filesDisco[j].Sha256());
						if(!encontrado)
							encontrado=itemsFaltantes[i].item.Name.Equals(filesDisco[j].Name);
						
						if(encontrado){
							itemsFaltantes[i].item=filesDisco[j];
							itemsFaltantes[i].GenInfo();
						}
						
					}
					
				}
			}
			
			
		}
	}
}
