[![NuGet version](https://badge.fury.io/nu/Cryptography.ECDSA.Secp256k1.svg)](https://badge.fury.io/nu/Cryptography.ECDSA.Secp256k1)
[![Build Status](https://travis-ci.org/Chainers/Cryptography.ECDSA.svg?branch=master)](https://travis-ci.org/Chainers/Cryptography.ECDSA)

# Cryptography.ECDSA (secp256k1 only)

This library implements transaction signing algorithm secp256k1 which is used in several blockchains like Bitcoin, EOS and Graphene-based Steem, Golos, BitShares. The library is based on https://github.com/warner/python-ecdsa and https://github.com/bitcoin-core/secp256k1)
No other curves are included.
C#, MIT license.

### Usage
```
//Sign message
var seckey = Hex.HexToBytes("80f3a375e00cc5147f30bee97bb5d54b31a12eee148a1ac31ac9edc4ecd13bc1f80cc8148e");
var data = Sha256Manager.GetHash(msg);
var sig = Secp256K1Manager.SignCompressedCompact(data, seckey);
```

### Instalation
```
Install-Package Cryptography.ECDSA.Secp256k1
```
