using System;
using System.Runtime.InteropServices;

namespace ClrAnalyzer.Core.Compiler
{
    [StructLayout(layoutKind: LayoutKind.Sequential, Pack = 1, Size = 0x88)]
    public unsafe struct CorInfo
    {
        //ftn CORINFO_METHOD_HANDLE
        public IntPtr methodHandle;
        //scope CORINFO_MODULE_HANDLE
        public IntPtr moduleHandle;
        //BYTE*
        public IntPtr ILCode;
        public UInt32 ILCodeSize;
        public UInt16 maxStack;
        public UInt16 EHcount;
        //options CorInfoOptions
        public CorInfoOptions options;
        //regionKind CorInfoRegionKind
        public CorInfoRegionKind regionKind;
        //CORINFO_SIG_INFO
        public CorInfoSigInfo args;
        //CORINFO_SIG_INFO
        public CorInfoSigInfo locals;
    }

    public enum CorInfoOptions : UInt32
    {
        CORINFO_OPT_INIT_LOCALS = 0x00000010, // zero initialize all variables

        CORINFO_GENERICS_CTXT_FROM_THIS = 0x00000020, // is this shared generic code that access the generic context from the this pointer?  If so, then if the method has SEH then the 'this' pointer must always be reported and kept alive.
        CORINFO_GENERICS_CTXT_FROM_METHODDESC = 0x00000040, // is this shared generic code that access the generic context from the ParamTypeArg(that is a MethodDesc)?  If so, then if the method has SEH then the 'ParamTypeArg' must always be reported and kept alive. Same as CORINFO_CALLCONV_PARAMTYPE
        CORINFO_GENERICS_CTXT_FROM_METHODTABLE = 0x00000080, // is this shared generic code that access the generic context from the ParamTypeArg(that is a MethodTable)?  If so, then if the method has SEH then the 'ParamTypeArg' must always be reported and kept alive. Same as CORINFO_CALLCONV_PARAMTYPE
        CORINFO_GENERICS_CTXT_MASK = (CORINFO_GENERICS_CTXT_FROM_THIS |
                                                   CORINFO_GENERICS_CTXT_FROM_METHODDESC |
                                                   CORINFO_GENERICS_CTXT_FROM_METHODTABLE),
        CORINFO_GENERICS_CTXT_KEEP_ALIVE = 0x00000100, // Keep the generics context alive throughout the method even if there is no explicit use, and report its location to the CLR

    };

    public enum CorInfoRegionKind : UInt32
    {
        CORINFO_REGION_NONE,
        CORINFO_REGION_HOT,
        CORINFO_REGION_COLD,
        CORINFO_REGION_JIT,
    };

    //CORINFO_SIG_INFO
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public unsafe struct CorInfoSigInfo
    {
        //CorInfoCallConv
        public CorInfoCallConv callConv;
        //CORINFO_CLASS_HANDLE
        public IntPtr retTypeClass;   // if the return type is a value class, this is its handle (enums are normalized)
        public IntPtr retTypeSigClass;// returns the value class as it is in the sig (enums are not converted to primitives)
        public CorInfoType retType;
        public byte flags;    // used by IL stubs code
        public UInt16 numArgs;
        public CorinfoSigInst sigInst;  // information about how type variables are being instantiated in generic code
        public IntPtr args;
        public IntPtr pSig;
        public UInt64 cbSig;
        //scope CORINFO_MODULE_HANDLE
        public IntPtr moduleHandle;          // passed to getArgClass
        public UInt32 token;
    }

    public enum CorInfoCallConv
    {
        // These correspond to CorCallingConvention

        CORINFO_CALLCONV_DEFAULT = 0x0,
        CORINFO_CALLCONV_C = 0x1,
        CORINFO_CALLCONV_STDCALL = 0x2,
        CORINFO_CALLCONV_THISCALL = 0x3,
        CORINFO_CALLCONV_FASTCALL = 0x4,
        CORINFO_CALLCONV_VARARG = 0x5,
        CORINFO_CALLCONV_FIELD = 0x6,
        CORINFO_CALLCONV_LOCAL_SIG = 0x7,
        CORINFO_CALLCONV_PROPERTY = 0x8,
        CORINFO_CALLCONV_NATIVEVARARG = 0xb,    // used ONLY for IL stub PInvoke vararg calls

        CORINFO_CALLCONV_MASK = 0x0f,     // Calling convention is bottom 4 bits
        CORINFO_CALLCONV_GENERIC = 0x10,
        CORINFO_CALLCONV_HASTHIS = 0x20,
        CORINFO_CALLCONV_EXPLICITTHIS = 0x40,
        CORINFO_CALLCONV_PARAMTYPE = 0x80,     // Passed last. Same as CORINFO_GENERICS_CTXT_FROM_PARAMTYPEARG
    };

