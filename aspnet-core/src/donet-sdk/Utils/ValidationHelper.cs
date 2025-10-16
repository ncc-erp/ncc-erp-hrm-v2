using System.Numerics;

namespace MmnDotNetSdk.Utils
{
    public static class Constants
    {
        public const int NativeDecimal = 6;
        public const int AddressDecodedExpectedLength = 32;
    }

    public class ValidationException : Exception
    {
        public ValidationException(string message) : base(message) { }
    }

    public static class ValidationHelper
    {
        public static void ValidateAddress(string addr)
        {
            if (string.IsNullOrEmpty(addr))
                throw new ValidationException("Invalid address format");

            try
            {
                var decoded = CryptoHelper.Base58Decode(addr);
                if (decoded.Length != Constants.AddressDecodedExpectedLength)
                    throw new ValidationException("Invalid address format");
            }
            catch
            {
                throw new ValidationException("Invalid address format");
            }
        }

        public static void ValidateAmount(BigInteger amount)
        {
            if (amount <= 0)
                throw new ValidationException("Amount must be > 0");
        }

        public static string SerializeTxExtraInfo(Dictionary<string, string>? data)
        {
            if (data == null)
                return string.Empty;

            try
            {
                return System.Text.Json.JsonSerializer.Serialize(data);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Unable to marshal tx extra info: {ex.Message}");
            }
        }

        public static Dictionary<string, string>? DeserializeTxExtraInfo(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return null;

            try
            {
                return System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, string>>(raw);
            }
            catch (Exception ex)
            {
                throw new ValidationException($"Unable to deserialize extra info: {ex.Message}");
            }
        }

        public static BigInteger AmountToDecimal(BigInteger amount)
        {
            var multiplier = BigInteger.Pow(10, Constants.NativeDecimal);
            return amount * multiplier;
        }
    }
}
