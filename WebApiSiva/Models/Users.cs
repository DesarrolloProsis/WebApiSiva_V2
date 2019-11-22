using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiSiva.Models
{
    public class Users
    {
        public string Id { get; set; }
        public string NumeroCliente { get; set; }
        public string Email { get; set; }
        public string NumeroVerificacion { get; set; }
        public bool Validado { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
    }
}
