// Copyright (C) 2015-2024 The Neo Project.
//
// Helper.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using LevelDB;
using Neo.Persistence;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Neo.IO.Data.LevelDB
{
    public static class Helper
    {
        public static IEnumerable<T> Seek<T>(this DB db, ReadOptions options, byte[] prefix, SeekDirection direction, Func<byte[], byte[], T> resultSelector)
        {
            using var it = db.CreateIterator(options);
            if (direction == SeekDirection.Forward)
            {
                for (it.Seek(prefix); it.IsValid(); it.Next())
                    yield return resultSelector(it.Key(), it.Value());
            }
            else
            {
                // SeekForPrev

                it.Seek(prefix);
                if (!it.IsValid())
                    it.SeekToLast();
                else if (it.Key().AsSpan().SequenceCompareTo(prefix) > 0)
                    it.Prev();

                for (; it.IsValid(); it.Prev())
                    yield return resultSelector(it.Key(), it.Value());
            }
        }

        public static IEnumerable<T> FindRange<T>(this DB db, ReadOptions options, byte[] startKey, byte[] endKey, Func<byte[], byte[], T> resultSelector)
        {
            using var it = db.CreateIterator(options);
            for (it.Seek(startKey); it.IsValid(); it.Next())
            {
                var key = it.Key();
                if (key.AsSpan().SequenceCompareTo(endKey) > 0) break;
                yield return resultSelector(key, it.Value());
            }
        }

        internal static byte[]? ToByteArray(this IntPtr data, UIntPtr length)
        {
            if (data == IntPtr.Zero) return null;
            var buffer = new byte[(int)length];
            Marshal.Copy(data, buffer, 0, (int)length);
            return buffer;
        }
    }
}
