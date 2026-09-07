namespace CalculatorApp.Tests;

public class CollectionServiceTests
{
    private readonly CollectionService _sut = new CollectionService();

    // Assert.Contains(expected, collection);
    [Fact]
    public void GetItems_ContainsTwo()
    {
        // Act
        List<string> actual = _sut.GetItems();

        // Assert
        Assert.Contains("two", actual);
    }

    // Assert.DoesNotContain(expected, collection);
    [Fact]
    public void GetItems_DoesNotContainFour()
    {
        // Act
        List<string> actual = _sut.GetItems();

        // Assert
        Assert.DoesNotContain("four", actual);
    }

    // Assert.Empty(collection);
    [Fact]
    public void GetEmpty_IsEmpty()
    {
        // Act
        List<string> actual = _sut.GetEmpty();

        // Assert
        Assert.Empty(actual);
    }

    // Assert.NotEmpty(collection);
    [Fact]
    public void GetItems_IsNotEmpty()
    {
        // Act
        List<string> actual = _sut.GetItems();

        // Assert
        Assert.NotEmpty(actual);
    }

    // Assert.All(collection, item => ...);
    [Fact]
    public void GetItems_AllAreLowerCase()
    {
        // Act
        List<string> actual = _sut.GetItems();

        // Assert - the assertion runs once per item
        Assert.All(actual, item => Assert.Equal(item.ToLower(), item));
    }
}
