// Copyright (C) 2015-2024 The Neo Project.
//
// Store.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using LevelDB;
using Neo.IO.Data.LevelDB;
using Neo.Persistence;
using System.Collections.Generic;

namespace Neo.Plugins.Storage
{
    internal class Store : IStore
    {
        private readonly DB db;

        public Store(string path)
        {
            this.db = new DB(new Options { CreateIfMissing = true, }, path);
        }

        public void Delete(byte[] key)
        {
            db.Delete(key, WriteOptions.Default);
        }

        public void Dispose()
        {
            db.Dispose();
        }

        public IEnumerable<(byte[], byte[])> Seek(byte[] prefix, SeekDirection direction = SeekDirection.Forward)
        {
            return db.Seek(ReadOptions.Default, prefix, direction, (k, v) => (k, v));
        }

        public ISnapshot GetSnapshot()
        {
            return new Snapshot(db);
        }

        public void Put(byte[] key, byte[] value)
        {
            db.Put(key, value, WriteOptions.Default);
        }

        public void PutSync(byte[] key, byte[] value)
        {
            db.Put(key, value, WriteOptions.SyncWrite);
        }

        public bool Contains(byte[] key)
        {
            return db.Contains(key, ReadOptions.Default);
        }

        public byte[]? TryGet(byte[] key)
        {
            return db.Get(key, ReadOptions.Default);
        }
    }
}
