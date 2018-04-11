using System;

namespace ClrAnalyzer.Core.Utils
{
    public class Win32IOUtils
    {
        public const int STD_OUTPUT_HANDLE = -11;

        [System.Runtime.InteropServices.DllImport("Kernel32.dll", SetLastError = true)]
        public static extern int SetStdHandle(int device, IntPtr handle);        
    }
}
