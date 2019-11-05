using System;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebApiSiva.Data;
using WebApiSiva.Models;
using WebApiSiva.Dtos;
using WebApiSiva.Entities;
using System.Linq;

namespace WebApiSiva.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthRepository _repo;
        private readonly IConfiguration _config;
        private readonly GTDbContext _context;
        public AuthController(IAuthRepository repo, IConfiguration config, GTDbContext context)
        {
            _repo = repo;
            _config = config;
            _context = context;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserForLoginDto userForRegisterDto)
        {
            var userFromRepo = await _repo.Login(userForRegisterDto.Email.ToLower(), userForRegisterDto.Password);
            if (userFromRepo == null) //User login failed
                return Unauthorized();

            //generate token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_config.GetSection("AppSettings:Token").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]{
                    new Claim(ClaimTypes.NameIdentifier,userFromRepo.Id.ToString()),
                    new Claim(ClaimTypes.Name, userFromRepo.Email)
                }),
                Expires = DateTime.Now.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
            };

            var tokenCreated = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(tokenCreated);

            TokenValidate tokenValidate = new TokenValidate();
            
            tokenValidate.Cliente = userFromRepo.NumeroCliente;
            tokenValidate.WebToken = token;
            tokenValidate.FechaGeneracion = DateTime.Now;

            await _repo.InsertToken(tokenValidate);
            
            return Ok(new { token, userFromRepo.NumeroCliente });
        }

        [HttpPost("register")] //<host>/api/auth/register
        public async Task<IActionResult> Register([FromBody] UserForRegisterDto userForRegisterDto)
        {   // Data Transfer Object containing username and password.
            // validate request
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            userForRegisterDto.Email = userForRegisterDto.Email.ToLower(); //Convert username to lower case before storing in database.

            if (await _repo.UserExists(userForRegisterDto.Email, userForRegisterDto.NumeroCliente))
                return BadRequest(false);

            var userToCreate = new Users
            {
                Email = userForRegisterDto.Email,
                NumeroCliente = userForRegisterDto.NumeroCliente
            };

            var createUser = await _repo.Register(userToCreate, userForRegisterDto.Password);

            return Ok(true);
        }

        [HttpPost("validateToken")]
        public IActionResult ValidarTokenWeb([FromBody] TokenValidate validate)
        {
            if (_repo.ValidarToken(validate.WebToken))
                return Ok(true);
            else
                return BadRequest(false);
        }

        [HttpGet("clientexists/{numclient}/{email}")] //<host>/api/auth/clientexists/?numclient=190311100051 or "/clientexists/?email=xxx@xxx.com
        public async Task<IActionResult> ClientExists(string numclient, string email)
        {

            object Cliente = null;
            object Email = null;
            string[] status = new string[1];

            if (numclient != null && email != null)
            {
                Cliente = await _context.Clientes.FirstOrDefaultAsync(x => x.NumCliente == numclient);

                if (Cliente != null)
                {
                    Email = await _context.Clientes.FirstOrDefaultAsync(x => x.NumCliente == numclient && x.EmailCliente == email);

                    if (Email != null)
                        return Ok(true);
                    else
                    {
                        status[0] = "Email Diferente";
                        return BadRequest(status);
                    }
                }
                else
                {
                    status[0] = "No Existe";
                    return BadRequest(status);
                }
            }
            else             
                return BadRequest(false);
        }

    
    }
}