using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LauPas.AnsibleVault
{
    public static class Extensions
    {
        public static byte[] ConvertHexStringListToByteArray(this IEnumerable<string> hexList)
        {
            return hexList.SelectMany(ConvertHexStringToBytes).ToArray();
        }

        public static byte[] ConvertHexStringToBytes(this string input)
        {
            var result = new List<byte>();
            var span = input.AsSpan();
            
            for (var i = 0; i < input.Length / 2; i++)
            {
                result.Add((byte)((ConvertHexCharToByte(span[i * 2]) << 4) | ConvertHexCharToByte(span[i * 2 + 1])));
            }

            return result.ToArray();
        }
        
        public static string ConvertToHexString(this byte[] data)
        {
            return string.Create(data.Length * 2, data.ToArray(), (c, state) =>
            {
                for(var i = 0; i < state.Length; i++)
                {
                    c[i * 2] = ConvertByteToHexChar((byte)(state[i] >> 4));
                    c[i * 2 + 1] = ConvertByteToHexChar((byte)(state[i] & 0xf));
                }
            });
        }

        public static string ConvertToHexString(this string inputString)
        {
            return Encoding.ASCII.GetBytes(inputString).ConvertToHexString();
        }

        public static char ConvertByteToHexChar(this byte inputByte)
        {
            if(inputByte < 10)
            {
                return (char)('0' + inputByte);
            }

            if(inputByte < 16)
            {
                return (char)('a' + inputByte - 10);
            }

            throw new ArgumentOutOfRangeException($"invalid byte value:{(int)inputByte}");
        }

        private static byte ConvertHexCharToByte(this char inputChar)
        {
            if ('0' <= inputChar && inputChar <= '9')
            {
                return (byte)(inputChar - '0');
            }
            
            if ('a' <= inputChar && inputChar <= 'f')
            {
                return (byte)(inputChar - 'a' + 10);
            }

            if ('A' <= inputChar && inputChar <= 'F')
            {
                return (byte)(inputChar - 'A');
            }
            
            throw new Exception($"invalid value:{inputChar}({(int)inputChar})");
        }
    }
}