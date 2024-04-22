// Copyright (C) 2015-2024 The Neo Project.
//
// Snapshot.cs file belongs to the neo project and is free
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
    internal class Snapshot : ISnapshot
    {
        private readonly DB _db;
        private readonly SnapShot _snapshot;
        private readonly ReadOptions _options;
        private readonly WriteBatch _batch;

        public Snapshot(DB db)
        {
            this._db = db;
            this._snapshot = db.CreateSnapshot();
            this._options = new ReadOptions { FillCache = false, Snapshot = _snapshot };
            this._batch = new WriteBatch();
        }

        public void Commit()
        {
            _db.Write(_batch);
        }

        public void Delete(byte[] key)
        {
            _batch.Delete(key);
        }

        public void Dispose()
        {
            _snapshot.Dispose();
        }

        public IEnumerable<(byte[] Key, byte[] Value)> Seek(byte[] prefix, SeekDirection direction = SeekDirection.Forward)
        {
            return _db.Seek(_options, prefix, direction, (k, v) => (k, v));
        }

        public void Put(byte[] key, byte[] value)
        {
            _batch.Put(key, value);
        }

        public bool Contains(byte[] key)
        {
            return _db.Contains(key, _options);
        }

        public byte[] TryGet(byte[] key)
        {
            return _db.Get(key, _options);
        }
    }
}
