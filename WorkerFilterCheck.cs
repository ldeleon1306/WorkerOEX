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
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Workers.Models;
using Workers.DataContext;
using Microsoft.Data.SqlClient;

namespace Workers
{
    public class WorkerFilterCheck : BackgroundService
    {
        private readonly ILogger<WorkerFilterCheck> _logger;
        private readonly IConfiguration _configuration;

        public WorkerFilterCheck(ILogger<WorkerFilterCheck> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _logger.LogInformation("inicio: {time}", DateTimeOffset.Now);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("ANTES DE COMPARAR: {time}", DateTimeOffset.Now);
                    CompareAsync();
                    await Task.Delay(3000000, stoppingToken);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.InnerException);
                    throw;
                }

            }
        }

        public async Task CompareAsync()
        {
            try
            {
                try
                {
             /////////MONGO
                    List<CollectionMongo> ListPedMongo = await Mongo.GetMongoCollectionAsync();
                    WAP_INGRESOPEDIDOS wp = new WAP_INGRESOPEDIDOS();
                    List<WAP_INGRESOPEDIDOS> listWapReprocesar = new List<WAP_INGRESOPEDIDOS>();
                    List<CollectionMongo> listpApiProcesar = new List<CollectionMongo>();
                    try
                    {
                        foreach (var listpedmongo in ListPedMongo)  
                        {
                            Console.WriteLine("");
                            Console.WriteLine("----------------------------------------------------------------------------------------------------------------------------------------------------------");
                            Console.WriteLine("---------------MONGO--------------------");
                            Console.WriteLine("Idtransaccion: " + listpedmongo.remito + "  Estado: " + listpedmongo.estado + "  Fecha " + listpedmongo.fechacreacion +"  Razon " + listpedmongo.razon);
            /////////FIN MONGO
            /////////WAP
                            using (Wap_IngresosPedidosContext db = new Wap_IngresosPedidosContext())
                            {
                                var encontroWap = Wap.GetWap(Convert.ToString(listpedmongo.idtransaccion));//PONER ALMACEN
                                Console.WriteLine("---------------WAP--------------------");
                                if (encontroWap.Item1>0) 
                                {
                                    wp.OrdenExterna1 = encontroWap.Item2; wp.Almacen = encontroWap.Item3; wp.RazonFalla = encontroWap.Item4; wp.Estado = encontroWap.Item5; 
                                    Console.WriteLine("OrdenExterna: " + wp.OrdenExterna1 + "  Almacen: " + wp.Almacen + "  Estado: " + wp.Estado + "  RazonFalla: " + wp.RazonFalla);                                                     

                               
            /////////FIN WAP
            ///////SCE
                                    int encontroSCE = Sce.conectarSce(wp.OrdenExterna1, wp.Almacen);
                                    if (encontroSCE==0)//no encontro en sce pero si en wap
                                    {
                                        //reprocesar back
                                        listWapReprocesar.Add(new WAP_INGRESOPEDIDOS() {OrdenExterna1 = wp.OrdenExterna1,Almacen=wp.Almacen,Estado=wp.Estado,RazonFalla=wp.RazonFalla });
                                    }
                                }
            /////////FIN SCE}  

                                else
                                {
                                    listpApiProcesar.Add(new CollectionMongo() { idtransaccion = listpedmongo.idtransaccion,estado= listpedmongo.estado, razon = listpedmongo.razon });
                                    //reprocesar API o CLIENTE
                                }
                            }
                        }
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("---------------Reprocesar API--------------------");
                        if (File.Exists("output.txt")) File.Delete("output.txt");
                        using StreamWriter streamwriterAPI = File.AppendText("output.txt");
                        foreach (var line in listpApiProcesar)
                        {

                            streamwriterAPI.WriteLine( line.estado, line.idtransaccion);
                            Console.WriteLine(line.idtransaccion + " " + line.estado + " " + line.razon);
                        }
                        streamwriterAPI.Close();
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("--------------------------------------------------");
                        Console.WriteLine("---------------Reprocesar Back--------------------");
                        if (File.Exists("output.txt")) File.Delete("output.txt");
                        using StreamWriter streamwriterBack = File.AppendText("output.txt");
                        foreach (var line in listWapReprocesar)
                        {

                            streamwriterBack.WriteLine(line.OrdenExterna1,line.Almacen,line.Estado,line.RazonFalla);                         
                           Console.WriteLine("OrdenExterna: " +line.OrdenExterna1 + "  Almacen: " + line.Almacen + "  Estado: " + line.Estado + "  RazonFalla: " + line.RazonFalla);
                        }
                        streamwriterBack.Close();
                        //foreach (var item in listWapReprocesar)
                        //{
                        //    Console.WriteLine(item.OrdenExterna1,item.Almacen,item.Estado,item.RazonFalla);
                        //}
                        Mail.Mail m = new Mail.Mail(_configuration);
                        m.SendEmail(listWapReprocesar, "cliente");
                    }
                    catch (Exception ex)
                    {
                        throw ex;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
            }
            catch (Exception ex)
            {
                _logger.LogInformation(ex.Message);
                throw;
            }
        }
    }
}
