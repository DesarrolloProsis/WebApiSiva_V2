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

        
        public bool CrearCorreo(string emailDestino, string numConfirmacion)
        {
            System.Net.Mail.MailMessage mensaje = new System.Net.Mail.MailMessage();

          
            
            mensaje.To.Add(emailDestino);
            mensaje.Subject = "Confirmacion de Activacion SIVA";
            mensaje.SubjectEncoding = System.Text.Encoding.UTF8;

            mensaje.Body = "Acabas de hacer un registro en la pagina de SIVA PASS " +
                            "si usted no realizo este registro porfavor de ignorar este correo"+
                            "para completar tu registro ingrese el siguiente codigo de verificacion "+
                            "CODIGO: "+numConfirmacion+"";


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


      
  



    }
}
