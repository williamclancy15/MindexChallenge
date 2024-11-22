using CodeChallenge.Models;
using System;
using System.Threading.Tasks;

namespace CodeChallenge.Repositories
{
    public interface IEmployeeRepository
    {
        Employee GetEmployeeById(String id);
        Employee AddEmployee(Employee employee);
        Employee RemoveEmployee(Employee employee);
        Compensation AddCompensation(Compensation compensation);
        Compensation GetCompensationById(String id);
        Task SaveAsync();
    }
}