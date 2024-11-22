using CodeChallenge.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CodeChallenge.Services
{
    public interface IEmployeeService
    {
        Employee GetEmployeeById(String id);
        Employee CreateEmployee(Employee employee);
        Employee ReplaceEmployee(Employee originalEmployee, Employee newEmployee);
        Compensation CreateCompensation(Compensation compensation);
        Compensation GetCompensationById(String id);
        ReportingStructure GetReportingStructure(String id);
    }
}
