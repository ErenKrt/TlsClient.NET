using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TlsClient.Native.NativeMethods
{
    public static class NativeWindowsMethods
    {

        [DllImport("kernel32.dll", ExactSpelling = true, SetLastError = true, EntryPoint = "LoadLibraryW")]
        public static extern IntPtr LoadLibrary([In][MarshalAs(UnmanagedType.LPWStr)] string path);

        [DllImport("kernel32.dll", SetLastError = true)]
        public static extern IntPtr GetProcAddress([In] IntPtr hModule, [In] string procName);

        [DllImport("kernel32.dll", SetLastError = true, EntryPoint = "FreeLibrary")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary([In] IntPtr hLibrary);

    }
}
