/*
 * Creado por SharpDevelop.
 * Usuario: tetradog
 * Fecha: 03/01/2018
 * Licencia GNU v3
 */
using System;
using System.IO;
using Gabriel.Cat.Binaris;
namespace CalendarioSimple
{
	public class Periodo
	{
		public class Serializar:ElementoComplejoBinario
		{
			public Serializar()
			{
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.DateTime));
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.TimeSpan));
				Partes.Add(ElementoBinario.ElementosTipoAceptado(Gabriel.Cat.Serializar.TiposAceptados.DateTime));
			}
			
			
			
			
			
			#region implemented abstract members of ElementoBinarioNullable
			protected override object IGetObject(MemoryStream bytes)
			{
				const int INICIO = 0;
				const int TIEMPOPERIODO = INICIO + 1;
				const int AÑOFINRECORDATORIO = TIEMPOPERIODO + 1;
				object[] partes=GetPartsObject(bytes);
				
				Periodo perido=new Periodo();
				
				perido.inicio=(DateTime)partes[INICIO];
				perido.tiempoPeriodo=(TimeSpan)partes[TIEMPOPERIODO];
				perido.añoFinRecordatorio=(DateTime)partes[AÑOFINRECORDATORIO];
				
				return perido;
			}
			#endregion
			#region implemented abstract members of ElementoComplejoBinarioNullable
			protected override System.Collections.IList GetPartsObject(object obj)
			{
				Periodo periodo=obj as Periodo;
				if(periodo==null)
					throw new ArgumentException(String.Format("El objeto a serializar no es {0}",new Periodo().GetType().FullName));
				
				return 	new object[]{periodo.inicio,periodo.tiempoPeriodo,periodo.añoFinRecordatorio};
			}
			#endregion
		
			
		
		}
		public static readonly Serializar Formato=new Serializar();
		
		
		DateTime inicio;

		TimeSpan tiempoPeriodo;

		DateTime añoFinRecordatorio;
		DateTime? fin;

		public Periodo()
		{}
		public Periodo(DateTime inicio,TimeSpan tiempoPeriodo,DateTime añoFinRecordatorio)
		{
			Inicio=inicio;
			TiempoPeriodo=tiempoPeriodo;
			AñoFinRecordatorio=añoFinRecordatorio;
		}
		public DateTime AñoFinRecordatorio {
			get {
				return añoFinRecordatorio;
			}
			set {
				if (value < Inicio)
					value = Inicio;
				añoFinRecordatorio = value;
			}
		}

		public DateTime Inicio {
			get {
				return inicio;
			}
			set {
				inicio = value;
				fin=null;
			}
		}

		public TimeSpan TiempoPeriodo {
			get {
				return tiempoPeriodo;
			}
			set {
				tiempoPeriodo = value;
				fin=null;
			}
		}

		public DateTime Fin {
			get {
				if(!fin.HasValue)
					fin=inicio + tiempoPeriodo;
				return fin.Value;
			}
		}

		public bool EstaDentro(DateTime fecha)
		{
			return Inicio<=fecha&&fecha<=Fin;
		}
		#region implemented abstract members of ElementoBinario

		public  byte[] GetBytes()
		{
			return Formato.GetBytes(this);
		}



		#endregion
	}
}


