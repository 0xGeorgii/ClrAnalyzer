using System;
using System.Runtime.InteropServices;

namespace ClrAnalyzer.Core.Compiler
{
    public unsafe class CorDynamicInfo : CorStaticInfo
    {
        public unsafe struct CEEInfoNative
        {
            public GetCallInfoDel GetCallInfo;
        }

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate void GetCallInfoDel(IntPtr thisPtr, [In] CorinfoResolvedToken* pResolvedToken, [In] CorinfoResolvedToken* pConstrainedResolvedToken,
            IntPtr callerHandle, CORINFO_CALLINFO_FLAGS flags, [Out] IntPtr* pResult);

        public enum CORINFO_CALLINFO_FLAGS
        {
            CORINFO_CALLINFO_NONE = 0x0000,
            CORINFO_CALLINFO_ALLOWINSTPARAM = 0x0001,   // Can the compiler generate code to pass an instantiation parameters? Simple compilers should not use this flag
            CORINFO_CALLINFO_CALLVIRT = 0x0002,   // Is it a virtual call?
            CORINFO_CALLINFO_KINDONLY = 0x0004,   // This is set to only query the kind of call to perform, without getting any other information
            CORINFO_CALLINFO_VERIFICATION = 0x0008,   // Gets extra verification information.
            CORINFO_CALLINFO_SECURITYCHECKS = 0x0010,   // Perform security checks.
            CORINFO_CALLINFO_LDFTN = 0x0020,   // Resolving target of LDFTN
            CORINFO_CALLINFO_ATYPICAL_CALLSITE = 0x0040, // Atypical callsite that cannot be disassembled by delay loading helper
        }
    }
}
