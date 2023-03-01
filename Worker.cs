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

namespace WorkerOE
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                CompareAsync();
                await Task.Delay(100000, stoppingToken);
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
                int cont = 0;
                    //using (StreamReader leer = new StreamReader(@"C:\Users\ldeleon\OneDrive - ANDREANI LOGISTICA SA\Documentos\Bp\vane caso tarejta naranja\ordenExt.txt"))
                using (StreamReader leer = new StreamReader(@"C:\Users\ldeleon\OneDrive - ANDREANI LOGISTICA SA\Documentos\Bp\vane caso tarejta naranja\idtran.txt"))
                {
                        while (!leer.EndOfStream)
                        {
                            string url = leer.ReadLine();

                        try
                        {
                            //var filter = Builders<BsonDocument>.Filter.Eq("response.idtransaccion",Convert.ToInt32(url));
                            var filter = Builders<BsonDocument>.Filter.Eq("response.idtransaccion", Convert.ToInt32(url));

                            var document = collection.Find(filter).First();

                            Console.WriteLine("Idtransaccion: " + document["response"]["idtransaccion"]  +"  Razon: " + document["razon"]);
                            //Console.WriteLine(document["response"]["idtransaccion"]);
                            //{ "request.pedido.idpedido": "coc5i0y2fqhw"}
                            cont++;
                        }
                        catch (Exception ex)
                        {

                           
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
