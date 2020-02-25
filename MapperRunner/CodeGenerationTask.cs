using BinaryObjectMapper;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;

namespace MapperRunner
{
    public class CodeGenerationTask 
    {
        public static async Task Main(string[] args)
        {

            MSBuildLocator.RegisterDefaults();

            var workspace = MSBuildWorkspace.Create();
            var proj = await workspace.OpenProjectAsync(args[0]);
            foreach(var x in workspace.Diagnostics)
            {
                Debug.WriteLine(x);
            }
            proj = Mapping.ProcessProject(proj);
            if (!workspace.TryApplyChanges(proj.Solution))
            {
                Debug.Fail("");
            };
        }
    }
}
