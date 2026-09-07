namespace CalculatorApp.Tests;

public class ObjectServiceTests
{
    private readonly ObjectService _sut = new ObjectService();

    // Assert.Null(obj);
    [Fact]
    public void GetNull_ReturnsNull()
    {
        // Act
        object? actual = _sut.GetNull();

        // Assert
        Assert.Null(actual);
    }

    // Assert.NotNull(obj);
    [Fact]
    public void GetSame_ReturnsAnObject()
    {
        // Act
        object actual = _sut.GetSame();

        // Assert
        Assert.NotNull(actual);
    }

    // Assert.True(condition);
    [Fact]
    public void True_IsTrue()
    {
        // Assert
        Assert.True(1 + 1 == 2);
    }

    // Assert.Same(expected, actual);
    [Fact]
    public void Same_TwoVariablesPointingAtOneObject_AreSame()
    {
        // Arrange
        object expected = _sut.GetSame();

        // Act
        object actual = expected;

        // Assert - Same asks if it is the very same object, not just an equal one
        Assert.Same(expected, actual);
    }

    // Assert.NotSame(expected, actual);
    [Fact]
    public void NotSame_TwoSeparateObjects_AreNotSame()
    {
        // Arrange
        object expected = _sut.GetSame();

        // Act
        object actual = _sut.GetSame();

        // Assert
        Assert.NotSame(expected, actual);
    }

    // Assert.IsType<T>(obj);
    [Fact]
    public void IsType_OneTwoThree_IsAnInt()
    {
        // Arrange
        object actual = 123;

        // Assert
        Assert.IsType<int>(actual);
    }

    // Assert.Throws<T>(() => sut.Method());
    [Fact]
    public void Crash_ThrowsInvalidOperationException()
    {
        // Act & Assert - the call is passed in so the test can catch the exception
        Assert.Throws<InvalidOperationException>(() => _sut.Crash());
    }
}