    // The enumeration is returned in 'getSig','getType', getArgType methods
    public enum CorInfoType
    {
        CORINFO_TYPE_UNDEF = 0x0,
        CORINFO_TYPE_VOID = 0x1,
        CORINFO_TYPE_BOOL = 0x2,
        CORINFO_TYPE_CHAR = 0x3,
        CORINFO_TYPE_BYTE = 0x4,
        CORINFO_TYPE_UBYTE = 0x5,
        CORINFO_TYPE_SHORT = 0x6,
        CORINFO_TYPE_USHORT = 0x7,
        CORINFO_TYPE_INT = 0x8,
        CORINFO_TYPE_UINT = 0x9,
        CORINFO_TYPE_LONG = 0xa,
        CORINFO_TYPE_ULONG = 0xb,
        CORINFO_TYPE_NATIVEINT = 0xc,
        CORINFO_TYPE_NATIVEUINT = 0xd,
        CORINFO_TYPE_FLOAT = 0xe,
        CORINFO_TYPE_DOUBLE = 0xf,
        CORINFO_TYPE_STRING = 0x10,         // Not used, should remove
        CORINFO_TYPE_PTR = 0x11,
        CORINFO_TYPE_BYREF = 0x12,
        CORINFO_TYPE_VALUECLASS = 0x13,
        CORINFO_TYPE_CLASS = 0x14,
        CORINFO_TYPE_REFANY = 0x15,

        // CORINFO_TYPE_VAR is for a generic type variable.
        // Generic type variables only appear when the JIT is doing
        // verification (not NOT compilation) of generic code
        // for the EE, in which case we're running
        // the JIT in "import only" mode.

        CORINFO_TYPE_VAR = 0x16,
        CORINFO_TYPE_COUNT,                         // number of jit types
    };

    //CORINFO_SIG_INST
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public unsafe struct CorinfoSigInst
    {
        public UInt64 classInstCount;
        public IntPtr* classInst; // (representative, not exact) instantiation for class type variables in signature
        public UInt64 methInstCount;
        public IntPtr* methInst; // (representative, not exact) instantiation for method type variables in signature
    }

    //corinfo.h
    
    //CORINFO_RESOLVED_TOKEN
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public unsafe struct CorinfoResolvedToken
    {
        //
        // [In] arguments of resolveToken
        //
        public IntPtr tokenContext;       //Context for resolution of generic arguments
        public IntPtr tokenScope;
        public UInt32 token;              //The source token
        public CorInfoTokenKind tokenType;

        //
        // [Out] arguments of resolveToken. 
        // - Type handle is always non-NULL.
        // - At most one of method and field handles is non-NULL (according to the token type).
        // - Method handle is an instantiating stub only for generic methods. Type handle 
        //   is required to provide the full context for methods in generic types.
        //
        public IntPtr hClass;
        public IntPtr hMethod;
        public IntPtr hField;

        //
        // [Out] TypeSpec and MethodSpec signatures for generics. NULL otherwise.
        //
        public Byte pTypeSpec;
        public UInt32 cbTypeSpec;
        public Byte pMethodSpec;
        public UInt32 cbMethodSpec;
    }

    public enum CorInfoTokenKind
    {
        CORINFO_TOKENKIND_Class = 0x01,
        CORINFO_TOKENKIND_Method = 0x02,
        CORINFO_TOKENKIND_Field = 0x04,
        CORINFO_TOKENKIND_Mask = 0x07,

        // token comes from CEE_LDTOKEN
        CORINFO_TOKENKIND_Ldtoken = 0x10 | CORINFO_TOKENKIND_Class | CORINFO_TOKENKIND_Method | CORINFO_TOKENKIND_Field,

        // token comes from CEE_CASTCLASS or CEE_ISINST
        CORINFO_TOKENKIND_Casting = 0x20 | CORINFO_TOKENKIND_Class,

        // token comes from CEE_NEWARR
        CORINFO_TOKENKIND_Newarr = 0x40 | CORINFO_TOKENKIND_Class,

        // token comes from CEE_BOX
        CORINFO_TOKENKIND_Box = 0x80 | CORINFO_TOKENKIND_Class,

        // token comes from CEE_CONSTRAINED
        CORINFO_TOKENKIND_Constrained = 0x100 | CORINFO_TOKENKIND_Class,

        // token comes from CEE_NEWOBJ
        CORINFO_TOKENKIND_NewObj = 0x200 | CORINFO_TOKENKIND_Method,

        // token comes from CEE_LDVIRTFTN
        CORINFO_TOKENKIND_Ldvirtftn = 0x400 | CORINFO_TOKENKIND_Method,
    }

