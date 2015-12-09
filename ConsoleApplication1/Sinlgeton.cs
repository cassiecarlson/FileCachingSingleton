using ConsoleApplication1;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace FileCachingSingleton
{
    public sealed class Singleton : IDisposable
    {
        private IDictionary<string, string> _fileLookupTable;
        private bool _CacheHasBeenDisposed;

        private MemoryCache _memoryCache;

        private static readonly Lazy<Singleton> _lazy = new Lazy<Singleton>(() => new Singleton());

        /// <summary>
        /// Gets an Instance of the singleton. Ensures that the singleton has a valid MemoryCache.
        /// </summary>
        public static Singleton Instance { get { return _lazy.Value; } }

        private Singleton()
        {
            var hashSet = ConfigurationManager.GetSection("Files") as NameValueCollection;

            if (hashSet == null)
                throw new ArgumentException("File section not found in app.config");

            _fileLookupTable = hashSet.ToDictionary();

            BuildCache();

            _CacheHasBeenDisposed = false;

            AppDomain.CurrentDomain.ProcessExit += OnProcessExit;
        }

        ~Singleton()
        {
            Dispose();
        }

        /// <summary>
        /// Returns a byte array with the contents of a file found by it's friendly 
        /// name in the app.config. Files are intelligently cached.
        /// </summary>
        /// <param name="fileName">The friendly name of the file to read</param>
        /// <returns>The bytes of the filepath found in the app config.</returns>
        public byte[] GetBytesFromFile(string fileName)
        {
            if (_CacheHasBeenDisposed)
                BuildCache();

            if (_memoryCache.Contains(fileName))
                return _memoryCache.Get(fileName) as byte[];

            if (!_fileLookupTable.ContainsKey(fileName))
                throw new ArgumentException("File not found in File section of app.config");

            var filePath = _fileLookupTable[fileName];

            var bytes = File.ReadAllBytes(filePath);

            _memoryCache.Add(fileName, bytes, null);

            return bytes;
        }

        private void BuildCache()
        {
            _memoryCache = new MemoryCache("FileCache");
            _CacheHasBeenDisposed = false;
        }

        private void OnProcessExit(object sender, EventArgs e)
        {
            _memoryCache.Dispose();
            _CacheHasBeenDisposed = true;
            AppDomain.CurrentDomain.ProcessExit -= OnProcessExit;
        }

        /// <summary>
        /// Disposes of the instance of MemoryCache used by the singleton
        /// </summary>
        public void Dispose()
        {
            _memoryCache.Dispose();
            _CacheHasBeenDisposed = true;
            GC.SuppressFinalize(this);
        }
    }
}
