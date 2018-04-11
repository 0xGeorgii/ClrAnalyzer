using System;
using System.Runtime.InteropServices;

namespace ClrAnalyzer.Core.Utils
{
    public class Win32MemoryUtils
    {
        /// <summary>
        /// From Windows.h
        /// https://msdn.microsoft.com/ru-ru/library/windows/desktop/aa366786(v=vs.85).aspx
        /// </summary>
        public enum MemoryProtectionConstants
        {
            PAGE_EXECUTE = 0x10,
            PAGE_EXECUTE_READ = 0x20,
            PAGE_EXECUTE_READWRITE = 0x40,
            PAGE_EXECUTE_WRITECOPY = 0x80,
            PAGE_NOACCESS = 0x01,
            PAGE_READONLY = 0x02,
            PAGE_READWRITE = 0x04,
            PAGE_WRITECOPY = 0x08,
            PAGE_TARGETS_INVALID = 0x40000000,
            PAGE_TARGETS_NO_UPDATE = 0x40000000,
            PAGE_GUARD = 0x100,
            PAGE_NOCACHE = 0x200,
            PAGE_WRITECOMBINE = 0x400
        }

        /// <summary>
        /// From Windows.h
        /// https://msdn.microsoft.com/ru-ru/library/windows/desktop/aa366898(v=vs.85).aspx
        /// </summary>
        [DllImport("kernel32.dll", BestFitMapping = true, CallingConvention = CallingConvention.Winapi, SetLastError = true, ExactSpelling = true)]
        public static extern bool VirtualProtect(IntPtr lpAddress, UInt32 dwSize, MemoryProtectionConstants flNewProtect, out UInt32 lpflOldProtect);
    }
}