    //CORINFO_CALL_INFO
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public unsafe struct CorinfoCallInfo
    {
        public IntPtr hMethod;            //target method handle
        public UInt32 methodFlags;        //flags for the target method

        public UInt32 classFlags;         //flags for CORINFO_RESOLVED_TOKEN::hClass

        public CorInfoSigInfo sig;

        //Verification information
        public UInt32 verMethodFlags;     // flags for CORINFO_RESOLVED_TOKEN::hMethod
        public CorInfoSigInfo verSig;
        //All of the regular method data is the same... hMethod might not be the same as CORINFO_RESOLVED_TOKEN::hMethod


        //If set to:
        //  - CORINFO_ACCESS_ALLOWED - The access is allowed.
        //  - CORINFO_ACCESS_ILLEGAL - This access cannot be allowed (i.e. it is public calling private).  The
        //      JIT may either insert the callsiteCalloutHelper into the code (as per a verification error) or
        //      call throwExceptionFromHelper on the callsiteCalloutHelper.  In this case callsiteCalloutHelper
        //      is guaranteed not to return.
        //  - CORINFO_ACCESS_RUNTIME_CHECK - The jit must insert the callsiteCalloutHelper at the call site.
        //      the helper may return
        public CorInfoIsAccessAllowedResult accessAllowed;
        public CorinfoHelperDesc callsiteCalloutHelper;

        // See above section on constraintCalls to understand when these are set to unusual values.
        public CorinfoThisTransform thisTransform;

        public CorinfoCallKind kind;
        public bool nullInstanceCheck;

        // Context for inlining and hidden arg
        public IntPtr contextHandle;
        public bool exactContextNeedsRuntimeLookup; // Set if contextHandle is approx handle. Runtime lookup is required to get the exact handle.

        // If kind.CORINFO_VIRTUALCALL_STUB then stubLookup will be set.
        // If kind.CORINFO_CALL_CODE_POINTER then entryPointLookup will be set.
        [StructLayout(LayoutKind.Explicit)]
        public struct lookup
        {
            [FieldOffset(0)]
            CorinfoLookup stubLookup;
            [FieldOffset(0)]
            CorinfoLookup codePointerLookup;
        }

        public CorinfoConstLookup instParamLookup;    // Used by Ready-to-Run

        public bool secureDelegateInvoke;
    }

    //CORINFO_HELPER_DESC
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public unsafe struct CorinfoHelperDesc
    {
        public CorInfoHelpFunc helperNum;
        public UInt16 numArgs;

        [StructLayout(LayoutKind.Explicit)]
        public struct args
        {
            [FieldOffset(0)]
            UInt32 fieldHandle;
            [FieldOffset(0)]
            UInt32 methodHandle;
            [FieldOffset(0)]
            UInt32 classHandle;
            [FieldOffset(0)]
            UInt32 moduleHandle;
            [FieldOffset(0)]
            UInt32 constant;
        };
    }

    public enum CorInfoHelpFunc
    {
        CORINFO_HELP_UNDEF,         // invalid value. This should never be used

        /* Arithmetic helpers */

        CORINFO_HELP_DIV,           // For the ARM 32-bit integer divide uses a helper call :-(
        CORINFO_HELP_MOD,
        CORINFO_HELP_UDIV,
        CORINFO_HELP_UMOD,

        CORINFO_HELP_LLSH,
        CORINFO_HELP_LRSH,
        CORINFO_HELP_LRSZ,
        CORINFO_HELP_LMUL,
        CORINFO_HELP_LMUL_OVF,
        CORINFO_HELP_ULMUL_OVF,
        CORINFO_HELP_LDIV,
        CORINFO_HELP_LMOD,
        CORINFO_HELP_ULDIV,
        CORINFO_HELP_ULMOD,
        CORINFO_HELP_LNG2DBL,               // Convert a signed int64 to a double
        CORINFO_HELP_ULNG2DBL,              // Convert a unsigned int64 to a double
        CORINFO_HELP_DBL2INT,
        CORINFO_HELP_DBL2INT_OVF,
        CORINFO_HELP_DBL2LNG,
        CORINFO_HELP_DBL2LNG_OVF,
        CORINFO_HELP_DBL2UINT,
        CORINFO_HELP_DBL2UINT_OVF,
        CORINFO_HELP_DBL2ULNG,
        CORINFO_HELP_DBL2ULNG_OVF,
        CORINFO_HELP_FLTREM,
        CORINFO_HELP_DBLREM,
        CORINFO_HELP_FLTROUND,
        CORINFO_HELP_DBLROUND,

        /* Allocating a new object. Always use ICorClassInfo::getNewHelper() to decide 
           which is the right helper to use to allocate an object of a given type. */

