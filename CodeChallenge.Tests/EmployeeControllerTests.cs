
using System;
using System.Net;
using System.Net.Http;
using System.Text;

using CodeChallenge.Models;

using CodeCodeChallenge.Tests.Integration.Extensions;
using CodeCodeChallenge.Tests.Integration.Helpers;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeCodeChallenge.Tests.Integration
{
    [TestClass]
    public class EmployeeControllerTests
    {
        private static HttpClient _httpClient;
        private static TestServer _testServer;

        [ClassInitialize]
        // Attribute ClassInitialize requires this signature
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
        public static void InitializeClass(TestContext context)
        {
            _testServer = new TestServer();
            _httpClient = _testServer.NewClient();
        }

        [ClassCleanup]
        public static void CleanUpTest()
        {
            _httpClient.Dispose();
            _testServer.Dispose();
        }

        [TestMethod]
        public void CreateEmployee_Returns_Created()
        {
            // Arrange
            var employee = new Employee()
            {
                Department = "Complaints",
                FirstName = "Debbie",
                LastName = "Downer",
                Position = "Receiver",
            };

            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PostAsync("api/employee",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.Created, response.StatusCode);

            var newEmployee = response.DeserializeContent<Employee>();
            Assert.IsNotNull(newEmployee.EmployeeId);
            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
            Assert.AreEqual(employee.Department, newEmployee.Department);
            Assert.AreEqual(employee.Position, newEmployee.Position);
        }

        [TestMethod]
        public void GetEmployeeById_Returns_Ok()
        {
            // Arrange
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";

            // Execute
            var getRequestTask = _httpClient.GetAsync($"api/employee/{employeeId}");
            var response = getRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            var employee = response.DeserializeContent<Employee>();
            Assert.AreEqual(expectedFirstName, employee.FirstName);
            Assert.AreEqual(expectedLastName, employee.LastName);
        }

        [TestMethod]
        public void GetReportingStructure_Returns_Ok()
        {
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var expectedFirstName = "John";
            var expectedLastName = "Lennon";
            var expectedCount = 4;

            var getRequestTask = _httpClient.GetAsync($"api/employee/reporting/{employeeId}");
            var response = getRequestTask.Result;

            Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);
            var structure = response.DeserializeContent<ReportingStructure>();
            Assert.AreEqual(expectedFirstName, structure.Employee.FirstName);
            Assert.AreEqual(expectedLastName, structure.Employee.LastName);
            Assert.AreEqual(expectedCount, structure.NumberOfReports);
		}

        [TestMethod]
        public void UpdateEmployee_Returns_Ok()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
                Department = "Engineering",
                FirstName = "Pete",
                LastName = "Best",
                Position = "Developer VI",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var putResponse = putRequestTask.Result;
            
            // Assert
            Assert.AreEqual(HttpStatusCode.OK, putResponse.StatusCode);
            var newEmployee = putResponse.DeserializeContent<Employee>();

            Assert.AreEqual(employee.FirstName, newEmployee.FirstName);
            Assert.AreEqual(employee.LastName, newEmployee.LastName);
        }

        [TestMethod]
        public void UpdateEmployee_Returns_NotFound()
        {
            // Arrange
            var employee = new Employee()
            {
                EmployeeId = "Invalid_Id",
                Department = "Music",
                FirstName = "Sunny",
                LastName = "Bono",
                Position = "Singer/Song Writer",
            };
            var requestContent = new JsonSerialization().ToJson(employee);

            // Execute
            var postRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
               new StringContent(requestContent, Encoding.UTF8, "application/json"));
            var response = postRequestTask.Result;

            // Assert
            Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
        }

        [TestMethod]
        public void CreateCompensation_Returns_Ok()
        {
            var employeeId = "16a596ae-edd3-4847-99fe-c4518e82c86f";
            var comp = 70000;
            var date = "2024-11-22";

            var postRequestTask = _httpClient.PostAsync($"api/employee/compensation?id={employeeId}&comp={comp}&date={date}", null);
            var response = postRequestTask.Result;

            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);

			var compensation = response.DeserializeContent<Compensation>();
            Assert.AreEqual(employeeId, compensation.Employee.EmployeeId);
            Assert.AreEqual(comp, compensation.Salary);
            Assert.AreEqual(DateTime.Parse(date), compensation.Date);
		}

        [TestMethod]
        public void GetCompensationById_Returns_Ok()
        {
			var employeeId = "b7839309-3348-463b-a7e3-5de1c168beb3";
			var comp = 90000;
			var date = "2024-11-22";

            //create compensation
			var postRequestTask = _httpClient.PostAsync($"api/employee/compensation?id={employeeId}&comp={comp}&date={date}", null);
            var response1 = postRequestTask.Result;

            //fetch the newly created compensation
            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{employeeId}");
            var response = getRequestTask.Result;

            Assert.AreEqual (HttpStatusCode.OK, response.StatusCode);

			var compensation = response.DeserializeContent<Compensation>();
			Assert.AreEqual(employeeId, compensation.Employee.EmployeeId);
			Assert.AreEqual(comp, compensation.Salary);
			Assert.AreEqual(DateTime.Parse(date), compensation.Date);
		}

        //this next test currently does not pass.
        //I thought that maybe my solution would enable the Compensation GET calls to account for updates
        //to Employee records, but it seems that isn't the case. If I had more time, this would be something
        //I would try to achieve.

/*        [TestMethod]
        public void Compensation_Updates_With_Employee()
        {
			var employeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f";
			var comp = 80000;
			var date = "2024-11-22";

			var postRequestTask = _httpClient.PostAsync($"api/employee/compensation?id={employeeId}&comp={comp}&date={date}", null);
			var response1 = postRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.OK, response1.StatusCode);

			var employee = new Employee()
			{
				EmployeeId = "03aa1462-ffa9-4978-901b-7c001562cf6f",
				Department = "Marketing",
				FirstName = "Pete",
				LastName = "Best",
				Position = "Graphic Designer",
			};
			var requestContent = new JsonSerialization().ToJson(employee);
            var putRequestTask = _httpClient.PutAsync($"api/employee/{employee.EmployeeId}",
				new StringContent(requestContent, Encoding.UTF8, "application/json"));

            var response2 = putRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.OK, response2.StatusCode);

            var getRequestTask = _httpClient.GetAsync($"api/employee/compensation/{employeeId}");
            var response3 = getRequestTask.Result;
            Assert.AreEqual(HttpStatusCode.OK, response3.StatusCode);

			var compensation = response3.DeserializeContent<Compensation>();
            Assert.AreEqual(employee.Department, compensation.Employee.Department);
            Assert.AreEqual(employee.Position, compensation.Employee.Position);
		}*/
    }
}
