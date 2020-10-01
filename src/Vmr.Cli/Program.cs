using CommandLine;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using Vmr.Cli;

try
{
    try
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json");

        IConfiguration config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", true, true)
            .Build();

        return Application.Execute(args, config);
    }
    catch (Exception ex)
    {
        return Application.ReportCrash(ex);
    }
}
catch (Exception fallback)
{
    Console.WriteLine("Create Crash dump failed:");
    Console.WriteLine(fallback.Message);
}

return 1;
