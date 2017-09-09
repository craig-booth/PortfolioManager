using System;
using Microsoft.Extensions.CommandLineUtils;


namespace PortfolioManager.Housekeeping
{
    class Program
    {

        static int Main(string[] args)
        {
            try
            {
                return new App().Execute(args);
            }
            catch (Exception ex)
            {
                Console.Write(ex);
                return 1;
            }

        }
    }

    class App : CommandLineApplication
    {

        public App()
        {
            Commands.Add(new DownloadCommand());

            HelpOption("-h | -? | --help");
        }
    }

 
}