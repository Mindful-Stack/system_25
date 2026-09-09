using EmployeeAccessDemo.Models;

namespace EmployeeAccessDemo.Services;

public class AccessService
{
    public bool CanAccessRestrictedArea(Employee employee)
    {
        return employee.Role == "Manager" && employee.IsClockedIn;
    }
}