///
///  Hashing Function (xxHash64) modified from this library with MIT liscense
///  https://github.com/uranium62/xxHash
///
///  Nuget doesn't seem to work for us, so we are going to rip source codes of nugets we need
///  Sorry mate. Would 100% use your nuget
///

using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Standart.Hash.xxHash {
    internal static class xxHash64 {

        private static class BitUtils {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static uint RotateLeft(uint value, int offset) {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
                return (value << offset) | (value >> (32 - offset));
#endif
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static ulong RotateLeft(ulong value, int offset) // Taken help from: https://stackoverflow.com/a/48580489/5592276
            {
#if FCL_BITOPS
            return System.Numerics.BitOperations.RotateLeft(value, offset);
#else
                return (value << offset) | (value >> (64 - offset));
#endif
            }
        }

        /// <summary>
        /// Compute xxHash for the data byte span
        /// </summary>
        /// <param name="data">The source of data</param>
        /// <param name="length">The length of the data for hashing</param>
        /// <param name="seed">The seed number</param>
        /// <returns>hash</returns>
        public static unsafe ulong ComputeHash(Span<byte> data, int length, ulong seed = 0) {
            Debug.Assert(data != null);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= data.Length);

            fixed (byte* pData = &MemoryMarshal.GetReference(data)) {
                return UnsafeComputeHash(pData, length, seed);
            }
        }

        /// <summary>
        /// Compute xxHash for the data byte span
        /// </summary>
        /// <param name="data">The source of data</param>
        /// <param name="length">The length of the data for hashing</param>
        /// <param name="seed">The seed number</param>
        /// <returns>hash</returns>
        public static unsafe ulong ComputeHash(ReadOnlySpan<byte> data, int length, ulong seed = 0) {
            Debug.Assert(data != null);
            Debug.Assert(length >= 0);
            Debug.Assert(length <= data.Length);

            fixed (byte* pData = &MemoryMarshal.GetReference(data)) {
                return UnsafeComputeHash(pData, length, seed);
            }
        }


        private const ulong p1 = 11400714785074694791UL;
        private const ulong p2 = 14029467366897019727UL;
        private const ulong p3 = 1609587929392839161UL;
        private const ulong p4 = 9650029242287828579UL;
        private const ulong p5 = 2870177450012600261UL;

        /// <summary>
        /// Compute xxhash64 for the unsafe array of memory
        /// </summary>
        /// <param name="ptr"></param>
        /// <param name="length"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong UnsafeComputeHash(byte* ptr, int length, ulong seed) {
            byte* end = ptr + length;
            ulong h64;

            if (length >= 32) {
                byte* limit = end - 32;

                ulong v1 = seed + p1 + p2;
                ulong v2 = seed + p2;
                ulong v3 = seed + 0;
                ulong v4 = seed - p1;

                do {
                    v1 += *((ulong*)ptr) * p2;
                    v1 = BitUtils.RotateLeft(v1, 31); // rotl 31
                    v1 *= p1;
                    ptr += 8;

                    v2 += *((ulong*)ptr) * p2;
                    v2 = BitUtils.RotateLeft(v2, 31); // rotl 31
                    v2 *= p1;
                    ptr += 8;

                    v3 += *((ulong*)ptr) * p2;
                    v3 = BitUtils.RotateLeft(v3, 31); // rotl 31
                    v3 *= p1;
                    ptr += 8;

                    v4 += *((ulong*)ptr) * p2;
                    v4 = BitUtils.RotateLeft(v4, 31); // rotl 31
                    v4 *= p1;
                    ptr += 8;

                } while (ptr <= limit);

                h64 = BitUtils.RotateLeft(v1, 1) +  // rotl 1
                      BitUtils.RotateLeft(v2, 7) +  // rotl 7
                      BitUtils.RotateLeft(v3, 12) + // rotl 12
                      BitUtils.RotateLeft(v4, 18);  // rotl 18

                // merge round
                v1 *= p2;
                v1 = BitUtils.RotateLeft(v1, 31); // rotl 31
                v1 *= p1;
                h64 ^= v1;
                h64 = h64 * p1 + p4;

                // merge round
                v2 *= p2;
                v2 = BitUtils.RotateLeft(v2, 31); // rotl 31
                v2 *= p1;
                h64 ^= v2;
                h64 = h64 * p1 + p4;

                // merge round
                v3 *= p2;
                v3 = BitUtils.RotateLeft(v3, 31); // rotl 31
                v3 *= p1;
                h64 ^= v3;
                h64 = h64 * p1 + p4;

                // merge round
                v4 *= p2;
                v4 = BitUtils.RotateLeft(v4, 31); // rotl 31
                v4 *= p1;
                h64 ^= v4;
                h64 = h64 * p1 + p4;
            }
            else {
                h64 = seed + p5;
            }

            h64 += (ulong)length;

            // finalize
            while (ptr <= end - 8) {
                ulong t1 = *((ulong*)ptr) * p2;
                t1 = BitUtils.RotateLeft(t1, 31); // rotl 31
                t1 *= p1;
                h64 ^= t1;
                h64 = BitUtils.RotateLeft(h64, 27) * p1 + p4; // (rotl 27) * p1 + p4
                ptr += 8;
            }

            if (ptr <= end - 4) {
                h64 ^= *((uint*)ptr) * p1;
                h64 = BitUtils.RotateLeft(h64, 23) * p2 + p3; // (rotl 23) * p2 + p3
                ptr += 4;
            }

            while (ptr < end) {
                h64 ^= *((byte*)ptr) * p5;
                h64 = BitUtils.RotateLeft(h64, 11) * p1; // (rotl 11) * p1
                ptr += 1;
            }

            // avalanche
            h64 ^= h64 >> 33;
            h64 *= p2;
            h64 ^= h64 >> 29;
            h64 *= p3;
            h64 ^= h64 >> 32;

            return h64;
        }

        /// <summary>
        /// Compute the first part of xxhash64 (need for the streaming api)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="l"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe void UnsafeAlign(byte[] data, int l, ref ulong v1, ref ulong v2, ref ulong v3, ref ulong v4) {
            fixed (byte* pData = &data[0]) {
                byte* ptr = pData;
                byte* limit = ptr + l;

                do {
                    v1 += *((ulong*)ptr) * p2;
                    v1 = BitUtils.RotateLeft(v1, 31); // rotl 31
                    v1 *= p1;
                    ptr += 8;

                    v2 += *((ulong*)ptr) * p2;
                    v2 = BitUtils.RotateLeft(v2, 31); // rotl 31
                    v2 *= p1;
                    ptr += 8;

                    v3 += *((ulong*)ptr) * p2;
                    v3 = BitUtils.RotateLeft(v3, 31); // rotl 31
                    v3 *= p1;
                    ptr += 8;

                    v4 += *((ulong*)ptr) * p2;
                    v4 = BitUtils.RotateLeft(v4, 31); // rotl 31
                    v4 *= p1;
                    ptr += 8;

                } while (ptr < limit);
            }
        }

        /// <summary>
        /// Compute the second part of xxhash64 (need for the streaming api)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="l"></param>
        /// <param name="v1"></param>
        /// <param name="v2"></param>
        /// <param name="v3"></param>
        /// <param name="v4"></param>
        /// <param name="length"></param>
        /// <param name="seed"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static unsafe ulong UnsafeFinal(byte[] data, int l, ref ulong v1, ref ulong v2, ref ulong v3, ref ulong v4, long length, ulong seed) {
            fixed (byte* pData = &data[0]) {
                byte* ptr = pData;
                byte* end = pData + l;
                ulong h64;

                if (length >= 32) {
                    h64 = BitUtils.RotateLeft(v1, 1) +  // rotl 1
                          BitUtils.RotateLeft(v2, 7) +  // rotl 7
                          BitUtils.RotateLeft(v3, 12) + // rotl 12
                          BitUtils.RotateLeft(v4, 18);  // rotl 18

                    // merge round
                    v1 *= p2;
                    v1 = BitUtils.RotateLeft(v1, 31); // rotl 31
                    v1 *= p1;
                    h64 ^= v1;
                    h64 = h64 * p1 + p4;

                    // merge round
                    v2 *= p2;
                    v2 = BitUtils.RotateLeft(v2, 31); // rotl 31
                    v2 *= p1;
                    h64 ^= v2;
                    h64 = h64 * p1 + p4;

                    // merge round
                    v3 *= p2;
                    v3 = BitUtils.RotateLeft(v3, 31); // rotl 31
                    v3 *= p1;
                    h64 ^= v3;
                    h64 = h64 * p1 + p4;

                    // merge round
                    v4 *= p2;
                    v4 = BitUtils.RotateLeft(v4, 31); // rotl 31
                    v4 *= p1;
                    h64 ^= v4;
                    h64 = h64 * p1 + p4;

                }
                else {
                    h64 = seed + p5;
                }

                h64 += (ulong)length;

                // finalize
                while (ptr <= end - 8) {
                    ulong t1 = *((ulong*)ptr) * p2;
                    t1 = BitUtils.RotateLeft(t1, 31); // rotl 31
                    t1 *= p1;
                    h64 ^= t1;
                    h64 = BitUtils.RotateLeft(h64, 27) * p1 + p4; // (rotl 27) * p1 + p4
                    ptr += 8;
                }

                if (ptr <= end - 4) {
                    h64 ^= *((uint*)ptr) * p1;
                    h64 = BitUtils.RotateLeft(h64, 23) * p2 + p3; // (rotl 27) * p2 + p3
                    ptr += 4;
                }

                while (ptr < end) {
                    h64 ^= *((byte*)ptr) * p5;
                    h64 = BitUtils.RotateLeft(h64, 11) * p1; // (rotl 11) * p1
                    ptr += 1;
                }

                // avalanche
                h64 ^= h64 >> 33;
                h64 *= p2;
                h64 ^= h64 >> 29;
                h64 *= p3;
                h64 ^= h64 >> 32;

                return h64;
            }
        }
    }
}
