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

        //public void AgregarCuentasDB()
        //{
        //    string query = "select NumCuenta, Id, TypeCuenta from CuentasTelepeajes";

        //    var dt = ConsultasInternas(query);

        //    foreach(DataRow item in dt.Rows)
        //    {
        //        query = "select NumTag from Tags where CuentaId = '" + item["Id"].ToString() + "'";

        //        var dtTag = ConsultasInternas(query);


        //        foreach (DataRow item2 in dtTag.Rows)
        //        {
        //            query = "UPDATE Historico set NumeroCuenta = '" + item["NumCuenta"].ToString() + "', TipoCuenta = '" + item["TypeCuenta"].ToString() + "' where Tag = '" + item2["NumTag"].ToString() + "' and NumeroCuenta is null";
        //            ConsultasInternas(query);
        //        }
        //    }

        //}

        public object ObtnerMovimientoTag(string numCuenta, string numTag, string fechaInicio, string fechaFin)
        {
            string query = "Select Concepto, DateTOperacion, Monto from OperacionesCajeroes where DateTOperacion >= '" + fechaInicio + "' and DateTOperacion < '" + fechaFin + "' and Numero = '" + numTag + "' order by DateTOperacion asc";

            var dtRecargas = ConsultasInternas(query);

            List<MovimientosJson> value1 = new List<MovimientosJson>();

            foreach (DataRow item in dtRecargas.Rows)
            {
                string[] movimientoRecargas = new string[4];


                if (item["Concepto"].ToString() == "TAG RECARGA")
                {

                    value1.Add(new MovimientosJson
                    {
                        concepto = item["Concepto"].ToString(),
                        fecha = Convert.ToDateTime(item["DateTOperacion"]),
                        montoInicio = item["Monto"].ToString(),
                        montoFin = buscaSaldoRecagasTag(numTag, Convert.ToDateTime(item["DateTOperacion"]).ToString("dd/MM/yyyy HH:mm:ss")),
                        carril = "No Aplica"
                    });
                }
                else
                {

                    value1.Add(new MovimientosJson
                    {
                        concepto = item["Concepto"].ToString(),
                        fecha = Convert.ToDateTime(item["DateTOperacion"]),
                        montoInicio = item["Monto"].ToString(),
                        montoFin = "-------",
                        carril = "No Aplica"
                    });

                }

            }

            query = "select Fecha, Saldo, SaldoActualizado, Carril from Historico where Fecha >= '" + fechaInicio + "' and Fecha < '" + fechaFin + "' and Tag = '" + numTag + "' order by Fecha asc";

            var dtCruces = ConsultasInternas(query);

            foreach (DataRow item2 in dtCruces.Rows)
            {
                string[] movimientoCruces = new string[4];


                value1.Add(new MovimientosJson
                {
                    concepto = "TRANSACCION",
                    fecha = Convert.ToDateTime(item2["Fecha"]),
                    montoInicio = item2["Saldo"].ToString(),
                    montoFin = item2["SaldoActualizado"].ToString(),
                    carril = item2["Carril"].ToString(),
                    

                });


            }


            var MovimientosOrder = value1.OrderBy(x => x.fecha).ToList();


            List<MovimientosJson2> Movimientos = new List<MovimientosJson2>();
            object[] ListTransaccion = new object[MovimientosOrder.Count()];
            int i = 0;

            foreach (var item in MovimientosOrder)
            {
                Movimientos.Add(new MovimientosJson2
                {
                    concepto = item.concepto,
                    plazas = "Autopista Palin-Escuintla",
                    fecha = item.fecha.ToString("dd/MM/yyyy HH:mm:sss"),
                    montoInicio = item.montoInicio,
                    montoFin = item.montoFin,
                    carril = item.carril
                    
                });

                string[] transacciones = new string[6];

                transacciones[0] = item.concepto;
                transacciones[1] = "Autopista Palin-Escuintla";
                transacciones[2] = item.carril;
                transacciones[3] = item.fecha.ToString("dd/MM/yyyy HH:mm:sss");
                transacciones[4] = item.montoInicio;
                transacciones[5] = item.montoFin;


                ListTransaccion[i] = transacciones;


                i++;

                //object jsonPrueba = new { transacciones };

                //ListTransaccion.Add(jsonPrueba);


            }

            if (MovimientosOrder.Count() == 0)
                return null;
            else
                return new { ListTransaccion };

        }

            

        public Object ObtenerMovimientoCuenta(string IdCuenta, string numCuenta, string fechaInicio, string fechaFin)
        {

            string query = "Select Concepto, DateTOperacion, Monto from OperacionesCajeroes where DateTOperacion >= '"+fechaInicio+"' and DateTOperacion < '"+fechaFin+"' and Numero = '"+numCuenta+ "' order by DateTOperacion asc";

            var dtRecargas = ConsultasInternas(query);

            List<MovimientosJson> value1 = new List<MovimientosJson>();
            

            foreach(DataRow item in dtRecargas.Rows)
            {
                string[] movimientoRecargas = new string[4];

                if (item["Concepto"].ToString() == "CUENTA RECARGA")
                {
                 
                    value1.Add(new MovimientosJson 
                    {

                        concepto = item["Concepto"].ToString(),
                        fecha = Convert.ToDateTime(item["DateTOperacion"]),
                        montoInicio = item["Monto"].ToString(),
                        montoFin = buscaSaldoRecargasCuenta(numCuenta, Convert.ToDateTime(item["DateTOperacion"]).ToString("dd/MM/yyyy HH:mm:ss")),
                        carril = "No Aplica"

                    });
                                     

                }
                else
                {
               
                    value1.Add(new MovimientosJson 
                    {
                        concepto = item["Concepto"].ToString(),
                    fecha = Convert.ToDateTime(item["DateTOperacion"]),
                    montoInicio = item["Monto"].ToString(),
                        montoFin = "-----",
                        carril = "No Aplica"


                    });
                }
            }

            query = "Select * from Tags where CuentaId = '" + IdCuenta + "'";

            var dtRecargaSobreTag = ConsultasInternas(query);

            foreach(DataRow itemIn in dtRecargaSobreTag.Rows)
            {
                query = "Select Concepto, DateTOperacion, Monto from OperacionesCajeroes where DateTOperacion >= '" + fechaInicio + "' and DateTOperacion < '" + fechaFin + "' and Numero = '" + itemIn["NumTag"].ToString() + "' order by DateTOperacion asc";

                var dtMovimientoTag = ConsultasInternas(query);

                foreach(DataRow itemOn in dtMovimientoTag.Rows)
                {

                    string[] movimientoRecargasTag = new string[4];

                    if (itemOn["Concepto"].ToString() == "TAG RECARGA")
                    {


                        value1.Add(new MovimientosJson                         
                        {

                            concepto = itemOn["Concepto"].ToString(),
                        fecha = Convert.ToDateTime(itemOn["DateTOperacion"]),
                        montoInicio = itemOn["Monto"].ToString(),
                        montoFin = buscaSaldoRecagasTag(itemIn["NumTag"].ToString(), Convert.ToDateTime(itemOn["DateTOperacion"]).ToString("dd/MM/yyyy HH:mm:ss")),
                            carril = "No Aplica",


                        });

                    }
                    else
                    {
                        
              
                        value1.Add(new MovimientosJson
                        { 
                            
                        concepto = itemOn["Concepto"].ToString(),
                        fecha = Convert.ToDateTime(itemOn["DateTOperacion"]),
                        montoInicio = itemOn["Monto"].ToString(),
                        montoFin = "-----",
                            carril = "No Aplica",


                        });
                    }
                }
            }


            query = "select Fecha, Saldo, SaldoActualizado, Carril from Historico where Fecha >= '" + fechaInicio+"' and Fecha < '"+fechaFin+"' and NumeroCuenta = '"+ numCuenta + "'  order by Fecha asc";

            var dtCruces = ConsultasInternas(query);

            foreach(DataRow item2 in dtCruces.Rows)
            {
                string[] movimientoCruces = new string[4];              

                value1.Add(new MovimientosJson
                {
                    concepto = "TRANSACCION",
                fecha = Convert.ToDateTime(item2["Fecha"]),
                montoInicio = item2["Saldo"].ToString(),
                montoFin = item2["SaldoActualizado"].ToString(),
                    carril = item2["Carril"].ToString(),

                });

            }

            var JsonList = value1.OrderByDescending(x => x.fecha).ToList();
            List <MovimientosJson2>  Movimientos = new List<MovimientosJson2>();

            object[] ListTransaccion = new object[JsonList.Count()];
            int i = 0;

            foreach (var item in JsonList)
            {
                Movimientos.Add(new MovimientosJson2
                {
                    concepto = item.concepto,
                    plazas = "Autopista Palin-Escuintla",
                    fecha = item.fecha.ToString("dd/MM/yyyy HH:mm:sss"),
                    montoInicio = item.montoInicio,
                    montoFin = item.montoFin,
                    carril = item.carril,
                });

                string[] transacciones = new string[6];

                transacciones[0] = item.concepto;
                transacciones[1] = "Autopista Palin-Escuintla";
                transacciones[2] = item.carril;
                transacciones[3] = item.fecha.ToString("dd/MM/yyyy HH:mm:sss");
                transacciones[4] = item.montoInicio;
                transacciones[5] = item.montoFin;





                ListTransaccion[i] = transacciones;
                i++;


            }



            if (JsonList.Count() == 0)
                return null;
            else
                return new { ListTransaccion };
        }


        public Object ObtenerCuentasTags(string numeroCliente)
        {

            string query = "Select c.Id, c.NumCuenta,c.TypeCuenta from CuentasTelepeajes c inner join Clientes g on c.ClienteId = g.Id where g.NumCliente = '" + numeroCliente + "'";
            var dtCuentas = ConsultasInternas(query);


            //string[] value1 = new string[dtCuentas.Columns.Count];
            List<Array> Cuentas = new List<Array>();
            //List<Array> Tags = new List<Array>();
            List<TagJson> Tags = new List<TagJson>();
            List<object> ListTransaccion = new List<object>();


            foreach (DataRow item in dtCuentas.Rows)
                {
                    query = "select NumTag from Tags where CuentaId = '" + item["Id"].ToString() + "'";

                    var tagAsociados = ConsultasInternas(query);

                    string[] cuentaId = new string[3];

                    cuentaId[0] = item["Id"].ToString();
                    cuentaId[1] = item["NumCuenta"].ToString();
                    cuentaId[2] = item["TypeCuenta"].ToString();

                    Cuentas.Add(cuentaId);

                    foreach (DataRow item2 in tagAsociados.Rows)
                    {
                       //string[] item2. = new string[2];

                        //cuentaTag[0] = item["NumCuenta"].ToString();
                        //cuentaTag[1] = item2["NumTag"].ToString(); 

                        Tags.Add(new TagJson
                        {
                            cuentaId = item["NumCuenta"].ToString(),
                            numTag = item2["NumTag"].ToString()
                        });

                    }                    

                }

                object Json = new { Cuentas, Tags};            

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
        private string buscaSaldoRecargasCuenta(string numCuenta, string fechaInicio)
        {


            List<MovimientosJson> list = new List<MovimientosJson>();


            string query = "select Fecha, SaldoAnterior, SaldoActualizado from Historico where Fecha < '" + fechaInicio + "' and NumeroCuenta = '" + numCuenta + "' order by Fecha desc";

            var dtConsentrado = ConsultasInternas(query);

            foreach (DataRow item1 in dtConsentrado.Rows)
            {
                list.Add(new MovimientosJson
                {
                    concepto = "PEAJE",
                    fecha = Convert.ToDateTime(item1["Fecha"]),
                    montoInicio = item1["SaldoAnterior"].ToString(),
                    montoFin = item1["SaldoActualizado"].ToString(),

                });
            }

            if (list.Count() > 0)
                return list[0].montoInicio.ToString();
            else
                return "------";
      
        }
        private string buscaSaldoRecagasTag(string numTag, string fechaInicio)
        {


            List<MovimientosJson> list = new List<MovimientosJson>();


            string query = "select Fecha, SaldoAnterior, SaldoActualizado from Historico where Fecha < '" + fechaInicio + "' and NumeroCuenta = '" + numTag + "' order by Fecha desc";

            var dtConsentrado = ConsultasInternas(query);

            foreach (DataRow item1 in dtConsentrado.Rows)
            {
                list.Add(new MovimientosJson
                {
                    concepto = "PEAJE",
                    fecha = Convert.ToDateTime(item1["Fecha"]),
                    montoInicio = item1["SaldoAnterior"].ToString(),
                    montoFin = item1["SaldoActualizado"].ToString(),

                });


            }

            if (list.Count() > 0)
                return list[0].montoInicio.ToString();
            else
                return "------";

        }

    }
    class TagJson
    {
        public string cuentaId { get; set; }
        public string numTag { get; set; }
    }
    class MovimientosJson
    {
        public string concepto { get; set; }
        public DateTime fecha { get; set; }
        public string montoInicio { get; set; }
        public string montoFin { get; set; }
        public string carril { get; set; }

    }

    class MovimientosJson2
    {
        public string concepto { get; set; }
        public string plazas { get; set; }
        public string fecha { get; set; }
        public string montoInicio { get; set; }
        public string montoFin { get; set; }
        public string carril { get; set; }
    }

}
