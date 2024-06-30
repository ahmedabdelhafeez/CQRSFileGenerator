namespace CQRSGenerator.Shared;
using System;

public static class Pluralizer
{
    public static string Pluralize(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;

        if (word.EndsWith("y", StringComparison.OrdinalIgnoreCase) &&
            !IsVowel(word[word.Length - 2]))
        {
            return word.Substring(0, word.Length - 1) + "ies";
        }

        if (word.EndsWith("s", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("sh", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("ch", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("x", StringComparison.OrdinalIgnoreCase) ||
            word.EndsWith("z", StringComparison.OrdinalIgnoreCase))
        {
            return word + "es";
        }

        return word + "s";
    }

    private static bool IsVowel(char c)
    {
        return "aeiouAEIOU".IndexOf(c) >= 0;
    }
}
