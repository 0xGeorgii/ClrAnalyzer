using ClrAnalyzer.Core.Compiler;
using System;
using System.Runtime.InteropServices;

namespace Sample
{
    unsafe class Program
    {

        static void Main(string[] args)
        {
            try
            {
                var compiler = ICorJitCompiler.GetJit();
                Marshal.GetDelegateForFunctionPointer(compiler, typeof(ICorJitCompiler.))
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
