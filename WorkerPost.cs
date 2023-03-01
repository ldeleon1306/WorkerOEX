using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MongoDB.Driver;
using MongoDB.Bson;
using System.IO;
using System.Net.Http;
using System.Text;
using Serilog;

namespace WorkerOE
{
    public class WorkerPost : BackgroundService
    {
        private readonly ILogger<WorkerPost> _logger;

        public WorkerPost(ILogger<WorkerPost> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Log.Logger = new LoggerConfiguration().WriteTo.File(@"C:\Users\ldeleon\OneDrive - ANDREANI LOGISTICA SA\Documentos\Bp\vane caso tarejta naranja\EventosLog\LogEvents" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".log").CreateLogger();
                CompareAsync();
                await Task.Delay(9000000, stoppingToken);
            }
        }

        private async Task CompareAsync()
        {
            try
            {
                var client = new MongoClient("mongodb://10.20.2.46:27017");
                List<string> NombrebaseDatos = client.ListDatabaseNames().ToList();
                var database = client.GetDatabase("APIAlmacenes");

                var collection = database.GetCollection<BsonDocument>("TransaccionesPedidos");
                string url = "http://api-pedidos-almacenes.apps.aro.andreani.com.ar/api/v1/almacenes/wmwhse4/pedidos";//ver almacen ej wmwhse4
                int cont = 0;
                //using (StreamReader leer = new StreamReader(@"C:\Users\ldeleon\OneDrive - ANDREANI LOGISTICA SA\Documentos\Bp\vane caso tarejta naranja\ordenExt.txt"))
                using (StreamReader leer = new StreamReader(@"C:\Users\ldeleon\OneDrive - ANDREANI LOGISTICA SA\Documentos\Bp\vane caso tarejta naranja\ordenExtC.txt"))
                {
                    while (!leer.EndOfStream)
                    {
                        string OrdenExterna = leer.ReadLine();

                        try
                        {
                            //var filter = Builders<BsonDocument>.Filter.Eq("response.idtransaccion",Convert.ToInt32(url));
                            var filter = Builders<BsonDocument>.Filter.Eq("request.pedido.idpedido", OrdenExterna);

                            try
                            {
                                var document = collection.Find(filter).First();

                                var contentPost = document["request"]; //obtengo el request extraido de mongo con el listado

                                using (HttpClient cliente = new HttpClient())
                                {
                                    try
                                    {
                                        cont++;
                                        var content = contentPost.ToJson();
                                        var result = await cliente.PostAsync(url, new StringContent(content, Encoding.UTF8, "application/json"));
                                        string resultContent = await result.Content.ReadAsStringAsync();
                                        //Log.Information("Idtransaccion: " + document["response"]["idtransaccion"] + " OrdenExterna: " + document["request"]["pedido"]["idpedido"] + "  Estado: " + document["estado"] + "  Razon: " + document["razon"]);
                                        Log.Information("Response: " + resultContent + "Orden externa: " + OrdenExterna);
                                        Console.WriteLine(cont);
                                    }
                                    catch (Exception ex)
                                    {
                                        Log.Information(ex.InnerException.Message);
                                        Log.Information("Orden externa: " + OrdenExterna);
                                        //throw;
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Log.Information("Orden externa: " + OrdenExterna);
                            }
                            //Console.WriteLine(cont);
                        }
                        catch (Exception ex)
                        {
                            Log.Information(ex.InnerException.Message);
                        }


                    }
                }


                Console.WriteLine(cont);

            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
