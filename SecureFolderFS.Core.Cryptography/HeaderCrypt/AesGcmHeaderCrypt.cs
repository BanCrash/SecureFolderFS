﻿using SecureFolderFS.Core.Cryptography.SecureStore;
using System;
using static SecureFolderFS.Core.Cryptography.Constants.Crypt.Headers.AesGcm;
using static SecureFolderFS.Core.Cryptography.Extensions.HeaderCryptExtensions.AesGcmHeaderExtensions;

namespace SecureFolderFS.Core.Cryptography.HeaderCrypt
{
    /// <inheritdoc cref="IHeaderCrypt"/>
    public sealed class AesGcmHeaderCrypt : BaseHeaderCrypt
    {
        /// <inheritdoc/>
        public override int HeaderCiphertextSize { get; } = HEADER_SIZE;

        /// <inheritdoc/>
        public override int HeaderCleartextSize { get; } = HEADER_NONCE_SIZE + HEADER_CONTENTKEY_SIZE;

        public AesGcmHeaderCrypt(SecretKey encKey, SecretKey macKey, CipherProvider cipherProvider)
            : base(encKey, macKey, cipherProvider)
        {
        }

        /// <inheritdoc/>
        public override void CreateHeader(Span<byte> cleartextHeader)
        {
            // Nonce
            secureRandom.GetNonZeroBytes(cleartextHeader.Slice(0, HEADER_NONCE_SIZE));

            // Content key
            secureRandom.GetBytes(cleartextHeader.Slice(HEADER_NONCE_SIZE, HEADER_CONTENTKEY_SIZE));
        }

        /// <inheritdoc/>
        public override void EncryptHeader(ReadOnlySpan<byte> cleartextHeader, Span<byte> ciphertextHeader)
        {
            // Nonce
            cleartextHeader.GetHeaderNonce().CopyTo(ciphertextHeader);

            // Encrypt
            cipherProvider.AesGcmCrypt.Encrypt(
                cleartextHeader.GetHeaderContentKey(),
                encKey,
                cleartextHeader.GetHeaderNonce(),
                ciphertextHeader.GetHeaderTag(),
                ciphertextHeader.Slice(HEADER_NONCE_SIZE, HEADER_CONTENTKEY_SIZE),
                default);
        }

        /// <inheritdoc/>
        public override bool DecryptHeader(ReadOnlySpan<byte> ciphertextHeader, Span<byte> cleartextHeader)
        {
            // Nonce
            ciphertextHeader.GetHeaderNonce().CopyTo(cleartextHeader);

            // Decrypt
            return cipherProvider.AesGcmCrypt.Decrypt(
                ciphertextHeader.GetHeaderContentKey(),
                encKey,
                ciphertextHeader.GetHeaderNonce(),
                ciphertextHeader.GetHeaderTag(),
                cleartextHeader.Slice(HEADER_NONCE_SIZE),
                default);
        }
    }
}
