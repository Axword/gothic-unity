using System;
using System.Collections.Generic;

namespace ZelaznaDroga.Core.Utilities
{
    /// <summary>
    /// Unique ID generator for runtime objects.
    /// </summary>
    public static class UniqueIdGenerator
    {
        private static int _counter = 0;
        private static readonly object _lock = new object();

        /// <summary>
        /// Generates a unique string ID.
        /// </summary>
        public static string Generate(string prefix = "id")
        {
            lock (_lock)
            {
                _counter++;
                return $"{prefix}_{DateTime.Now.Ticks}_{_counter}";
            }
        }

        /// <summary>
        /// Generates a short unique ID (for temporary use).
        /// </summary>
        public static string GenerateShort()
        {
            lock (_lock)
            {
                _counter++;
                return $"{_counter:X4}";
            }
        }

        /// <summary>
        /// Resets the counter (for testing).
        /// </summary>
        public static void Reset()
        {
            lock (_lock)
            {
                _counter = 0;
            }
        }
    }
}
