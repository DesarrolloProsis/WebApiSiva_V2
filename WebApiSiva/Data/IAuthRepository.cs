using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiSiva.Models;

namespace WebApiSiva.Data
{
    public interface IAuthRepository
    {

        Task<bool> InsertToken(TokenValidate validate);
        Task<Users> Register(Users user, string password);
        Task<Users> Login(string username, string pasword);
        Task<bool> UserExists(string username, string numeroCliente);
        bool ConfirmCliente(string tokenID, string NumConfirmacion);
        bool ValidarToken(string webToken);
        bool InsertarNumero(string NumConfirmacion, string tokenID);
        string NumeroConfirmacion();
    }
}
