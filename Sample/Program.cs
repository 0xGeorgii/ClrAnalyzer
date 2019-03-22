using ClrAnalyzer.Core.Dumps;
using ClrAnalyzer.Core.Utils;
using System;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Sample
{
    abstract class AbstractSampleClass
    {
        public abstract int Calc(int x, int y);
    }

    class SampleClass : AbstractSampleClass
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public sealed override int Calc(int x, int y)
        {
            var r = Math.Asin((double)x);
            return (int)r * y * 1;
        }
    }
    
    class SampleClass1
    {

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual int Calc(double x, int y)
        {
            var r = Math.Asin((double)x);
            return (int)r * y * 2;
        }
    }

    class SampleClass2
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Calc(int x, decimal y)
        {
            var r = Math.Asin((double)x);
            return (int)r * y.CompareTo(x) * 3;
        }
    }

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
                    var cmi = new CompiledMethodInfo<Program>();
                    cmi.SetUp();
                    var sample = new SampleClass();
                    cmi.SetUp();
                    Console.WriteLine(sample.Calc(0x0fffffff, 0x11111111));
                    var sample1 = new SampleClass1();
                    cmi.SetUp();
                    Console.WriteLine(sample1.Calc(0x0fffffff, 0x11111111));
                    cmi.SetUp();
                    Console.WriteLine(SampleClass2.Calc(0x0fffffff, 0x11111111));
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
    }
}
