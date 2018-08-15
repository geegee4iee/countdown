using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CountdownWPF.Configurations
{
    public class DebugWrapper
    {
        private static readonly Lazy<DebugWrapper> _wrapper = new Lazy<DebugWrapper>();
        readonly DebugStdOutOptions Options;
        private DebugWrapper()
        {
            ArgumentParser parser = new ArgumentParser();
            var argument = parser.Parse(Environment.GetCommandLineArgs());
            Options = argument.DebugOptions;
        }

        public static void WriteLine(string text, DebugStdOutOptions option)
        {
            if (_wrapper.Value.Options == option)
            {
                Debug.WriteLine(text);
            }
        }
    }

    public enum DebugStdOutOptions
    {
        SessionState
    }
}