        CORINFO_HELP_NEW_CROSSCONTEXT,  // cross context new object
        CORINFO_HELP_NEWFAST,
        CORINFO_HELP_NEWSFAST,          // allocator for small, non-finalizer, non-array object
        CORINFO_HELP_NEWSFAST_ALIGN8,   // allocator for small, non-finalizer, non-array object, 8 byte aligned
        CORINFO_HELP_NEW_MDARR,         // multi-dim array helper (with or without lower bounds - dimensions passed in as vararg)
        CORINFO_HELP_NEW_MDARR_NONVARARG,// multi-dim array helper (with or without lower bounds - dimensions passed in as unmanaged array)
        CORINFO_HELP_NEWARR_1_DIRECT,   // helper for any one dimensional array creation
        CORINFO_HELP_NEWARR_1_R2R_DIRECT, // wrapper for R2R direct call, which extracts method table from ArrayTypeDesc
        CORINFO_HELP_NEWARR_1_OBJ,      // optimized 1-D object arrays
        CORINFO_HELP_NEWARR_1_VC,       // optimized 1-D value class arrays
        CORINFO_HELP_NEWARR_1_ALIGN8,   // like VC, but aligns the array start

        CORINFO_HELP_STRCNS,            // create a new string literal
        CORINFO_HELP_STRCNS_CURRENT_MODULE, // create a new string literal from the current module (used by NGen code)

        /* Object model */

        CORINFO_HELP_INITCLASS,         // Initialize class if not already initialized
        CORINFO_HELP_INITINSTCLASS,     // Initialize class for instantiated type

        // Use ICorClassInfo::getCastingHelper to determine
        // the right helper to use

        CORINFO_HELP_ISINSTANCEOFINTERFACE, // Optimized helper for interfaces
        CORINFO_HELP_ISINSTANCEOFARRAY,  // Optimized helper for arrays
        CORINFO_HELP_ISINSTANCEOFCLASS, // Optimized helper for classes
        CORINFO_HELP_ISINSTANCEOFANY,   // Slow helper for any type

        CORINFO_HELP_CHKCASTINTERFACE,
        CORINFO_HELP_CHKCASTARRAY,
        CORINFO_HELP_CHKCASTCLASS,
        CORINFO_HELP_CHKCASTANY,
        CORINFO_HELP_CHKCASTCLASS_SPECIAL, // Optimized helper for classes. Assumes that the trivial cases 
                                           // has been taken care of by the inlined check

        CORINFO_HELP_BOX,
        CORINFO_HELP_BOX_NULLABLE,      // special form of boxing for Nullable<T>
        CORINFO_HELP_UNBOX,
        CORINFO_HELP_UNBOX_NULLABLE,    // special form of unboxing for Nullable<T>
        CORINFO_HELP_GETREFANY,         // Extract the byref from a TypedReference, checking that it is the expected type

        CORINFO_HELP_ARRADDR_ST,        // assign to element of object array with type-checking
        CORINFO_HELP_LDELEMA_REF,       // does a precise type comparision and returns address

        /* Exceptions */

        CORINFO_HELP_THROW,             // Throw an exception object
        CORINFO_HELP_RETHROW,           // Rethrow the currently active exception
        CORINFO_HELP_USER_BREAKPOINT,   // For a user program to break to the debugger
        CORINFO_HELP_RNGCHKFAIL,        // array bounds check failed
        CORINFO_HELP_OVERFLOW,          // throw an overflow exception
        CORINFO_HELP_THROWDIVZERO,      // throw a divide by zero exception
        CORINFO_HELP_THROWNULLREF,      // throw a null reference exception

        CORINFO_HELP_INTERNALTHROW,     // Support for really fast jit
        CORINFO_HELP_VERIFICATION,      // Throw a VerificationException
        CORINFO_HELP_SEC_UNMGDCODE_EXCPT, // throw a security unmanaged code exception
        CORINFO_HELP_FAIL_FAST,         // Kill the process avoiding any exceptions or stack and data dependencies (use for GuardStack unsafe buffer checks)

        CORINFO_HELP_METHOD_ACCESS_EXCEPTION,//Throw an access exception due to a failed member/class access check.
        CORINFO_HELP_FIELD_ACCESS_EXCEPTION,
        CORINFO_HELP_CLASS_ACCESS_EXCEPTION,

        CORINFO_HELP_ENDCATCH,          // call back into the EE at the end of a catch block

        /* Synchronization */

        CORINFO_HELP_MON_ENTER,
        CORINFO_HELP_MON_EXIT,
        CORINFO_HELP_MON_ENTER_STATIC,
        CORINFO_HELP_MON_EXIT_STATIC,

        CORINFO_HELP_GETCLASSFROMMETHODPARAM, // Given a generics method handle, returns a class handle
        CORINFO_HELP_GETSYNCFROMCLASSHANDLE,  // Given a generics class handle, returns the sync monitor 
                                              // in its ManagedClassObject

        /* Security callout support */

