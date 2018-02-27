using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using static ClrAnalyzer.Core.Compiler.CorJitFlags;

namespace ClrAnalyzer.Core.Compiler
{
    public unsafe class ICorJitCompiler
    {
        [DllImport("Clrjit.dll", CallingConvention = CallingConvention.StdCall, PreserveSig = true, EntryPoint = "getJit")]
        public static extern IntPtr GetJit();

        [DllImport("Clrjit.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, EntryPoint = "compileMethod")]
        public static extern IntPtr CompileMethod([In] IntPtr ICorJitInfo, [In] CorInfoMethodInfo* info, [In] UInt64 flags, [Out] CorJitFlag nativeEntry, [Out] UInt64 nativeSizeOfCode);

    }
}
