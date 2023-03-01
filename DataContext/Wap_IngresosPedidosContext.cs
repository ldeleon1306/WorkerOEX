using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.Models;

namespace Workers
{
     class Wap_IngresosPedidosContext : DbContext
    {       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=SQLSCECYPESRV;Initial catalog=InterfacesWHS;Integrated Security=true");
            //optionsBuilder.UseSqlServer(@"Data Source=ITGDESAOCSRV;Initial catalog=AccionesBBVA;Integrated Security=true");
        }

        public DbSet<WAP_INGRESOPEDIDOS> WAP_INGRESOPEDIDOS { get; set; }
    }
}
