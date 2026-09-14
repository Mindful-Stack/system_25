using Day05;

namespace Day05.Tests;

public class NiceStringTests {
    
    // with vowel we mean aeiou  

    [Theory]
    [InlineData("aei", true)]
    [InlineData("xazegov", true)]
    [InlineData("aeiouaeiouaeiou", true)]
    [InlineData("ooo", true)]
    [InlineData("o........o......o", true)]
    [InlineData("oor", false)]
    [InlineData("ssssslkipnvcmclsghrt", false)]
    [InlineData("oo", false)]
    [InlineData("lklklklkoa", false)]
    [InlineData("o.l.l.k.lk.o", false)]
    [InlineData("000001203084009080988236556", false)]
    public void HasAtLeastThreeVowelsTest(string input, bool expected){
        // Arrange
        // Act
        var actual = NiceStringParser.HasThreeVowels(input);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    // It contains at least one letter that appears twice in a row, 
    // like xx, abcdde (dd), or aabbccdd (aa, bb, cc, or dd).
    [Theory]
    [InlineData("aa", true)]
    [InlineData("bb", true)]
    [InlineData("lmlmlmlmlmlmlmllmlml", true)]
    [InlineData("aabbccddee", true)]
    [InlineData("aaaaaaaaaa", true)]
    [InlineData("a", false)]
    [InlineData("ababababab", false)]
    [InlineData("abcdefghijkl", false)]
    [InlineData("a.a", false)]
    public void HasRepeatingLetterTest(string input, bool expected){
        // Arrange
        // Act
        var actual = NiceStringParser.HasRepeatingLetters(input);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    // It does not contain the strings ab, cd, pq, or xy, 
    // even if they are part of one of the other requirements.
    [Theory]
    [InlineData("ab", false)]
    [InlineData("cd", false)]
    [InlineData("pq", false)]
    [InlineData("xy", false)]
    [InlineData("aaaaabaaaa", false)]
    [InlineData("abcdefpqxy", false)]
    [InlineData("acfghjk", true)]
    [InlineData("a.b", true)]
    [InlineData("dcba", true)]
    [InlineData("lklklklklklklklk", true)]
    public void DoesNotContainForbiddenStringTest(string input, bool expected){
        // Arrange
        // Act
        var actual = NiceStringParser.DoesNotContainForbiddenString(input);
        
        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData("ugknbfddgicrmopn", true)]
    [InlineData("aaa", true)]
    [InlineData("aoexx", true)]
    [InlineData("jchzalrnumimnmhp", false)]
    [InlineData("haegwjzuvuyypxyu", false)]
    [InlineData("dvszwmarrgswjxmb", false)]
    public void DoesCheckAllThreeConditions(string input, bool expected){
        // Arrange
        // Act
        var actual = NiceStringParser.IsNiceString(input);
        
        // Assert
        Assert.Equal(expected, actual);
    }
}