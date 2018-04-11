using ClrAnalyzer.Core.Utils;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using static ClrAnalyzer.Core.Compiler.CorJitCompiler;

namespace ClrAnalyzer.Core.Hooks
{
    public unsafe class CompilerHook
    {
        public CompileMethodDel Compile = null;

        private IntPtr pJit;
        private IntPtr pVTable;
        private bool isHooked = false;
        private readonly CorJitCompilerNative compiler;
        private uint lpflOldProtect;

        public CompilerHook()
        {
            if (pJit == IntPtr.Zero) pJit = GetJit();
            Debug.Assert(pJit != null);
            compiler = Marshal.PtrToStructure<CorJitCompilerNative>(Marshal.ReadIntPtr(pJit));
            Debug.Assert(compiler.CompileMethod != null);
            pVTable = Marshal.ReadIntPtr(pJit);
            
            RuntimeHelpers.PrepareMethod(GetType().GetMethod("RemoveHook").MethodHandle);
            RuntimeHelpers.PrepareMethod(GetType().GetMethod("LockpVTable", System.Reflection.BindingFlags.Instance|System.Reflection.BindingFlags.NonPublic).MethodHandle);            
        }
        
        private bool UnlockpVTable()
        {
            if (!Win32MemoryUtils.VirtualProtect(pVTable, (uint)IntPtr.Size, Win32MemoryUtils.MemoryProtectionConstants.PAGE_EXECUTE_READWRITE, out lpflOldProtect))
            {
                Console.WriteLine(new Win32Exception(Marshal.GetLastWin32Error()).Message);
                return false;
            }
            return true;
        }

        private bool LockpVTable()
        {
            return Win32MemoryUtils.VirtualProtect(pVTable, (uint)IntPtr.Size, (Win32MemoryUtils.MemoryProtectionConstants)lpflOldProtect, out lpflOldProtect);
        }

        public bool Hook(CompileMethodDel hook)
        {
            if (!UnlockpVTable()) return false;

            Compile = compiler.CompileMethod;
            Debug.Assert(Compile != null);

            RuntimeHelpers.PrepareDelegate(hook);
            RuntimeHelpers.PrepareDelegate(Compile);

            Marshal.WriteIntPtr(pVTable, Marshal.GetFunctionPointerForDelegate(hook));

            return isHooked = LockpVTable();
        }

        public bool RemoveHook()
        {
            if (!isHooked) throw new InvalidOperationException("Impossible unhook not hooked compiler");
            if (!UnlockpVTable()) return false;

            Marshal.WriteIntPtr(pVTable, Marshal.GetFunctionPointerForDelegate(Compile));

            return LockpVTable();
        }
    }
}
