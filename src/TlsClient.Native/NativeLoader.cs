using System;
using System.IO;
using System.Runtime.InteropServices;
using TlsClient.Native.NativeMethods;

namespace TlsClient.Native
{
    public class NativeLoader
    {
        private static readonly string Platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
                             RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                             RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin" :
                             throw new PlatformNotSupportedException("Unsupported OS platform");

        private static readonly string Extension = Platform switch
        {
            "win" => "dll",
            "linux" => "so",
            "darwin" => "dylib",
            _ => throw new PlatformNotSupportedException("Unsupported OS platform")
        };

        private static readonly string BaseArch = RuntimeInformation.ProcessArchitecture switch
        {
            Architecture.X64 => "x64",
            Architecture.X86 => "x86",
            Architecture.Arm => "arm",
            Architecture.Arm64 => "arm64",
            _ => throw new PlatformNotSupportedException("Unsupported process architecture")
        };

        public static string GetLibraryPath()
        {
            string platform = Platform;
            string arch = BaseArch;

            if (platform == "linux")
            {
                string distro = NativeLinuxMethods.GetLinuxDistro() ?? "UNKNOWN";
                arch = arch switch
                {
                    "x64" => "amd64",
                    "x86" => "i386",
                    "arm" => "armhf",
                    "arm64" => "aarch64",
                    _ => arch
                };

                if (!distro.Equals("UNKNOWN", StringComparison.OrdinalIgnoreCase))
                {
                    platform = $"{platform}-{distro}";
                    arch = arch.Replace("x", string.Empty);
                }
            }

            return Path.GetFullPath($"runtimes/tls-client/{platform}/{arch}/tls-client.{Extension}");
        }

        public static IntPtr LoadNativeAssembly(string libraryPath)
        {
            if (!File.Exists(libraryPath))
            {
                throw new DllNotFoundException($"The native library '{libraryPath}' was not found.");
            }

            if (Platform == "win")
            {
                return NativeWindowsMethods.LoadLibrary(libraryPath);
            }
            else if (Platform == "linux" || Platform == "linux-ubuntu" || Platform == "linux-alpine")
            {
                return NativeLinuxMethods.LoadLibrary(libraryPath);
            }
            else if (Platform == "darwin")
            {
                return NativeDarwinMethods.LoadLibrary(libraryPath);
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform");
            }

        }


        public static bool FreeNativeAssembly(IntPtr libraryHandle)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? NativeWindowsMethods.FreeLibrary(libraryHandle) :
                   RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? NativeLinuxMethods.FreeLibrary(libraryHandle) == 0 :
                   RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? NativeDarwinMethods.FreeLibrary(libraryHandle) == 0 :
                   throw new PlatformNotSupportedException("Unsupported OS platform");
        }


        public static IntPtr GetProcAddress(IntPtr handle, string name)
        {
            return RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? NativeWindowsMethods.GetProcAddress(handle, name) :
                   RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? NativeLinuxMethods.GetProcAddress(handle, name) :
                   RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? NativeDarwinMethods.GetProcAddress(handle, name) :
                   throw new PlatformNotSupportedException("Unsupported OS platform");
        }
    }
}
