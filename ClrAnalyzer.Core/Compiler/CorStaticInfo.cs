using System;
using System.Runtime.InteropServices;
using static ClrAnalyzer.Core.Compiler.CorStaticInfo;

namespace ClrAnalyzer.Core.Compiler
{
    public interface ICorStaticInfo
    {
        CorInfoFlag GetMethodAttribs(IntPtr thisPtr, [In] IntPtr methodHandle);
        CorInfoFlag GetMethodAttribsInternal(IntPtr thisPtr, [In] IntPtr methodHandle);
    }

    public unsafe class CorStaticInfo
    {
        [StructLayout(layoutKind: LayoutKind.Sequential)]
        public unsafe struct CorStaticInfoNative
        {
            public getMethodAttribsDel GetMethodAttribs;
            public getMethodAttribsInternalDel GetMethodAttribsInternal;
        }
        
        public static ICorStaticInfo GetCorStaticInfoInterface(IntPtr ptr)
        {
            var corStaticInfoNative = Marshal.PtrToStructure<CorStaticInfoNative>(ptr);
            return new CorStaticInfoNativeWrapper(ptr, corStaticInfoNative.GetMethodAttribs, corStaticInfoNative.GetMethodAttribsInternal);
        }

        private sealed class CorStaticInfoNativeWrapper : ICorStaticInfo
        {
            private IntPtr _pThis;
            private getMethodAttribsDel _getMethodAttribs;
            private getMethodAttribsInternalDel _getMethodAttribsInternal;

            public CorStaticInfoNativeWrapper(IntPtr pThis, getMethodAttribsDel getMethodAttribs, getMethodAttribsInternalDel getMethodAttribsInternal)
            {
                _pThis = pThis;
                _getMethodAttribs = getMethodAttribs;
                _getMethodAttribsInternal = getMethodAttribsInternal;
            }

            public CorInfoFlag GetMethodAttribs(IntPtr thisPtr, [In] IntPtr methodHandle)
            {
                return _getMethodAttribs(thisPtr, methodHandle);
            }

            public CorInfoFlag GetMethodAttribsInternal(IntPtr thisPtr, [In] IntPtr methodHandle)
            {
                return _getMethodAttribsInternal(thisPtr, methodHandle);
            }
        }

        //from coreinfo.h
        // these are the attribute flags for fields and methods (getMethodAttribs)
        public enum CorInfoFlag : UInt32
        {
            //  CORINFO_FLG_UNUSED                = 0x00000001,
            //  CORINFO_FLG_UNUSED                = 0x00000002,
            CORINFO_FLG_PROTECTED = 0x00000004,
            CORINFO_FLG_STATIC = 0x00000008,
            CORINFO_FLG_FINAL = 0x00000010,
            CORINFO_FLG_SYNCH = 0x00000020,
            CORINFO_FLG_VIRTUAL = 0x00000040,
            //  CORINFO_FLG_UNUSED                = 0x00000080,
            CORINFO_FLG_NATIVE = 0x00000100,
            CORINFO_FLG_INTRINSIC_TYPE = 0x00000200, // This type is marked by [Intrinsic]
            CORINFO_FLG_ABSTRACT = 0x00000400,

            CORINFO_FLG_EnC = 0x00000800, // member was added by Edit'n'Continue

            // These are internal flags that can only be on methods
            CORINFO_FLG_FORCEINLINE = 0x00010000, // The method should be inlined if possible.
            CORINFO_FLG_SHAREDINST = 0x00020000, // the code for this method is shared between different generic instantiations (also set on classes/types)
            CORINFO_FLG_DELEGATE_INVOKE = 0x00040000, // "Delegate
            CORINFO_FLG_PINVOKE = 0x00080000, // Is a P/Invoke call
            CORINFO_FLG_SECURITYCHECK = 0x00100000, // Is one of the security routines that does a stackwalk (e.g. Assert, Demand)
            CORINFO_FLG_NOGCCHECK = 0x00200000, // This method is FCALL that has no GC check.  Don't put alone in loops
            CORINFO_FLG_INTRINSIC = 0x00400000, // This method MAY have an intrinsic ID
            CORINFO_FLG_CONSTRUCTOR = 0x00800000, // This method is an instance or type initializer
                                                  //  CORINFO_FLG_UNUSED                = 0x01000000,
                                                  //  CORINFO_FLG_UNUSED                = 0x02000000,
            CORINFO_FLG_NOSECURITYWRAP = 0x04000000, // The method requires no security checks
            CORINFO_FLG_DONT_INLINE = 0x10000000, // The method should not be inlined
            CORINFO_FLG_DONT_INLINE_CALLER = 0x20000000, // The method should not be inlined, nor should its callers. It cannot be tail called.
            CORINFO_FLG_JIT_INTRINSIC = 0x40000000, // Method is a potential jit intrinsic; verify identity by name check

            // These are internal flags that can only be on Classes
            CORINFO_FLG_VALUECLASS = 0x00010000, // is the class a value class
                                                 //  This flag is define din the Methods section, but is also valid on classes.
                                                 //  CORINFO_FLG_SHAREDINST            = 0x00020000, // This class is satisfies TypeHandle::IsCanonicalSubtype
            CORINFO_FLG_VAROBJSIZE = 0x00040000, // the object size varies depending of constructor args
            CORINFO_FLG_ARRAY = 0x00080000, // class is an array class (initialized differently)
            CORINFO_FLG_OVERLAPPING_FIELDS = 0x00100000, // struct or class has fields that overlap (aka union)
            CORINFO_FLG_INTERFACE = 0x00200000, // it is an interface
            CORINFO_FLG_CONTEXTFUL = 0x00400000, // is this a contextful class?
            CORINFO_FLG_CUSTOMLAYOUT = 0x00800000, // does this struct have custom layout?
            CORINFO_FLG_CONTAINS_GC_PTR = 0x01000000, // does the class contain a gc ptr ?
            CORINFO_FLG_DELEGATE = 0x02000000, // is this a subclass of delegate or multicast delegate ?
            CORINFO_FLG_MARSHAL_BYREF = 0x04000000, // is this a subclass of MarshalByRef ?
            CORINFO_FLG_CONTAINS_STACK_PTR = 0x08000000, // This class has a stack pointer inside it
            CORINFO_FLG_VARIANCE = 0x10000000, // MethodTable::HasVariance (sealed does *not* mean uncast-able)
            CORINFO_FLG_BEFOREFIELDINIT = 0x20000000, // Additional flexibility for when to run .cctor (see code:#ClassConstructionFlags)
            CORINFO_FLG_GENERIC_TYPE_VARIABLE = 0x40000000, // This is really a handle for a variable type
            CORINFO_FLG_UNSAFE_VALUECLASS = 0x80000000, // Unsafe (C++'s /GS) value type
            FLG_CCTOR = (CORINFO_FLG_CONSTRUCTOR | CORINFO_FLG_STATIC)
        };

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate CorInfoFlag getMethodAttribsDel(IntPtr thisPtr, [In] IntPtr methodHandle);

        [UnmanagedFunctionPointer(CallingConvention.StdCall, SetLastError = true)]
        public unsafe delegate CorInfoFlag getMethodAttribsInternalDel(IntPtr thisPtr, [In] IntPtr methodHandle);
    }
}
