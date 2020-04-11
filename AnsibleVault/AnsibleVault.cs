using System;
using System.Security.Cryptography;
using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace LauPas.AnsibleVault
{
    public class AnsibleVault : IAnsibleVault
    {
        public string Decode(string password, string input)
        {
            var ansibleValue = new VaultValue(input);

            var (aesKey, aes, derived) = Rfc2898DeriveBytes(ansibleValue.Salt, password);
            ValidatePassword(derived, ansibleValue);
            
            var cipher = Cipher(ansibleValue.Body, aesKey, aes, false);
            return Encoding.ASCII.GetString(cipher);
        }

        public string Encode(string password, string input)
        {
            var salt = this.CreateSalt();

            var (aesKey, aes, derived) = Rfc2898DeriveBytes(salt, password);

            var cipher = Cipher(Encoding.ASCII.GetBytes(input), aesKey, aes, true);

            var hmac = new HMACSHA256(derived.AsSpan(32, 32).ToArray()).ComputeHash(cipher);
            
            var value = new VaultValue
            {
                Salt = salt,
                Hamc = hmac,
                Body = cipher
            };

            return value.ToVaultString();
        }

        private static byte[] Cipher(byte[] input, byte[] aesKey, byte[] aes, bool forEncryption)
        {
            var cipher = CipherUtilities.GetCipher("AES/CTR/PKCS7Padding");
            var cipherParameters = new ParametersWithIV(ParameterUtilities.CreateKeyParameter("AES", aesKey), aes);
            cipher.Init(forEncryption, cipherParameters);
            var result = cipher.DoFinal(input);
            return result;
        }

        private byte[] CreateSalt()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var data = new byte[32];
                rng.GetBytes(data);
                return data;
            }
        }

        private static (byte[] aesKey, byte[] aes, byte[] derived) Rfc2898DeriveBytes(byte[] salt, string password)
        {
            var kdf = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            var derived = kdf.GetBytes(32 + 32 + 16);
            
            var aesKey = derived.AsSpan(0, 32).ToArray();
            var aes = derived.AsSpan(64, 16).ToArray();
            
            return (aesKey, aes, derived);
        }

        private static void ValidatePassword(byte[] derived, VaultValue ansibleValue)
        {
            var hmacKey = derived.AsSpan(32, 32).ToArray();
            var hmac256 = new HMACSHA256(hmacKey);
            var hash = hmac256.ComputeHash(ansibleValue.Body);

            if (!hash.AsSpan().SequenceEqual(ansibleValue.Hamc))
            {
                throw new Exception("Password was wrong");
            }
        }
    }
}