﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public async Task<Users> Login(string email, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email); //Get user from database.
            if (user == null)
                return null; // User does not exist.

            if (!VerifyPassword(password, user.PasswordHash, user.PasswordSalt))
                return null;

            return user;
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

            await _context.Users.AddAsync(user); // Adding the user to context of users.
            await _context.SaveChangesAsync(); // Save changes to database.

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
    }
}
