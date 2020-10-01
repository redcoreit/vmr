using Microsoft.Extensions.Configuration;
using System;
using System.Diagnostics;
using System.IO;
using Vmr.Cli.Options;

namespace Vmr.Cli.Commands
{
    internal abstract class BaseCommand<TOpts> where TOpts : BaseOptions
    {
        private readonly bool _measureExecutionTime;

        public BaseCommand(bool measureExecutionTime = true)
        {
            CurrentDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            _measureExecutionTime = measureExecutionTime;
        }

        protected DirectoryInfo CurrentDirectory { get; }

        protected abstract string Name { get; }

        protected int Execute(TOpts opts, IConfiguration config)
        {
            if (opts.Wait)
            {
                Console.WriteLine("Waiting for key...");
                Console.ReadKey();
            }

            if (_measureExecutionTime)
            {
                return ExecuteWithMeaseure(opts, config);
            }

            ExecuteInternal(opts, config);

            return 0;
        }

        private int ExecuteWithMeaseure(TOpts opts, IConfiguration config)
        {
            var timer = new Stopwatch();
            Console.WriteLine($"{Name} started...");
            timer.Start();

            ExecuteInternal(opts, config);

            timer.Stop();
            Console.WriteLine($"{Name} finished: {timer.ElapsedMilliseconds} ms.");

            return 0;
        }

        protected abstract void ExecuteInternal(TOpts opts, IConfiguration config);
    }
}