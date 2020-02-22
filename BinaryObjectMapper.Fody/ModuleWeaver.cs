using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Fody;
using Mono.Cecil.Cil;

namespace BinaryObjectMapper.Weaver
{
    public class ModuleWeaver :
        BaseModuleWeaver
    {
        private static Dictionary<string, MethodInfo> BinaryReaderReadMethods;

        static ModuleWeaver()
        {
            var pattern = new Regex(@"Read(\w+)");
            BinaryReaderReadMethods =
                typeof(BinaryReader)
                .GetMethods()
                .Where(x => x.GetParameters().Length == 0 && pattern.IsMatch(x.Name))
                .ToDictionary(x => pattern.Match(x.Name).Groups[1].Value, x => x);
        }

        public override void Execute()
        {
            throw new NotImplementedException();
        }

        public override IEnumerable<string> GetAssembliesForScanning()
        {
            yield return "netstandard";
            yield return "mscorlib";
        }


    }
}
