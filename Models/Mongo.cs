using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using MongoDB.Driver;
using MongoDB.Bson;
using System.Threading.Tasks;

namespace Workers.Models
{
    public class Mongo
    {     

        internal static List<CollectionMongo> GetMongoCollection()
        {         
            List<CollectionMongo> listRange = new List<CollectionMongo>();
            try
            {
                Console.WriteLine("entra a mongo 19");
                var client = new MongoClient("mongodb://10.20.2.46:27017?connect=replicaSet");
                Console.WriteLine("conecto a mongo 21");
                Console.WriteLine("entra a mongo 22");
                List<string> NombrebaseDatos = client.ListDatabaseNames().ToList();
                Console.WriteLine("entra a mongo 23");
                var database = client.GetDatabase("APIAlmacenes");
                Console.WriteLine("entra a mongo 24");

                var collection = database.GetCollection<BsonDocument>("TransaccionesPedidos");

                var list = collection.Find(new BsonDocument())
                     .Limit(2) //retrive only two documents
                    .ToList();
        
                foreach (var docs in list)
                {
                    //_logger.LogInformation(docs.ToString());
                    //Console.WriteLine("Idtransaccion: " + docs["response"]["idtransaccion"] + "  Estado: " + docs["estado"]);

                    listRange.Add(new CollectionMongo() { idtransaccion = (int)Convert.ToInt64(docs["response"]["idtransaccion"]), estado = (string)docs["estado"] });
                    //_logger.LogInformation(docs.ToString());
                    //Console.WriteLine(docs);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine("exepcion en mongp");
                throw ex;
            }            

            return listRange;
        }

        //internal static List<string> GetMongoCollection()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
