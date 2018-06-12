using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

using PortfolioManager.Common;
using PortfolioManager.ImportData;
using PortfolioManager.Data.SQLite.Stocks;

namespace PortfolioManager.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = new WebHostBuilder()
                .UseContentRoot(Directory.GetCurrentDirectory())
                .UseStartup<Startup>()
                .UseKestrel() 
                .Build();

            host.Run();          
        }
    }
}