        CORINFO_HELP_SECURITY_PROLOG,   // Required if CORINFO_FLG_SECURITYCHECK is set, or CORINFO_FLG_NOSECURITYWRAP is not set
        CORINFO_HELP_SECURITY_PROLOG_FRAMED, // Slow version of CORINFO_HELP_SECURITY_PROLOG. Used for instrumentation.

        CORINFO_HELP_METHOD_ACCESS_CHECK, // Callouts to runtime security access checks
        CORINFO_HELP_FIELD_ACCESS_CHECK,
        CORINFO_HELP_CLASS_ACCESS_CHECK,

        CORINFO_HELP_DELEGATE_SECURITY_CHECK, // Callout to delegate security transparency check

        /* Verification runtime callout support */

        CORINFO_HELP_VERIFICATION_RUNTIME_CHECK, // Do a Demand for UnmanagedCode permission at runtime

        /* GC support */

        CORINFO_HELP_STOP_FOR_GC,       // Call GC (force a GC)
        CORINFO_HELP_POLL_GC,           // Ask GC if it wants to collect

        CORINFO_HELP_STRESS_GC,         // Force a GC, but then update the JITTED code to be a noop call
        CORINFO_HELP_CHECK_OBJ,         // confirm that ECX is a valid object pointer (debugging only)

        /* GC Write barrier support */

        CORINFO_HELP_ASSIGN_REF,        // universal helpers with F_CALL_CONV calling convention
        CORINFO_HELP_CHECKED_ASSIGN_REF,
        CORINFO_HELP_ASSIGN_REF_ENSURE_NONHEAP,  // Do the store, and ensure that the target was not in the heap.

        CORINFO_HELP_ASSIGN_BYREF,
        CORINFO_HELP_ASSIGN_STRUCT,


        /* Accessing fields */

        // For COM object support (using COM get/set routines to update object)
        // and EnC and cross-context support
        CORINFO_HELP_GETFIELD8,
        CORINFO_HELP_SETFIELD8,
        CORINFO_HELP_GETFIELD16,
        CORINFO_HELP_SETFIELD16,
        CORINFO_HELP_GETFIELD32,
        CORINFO_HELP_SETFIELD32,
        CORINFO_HELP_GETFIELD64,
        CORINFO_HELP_SETFIELD64,
        CORINFO_HELP_GETFIELDOBJ,
        CORINFO_HELP_SETFIELDOBJ,
        CORINFO_HELP_GETFIELDSTRUCT,
        CORINFO_HELP_SETFIELDSTRUCT,
        CORINFO_HELP_GETFIELDFLOAT,
        CORINFO_HELP_SETFIELDFLOAT,
        CORINFO_HELP_GETFIELDDOUBLE,
        CORINFO_HELP_SETFIELDDOUBLE,

        CORINFO_HELP_GETFIELDADDR,

        CORINFO_HELP_GETSTATICFIELDADDR_CONTEXT,    // Helper for context-static fields
        CORINFO_HELP_GETSTATICFIELDADDR_TLS,        // Helper for PE TLS fields

        // There are a variety of specialized helpers for accessing static fields. The JIT should use 
        // ICorClassInfo::getSharedStaticsOrCCtorHelper to determine which helper to use

        // Helpers for regular statics
        CORINFO_HELP_GETGENERICS_GCSTATIC_BASE,
        CORINFO_HELP_GETGENERICS_NONGCSTATIC_BASE,
        CORINFO_HELP_GETSHARED_GCSTATIC_BASE,
        CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE,
        CORINFO_HELP_GETSHARED_GCSTATIC_BASE_NOCTOR,
        CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE_NOCTOR,
        CORINFO_HELP_GETSHARED_GCSTATIC_BASE_DYNAMICCLASS,
        CORINFO_HELP_GETSHARED_NONGCSTATIC_BASE_DYNAMICCLASS,
        // Helper to class initialize shared generic with dynamicclass, but not get static field address
        CORINFO_HELP_CLASSINIT_SHARED_DYNAMICCLASS,

        // Helpers for thread statics
        CORINFO_HELP_GETGENERICS_GCTHREADSTATIC_BASE,
        CORINFO_HELP_GETGENERICS_NONGCTHREADSTATIC_BASE,
        CORINFO_HELP_GETSHARED_GCTHREADSTATIC_BASE,
        CORINFO_HELP_GETSHARED_NONGCTHREADSTATIC_BASE,
        CORINFO_HELP_GETSHARED_GCTHREADSTATIC_BASE_NOCTOR,
        CORINFO_HELP_GETSHARED_NONGCTHREADSTATIC_BASE_NOCTOR,
        CORINFO_HELP_GETSHARED_GCTHREADSTATIC_BASE_DYNAMICCLASS,
        CORINFO_HELP_GETSHARED_NONGCTHREADSTATIC_BASE_DYNAMICCLASS,

        /* Debugger */

