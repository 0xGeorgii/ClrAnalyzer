#include "clrtypes.h"
#include "corinfo.h"
#include "jitee.h"
#include "corjitflags.h"
#include "ildump.h"
#include <iostream>
#include <iomanip>
#include <string>

#define NO_ERROR            0L
#define SEVERITY_SUCCESS    0
#define SEVERITY_ERROR      1
#define FACILITY_NULL       0

#define MAKE_HRESULT(sev,fac,code) \
    ((HRESULT) (((ULONG)(sev)<<31) | ((ULONG)(fac)<<16) | ((ULONG)(code))) )

typedef unsigned __int8 COR_SIGNATURE;
typedef const COR_SIGNATURE* PCCOR_SIGNATURE;

enum CorJitResult
{
    // Note that I dont use FACILITY_NULL for the facility number,
    // we may want to get a 'real' facility number
    CORJIT_OK = NO_ERROR,
    CORJIT_BADCODE = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_NULL, 1),
    CORJIT_OUTOFMEM = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_NULL, 2),
    CORJIT_INTERNALERROR = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_NULL, 3),
    CORJIT_SKIPPED = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_NULL, 4),
    CORJIT_RECOVERABLEERROR = MAKE_HRESULT(SEVERITY_ERROR, FACILITY_NULL, 5),
};

typedef struct CORINFO_METHOD_STRUCT_*      CORJIT_CORINFO_METHOD_HANDLE;
typedef struct CORINFO_MODULE_STRUCT_*      CORINFO_MODULE_HANDLE;
typedef struct CORINFO_CLASS_STRUCT_*       CORINFO_CLASS_HANDLE;
typedef struct CORINFO_ARG_LIST_STRUCT_*    CORINFO_ARG_LIST_HANDLE;    // represents a list of argument types

enum CorJitFuncKind
{
    CORJIT_FUNC_ROOT,          // The main/root function (always id==0)
    CORJIT_FUNC_HANDLER,       // a funclet associated with an EH handler (finally, fault, catch, filter handler)
    CORJIT_FUNC_FILTER         // a funclet associated with an EH filter
};

enum CorJitAllocMemFlag
{
    CORJIT_ALLOCMEM_DEFAULT_CODE_ALIGN = 0x00000000, // The code will be use the normal alignment
    CORJIT_ALLOCMEM_FLG_16BYTE_ALIGN = 0x00000001, // The code will be 16-byte aligned
    CORJIT_ALLOCMEM_FLG_RODATA_16BYTE_ALIGN = 0x00000002, // The read-only data will be 16-byte aligned
};

class ICorJitInfo : public ICorDynamicInfo
{
public:
    // return memory manager that the JIT can use to allocate a regular memory
    virtual void* getMemoryManager() = 0;

    // get a block of memory for the code, readonly data, and read-write data
    virtual void allocMem(
        ULONG               hotCodeSize,    /* IN */
        ULONG               coldCodeSize,   /* IN */
        ULONG               roDataSize,     /* IN */
        ULONG               xcptnsCount,    /* IN */
        CorJitAllocMemFlag  flag,           /* IN */
        void **             hotCodeBlock,   /* OUT */
        void **             coldCodeBlock,  /* OUT */
        void **             roDataBlock     /* OUT */
    ) = 0;

    // Reserve memory for the method/funclet's unwind information.
    // Note that this must be called before allocMem. It should be
    // called once for the main method, once for every funclet, and
    // once for every block of cold code for which allocUnwindInfo
    // will be called.
    //
    // This is necessary because jitted code must allocate all the
    // memory needed for the unwindInfo at the allocMem call.
    // For prejitted code we split up the unwinding information into
    // separate sections .rdata and .pdata.
    //
    virtual void reserveUnwindInfo(
        BOOL                isFunclet,             /* IN */
        BOOL                isColdCode,            /* IN */
        ULONG               unwindSize             /* IN */
    ) = 0;

    // Allocate and initialize the .rdata and .pdata for this method or
    // funclet, and get the block of memory needed for the machine-specific
    // unwind information (the info for crawling the stack frame).
    // Note that allocMem must be called first.
    //
    // Parameters:
    //
    //    pHotCode        main method code buffer, always filled in
    //    pColdCode       cold code buffer, only filled in if this is cold code, 
    //                      null otherwise
    //    startOffset     start of code block, relative to appropriate code buffer
    //                      (e.g. pColdCode if cold, pHotCode if hot).
    //    endOffset       end of code block, relative to appropriate code buffer
    //    unwindSize      size of unwind info pointed to by pUnwindBlock
    //    pUnwindBlock    pointer to unwind info
    //    funcKind        type of funclet (main method code, handler, filter)
    //
    virtual void allocUnwindInfo(
        BYTE *              pHotCode,              /* IN */
        BYTE *              pColdCode,             /* IN */
        ULONG               startOffset,           /* IN */
        ULONG               endOffset,             /* IN */
        ULONG               unwindSize,            /* IN */
        BYTE *              pUnwindBlock,          /* IN */
        CorJitFuncKind      funcKind               /* IN */
    ) = 0;

