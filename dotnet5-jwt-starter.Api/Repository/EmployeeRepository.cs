using dotnet5_jwt_starter.Api.Authentication;
using dotnet5_jwt_starter.Api.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet5_jwt_starter.Api.Repository
{
    public class EmployeeRepository : IRepository<Employee>
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Employee> GetAll()
        {
            return _context.Employees.ToList();
        }

        public async Task<Employee> Create(Employee _object)
        {
            var obj = await _context.Employees.AddAsync(_object);
            _context.SaveChanges();
            return obj.Entity;
        }

        public Employee GetByIdAsync(int id)
        {
            return _context.Employees.Find(id);
        }

        public void Update(Employee _object)
        {
            _context.Employees.Update(_object);
            _context.SaveChanges();
        }

        public void Delete(Employee _object)
        {
            _context.Remove(_object);
            _context.SaveChanges();
        }
    }
}
