using ClrAnalyzer.Core.Dumps;
using ClrAnalyzer.Core.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.InteropServices;

namespace Sample
{

    unsafe class Program
    {
        static void Main(string[] args)
        {
            Console.ReadKey();
            Console.WriteLine($"Last Win32 exception: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
            try
            {
                using (var fs = new FileStream("./Result.txt", FileMode.OpenOrCreate, FileAccess.Write))
                using (var sw = new StreamWriter(fs))
                {
                    Win32IOUtils.SetStdHandle(Win32IOUtils.STD_OUTPUT_HANDLE, fs.SafeFileHandle.DangerousGetHandle());
                    var IPCmi = new CompiledMethodInfo<InliningSample>();
                    InliningSample.Run(IPCmi);
                }
                Console.OpenStandardOutput().Flush();
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Console.WriteLine($"Win32 exception: {new Win32Exception(Marshal.GetLastWin32Error()).Message}");
                Console.ReadKey();
            }
        }
    }
}
