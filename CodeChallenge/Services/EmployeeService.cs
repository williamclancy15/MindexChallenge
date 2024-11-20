﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeChallenge.Models;
using Microsoft.Extensions.Logging;
using CodeChallenge.Repositories;

namespace CodeChallenge.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(ILogger<EmployeeService> logger, IEmployeeRepository employeeRepository)
        {
            _employeeRepository = employeeRepository;
            _logger = logger;
        }

        public Employee Create(Employee employee)
        {
            if(employee != null)
            {
                _employeeRepository.Add(employee);
                _employeeRepository.SaveAsync().Wait();
            }

            return employee;
        }

        public Employee GetById(string id)
        {
            if(!String.IsNullOrEmpty(id))
            {
                return _employeeRepository.GetById(id);
            }

            return null;
        }

        public Employee Replace(Employee originalEmployee, Employee newEmployee)
        {
            if(originalEmployee != null)
            {
                _employeeRepository.Remove(originalEmployee);
                if (newEmployee != null)
                {
                    // ensure the original has been removed, otherwise EF will complain another entity w/ same id already exists
                    _employeeRepository.SaveAsync().Wait();

                    _employeeRepository.Add(newEmployee);
                    // overwrite the new id with previous employee id
                    newEmployee.EmployeeId = originalEmployee.EmployeeId;
                }
                _employeeRepository.SaveAsync().Wait();
            }

            return newEmployee;
        }

        public ReportingStructure GetReportingStructure(string id) 
        {
            Employee employee = GetById(id); //make sure requested employee exists
            if (employee == null) return null;

            List<string> reports = new List<string>(); //create empty list for unique employeeIds
            reports = ReportingStructureHelper(reports, id);
            return new ReportingStructure()
            {
                Employee = employee,
                NumberOfReports = reports.Count
            };
        }

        private List<string> ReportingStructureHelper(List<string> reports, string id)
        {
            Employee employee = GetById(id);
            if (employee == null || employee.DirectReports == null) return reports;
            for (int i = 0; i < employee.DirectReports.Count; i++) //for each of the current employee's DirectReports
            {
                if (!reports.Contains(employee.DirectReports[i].EmployeeId)) //if we haven't counted this employee already...
                {
					reports.Add(employee.DirectReports[i].EmployeeId); //add their id to the list
                    ReportingStructureHelper(reports, employee.DirectReports[i].EmployeeId); //repeat this process for their DirectReport
				}
            }
            return reports;
        }
    }
}
