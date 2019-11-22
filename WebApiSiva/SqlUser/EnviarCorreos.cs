using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApiSiva.Data;
using WebApiSiva.Entities;
using WebApiSiva.Models;

namespace WebApiSiva.SqlUser
{
    public class EnviarCorreos
    {

        private readonly IAuthRepository _repo;
        public bool CrearCorreo(string emailDestino, string token)
        {
            System.Net.Mail.MailMessage mensaje = new System.Net.Mail.MailMessage();


            
            mensaje.To.Add(emailDestino);
            mensaje.Subject = "Confirmacion de Activacion SIVA";
            mensaje.SubjectEncoding = System.Text.Encoding.UTF8;

            mensaje.Body = "Acabas de hacer un registro en la pagina de SIVA PASS " +
                            "si usted no realizo este registro porfavor de ignorar este correo"+
                            "para completar tu registro ingrese el siguiente codigo de verificacion "+
                            "CODIGO: '"+NumeroConfirmacion(token)+"'";


            mensaje.BodyEncoding = System.Text.Encoding.UTF8;
            mensaje.IsBodyHtml = true;

            mensaje.From = new System.Net.Mail.MailAddress("geoffreyytorres25@outlook.com");

            System.Net.Mail.SmtpClient cliente = new System.Net.Mail.SmtpClient("smtp.office365.com");
            cliente.EnableSsl = true;
            cliente.UseDefaultCredentials = false;

            cliente.Credentials = new System.Net.NetworkCredential("geoffreyytorres25@outlook.com", "Vaca$Loca69");
            cliente.Port = 587;


            //cliente.Host = "smtp.gmail.com";

            try
            {
                cliente.Send(mensaje);
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }


        private string NumeroConfirmacion(string token)
        {

            
            var seed = Environment.TickCount;
            var random = new Random(seed);
            string[] codigoVerificacion = new string[6];

            for (int i = 0; i <= 5; i++)
            {
                var value = random.Next(0, 10);
                codigoVerificacion[i] = value.ToString();
            }
            var enviar = codigoVerificacion[0] + codigoVerificacion[1] + codigoVerificacion[2] + codigoVerificacion[3] + codigoVerificacion[4] + codigoVerificacion[5];

            var oldUser = _repo.InsertarNumero(enviar, token);

            return enviar;
        }
  



    }
}
