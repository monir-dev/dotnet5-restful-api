using dotnet5_jwt_starter.Api.Authentication;
using dotnet5_jwt_starter.Api.Dtos;
using dotnet5_jwt_starter.Api.Models;
using dotnet5_jwt_starter.Api.Repository;
using dotnet5_jwt_starter.Api.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnet5_jwt_starter.Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeesController : ControllerBase
    {

        #region Declaration
        private readonly IRepository<Employee> _employeeRepo;
        private readonly EmployeeService _employeeService;

        public EmployeesController(IRepository<Employee> employeeRepo, EmployeeService employeeService)
        {
            _employeeRepo = employeeRepo;
            _employeeService = employeeService;
        }

        #endregion

        #region EndPoints

        // Get All Employee
        [HttpGet]
        public IEnumerable<Employee> Get() => _employeeService.GetAllEmployees();

        // Get Employee Information
        [HttpGet("{id}")]
        public object Get(int id)
        {
            try
            {
                var employee = _employeeService.GetEmployeeById(id);

                if (employee == null) return NotFound();

                return employee;
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee not  failed! Please check user details and try again." });
            }
        }

        //Create Employee  
        [HttpPost]
        public async Task<object> Post([FromBody] CreateOrUpdateEmployeeDto employeeDto)
        {
            try
            {
                var employee = new Employee
                {
                     FirstName = employeeDto.FirstName,
                     MiddleName = employeeDto.MiddleName,
                     LastName = employeeDto.LastName
                };
                return await _employeeService.AddEmployee(employee);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee creation failed! Please check user details and try again." });
            }
        }

        // Edit Employee
        [HttpPut("{id}")]
        public object Put(int id, [FromBody] CreateOrUpdateEmployeeDto employeeDto)
        {
            try
            {
                var employee = _employeeService.GetEmployeeById(id);

                if (employee == null) return NotFound();

                // Update information
                employee.FirstName = employeeDto.FirstName;
                employee.MiddleName = employeeDto.MiddleName;
                employee.LastName = employeeDto.LastName;

                if (!_employeeService.UpdateEmployee(employee)) throw new Exception("Update Failed");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee update operation failed! Please check user details and try again." });
            }

            return Ok(new Response { Status = "Success", Message = "Employee updated successfully!" });
        }


        //Delete Employee
        [HttpDelete("{id}")]
        public object Delete(int id)
        {
            try
            {
                if (!_employeeService.DeleteEmployee(id)) throw new Exception("Delete Failed");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new Response { Status = "Error", Message = "Employee delete operation failed! Please check user details and try again." });
            }

            return Ok(new Response { Status = "Success", Message = "Employee Deleted successfully!" });
        }

        #endregion
    }
}
