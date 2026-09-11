namespace Day05;

public static class NiceStringParser{
    public static bool IsNiceString(string input){
        if (!HasThreeVowels(input)) return false;
        if (!HasRepeatingLetters(input)) return false;
        if (!DoesNotContainForbiddenString(input)) return false;
        return true; 
    }

    public static bool HasThreeVowels(string input){
        return false;
    }

    public static bool HasRepeatingLetters(string input){
        return false;
    }

    public static bool DoesNotContainForbiddenString(string input){
        return false;
    }
}