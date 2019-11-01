using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiSiva.Models
{
    public class TokenValidate
    {
        public int Id { get; set; }
        public  string Cliente { get; set; }
        public DateTime FechaGeneracion { get; set; }
        public string WebToken { get; set; }
    }
}
