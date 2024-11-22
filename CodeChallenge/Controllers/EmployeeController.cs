using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using CodeChallenge.Services;
using CodeChallenge.Models;

namespace CodeChallenge.Controllers
{
    [ApiController]
    [Route("api/employee")]
    public class EmployeeController : ControllerBase
    {
        private readonly ILogger _logger;
        private readonly IEmployeeService _employeeService;

        public EmployeeController(ILogger<EmployeeController> logger, IEmployeeService employeeService)
        {
            _logger = logger;
            _employeeService = employeeService;
        }

        [HttpPost]
        public IActionResult CreateEmployee([FromBody] Employee employee)
        {
            _logger.LogDebug($"Received employee create request for '{employee.FirstName} {employee.LastName}'");

            _employeeService.CreateEmployee(employee);

            return CreatedAtRoute("getEmployeeById", new { id = employee.EmployeeId }, employee);
        }

        [HttpGet("{id}", Name = "getEmployeeById")]
        public IActionResult GetEmployeeById(String id)
        {
            _logger.LogDebug($"Received employee get request for '{id}'");

            var employee = _employeeService.GetEmployeeById(id);

            if (employee == null)
                return NotFound();

            return Ok(employee);
        }

        [HttpGet("reporting/{id}", Name = "getReportingStructure")]
        public IActionResult GetReportingStructure(String id)
        {
            _logger.LogDebug($"Received employee reporting structure request for '{id}'");

            var reportingStructure = _employeeService.GetReportingStructure(id);

            if (reportingStructure == null) 
                return NotFound();

            return Ok(reportingStructure);
        }

        [HttpPut("{id}")]
        public IActionResult ReplaceEmployee(String id, [FromBody]Employee newEmployee)
        {
            _logger.LogDebug($"Recieved employee update request for '{id}'");

            var existingEmployee = _employeeService.GetEmployeeById(id);
            if (existingEmployee == null)
                return NotFound();

            _employeeService.ReplaceEmployee(existingEmployee, newEmployee);

            return Ok(newEmployee);
        }

        [HttpPost("compensation")]
        public IActionResult CreateCompensation(String id, int comp, string date)
        {
            _logger.LogDebug($"Received compensation creation request for {id}");

            //fetch the employee, if they exist
			var existingEmployee = _employeeService.GetEmployeeById(id);
			if (existingEmployee == null)
				return NotFound();

            //parse date string
			DateTime dateTime = DateTime.ParseExact(date, "yyyy-MM-dd", System.Globalization.CultureInfo.InvariantCulture);
			
            //assemble Compensation object
            Compensation compensation = new Compensation()
			{
				Employee = existingEmployee as Employee,
				Salary = comp,
				Date = dateTime
			};

            //add to database and return result
			var result = _employeeService.CreateCompensation(compensation);
			if (result == null) return NotFound();

			return Ok(result);
		}

        [HttpGet("compensation/{id}")]
        public IActionResult GetCompensationById(String id)
        {
			_logger.LogDebug($"Received compensation get request for {id}");

            var compensation = _employeeService.GetCompensationById(id);
            if (compensation == null)
                return NotFound();

            return Ok(compensation);
		}
    }
}
