// Copyright (C) 2015-2023 The Neo Project.
//
// The Neo.Plugins.RestServer is free software distributed under the MIT software license,
// see the accompanying file LICENSE in the main directory of the
// project or http://www.opensource.org/licenses/mit-license.php
// for more details.
//
// Redistribution and use in source and binary forms with or without
// modifications are permitted.

using Neo.Cryptography.ECC;

namespace Neo.Plugins.RestServer.Models.Wallet
{
    public class WalletKeyModel
    {
        /// <summary>
        /// Wallet address that was exported.
        /// </summary>
        /// <example>NNLi44dJNXtDNSBkofB48aTVYtb1zZrNEs</example>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Scripthash of the wallet account exported.
        /// </summary>
        /// <example>0xed7cc6f5f2dd842d384f254bc0c2d58fb69a4761</example>
        public UInt160 ScriptHash { get; set; } = UInt160.Zero;

        /// <summary>
        /// Public key of the wallet address.
        /// </summary>
        /// <example>03cdb067d930fd5adaa6c68545016044aaddec64ba39e548250eaea551172e535c</example>
        public ECPoint PublicKey { get; set; } = ECCurve.Secp256r1.Infinity;
    }
}
