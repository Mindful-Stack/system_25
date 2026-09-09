using System.Collections;
using EmployeeAccessDemo.Models;

namespace EmployeeAccessDemo.Tests.TestData;

public class EmployeeAccessTestData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return new object[] { new Employee { Name = "Anna", Role = "Manager", IsClockedIn = true }, true };
        yield return new object[] { new Employee { Name = "Erik", Role = "Staff", IsClockedIn = true }, false };
        yield return new object[] { new Employee { Name = "Lisa", Role = "Manager", IsClockedIn = false }, false };
        yield return new object[] { new Employee { Name = "Tom", Role = "Staff", IsClockedIn = false }, false };
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}