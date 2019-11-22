using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using WebApiSiva.Models;

namespace WebApiSiva.Data
{
    public class AuthRepository: IAuthRepository
    {
        private readonly AppDbContext _context;

        public AuthRepository(AppDbContext context)
        {
            _context = context;
        }

        public bool ValidarToken(string webToken)
        {

            var fechaActual = DateTime.Now;
            var tokenConsult = _context.TokenValidates.FirstOrDefault(x => x.WebToken == webToken && x.FechaGeneracion.AddHours(1) > fechaActual);
            

            if (tokenConsult != null)
            {

                var deleteFromUser = _context.TokenValidates.Where(x => x.FechaGeneracion.AddHours(1) < DateTime.Now).ToList();

                foreach (var item in deleteFromUser)
                {

                    _context.TokenValidates.Remove(item);
                    _context.SaveChanges();

                }

                return true;
              
            }

            return false;
     
        }

        public async Task<bool> InsertToken(TokenValidate validate)
        {
            bool Regresa = false;
            try
            {

                var TokenExist = _context.TokenValidates.Where(x => x.WebToken == validate.WebToken && x.FechaGeneracion.AddHours(1) < DateTime.Now).ToList().Count();

                if (TokenExist == 0)
                {


                    _context.TokenValidates.Add(validate);
                    var boleana = await _context.SaveChangesAsync();

                    if (boleana == 1)
                    {
                        Regresa = true;

                        var deleteFromUser = _context.TokenValidates.Where(x => x.FechaGeneracion.AddHours(1) < DateTime.Now).ToList();

                        foreach (var item in deleteFromUser)
                        {

                            _context.TokenValidates.Remove(item);
                            _context.SaveChanges();

                        }
                    }
                    else
                        Regresa = false;
                }
                else
                {
                    Regresa = false;
                }


            }
            catch(Exception ex)
            {

            }

            return Regresa;
               
        }

        public async Task<Users> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email); //Get user from database.
            if (user == null)
                return null; // User does not exist.

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
        }

        public bool InsertarNumero(string numConfirmacion, string tokenID)
        {
            var oldUser = _context.Users.Where(x => x.Id == tokenID && x.Validado == false);
            Users userNew = new Users();

            foreach (var item in oldUser)
            {
                userNew = new Users
                {
                    Id = item.Id,
                    NumeroCliente = item.NumeroCliente,
                    PasswordHash = item.PasswordHash,
                    Validado = true,
                    PasswordSalt = item.PasswordSalt,
                    NumeroVerificacion = numConfirmacion,
                };
            }

            try
            {
                _context.Update(userNew);   
                _context.SaveChanges();
            }

            catch (Exception ex)
            {
                var str = ex;
            }


            return true;

        }

        private bool VerifyPassword(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)); // Create hash using password salt.
                for (int i = 0; i < computedHash.Length; i++)
                { // Loop through the byte array
                    if (computedHash[i] != passwordHash[i]) return false; // if mismatch
                }
            }
            return true; //if no mismatches.
        }

        public async Task<Users> Register(Users user, string password)
        {
            byte[] passwordHash, passwordSalt;
            CreatePasswordHash(password, out passwordHash, out passwordSalt);

            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;

            user.Id = Guid.NewGuid().ToString();
            user.Validado = false;
            user.NumeroVerificacion = NumeroConfirmacion();

            try
            {


                await _context.Users.AddAsync(user); // Adding the user to context of users.
                await _context.SaveChangesAsync(); // Save changes to database.
            }
            catch (Exception ex)
            {
                var str = ex;
            }

            return user;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<bool> UserExists(string email, string numeroCliente)
        {
            if (await _context.Users.AnyAsync(x => x.Email == email && x.NumeroCliente == numeroCliente))
                return true;
            return false;
        }

        public bool ConfirmCliente(string tokenID, string NumConfirmacion)
        {
            if(_context.Users.Any(x => x.Id == tokenID))
            {

                var userOld = _context.Users.Where(x => x.Id == tokenID && x.NumeroVerificacion == NumConfirmacion);
                Users userNew = userOld.First();

                userNew.Validado = true;

             

    
                if(_context.SaveChanges() == 1)                 
                return true;
                else
                  return false;
            }
            else
            {
                return false;
            }


        }
        public string NumeroConfirmacion()
        {


            var seed = Environment.TickCount;
            var random = new Random(seed);
            string[] codigoVerificacion = new string[6];

            for (int i = 0; i <= 5; i++)
            {
                var value = random.Next(0, 10);
                codigoVerificacion[i] = value.ToString();
            }
            string enviar = codigoVerificacion[0] + codigoVerificacion[1] + codigoVerificacion[2] + codigoVerificacion[3] + codigoVerificacion[4] + codigoVerificacion[5];

            return enviar;

        }
    }
}
