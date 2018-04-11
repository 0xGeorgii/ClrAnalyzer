using ClrAnalyzer.Core.Dumps;
using ClrAnalyzer.Core.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
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
                using (var fs = new FileStream("./Result.txt", FileMode.OpenOrCreate, FileAccess.ReadWrite))
                using (var sw = new StreamWriter(fs))
                {
                    Win32IOUtils.SetStdHandle(Win32IOUtils.STD_OUTPUT_HANDLE, fs.SafeFileHandle.DangerousGetHandle());
                    var cmi = new CompiledMethodInfo<Program>();
                    cmi.SetUp();
                    Calc(0x0fffffff, 0x11111111);
                    cmi.Release();
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int Calc(int x, int y)
        {
            var r = Math.Asin((double)x);
            return (int)r * y;
        }
    }
}
