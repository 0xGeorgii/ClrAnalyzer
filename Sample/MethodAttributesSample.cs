using System;
using System.Runtime.CompilerServices;

namespace Sample.MethodAttributesSample
{
    internal abstract class AbstractSampleClass
    {
        public abstract int Calc(int x, int y);
    }

    internal class SampleClass : AbstractSampleClass
    {
        [MethodImpl(MethodImplOptions.NoInlining)]
        public sealed override int Calc(int x, int y)
        {
            var r = Math.Asin((double)x);
            return (int)r * y * 1;
        }
    }

    internal class SampleClass1
    {

        [MethodImpl(MethodImplOptions.Synchronized)]
        public virtual int Calc(double x, int y)
        {
            var r = Math.Asin((double)x);
            return (int)r * y * 2;
        }
    }

    internal class SampleClass2
    {

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Calc(int x, decimal y)
        {
            var r = Math.Asin((double)x);
            return (int)r * y.CompareTo(x) * 3;
        }
    }
}
