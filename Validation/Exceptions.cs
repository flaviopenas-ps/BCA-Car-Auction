namespace BCA_Car_Auction.Validation
{
    public static class ValidationExtensions
    {
        public static string ThrowIfNullOrEmpty(this string value, string paramName)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"{paramName} cannot be null or empty", paramName);
            return value;
        }

        public static T ThrowIfNull<T>(this T value, string message) where T : class
        {
            if (value is null)
                throw new ArgumentNullException(message);
            return value;
        }

        public static T ThrowIfNull<T>(this T? value, string message) where T : struct
        {
            if (!value.HasValue)
                throw new ArgumentException(message);
            return value.Value;
        }
    }
}
