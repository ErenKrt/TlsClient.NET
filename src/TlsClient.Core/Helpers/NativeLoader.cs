﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using TlsClient.Core.Helpers.Natives;

namespace TlsClient.Core.Helpers
{
    public class NativeLoader
    {
        
        public static IntPtr LoadNativeAssembly()
        {
            string platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "win" :
                              RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" :
                              RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "darwin" :
                              throw new PlatformNotSupportedException("Unsupported OS platform");

            string extension = platform switch
            {
                "win" => "dll",
                "linux" => "so",
                "darwin" => "dylib",
                _ => throw new PlatformNotSupportedException("Unsupported OS platform")
            };

            string architecture = RuntimeInformation.ProcessArchitecture switch
            {
                Architecture.X64 => "x64",
                Architecture.X86 => "x86",
                Architecture.Arm => "arm",
                Architecture.Arm64 => "arm64",
                _ => throw new PlatformNotSupportedException("Unsupported process architecture")
            };

            string libraryPath = $"runtimes/tls-client/{platform}/{architecture}/tls-client-latest.{extension}";

            if(!File.Exists(libraryPath))
            {
                throw new DllNotFoundException($"The native library '{libraryPath}' was not found.");
            }

            return platform switch
            {
                "win" => NativeWindowsMethods.LoadLibrary(libraryPath),
                "linux" => throw new PlatformNotSupportedException("Linux loading not implemented yet"),
                "osx" => NativeDarwinMethods.LoadLibrary(libraryPath),
                _ => throw new PlatformNotSupportedException("Unsupported OS platform")
            };

        }

        public static bool FreeNativeAssembly(IntPtr libraryHandle)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                return NativeWindowsMethods.FreeLibrary(libraryHandle);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                throw new NotImplementedException("Linux unloading not implemented yet");
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                throw new NotImplementedException("OSX unloading not implemented yet");
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS platform");
            }
        }
    }
}
