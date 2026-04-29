using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace TlsClient.Native.Wrappers
{
    public static class TlsClientWrapper
    {
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr RequestDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate void FreeMemoryDelegate(string sessionID);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr GetCookiesFromSessionDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr AddCookiesToSessionDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr DestroySessionDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr DestroyAllDelegate();

        // Streaming exports (added in tls-client v1.11.2-stream)
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr RequestStreamDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr ReadStreamDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr ReadStreamAllDelegate(byte[] payload);

        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        private delegate IntPtr CancelStreamDelegate(byte[] payload);

        private static bool _isInitialized;

        private static IntPtr _module;
        private static RequestDelegate _requestDelegate = null!;
        private static FreeMemoryDelegate _freeMemoryDelegate = null!;
        private static GetCookiesFromSessionDelegate _getCookiesFromSessionDelegate = null!;
        private static AddCookiesToSessionDelegate _addCookiesToSessionDelegate = null!;
        private static DestroySessionDelegate _destroySessionDelegate = null!;
        private static DestroyAllDelegate _destroyAllDelegate = null!;
        // Nullable: may remain null after Initialize when the loaded native library predates
        // the streaming exports. RequireStreamingDelegate enforces non-null on first use.
        private static RequestStreamDelegate? _requestStreamDelegate;
        private static ReadStreamDelegate? _readStreamDelegate;
        private static ReadStreamAllDelegate? _readStreamAllDelegate;
        private static CancelStreamDelegate? _cancelStreamDelegate;

        public static void Initialize(string? libraryPath = null)
        {
            if (_isInitialized) return;

            libraryPath ??= NativeLoader.GetLibraryPath();

            _module = NativeLoader.LoadNativeAssembly(libraryPath);

            if (_module == IntPtr.Zero)
                throw new DllNotFoundException($"Failed to load native library: {libraryPath}");

            _requestDelegate = GetDelegate<RequestDelegate>("request");
            _freeMemoryDelegate = GetDelegate<FreeMemoryDelegate>("freeMemory");
            _getCookiesFromSessionDelegate = GetDelegate<GetCookiesFromSessionDelegate>("getCookiesFromSession");
            _addCookiesToSessionDelegate = GetDelegate<AddCookiesToSessionDelegate>("addCookiesToSession");
            _destroySessionDelegate = GetDelegate<DestroySessionDelegate>("destroySession");
            _destroyAllDelegate = GetDelegate<DestroyAllDelegate>("destroyAll");

            // Streaming exports — optional. Older native libraries don't expose these,
            // so the lookup is allowed to fail silently. The streaming wrapper methods
            // re-check and throw a clearer error when actually invoked.
            _requestStreamDelegate = TryGetDelegate<RequestStreamDelegate>("requestStream");
            _readStreamDelegate = TryGetDelegate<ReadStreamDelegate>("readStream");
            _readStreamAllDelegate = TryGetDelegate<ReadStreamAllDelegate>("readStreamAll");
            _cancelStreamDelegate = TryGetDelegate<CancelStreamDelegate>("cancelStream");

            _isInitialized = true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
                throw new InvalidOperationException("TlsClientStaticWrapper not initialized. Call Initialize(string libraryPath) first.");
        }

        private static T GetDelegate<T>(string functionName) where T : Delegate
        {
            var functionPtr = NativeLoader.GetProcAddress(_module, functionName);
            if (functionPtr == IntPtr.Zero)
                throw new EntryPointNotFoundException($"Failed to get address of native function '{functionName}'.");

            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }

        private static T? TryGetDelegate<T>(string functionName) where T : Delegate
        {
            var functionPtr = NativeLoader.GetProcAddress(_module, functionName);
            if (functionPtr == IntPtr.Zero)
                return null;

            return Marshal.GetDelegateForFunctionPointer<T>(functionPtr);
        }

        private static T RequireStreamingDelegate<T>(T? del, string functionName) where T : Delegate
        {
            if (del is null)
                throw new EntryPointNotFoundException(
                    $"Native function '{functionName}' is not exported by the loaded tls-client library. " +
                    "Streaming requires a native library built with the streaming exports (requestStream, " +
                    "readStream, readStreamAll, cancelStream). Update your native library.");
            return del;
        }

        private static string ExecuteNative(Func<IntPtr> nativeCall)
        {
            var ptr = nativeCall();
            if (ptr == IntPtr.Zero)
                throw new InvalidOperationException("Native method returned a null pointer.");

            return Marshal.PtrToStringUTF8(ptr) ?? throw new InvalidOperationException("Failed to convert UTF-8 string from native pointer.");
        }

        public static string Request(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            return ExecuteNative(() => _requestDelegate(payload));
        }

        public static void FreeMemory(string sessionId)
        {
            EnsureInitialized();
            if (string.IsNullOrWhiteSpace(sessionId))
                throw new ArgumentException("sessionId cannot be null or empty.", nameof(sessionId));

            _freeMemoryDelegate(sessionId);
        }

        public static string GetCookiesFromSession(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            return ExecuteNative(() => _getCookiesFromSessionDelegate(payload));
        }

        public static string AddCookiesToSession(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            return ExecuteNative(() => _addCookiesToSessionDelegate(payload));
        }

        public static string DestroySession(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            return ExecuteNative(() => _destroySessionDelegate(payload));
        }

        public static string DestroyAll()
        {
            EnsureInitialized();
            return ExecuteNative(() => _destroyAllDelegate());
        }

        public static string RequestStream(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            var del = RequireStreamingDelegate(_requestStreamDelegate, "requestStream");
            return ExecuteNative(() => del(payload));
        }

        public static string ReadStream(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            var del = RequireStreamingDelegate(_readStreamDelegate, "readStream");
            return ExecuteNative(() => del(payload));
        }

        public static string ReadStreamAll(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            var del = RequireStreamingDelegate(_readStreamAllDelegate, "readStreamAll");
            return ExecuteNative(() => del(payload));
        }

        public static string CancelStream(byte[] payload)
        {
            EnsureInitialized();
            if (payload is null) throw new ArgumentNullException(nameof(payload));
            var del = RequireStreamingDelegate(_cancelStreamDelegate, "cancelStream");
            return ExecuteNative(() => del(payload));
        }

        /// <summary>
        /// True when the loaded native library exposes the streaming exports
        /// (<c>requestStream</c>, <c>readStream</c>, <c>readStreamAll</c>, <c>cancelStream</c>).
        /// Older builds of the tls-client library may not expose these, in which case the streaming
        /// methods on <see cref="TlsClient.Native.NativeTlsClient"/> will throw on first use.
        /// </summary>
        public static bool IsStreamingSupported
        {
            get
            {
                EnsureInitialized();
                return _requestStreamDelegate != null
                    && _readStreamDelegate != null
                    && _readStreamAllDelegate != null
                    && _cancelStreamDelegate != null;
            }
        }
        public static void Destroy()
        {
            if (!_isInitialized)
                return;

            _ = DestroyAll();

            var module = _module;
            try
            {
                if (module != IntPtr.Zero)
                {
                    NativeLoader.FreeNativeAssembly(module);
                }
            }
            finally
            {
                _requestDelegate = null!;
                _freeMemoryDelegate = null!;
                _getCookiesFromSessionDelegate = null!;
                _addCookiesToSessionDelegate = null!;
                _destroySessionDelegate = null!;
                _destroyAllDelegate = null!;
                _requestStreamDelegate = null;
                _readStreamDelegate = null;
                _readStreamAllDelegate = null;
                _cancelStreamDelegate = null;

                _module = IntPtr.Zero;

                _isInitialized = false;
            }
        }

    }
}
