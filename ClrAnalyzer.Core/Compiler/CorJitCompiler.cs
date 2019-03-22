using System;
using System.Runtime.InteropServices;
using static ClrAnalyzer.Core.Compiler.CorJitFlags;
using static ClrAnalyzer.Core.Compiler.CorJitCompiler;

namespace ClrAnalyzer.Core.Compiler
{
    /// <summary>
    /// corjit.h
    /// </summary>
    /// 
    public unsafe interface ICorJitCompiler
    {
        CorJitResult CompileMethod(IntPtr thisPtr, [In] IntPtr corJitInfo, [In] CorInfo* methodInfo, CorJitFlag flags, [Out] IntPtr nativeEntry, [Out] IntPtr nativeSizeOfCode);
        void ProcessShutdownWork(IntPtr thisPtr, [In] IntPtr corStaticInfo);
    }

    public unsafe class CorJitCompiler
    {
        public unsafe struct CorJitCompilerNative
        {
            public CompileMethodDel CompileMethod;
            public ProcessShutdownWorkDel ProcessShutdownWork;
            public isCacheCleanupRequiredDel isCacheCleanupRequired;
            public getMethodAttribs getMethodAttribs;
        }

        public static ICorJitCompiler GetCorJitCompilerInterface()
        {
            var pJit = GetJit();
            var nativeCompiler = Marshal.PtrToStructure<CorJitCompilerNative>(pJit);
            return new CorJitCompilerNativeWrapper(pJit, nativeCompiler.CompileMethod, nativeCompiler.ProcessShutdownWork, nativeCompiler.getMethodAttribs);
        }

        private sealed class CorJitCompilerNativeWrapper : ICorJitCompiler
        {
            private IntPtr _pThis;
            private CompileMethodDel _compileMethod;
            private ProcessShutdownWorkDel _processShutdownWork;
            private getMethodAttribs _getMethodAttribs;

            public CorJitCompilerNativeWrapper(IntPtr pThis, CompileMethodDel compileMethodDel, ProcessShutdownWorkDel processShutdownWork, getMethodAttribs getMethodAttribs)
            {
                _pThis = pThis;
                _compileMethod = compileMethodDel;
                _processShutdownWork = processShutdownWork;
                _getMethodAttribs = getMethodAttribs;
            }

            public CorJitResult CompileMethod(IntPtr thisPtr, [In] IntPtr corJitInfo, [In] CorInfo* methodInfo, CorJitFlag flags, [Out] IntPtr nativeEntry, [Out] IntPtr nativeSizeOfCode)
            {
                return _compileMethod(thisPtr, corJitInfo, methodInfo, flags, nativeEntry, nativeSizeOfCode);
            }

            public void ProcessShutdownWork(IntPtr thisPtr, [In] IntPtr corStaticInfo)
            {
                _processShutdownWork(thisPtr, corStaticInfo);
            }

            public UInt32 getMethodAttribs(IntPtr methodHandle)
            {
                return _getMethodAttribs(methodHandle);
            }
        }

        [DllImport(
#if _TARGET_X64_
            "Clrjit.dll"
#else
            "Mscorjit.dll"
#endif
            , CallingConvention = CallingConvention.StdCall, SetLastError = true, EntryPoint = "getJit", BestFitMapping = true)]
        public static extern IntPtr GetJit();

        [DllImport("Win32Native.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true, EntryPoint = "DumpMethodInfo", BestFitMapping = true)]
        public static extern void DumpMethodInfo(IntPtr corJitInfo, CorInfo* methodInfo, CorJitFlag flags, IntPtr nativeEntry,  IntPtr nativeSizeOfCode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate CorJitResult CompileMethodDel(IntPtr thisPtr, [In] IntPtr corJitInfo, [In] CorInfo* methodInfo, CorJitFlag flags, [Out] IntPtr nativeEntry, [Out] IntPtr nativeSizeOfCode);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate void ProcessShutdownWorkDel(IntPtr thisPtr, [Out] IntPtr corStaticInfo);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate Byte isCacheCleanupRequiredDel();

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate UInt32 getMethodAttribs(IntPtr methodHandle);

        // These are error codes returned by CompileMethod
        public const Int32 SEVERITY_ERROR = 1;
        public const Int32 FACILITY_NULL = 0;

        public enum CorJitResult : Int32
        {
            CORJIT_OK = 0,
            CORJIT_BADCODE = unchecked ((Int32)(((UInt32)(SEVERITY_ERROR) << 31) | ((UInt32)(FACILITY_NULL) << 16) | ((UInt32)(1)))),
            CORJIT_OUTOFMEM = unchecked((Int32)(((UInt32)(SEVERITY_ERROR) << 31) | ((UInt32)(FACILITY_NULL) << 16) | ((UInt32)(2)))),
            CORJIT_INTERNALERROR = unchecked((Int32)(((UInt32)(SEVERITY_ERROR) << 31) | ((UInt32)(FACILITY_NULL) << 16) | ((UInt32)(3)))),
            CORJIT_SKIPPED = unchecked((Int32)(((UInt32)(SEVERITY_ERROR) << 31) | ((UInt32)(FACILITY_NULL) << 16) | ((UInt32)(4)))),
            CORJIT_RECOVERABLEERROR = unchecked((Int32)(((UInt32)(SEVERITY_ERROR) << 31) | ((UInt32)(FACILITY_NULL) << 16) | ((UInt32)(5)))),
        };

        public enum CodeOptimize
        {
            BLENDED_CODE,
            SMALL_CODE,
            FAST_CODE,
            COUNT_OPT_CODE
        };
    }
}
