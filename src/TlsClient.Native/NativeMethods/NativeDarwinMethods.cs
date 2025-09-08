using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace TlsClient.Native.NativeMethods
{
    public static class NativeDarwinMethods
    {
        [Flags]
        public enum LoadLibraryFlags
        {
            None = 0,
            Lazy = 0x01,
            Now = 0x02,
            Local = 0x04,
            Global = 0x08,
            NoLoad = 0x10,
            NoDelete = 0x80
        }

        [DllImport("libdl.dylib", EntryPoint = "dlopen")]
        public static extern IntPtr LoadLibrary(
            [In][MarshalAs(UnmanagedType.LPStr)] string path,
            [In] LoadLibraryFlags flags = LoadLibraryFlags.Now | LoadLibraryFlags.Global
        );

        [DllImport("libdl.dylib", EntryPoint = "dlclose")]
        public static extern int FreeLibrary(
            [In] IntPtr hLibrary
        );

        [DllImport("libdl.dylib", EntryPoint = "dlsym")]
        public static extern IntPtr GetProcAddress([In]IntPtr handle, [In]string symbol);
    }
}
