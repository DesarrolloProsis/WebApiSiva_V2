using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiSiva.Entities
{
    public class OperacionesCajeroes
    {

      
        public long Id { get; set; }

        
        
        public string Concepto { get; set; }


        public string Tipo { get; set; }


        public string Numero { get; set; }

        public double? Monto { get; set; }


        public string TipoPago { get; set; }

        public double? CobroTag { get; set; }


        public DateTime DateTOperacion { get; set; }

        public virtual long CorteId { get; set; }
        

        public string NoReferencia { get; set; }

        public bool StatusCancelacion { get; set; }

    }
}