    // Get a block of memory needed for the code manager information,
    // (the info for enumerating the GC pointers while crawling the
    // stack frame).
    // Note that allocMem must be called first
    virtual void * allocGCInfo(
        size_t                  size        /* IN */
    ) = 0;

    virtual void yieldExecution() = 0;

    // Indicate how many exception handler blocks are to be returned.
    // This is guaranteed to be called before any 'setEHinfo' call.
    // Note that allocMem must be called before this method can be called.
    virtual void setEHcount(
        unsigned                cEH          /* IN */
    ) = 0;

    // Set the values for one particular exception handler block.
    //
    // Handler regions should be lexically contiguous.
    // This is because FinallyIsUnwinding() uses lexicality to
    // determine if a "finally" clause is executing.
    virtual void setEHinfo(
        unsigned                 EHnumber,   /* IN  */
        const CORINFO_EH_CLAUSE *clause      /* IN */
    ) = 0;

    // Level -> fatalError, Level 2 -> Error, Level 3 -> Warning
    // Level 4 means happens 10 times in a run, level 5 means 100, level 6 means 1000 ...
    // returns non-zero if the logging succeeded
    virtual BOOL logMsg(unsigned level, const char* fmt, va_list args) = 0;

    // do an assert.  will return true if the code should retry (DebugBreak)
    // returns false, if the assert should be igored.
    virtual int doAssert(const char* szFile, int iLine, const char* szExpr) = 0;

    virtual void reportFatalError(CorJitResult result) = 0;

    struct ProfileBuffer  // Also defined here: code:CORBBTPROF_BLOCK_DATA
    {
        ULONG ILOffset;
        ULONG ExecutionCount;
    };

    // allocate a basic block profile buffer where execution counts will be stored
    // for jitted basic blocks.
    virtual HRESULT allocBBProfileBuffer(
        ULONG                 count,           // The number of basic blocks that we have
        ProfileBuffer **      profileBuffer
    ) = 0;

    // get profile information to be used for optimizing the current method.  The format
    // of the buffer is the same as the format the JIT passes to allocBBProfileBuffer.
    virtual HRESULT getBBProfileData(
        CORJIT_CORINFO_METHOD_HANDLE ftnHnd,
        ULONG *               count,           // The number of basic blocks that we have
        ProfileBuffer **      profileBuffer,
        ULONG *               numRuns
    ) = 0;

    // Associates a native call site, identified by its offset in the native code stream, with
    // the signature information and method handle the JIT used to lay out the call site. If
    // the call site has no signature information (e.g. a helper call) or has no method handle
    // (e.g. a CALLI P/Invoke), then null should be passed instead.
    virtual void recordCallSite(
        ULONG                 instrOffset,  /* IN */
        CORINFO_SIG_INFO *    callSig,      /* IN */
        CORJIT_CORINFO_METHOD_HANDLE methodHandle  /* IN */
    ) = 0;

    // A relocation is recorded if we are pre-jitting.
    // A jump thunk may be inserted if we are jitting
    virtual void recordRelocation(
        void *                 location,   /* IN  */
        void *                 target,     /* IN  */
        WORD                   fRelocType, /* IN  */
        WORD                   slotNum = 0,  /* IN  */
        INT32                  addlDelta = 0 /* IN  */
    ) = 0;

    virtual WORD getRelocTypeHint(void * target) = 0;

    // A callback to identify the range of address known to point to
    // compiler-generated native entry points that call back into
    // MSIL.
    virtual void getModuleNativeEntryPointRange(
        void ** pStart, /* OUT */
        void ** pEnd    /* OUT */
    ) = 0;

    // For what machine does the VM expect the JIT to generate code? The VM
    // returns one of the IMAGE_FILE_MACHINE_* values. Note that if the VM
    // is cross-compiling (such as the case for crossgen), it will return a
    // different value than if it was compiling for the host architecture.
    // 
    virtual DWORD getExpectedTargetArchitecture() = 0;

    // Fetches extended flags for a particular compilation instance. Returns
    // the number of bytes written to the provided buffer.
    virtual DWORD getJitFlags(
        CORJIT_FLAGS* flags,       /* IN: Points to a buffer that will hold the extended flags. */
        DWORD        sizeInBytes   /* IN: The size of the buffer. Note that this is effectively a
                                   version number for the CORJIT_FLAGS value. */
    ) = 0;
};

