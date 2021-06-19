using dotnet5_jwt_starter.Api.Models;
using dotnet5_jwt_starter.Api.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace dotnet5_jwt_starter.Api.Service
{
    public class EmployeeService
    {
        private readonly IRepository<Employee> _employeeRepo;

        public EmployeeService(IRepository<Employee> employee)
        {
            _employeeRepo = employee;
        }

        //GET All Employee 
        public IEnumerable<Employee> GetAllEmployees()
        {
            return _employeeRepo.GetAll();
        }

        //Add Employee
        public async Task<Employee> AddEmployee(Employee Employee)
        {
            return await _employeeRepo.Create(Employee);
        }

        //Get Employee Details By Employee Id  
        public Employee GetEmployeeById(int id)
        {
            return _employeeRepo.GetByIdAsync(id);
        }


        //Update Employee Details  
        public bool UpdateEmployee(Employee Employee)
        {
            try
            {
                _employeeRepo.Update(Employee);
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }



        //Delete Employee   
        public bool DeleteEmployee(int id)
        {

            try
            {
                var employee = _employeeRepo.GetByIdAsync(id);
                _employeeRepo.Delete(employee);
                
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        
    }
}
