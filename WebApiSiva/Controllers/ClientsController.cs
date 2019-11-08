using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebApiSiva.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using WebApiSiva.SqlUser;

namespace WebApiSiva.Controllers
{

    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ClientsController : ControllerBase
    {
        private readonly GTDbContext _context;
        private QuerysGTDB querysGTDB = new QuerysGTDB();
        public ClientsController(GTDbContext context)
        {
            _context = context;
        }

        

        [HttpGet("movimientosTag/{numCuenta}/{numTag}/{fechaInicio}/{fechaFin}")]
        public IActionResult GetCuentaMovimientosTag(string numCuenta, string numTag, string fechaInicio, string fechaFin)
        {
            var json = querysGTDB.ObtnerMovimientoTag(numCuenta, numTag, fechaInicio, fechaFin);

            if (json == null)
                return Ok("null");
            else
                return Ok(json);
        }



        [HttpGet("movimientosCuenta/{idCuenta}/{numCuenta}/{fechaInicio}/{fechaFin}")]
        public IActionResult GetCuentaMovimientosCuenta(string idCuenta, string numCuenta, string fechaInicio, string fechaFin)
        {

            var json = querysGTDB.ObtenerMovimientoCuenta(idCuenta, numCuenta, fechaInicio, fechaFin);

            if (json == null)
                return Ok("null");
            else
                return Ok(json);
        }



        [HttpGet("{numCliente}")]
        public IActionResult GetCuentaTag(string numCliente)
        {
          
            var json =  querysGTDB.ObtenerCuentasTags(numCliente);
           
            return Ok(json);
        }



        // GET api/clients/190311100051
        //[HttpGet("{NumClient}")]
        //public IActionResult GetCuentasTag(string NumClient)
        //{
        //    var ListCuentas = (from cliente in _context.Clientes
        //                        join cuentas in _context.CuentasTelepeajes 
        //                        on cliente.Id equals cuentas.ClienteId                                
        //                        where cliente.NumCliente == NumClient
        //                        select new {
        //                            cuentaId = cuentas.Id,
        //                            numCuenta = cuentas.NumCuenta
        //                        }).ToList();

        //    string[] value1 = new string[ListCuentas.Count()];                       
        //    List<Array> value2 = new List<Array>();
        //    int i = 0;

        //    foreach (var item in ListCuentas)    {

                

        //         var tagAsociados = (from Tags in _context.Tags
        //                            where Tags.CuentaId == item.cuentaId
        //                            select new {                                        
        //                                numeroTag = Tags.NumTag
        //                            }).ToList();

        //        value1[i] = item.numCuenta;

        //        foreach (var item2 in tagAsociados)
        //        {

        //            string[] cuentaTag = new string[2];

        //            cuentaTag[0] = item.numCuenta;
        //            cuentaTag[1] = item2.numeroTag;

        //            value2.Add(cuentaTag);
        //        }

        //        i++;
        //    }

        //    object json = new {value1, value2};
            
        //    return Ok(json);
        //}
    }

}