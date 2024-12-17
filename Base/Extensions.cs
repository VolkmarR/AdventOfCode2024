namespace AdventOfCode.Base;

static class Extensions
{
    public static string Join(this IEnumerable<string> strings, string separator)
        => string.Join(separator, strings);

    public static void IncrementCount<T>(this Dictionary<T, int> countDictionary, T key)
    {
        countDictionary.TryGetValue(key, out var current);
        countDictionary[key] = ++current;
    }

    public static void IncrementCount<T>(this Dictionary<T, long> countDictionary, T key)
    {
        countDictionary.TryGetValue(key, out var current);
        countDictionary[key] = ++current;
    }

    public static string[] Split(this string text, StringSplitOptions stringSplitOptions, params char[] separators)
        => text.Split(separators, stringSplitOptions);

    public static string[] Split(this string text, params char[] separators)
        => text.Split(separators);

    public static string[] Split(this string text, StringSplitOptions stringSplitOptions, params string[] separators)
        => text.Split(separators, stringSplitOptions);
    
    public static List<string> ToList(this string item) => new() { item };
    public static int ToInt(this string item) => int.Parse(item);

    public static long ToLong(this string item) => long.Parse(item);

    public static string Reverse(this string text)
    {
        if (text == null)
            return null;

        return new string(text.ToCharArray().Reverse().ToArray());
    }

    public static int Pow(this int number, int exponent)
    {
        if (exponent == 0)
            return 1;
        var result = number;
        for (var i = 1; i <= exponent; i++)
            result *= number;
        return result;
    }
    
    public static long Pow(this long number, long exponent)
    {
        if (exponent == 0)
            return 1;
        if (exponent == 1)
            return number;
        
        var result = number;
        for (var i = 2; i <= exponent; i++)
            result *= number;
        return result;
    }
}
