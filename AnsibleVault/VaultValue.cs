using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LauPas.AnsibleVault
{
    /// <summary>
    /// Vault Value
    /// </summary>
    public class VaultValue
    {
        /// <summary>
        /// The Salt.
        /// </summary>
        public byte[] Salt { get; set; }
        /// <summary>
        /// The Hamc.
        /// </summary>
        public byte[] Hamc { get; set; }
        /// <summary>
        /// The Body.
        /// </summary>
        public byte[] Body { get; set; }
        /// <summary>
        /// The Id.
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// The Header.
        /// </summary>
        public string Header { get; set; }

        /// <summary>
        /// Create a new Instance.
        /// </summary>
        public VaultValue()
        {
            this.Header = "$ANSIBLE_VAULT;1.1;AES256";
        }
        
        /// <summary>
        /// Create a new Instance.
        /// </summary>
        public VaultValue(string text)
        {            
            var parts = text.Split(Environment.NewLine);
            this.ValidateHeader(parts);

            var temp =  parts.Skip(1).ConvertHexStringListToByteArray().ToList();
            this.Salt = Encoding.ASCII.GetString(temp.Take(64).ToArray()).ConvertHexStringToBytes().ToArray();
            this.Hamc = Encoding.ASCII.GetString(temp.Skip(65).Take(64).ToArray()).ConvertHexStringToBytes().ToArray();
            this.Body = Encoding.ASCII.GetString(temp.Skip(130).ToArray()).ConvertHexStringToBytes().ToArray();
        }
        
        private void ValidateHeader(string[] parts)
        {
            this.Header = parts[0];

            var header = this.Header.Split(';');
            if (header[0] != "$ANSIBLE_VAULT")
            {
                throw new ArgumentException($"No Valid ansible vault header: {this.Header}");
            }

            if (header[1] == "1.0")
            {
                throw new ArgumentException($"Version {header[1]} is not supported");
            }

            if (header[2] != "AES256")
            {
                throw new ArgumentException($"No Valid ansible vault header: {this.Header}");
            }

            if (header.Length > 3)
            {
                this.Id = header[3];
            }
        }

        /// <summary>
        /// Create a Vault sting
        /// </summary>
        /// <returns></returns>
        public string ToVaultString()
        {
            var result = new List<char>();
            result.AddRange(this.Salt.ConvertToHexString().ConvertToHexString());
            result.Add('0');
            result.Add(((byte)0x0a).ConvertByteToHexChar());
            result.AddRange(this.Hamc.ConvertToHexString().ConvertToHexString());
            result.Add('0');
            result.Add(((byte)0x0a).ConvertByteToHexChar());
            result.AddRange(this.Body.ConvertToHexString().ConvertToHexString());
            
            var sb = new StringBuilder();
            sb.Append(this.Header);
            for (var index = 0; index < result.Count; index++)
            {
                if (index % 80 == 0)
                {
                    sb.AppendLine("");
                }
                var c = result[index];
                sb.Append(c);
            }
            
            return sb.ToString();
        }
    }
}