        CORINFO_HELP_DBG_IS_JUST_MY_CODE,    // Check if this is "JustMyCode" and needs to be stepped through.

        /* Profiling enter/leave probe addresses */
        CORINFO_HELP_PROF_FCN_ENTER,        // record the entry to a method (caller)
        CORINFO_HELP_PROF_FCN_LEAVE,        // record the completion of current method (caller)
        CORINFO_HELP_PROF_FCN_TAILCALL,     // record the completionof current method through tailcall (caller)

        /* Miscellaneous */

        CORINFO_HELP_BBT_FCN_ENTER,         // record the entry to a method for collecting Tuning data

        CORINFO_HELP_PINVOKE_CALLI,         // Indirect pinvoke call
        CORINFO_HELP_TAILCALL,              // Perform a tail call

        CORINFO_HELP_GETCURRENTMANAGEDTHREADID,

        CORINFO_HELP_INIT_PINVOKE_FRAME,   // initialize an inlined PInvoke Frame for the JIT-compiler

        CORINFO_HELP_MEMSET,                // Init block of memory
        CORINFO_HELP_MEMCPY,                // Copy block of memory

        CORINFO_HELP_RUNTIMEHANDLE_METHOD,          // determine a type/field/method handle at run-time
        CORINFO_HELP_RUNTIMEHANDLE_METHOD_LOG,      // determine a type/field/method handle at run-time, with IBC logging
        CORINFO_HELP_RUNTIMEHANDLE_CLASS,           // determine a type/field/method handle at run-time
        CORINFO_HELP_RUNTIMEHANDLE_CLASS_LOG,       // determine a type/field/method handle at run-time, with IBC logging

        // These helpers are required for MDIL backward compatibility only. They are not used by current JITed code.
        CORINFO_HELP_TYPEHANDLE_TO_RUNTIMETYPEHANDLE_OBSOLETE, // Convert from a TypeHandle (native structure pointer) to RuntimeTypeHandle at run-time
        CORINFO_HELP_METHODDESC_TO_RUNTIMEMETHODHANDLE_OBSOLETE, // Convert from a MethodDesc (native structure pointer) to RuntimeMethodHandle at run-time
        CORINFO_HELP_FIELDDESC_TO_RUNTIMEFIELDHANDLE_OBSOLETE, // Convert from a FieldDesc (native structure pointer) to RuntimeFieldHandle at run-time

        CORINFO_HELP_TYPEHANDLE_TO_RUNTIMETYPE, // Convert from a TypeHandle (native structure pointer) to RuntimeType at run-time
        CORINFO_HELP_TYPEHANDLE_TO_RUNTIMETYPE_MAYBENULL, // Convert from a TypeHandle (native structure pointer) to RuntimeType at run-time, the type may be null
        CORINFO_HELP_METHODDESC_TO_STUBRUNTIMEMETHOD, // Convert from a MethodDesc (native structure pointer) to RuntimeMethodHandle at run-time
        CORINFO_HELP_FIELDDESC_TO_STUBRUNTIMEFIELD, // Convert from a FieldDesc (native structure pointer) to RuntimeFieldHandle at run-time

        CORINFO_HELP_VIRTUAL_FUNC_PTR,      // look up a virtual method at run-time
                                            //CORINFO_HELP_VIRTUAL_FUNC_PTR_LOG,  // look up a virtual method at run-time, with IBC logging

        // Not a real helpers. Instead of taking handle arguments, these helpers point to a small stub that loads the handle argument and calls the static helper.
        CORINFO_HELP_READYTORUN_NEW,
        CORINFO_HELP_READYTORUN_NEWARR_1,
        CORINFO_HELP_READYTORUN_ISINSTANCEOF,
        CORINFO_HELP_READYTORUN_CHKCAST,
        CORINFO_HELP_READYTORUN_STATIC_BASE,
        CORINFO_HELP_READYTORUN_VIRTUAL_FUNC_PTR,
        CORINFO_HELP_READYTORUN_GENERIC_HANDLE,
        CORINFO_HELP_READYTORUN_DELEGATE_CTOR,
        CORINFO_HELP_READYTORUN_GENERIC_STATIC_BASE,

        CORINFO_HELP_EE_PRESTUB,            // Not real JIT helper. Used in native images.

        CORINFO_HELP_EE_PRECODE_FIXUP,      // Not real JIT helper. Used for Precode fixup in native images.
        CORINFO_HELP_EE_PINVOKE_FIXUP,      // Not real JIT helper. Used for PInvoke target fixup in native images.
        CORINFO_HELP_EE_VSD_FIXUP,          // Not real JIT helper. Used for VSD cell fixup in native images.
        CORINFO_HELP_EE_EXTERNAL_FIXUP,     // Not real JIT helper. Used for to fixup external method thunks in native images.
        CORINFO_HELP_EE_VTABLE_FIXUP,       // Not real JIT helper. Used for inherited vtable slot fixup in native images.

