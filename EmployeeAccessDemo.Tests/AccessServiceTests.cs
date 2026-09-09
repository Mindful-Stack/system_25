using EmployeeAccessDemo.Models;
using EmployeeAccessDemo.Services;
using EmployeeAccessDemo.Tests.TestData;

namespace EmployeeAccessDemo.Tests;

public class AccessServiceTests
{
    private readonly AccessService _sut = new AccessService();

    [Theory]
    [ClassData(typeof(EmployeeAccessTestData))]
    public void FromClass_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
    {
        // Act
        var actual = _sut.CanAccessRestrictedArea(employee);

        //Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [ClassData(typeof(EmployeeAccessJsonTestData))]
    public void FromJson_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
    {
        var actual = _sut.CanAccessRestrictedArea(employee);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(GetAccessCases))]
    public void FromMember_CanAccessRestrictedArea_ReturnsExpected(Employee employee, bool expected)
    {
        var actual = _sut.CanAccessRestrictedArea(employee);
        Assert.Equal(expected, actual);
    }

    public static IEnumerable<object[]> GetAccessCases()
    {
        yield return new object[] { new Employee { Name = "Mathilda", Role = "Manager", IsClockedIn = true }, true };
        yield return new object[] { new Employee { Name = "Abdi", Role = "Staff", IsClockedIn = true }, false };
        yield return new object[] { new Employee { Name = "Monica", Role = "Manager", IsClockedIn = false }, false };
    }
}