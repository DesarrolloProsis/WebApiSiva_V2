using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace WebApiSiva.SqlUser
{
    public class QuerysGTDB
    {


        public Object ObtenerMovimientoCuenta(string IdCuenta, string numCuenta, string fechaInicio, string fechaFin)
        {

            string query = "Select Concepto, DateTOperacion, Monto from OperacionesCajeroes where DateTOperacion >= '"+fechaInicio+"' and DateTOperacion < '"+fechaFin+"' and Numero = '"+numCuenta+"'";

            var dtRecargas = ConsultasInternas(query);

            List<Array> value1 = new List<Array>();

            foreach(DataRow item in dtRecargas.Rows)
            {
                string[] movimientoRecargas = new string[3];

                movimientoRecargas[0] = item["Concepto"].ToString();
                movimientoRecargas[1] = item["DateTOperacion"].ToString();
                movimientoRecargas[2] = item["Monto"].ToString();

                value1.Add(movimientoRecargas);
            }

            query = "Select * from Tags where CuentaId = '" + IdCuenta + "'";

            var dtRecargaSobreTag = ConsultasInternas(query);

            foreach(DataRow itemIn in dtRecargaSobreTag.Rows)
            {
                query = "Select Concepto, DateTOperacion, Monto from OperacionesCajeroes where DateTOperacion >= '" + fechaInicio + "' and DateTOperacion < '" + fechaFin + "' and Numero = '" + itemIn["NumTag"].ToString() + "'";

                var dtMovimientoTag = ConsultasInternas(query);

                foreach(DataRow itemOn in dtMovimientoTag.Rows)
                {
                    string[] movimientoRecargasTag = new string[3];
                    movimientoRecargasTag[0] = itemOn["Concepto"].ToString();
                    movimientoRecargasTag[1] = itemOn["DateTOperacion"].ToString();
                    movimientoRecargasTag[2] = itemOn["Monto"].ToString();
                    value1.Add(movimientoRecargasTag);
                }
            }


            query = "select Fecha, Saldo from Historico where Fecha >= '"+fechaInicio+"' and Fecha < '"+fechaFin+"' and NumeroCuenta = '"+ numCuenta + "'";

            var dtCruces = ConsultasInternas(query);

            foreach(DataRow item2 in dtCruces.Rows)
            {
                string[] movimientoCruces = new string[3];

                movimientoCruces[0] = "PEAJE";
                movimientoCruces[1] = item2["Fecha"].ToString();
                movimientoCruces[2] = item2["Saldo"].ToString();

                value1.Add(movimientoCruces);


            }


            object Json = new { value1 };

            return Json;
        }


        public Object ObtenerCuentasTags(string numeroCliente)
        {

            string query = "Select c.Id, c.NumCuenta,c.TypeCuenta from CuentasTelepeajes c inner join Clientes g on c.ClienteId = g.Id where g.NumCliente = '" + numeroCliente + "'";
            var dtCuentas = ConsultasInternas(query);


            //string[] value1 = new string[dtCuentas.Columns.Count];
            List<Array> value1 = new List<Array>();
            List<Array> value2 = new List<Array>();



            foreach (DataRow item in dtCuentas.Rows)
            {
                query = "select NumTag from Tags where CuentaId = '" + item["Id"].ToString() + "'";

                var tagAsociados = ConsultasInternas(query);

                string[] cuentaId = new string[3];

                cuentaId[0] = item["Id"].ToString();
                cuentaId[1] = item["NumCuenta"].ToString();
                cuentaId[2] = item["TypeCuenta"].ToString();

                value1.Add(cuentaId);

                foreach(DataRow item2 in tagAsociados.Rows)
                {
                    string[] cuentaTag = new string[2];

                    cuentaTag[0] = item["NumCuenta"].ToString();
                    cuentaTag[1] = item2["NumTag"].ToString(); ;

                    value2.Add(cuentaTag);
                }

            }

            object Json = new { value1, value2 };

            return Json;
        }



        private DataTable ConsultasInternas(string Query)
        {

            DataTable dt = new DataTable();
            SqlConnection cn = new SqlConnection("Server=.;Database=GTDB;User Id=sa;Password=CAPUFE;ConnectRetryCount=0");
            using (SqlCommand cmd = new SqlCommand("", cn))
            {
                try
                {
                    cmd.CommandText = Query;
                    cn.Open();
                    cmd.ExecuteNonQuery();
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(dt);
                }
                catch(Exception ex)
                {

                }
                finally
                {
                    cn.Close();
                }

            }

                return dt;
        }
            

        
    }
}