        CORINFO_HELP_EE_REMOTING_THUNK,     // Not real JIT helper. Used for remoting precode in native images.

        CORINFO_HELP_EE_PERSONALITY_ROUTINE,// Not real JIT helper. Used in native images.
        CORINFO_HELP_EE_PERSONALITY_ROUTINE_FILTER_FUNCLET,// Not real JIT helper. Used in native images to detect filter funclets.

        // ASSIGN_REF_EAX - CHECKED_ASSIGN_REF_EBP: NOGC_WRITE_BARRIERS JIT helper calls
        //
        // For unchecked versions EDX is required to point into GC heap.
        //
        // NOTE: these helpers are only used for x86.
        CORINFO_HELP_ASSIGN_REF_EAX,    // EAX holds GC ptr, do a 'mov [EDX], EAX' and inform GC
        CORINFO_HELP_ASSIGN_REF_EBX,    // EBX holds GC ptr, do a 'mov [EDX], EBX' and inform GC
        CORINFO_HELP_ASSIGN_REF_ECX,    // ECX holds GC ptr, do a 'mov [EDX], ECX' and inform GC
        CORINFO_HELP_ASSIGN_REF_ESI,    // ESI holds GC ptr, do a 'mov [EDX], ESI' and inform GC
        CORINFO_HELP_ASSIGN_REF_EDI,    // EDI holds GC ptr, do a 'mov [EDX], EDI' and inform GC
        CORINFO_HELP_ASSIGN_REF_EBP,    // EBP holds GC ptr, do a 'mov [EDX], EBP' and inform GC

        CORINFO_HELP_CHECKED_ASSIGN_REF_EAX,  // These are the same as ASSIGN_REF above ...
        CORINFO_HELP_CHECKED_ASSIGN_REF_EBX,  // ... but also check if EDX points into heap.
        CORINFO_HELP_CHECKED_ASSIGN_REF_ECX,
        CORINFO_HELP_CHECKED_ASSIGN_REF_ESI,
        CORINFO_HELP_CHECKED_ASSIGN_REF_EDI,
        CORINFO_HELP_CHECKED_ASSIGN_REF_EBP,

        CORINFO_HELP_LOOP_CLONE_CHOICE_ADDR, // Return the reference to a counter to decide to take cloned path in debug stress.
        CORINFO_HELP_DEBUG_LOG_LOOP_CLONING, // Print a message that a loop cloning optimization has occurred in debug mode.

        CORINFO_HELP_THROW_ARGUMENTEXCEPTION,           // throw ArgumentException
        CORINFO_HELP_THROW_ARGUMENTOUTOFRANGEEXCEPTION, // throw ArgumentOutOfRangeException
        CORINFO_HELP_THROW_PLATFORM_NOT_SUPPORTED,      // throw PlatformNotSupportedException
        CORINFO_HELP_THROW_TYPE_NOT_SUPPORTED,          // throw TypeNotSupportedException

        CORINFO_HELP_JIT_PINVOKE_BEGIN, // Transition to preemptive mode before a P/Invoke, frame is the first argument
        CORINFO_HELP_JIT_PINVOKE_END,   // Transition to cooperative mode after a P/Invoke, frame is the first argument

        CORINFO_HELP_JIT_REVERSE_PINVOKE_ENTER, // Transition to cooperative mode in reverse P/Invoke prolog, frame is the first argument
        CORINFO_HELP_JIT_REVERSE_PINVOKE_EXIT,  // Transition to preemptive mode in reverse P/Invoke epilog, frame is the first argument

        CORINFO_HELP_GVMLOOKUP_FOR_SLOT,        // Resolve a generic virtual method target from this pointer and runtime method handle 

        CORINFO_HELP_COUNT,
    }

    public enum CorInfoIsAccessAllowedResult
    {
        CORINFO_ACCESS_ALLOWED = 0,           // Call allowed
        CORINFO_ACCESS_ILLEGAL = 1,           // Call not allowed
        CORINFO_ACCESS_RUNTIME_CHECK = 2,     // Ask at runtime whether to allow the call or not
    }

    public enum CorinfoThisTransform
    {
        CORINFO_NO_THIS_TRANSFORM,
        CORINFO_BOX_THIS,
        CORINFO_DEREF_THIS
    }

    public enum CorinfoCallKind
    {
        CORINFO_CALL,
        CORINFO_CALL_CODE_POINTER,
        CORINFO_VIRTUALCALL_STUB,
        CORINFO_VIRTUALCALL_LDVIRTFTN,
        CORINFO_VIRTUALCALL_VTABLE
    }