#define CompileFunctionSig CorJitResult(*compileMethod)(void*, struct CORINFO_METHOD_INFO*, unsigned flags, BYTE**, ULONG*)

extern "C" __declspec(dllexport) void __stdcall DumpMethodInfo(ICorJitInfo* comp, struct CORINFO_METHOD_INFO* info, unsigned flags, BYTE** nativeEntry, ULONG* nativeSizeOfCode)
{
    std::cout << "native size of code: " << info->ILCodeSize << std::endl;
    auto buff = new char[info->ILCodeSize];
    std::memcpy(buff, info->ILCode, info->ILCodeSize);
    auto code = new std::string(buff);
    std::cout << "IL code: " << code << std::endl;
    std::cout << "===" << std::endl;
    DumpILToConsole(info->ILCode, info->ILCodeSize);
    std::cout << "===" << std::endl;
    delete code;
    delete[] buff;
    auto attribs = comp->getMethodAttribs(info->ftn);
    std::cout << "method attribs: " << std::hex << attribs << std::endl;
    std::cout << "===" << std::endl;

    if (attribs & CorInfoFlag::CORINFO_FLG_PROTECTED)
    {
        std::cout << "\t" << "CORINFO_FLG_PROTECTED" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_STATIC)
    {
        std::cout << "\t" << "CORINFO_FLG_STATIC" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_FINAL)
    {
        std::cout << "\t" << "CORINFO_FLG_FINAL" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_SYNCH)
    {
        std::cout << "\t" << "CORINFO_FLG_SYNCH" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_VIRTUAL)
    {
        std::cout << "\t" << "CORINFO_FLG_VIRTUAL" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_NATIVE)
    {
        std::cout << "\t" << "CORINFO_FLG_NATIVE" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_INTRINSIC_TYPE)
    {
        std::cout << "\t" << "CORINFO_FLG_INTRINSIC_TYPE: This type is marked by [Intrinsic]" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_ABSTRACT)
    {
        std::cout << "\t" << "CORINFO_FLG_ABSTRACT" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_EnC)
    {
        std::cout << "\t" << "CORINFO_FLG_EnC: member was added by Edit'n'Continue" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_FORCEINLINE)
    {
        std::cout << "\t" << "CORINFO_FLG_FORCEINLINE: The method should be inlined if possible" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_SHAREDINST)
    {
        std::cout << "\t" << "CORINFO_FLG_SHAREDINST: the code for this method is shared between different generic instantiations (also set on classes/types)" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_DELEGATE_INVOKE)
    {
        std::cout << "\t" << "CORINFO_FLG_DELEGATE_INVOKE Delegate" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_PINVOKE)
    {
        std::cout << "\t" << "CORINFO_FLG_PINVOKE: Is a P/Invoke call" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_SECURITYCHECK)
    {
        std::cout << "\t" << "CORINFO_FLG_SECURITYCHECK: Is one of the security routines that does a stackwalk (e.g. Assert, Demand)" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_NOGCCHECK)
    {
        std::cout << "\t" << "CORINFO_FLG_NOGCCHECK: This method is FCALL that has no GC check.  Don't put alone in loops" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_INTRINSIC)
    {
        std::cout << "\t" << "CORINFO_FLG_INTRINSIC: This method MAY have an intrinsic ID" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_CONSTRUCTOR)
    {
        std::cout << "\t" << "CORINFO_FLG_CONSTRUCTOR: This method is an instance or type initializer" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_AGGRESSIVE_OPT)
    {
        std::cout << "\t" << "CORINFO_FLG_AGGRESSIVE_OPT: The method may contain hot code and should be aggressively optimized if possible" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_NOSECURITYWRAP)
    {
        std::cout << "\t" << "CORINFO_FLG_NOSECURITYWRAP: The method requires no security checks" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_DONT_INLINE)
    {
        std::cout << "\t" << "CORINFO_FLG_DONT_INLINE: The method should not be inlined" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_DONT_INLINE_CALLER)
    {
        std::cout << "\t" << "CORINFO_FLG_DONT_INLINE_CALLER: The method should not be inlined, nor should its callers. It cannot be tail called" << std::endl;
    }
    if (attribs & CorInfoFlag::CORINFO_FLG_JIT_INTRINSIC)
    {
        std::cout << "\t" << "CORINFO_FLG_JIT_INTRINSIC: Method is a potential jit intrinsic; verify identity by name check" << std::endl;
    }
    std::cout << "===" << std::endl;
}
