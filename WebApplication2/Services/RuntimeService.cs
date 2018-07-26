using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.CSharp;
using System.CodeDom.Compiler;

namespace WebApplication2.Services
{
    public class RuntimeService
    {
        private CSharpCodeProvider _provider;
        CompilerParameters _parameters;
        public RuntimeService()
        {          
            _provider = new CSharpCodeProvider();
            _parameters = new System.CodeDom.Compiler.CompilerParameters()
            {
                CoreAssemblyFileName = "System",
                GenerateExecutable = true,
                OutputAssembly = "Custom"
            };        
        }

        public void GenerateType(string className)
        {
            string code = @"
                public class Account
                {
                    public int ID {get;set;}
                    public string FirstName {get;set;}
                }";
            _provider.CompileAssemblyFromSource(_parameters, code);
        }
    }
}