    //CORINFO_CONST_LOOKUP
    [StructLayout(layoutKind: LayoutKind.Explicit)]
    public unsafe struct CorinfoConstLookup
    {
        // If the handle is obtained at compile-time, then this handle is the "exact" handle (class, method, or field)
        // Otherwise, it's a representative... 
        // If accessType is
        //     IAT_VALUE   --> "handle" stores the real handle or "addr " stores the computed address
        //     IAT_PVALUE  --> "addr" stores a pointer to a location which will hold the real handle
        //     IAT_PPVALUE --> "addr" stores a double indirection to a location which will hold the real handle
        [FieldOffset(0)]
        public InfoAccessType accessType;
        [FieldOffset(sizeof(Int32))]
        public IntPtr handle;
        [FieldOffset(sizeof(Int32))]
        public void* addr;
        
    }

    // Can a value be accessed directly from JITed code.
    public enum InfoAccessType
    {
        IAT_VALUE,      // The info value is directly available
        IAT_PVALUE,     // The value needs to be accessed via an       indirection
        IAT_PPVALUE     // The value needs to be accessed via a double indirection
    }

    // Result of calling embedGenericHandle
    //CORINFO_LOOKUP
    [StructLayout(layoutKind: LayoutKind.Explicit)]
    public unsafe struct CorinfoLookup
    {
        [FieldOffset(0)]
        public CORINFO_LOOKUP_KIND lookupKind;

        // If kind.needsRuntimeLookup then this indicates how to do the lookup
        [FieldOffset(sizeof(bool) + sizeof(UInt32) + sizeof(UInt16) +
#if _TARGET_X64_
            4
#else 
            2
#endif
            )]
        public CorinfoRuntimeLookup runtimeLookup;

        // If the handle is obtained at compile-time, then this handle is the "exact" handle (class, method, or field)
        // Otherwise, it's a representative...  If accessType is
        //     IAT_VALUE --> "handle" stores the real handle or "addr " stores the computed address
        //     IAT_PVALUE --> "addr" stores a pointer to a location which will hold the real handle
        //     IAT_PPVALUE --> "addr" stores a double indirection to a location which will hold the real handle
        [FieldOffset(sizeof(bool) + sizeof(UInt32) + sizeof(UInt16) +
#if _TARGET_X64_
            4
#else 
            2
#endif
            )]
        public CorinfoConstLookup constLookup;
    }

    //CORINFO_LOOKUP_KIND
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public unsafe struct CORINFO_LOOKUP_KIND
    {
        public bool needsRuntimeLookup;
        public CorinfoRuntimeLookupKind runtimeLookupKind;

        // The 'runtimeLookupFlags' and 'runtimeLookupArgs' fields
        // are just for internal VM / ZAP communication, not to be used by the JIT.
        public UInt16 runtimeLookupFlags;
        public void* runtimeLookupArgs;
    }

    public enum CorinfoRuntimeLookupKind
    {
        CORINFO_LOOKUP_THISOBJ,
        CORINFO_LOOKUP_METHODPARAM,
        CORINFO_LOOKUP_CLASSPARAM,
    }

    //CORINFO_RUNTIME_LOOKUP
    [StructLayout(layoutKind: LayoutKind.Sequential)]
    public unsafe struct CorinfoRuntimeLookup
    {
        // This is signature you must pass back to the runtime lookup helper
        public void* signature;

        // Here is the helper you must call. It is one of CORINFO_HELP_RUNTIMEHANDLE_* helpers.
        public CorInfoHelpFunc helper;

        // Number of indirections to get there
        // CORINFO_USEHELPER = don't know how to get it, so use helper function at run-time instead
        // 0 = use the this pointer itself (e.g. token is C<!0> inside code in sealed class C)
        //     or method desc itself (e.g. token is method void M::mymeth<!!0>() inside code in M::mymeth)
        // Otherwise, follow each byte-offset stored in the "offsets[]" array (may be negative)
        public UInt16 indirections;

        // If set, test for null and branch to helper if null
        public bool testForNull;

        // If set, test the lowest bit and dereference if set (see code:FixupPointer)
        public bool testForFixup;

        public IntPtr offsets; //UInt32[#define CORINFO_MAXINDIRECTIONS 4]

        // If set, first offset is indirect.
        // 0 means that value stored at first offset (offsets[0]) from pointer is next pointer, to which the next offset
        // (offsets[1]) is added and so on.
        // 1 means that value stored at first offset (offsets[0]) from pointer is offset1, and the next pointer is
        // stored at pointer+offsets[0]+offset1.
        public bool indirectFirstOffset;

        // If set, second offset is indirect.
        // 0 means that value stored at second offset (offsets[1]) from pointer is next pointer, to which the next offset
        // (offsets[2]) is added and so on.
        // 1 means that value stored at second offset (offsets[1]) from pointer is offset2, and the next pointer is
        // stored at pointer+offsets[1]+offset2.
        public bool indirectSecondOffset;
    }
}
