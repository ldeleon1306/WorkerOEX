using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Text;

namespace Workers.Models
{
   public  class Sce
    {
        internal static int conectarSce(string WpOrdenExterna1, string WpAlmacen)
        {
            WpAlmacen = "wmwhse4";
            string sql = "select EXTERNORDERKEY,WHSEID,STATUS FROM [LPNFD].[" + WpAlmacen + "].[ORDERS] WHERE EXTERNORDERKEY = '" + WpOrdenExterna1 + "'";
            int count = 0;
            //using (SqlConnection connection = new SqlConnection(@"Data Source=SQLSCECYPESRV;Initial catalog=LPNFD;Integrated Security=true"))
            using (SqlConnection connection = new SqlConnection(@"Data Source=DBSCEFARMATEST;Initial catalog=LPNFD;Trusted_Connection=True;Integrated Security=true"))
            {
                connection.Open();
               
                SqlCommand cmd = new SqlCommand(sql, connection);
                SqlDataReader reader = cmd.ExecuteReader();
                Console.WriteLine("---------------SCE--------------------");
                try
                {
                    while (reader.Read())
                    {
                        count++;                      
                        Console.WriteLine(String.Format("EXTERNORDERKEY: {0},WHSEID: {1},STATUS: {2}",
                        reader["EXTERNORDERKEY"], reader["WHSEID"], reader["STATUS"]));// etc
                    }
                }
                finally
                {
                    // Always call Close when done reading.
                    reader.Close();
                }
            }
            return count;
        }

    }
}
