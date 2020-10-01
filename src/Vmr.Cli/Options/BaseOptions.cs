using System;
using System.Collections.Generic;
using System.Text;
using CommandLine;

namespace Vmr.Cli.Options
{
    internal abstract class BaseOptions
    {
        [Option("wait", Hidden = true, Default = false)]
        public virtual bool Wait { get; set; }
    }
}
