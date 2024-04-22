// Copyright (C) 2015-2024 The Neo Project.
//
// LevelDbFreeHandle.cs file belongs to the neo project and is free
// software distributed under the MIT software license, see the
// accompanying file LICENSE in the main directory of the
// repository or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using System;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;

namespace LevelDB
{
    // Wraps pointers to be freed with leveldb_free (e.g. returned by leveldb_get)
    //
    // reference on safe handles: http://blogs.msdn.com/b/bclteam/archive/2006/06/23/644343.aspx
    internal class LevelDbFreeHandle : SafeHandle
    {
        public LevelDbFreeHandle()
            : base(default(IntPtr), true)
        {
        }

        [ReliabilityContract(Consistency.WillNotCorruptState, Cer.MayFail)]
        override protected bool ReleaseHandle()
        {
            if (this.handle != default(IntPtr))
                LevelDBInterop.leveldb_free(this.handle);
            this.handle = default(IntPtr);
            return true;
        }

        public override bool IsInvalid
        {
            get { return this.handle != default(IntPtr); }
        }

        public new void SetHandle(IntPtr p)
        {
            if (this.handle != default(IntPtr))
                ReleaseHandle();

            base.SetHandle(p);
        }
    }
}
