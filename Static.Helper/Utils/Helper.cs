namespace Static.Helper.Utils
{
    public class Helper
    {
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        /// <summary>
        /// Generates a random alphanumeric string of specified length.
        /// </summary>
        /// <param name="length">Number of characters</param>
        /// <returns>Random alpha-numeric string</returns>
        public static string GenerateRandomString(int length)
        {
            var random = Random.Shared;
            var stringChars = new char[length];
            for (var i = 0; i < length; i++)
            {
                stringChars[i] = Chars[random.Next(Chars.Length)];
            }

            return new string(stringChars);
        }

        /// <summary>
        /// Generates a random number of specified length. 
        /// The max integer number (2^31-1) has 10 digits. So, the length must be 10 digits or fewer.
        /// </summary>
        /// <param name="length">Number of digits</param>
        /// <returns>Random number</returns>
        public static int GenerateRandomNumber(int length)
        {
            var minValue =
                (int)Math.Pow(10, length - 1); // Minimum value based on length (e.g., for length 3, minValue = 100)
            var maxValue =
                (int)Math.Pow(10, length) - 1; // Maximum value based on length (e.g., for length 3, maxValue = 999)

            return Random.Shared.Next(minValue, maxValue == int.MaxValue ? maxValue : ++maxValue);
        }

        public class ConcatLists<T> where T : class
        {
            public List<T> Concat(IEnumerable<IEnumerable<T>> lists)
            {
                var list = new List<T>();

                lists.ToList().ForEach(x => list.AddRange(x));

                return list;
            }
        }

        public static string ShowLastFourCharacter(string requestValue)
        {
            if (string.IsNullOrEmpty(requestValue) || requestValue.Length < 4)
            {
                return "XXXX";
            }

            return requestValue.Substring(requestValue.Length - 4, 4);
        }
    }
}