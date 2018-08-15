using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Configurations
{
    class ArgumentParser
    {

        public ArgumentParser()
        {

        }

        public ComandLineArguments Parse(string[] args)
        {
            ComandLineArguments arguments = new ComandLineArguments();
            for (int i = 1; i < args.Length; i++)
            {
                switch(args[i])
                {
                    case "--debug_wrapper_state":
                        arguments.DebugOptions = (DebugStdOutOptions)Convert.ToInt32(args[i + 1]);
                        break;
                    default: break;
                }
            }

            return arguments;
        }
    }
}
