using PCLCrypto;
using System;
using System.Text;


namespace VTS.Core.CrossCutting.Extensions
{
    public static class SecurytyHash
    {
        public static string CalculateSha1Hash(string input)
        {
            string result = string.Empty;
            try
            {
                // step 1, calculate MD5 hash from input
                var hasher = WinRTCrypto.HashAlgorithmProvider.OpenAlgorithm(HashAlgorithm.Sha1);
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hash = hasher.HashData(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("X2"));
                }

                result = sb.ToString();
            }
            catch (Exception ex)
            {
                var err = ex.Message;
                result = input;
            }
            return result;
        }
    }
}
