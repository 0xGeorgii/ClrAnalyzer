using ClrAnalyzer.Core.Compiler;
using ClrAnalyzer.Core.Hooks;
using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static ClrAnalyzer.Core.Compiler.CorJitFlags;

namespace ClrAnalyzer.Core.Dumps
{
    public class CompiledMethodInfo<T>
    {
        public uint CodeSize;
        public uint PrologSize;
        public string ILCode { get; private set; }
        public bool IsBlendedCode
        {
            get
            {
                if (_corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_DEBUG_CODE))
                {
                    var compCodeOpt = CorJitCompiler.CodeOptimize.BLENDED_CODE;
                    // If the EE sets SIZE_OPT or if we are compiling a Class constructor
                    // we will optimize for code size at the expense of speed
                    //
                    if (_corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_SIZE_OPT) || ((_compFlags & CorStaticInfo.CorInfoFlag.FLG_CCTOR) == CorStaticInfo.CorInfoFlag.FLG_CCTOR))
                    {
                        compCodeOpt = CorJitCompiler.CodeOptimize.SMALL_CODE;
                    }
                    //
                    // If the EE sets SPEED_OPT we will optimize for speed at the expense of code size
                    //
                    else if (_corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_SPEED_OPT) ||
                             (_corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_TIER1) && !_corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_MIN_OPT)))
                    {
                        compCodeOpt = CorJitCompiler.CodeOptimize.FAST_CODE;
                        if (_corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_SIZE_OPT))
                            throw new Exception("Seems CorJitFlags corrupted");
                    }
                    return compCodeOpt == CorJitCompiler.CodeOptimize.BLENDED_CODE;
                }
                //TODO: track https://github.com/dotnet/coreclr/blob/dbd533372e41b029398839056450c0fcac2b91f0/src/jit/compiler.h#L8533
                return true;
            }
        }
        public bool IsOptimizedCode => _corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_SIZE_OPT) || _corJitFlags.IsSet(CorJitFlags.CorJitFlag.CORJIT_FLAG_SPEED_OPT);
        public bool IsRbpBasedFrame { get; private set; }
        public bool IsPartiallyInterruptible { get; private set; }
        public bool IsFinalLocalVariableAssignments { get; private set; }

        private CorJitFlags __corJitFlags;
        private CorJitFlags _corJitFlags
        {
            get
            {
                if(__corJitFlags == null) __corJitFlags = new CorJitFlags(_corJitFlag);
                return __corJitFlags;
            }
        }
        private CorJitFlag _corJitFlag;
        private CorStaticInfo.CorInfoFlag _compFlags;

        public void Build(string path)
        {
            //TODO: parse file to fill the fields
        }
        
        private static CompilerHook hook;

#pragma warning disable CS0693 // Type parameter has the same name as the type parameter from outer type
        public unsafe void SetUp()
#pragma warning restore CS0693 // Type parameter has the same name as the type parameter from outer type
        {
            RuntimeHelpers.PrepareMethod(GetType().GetMethod("Release", System.Reflection.BindingFlags.Instance 
                | System.Reflection.BindingFlags.Public).MethodHandle, new[] { typeof(T).TypeHandle });
            RuntimeHelpers.PrepareMethod(GetType().GetMethod("NativeDump", System.Reflection.BindingFlags.Instance
                | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).MethodHandle, new[] { typeof(T).TypeHandle });

            RuntimeHelpers.PrepareMethod(typeof(CorJitCompiler).GetMethod("DumpMethodInfo", 
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static).MethodHandle, null);
            hook = new CompilerHook();
            hook.Hook(CompileMethodDel);
        }

        public void Release() => hook.RemoveHook();
        public static unsafe void NativeDump(IntPtr corJitInfoPtr, CorInfo* methodInfo,
            CorJitFlag flags, IntPtr nativeEntry, IntPtr nativeSizeOfCode) =>   CorJitCompiler.DumpMethodInfo(corJitInfoPtr, methodInfo, flags, nativeEntry, nativeSizeOfCode);

        internal static unsafe CorJitCompiler.CorJitResult CompileMethodDel(IntPtr thisPtr, [In] IntPtr corJitInfoPtr, [In] CorInfo* methodInfo,
            CorJitFlag flags, [Out] IntPtr nativeEntry, [Out] IntPtr nativeSizeOfCode)
        {
            hook.RemoveHook();
            var res = hook.Compile(thisPtr, corJitInfoPtr, methodInfo, flags, nativeEntry, nativeSizeOfCode);
            NativeDump(corJitInfoPtr, methodInfo, flags, nativeEntry, nativeSizeOfCode);
            return res;
        }
    }
}
