using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sample
{
    internal class IntParam
    {
        public int Val;
    }

    internal struct sIntParam
    {
        public int Val;
    }

    internal class InliningSample
    {
        internal static int A(int v)
        {
            // This is a single method call.
            // ... It contains twenty increments.
            v++; v++; v++; v++; v++; v++; v++; v++; v++; v++;
            v++; v++; v++; v++; v++; v++; v++; v++; v++; v++;
            return v;
        }
                
        internal static int B(int v)
        {
            // This does ten increments.
            // ... Then it does ten more increments in another method.
            v++; v++; v++; v++; v++; v++; v++; v++; v++; v++;
            v = C(v);
            return v;
        }
        
        //[MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int C(int v)
        {
            // This does ten increments.
            v++; v++; v++; v++; v++; v++; v++; v++; v++; v++;
            return v;
        }

        internal static int A1(IntParam v)
        {
            // This is a single method call.
            // ... It contains twenty increments.
            v.Val++; v.Val++; v.Val++; v.Val++; v.Val++; v.Val++; v.Val++; v.Val++; v.Val++; v.Val++;
            return v.Val;
        }

        internal static int B1(sIntParam v)
        {
            // This does ten increments.
            // ... Then it does ten more increments in another method.
            v.Val++; v.Val++; v.Val++; v.Val++; v.Val++;
            v.Val = C1(v);
            return v.Val;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static int C1(sIntParam v)
        {
            // This does ten increments.
            v.Val++; v.Val++; v.Val++; v.Val++; v.Val++;
            return v.Val;
        }

        private Action _myAction;

        // SURPRISE! This method will not be inlined. 
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Test()
        {
            _myAction();
        }

        internal static void Run(ClrAnalyzer.Core.Dumps.CompiledMethodInfo<InliningSample> iPCmi)
        {
            const int max = 100000000;
            int temp1 = 0;
            int temp2 = 0;
            iPCmi.SetUp();
            A1(new IntParam { Val = 0 });
            //iPCmi.SetUp();
            //C1(new sIntParam { Val = 0 });
            iPCmi.SetUp();
            B1(new sIntParam { Val = 0 });

            var s1 = Stopwatch.StartNew();
            for (int i = 0; i < max; i++)
            {
                temp1 = A1(new IntParam { Val = i });
            }
            s1.Stop();
            var s2 = Stopwatch.StartNew();
            for (int i = 0; i < max; i++)
            {
                temp2 = B1(new sIntParam { Val = i });
            }
            s2.Stop();
            Console.WriteLine(s1.Elapsed.TotalMilliseconds * 1000 * 1000 / (double)max);
            Console.WriteLine(s2.Elapsed.TotalMilliseconds * 1000 * 1000 / (double)max);
            Console.WriteLine("{0} {1}", temp1, temp2);
            Console.Read();
        }
    }
}
