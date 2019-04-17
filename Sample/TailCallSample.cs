namespace Sample
{
    internal class TailCallSample
    {
        private static long FactorialWithoutTailing(int depth)
           => depth == 0 ? 1 : depth * FactorialWithoutTailing(depth - 1);

        private static long FactorialWithTailing(int pos, int depth)
            => pos == 0 ? depth : FactorialWithTailing(pos - 1, depth * pos);

        private static long FactorialWithTailing(int depth)
            => FactorialWithTailing(1, depth);

        internal static void Run(ClrAnalyzer.Core.Dumps.CompiledMethodInfo<TailCallSample> iPCmi)
        {
            iPCmi.SetUp();
            FactorialWithoutTailing(1);
                //iPCmi.SetUp();
                //FactorialWithTailing(0, 1);
            iPCmi.SetUp();
            FactorialWithTailing(1);
        }
    }
}
