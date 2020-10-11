using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GithubIssueWatcher.Helpers
{
    internal static class RoslynScripting
    {
        internal static Func<T, bool> Evaluate<T>(Assembly[] assemblies, string[] imports, string lambda)
        {
            var options = ScriptOptions.Default.AddReferences(assemblies).WithImports(imports);
            return CSharpScript.EvaluateAsync<Func<T, bool>>(lambda, options).Result;
        }
    }
}
