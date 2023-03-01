using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using Workers.Models;

namespace Workers.DataContext
{
    class DBSCEECOMPRODContext: DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Data Source=DBSCEECOMPROD;Initial catalog=LPNFD;Integrated Security=true");
            //optionsBuilder.UseSqlServer(@"Data Source=ITGDESAOCSRV;Initial catalog=AccionesBBVA;Integrated Security=true");
        }

        public DbSet<Orders> ORDERS { get; set; }
    }
}
