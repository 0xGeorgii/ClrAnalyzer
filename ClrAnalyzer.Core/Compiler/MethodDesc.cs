using System;
using System.Runtime.InteropServices;

namespace ClrAnalyzer.Core.Compiler
{
    public unsafe class MethodDesc
    {
        [StructLayout(layoutKind: LayoutKind.Sequential)]
        public unsafe struct MethodDescNative
        {
            public GetAttrsDel GetAtts;
            public GetImplAttrsDel GetImplAttrs;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate UInt32 GetAttrsDel(IntPtr thisPtr);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate UInt32 GetImplAttrsDel(IntPtr thisPtr);
    }
}
