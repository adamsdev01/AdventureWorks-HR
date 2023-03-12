using HumanResources.Data.Models;
using HumanResources.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using Telerik.DataSource;
using Telerik.DataSource.Extensions;

namespace HumanResources.Data.Services
{
    public class HumanResourcesService
    {
        private readonly AdventureContext _dbContext;

        public HumanResourcesService(AdventureContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Employees View Object
        public async Task<DataSourceResult> ReadVEmployeesByQueryArg(DataSourceRequest queryAttrib)
        {
            queryAttrib.Filters.Add(new FilterDescriptor
            {
                Member = "BusinessEntityId"
            });

            var employees = await _dbContext.VEmployees
                .Where(e => e.BusinessEntityId > 1)
                .OrderByDescending(e => e.BusinessEntityId)
                .ToDataSourceResultAsync(queryAttrib);

            return employees;
        }

        public async Task<List<VEmployee>> GetEmployees_View()
        {
            return await _dbContext.VEmployees.ToListAsync();
        }

        public VEmployee GetEmployee_View(int businessEntityId) 
        {
            var data = _dbContext.VEmployees
                .Where(e => e.BusinessEntityId == businessEntityId)
                .FirstOrDefault();

            return data;
        }
        #endregion

        #region Employee Object

        public async Task<DataSourceResult> ReadEmployeesByQueryArg(DataSourceRequest queryAttrib)
        {
            queryAttrib.Filters.Add(new FilterDescriptor
            {
                Member = "CurrentFlag",
                Value = true
            });

            var employees = await _dbContext.Employees
                .Where(e => e.CurrentFlag == true)
                .OrderByDescending(e => e.ModifiedDate)
                .ToDataSourceResultAsync(queryAttrib);

            return employees;
        }
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await _dbContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployee(int businessEntityId)
        {
            return await _dbContext.Employees
                .Include(e => e.BusinessEntity)
                .Include(e => e.SalesPerson)
                .Include(e => e.EmployeeDepartmentHistories)
                .Include(e => e.EmployeePayHistories)
                .Include(e => e.JobCandidates)
                .Include(e => e.PurchaseOrderHeaders)
                .FirstOrDefaultAsync(e => e.BusinessEntityId == businessEntityId);
        }

        public async Task<Employee> AddEmployee(Employee employee)
        {
            employee.ModifiedDate= DateTime.Now;

            var result = await _dbContext.Employees.AddAsync(employee);
            await _dbContext.SaveChangesAsync();

            return result.Entity;
        }

        public async Task<Employee> UpdateEmployee(Employee employee)
        {
            var result = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.BusinessEntityId == employee.BusinessEntityId);

            if (result != null)
            {
                result.NationalIdnumber = employee.NationalIdnumber;
                result.LoginId = employee.LoginId;
                result.OrganizationLevel = employee.OrganizationLevel;
                result.JobTitle = employee.JobTitle;
                result.Gender = employee.Gender;
                result.BirthDate = employee.BirthDate;
                result.Gender = employee.Gender;
                result.HireDate = employee.HireDate;
                result.SalariedFlag = employee.SalariedFlag;
                result.MaritalStatus = employee.MaritalStatus;
                result.VacationHours = employee.VacationHours;
                result.SickLeaveHours = employee.SickLeaveHours;
                result.CurrentFlag = employee.CurrentFlag;
                result.Rowguid = new Guid();
                result.ModifiedDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();

                return result;
            }

            return null;
        }

        public async void DeleteEmployee(int businessEntityId)
        {
            var result = await _dbContext.Employees
                .FirstOrDefaultAsync(e => e.BusinessEntityId == businessEntityId);
            
            if (result != null)
            {
                //_dbContext.Employees.Remove(result);
                result.CurrentFlag= false;
                result.ModifiedDate = DateTime.Now;

                await _dbContext.SaveChangesAsync();
            }
        }
        #endregion

        #region EmployeeDepartmentHistory Object
        public async Task<EmployeeDepartmentHistory> GetEmployeeDepartmentHistory(int businessEntityId)
        {
            return await _dbContext.EmployeeDepartmentHistories
                .Include(e => e.BusinessEntity)
                .Include(e => e.Department)
                .Include(e => e.Shift)
                .FirstOrDefaultAsync(e => e.BusinessEntityId == businessEntityId);
        }
        #endregion

        #region Shift Object
        public async Task<Shift> GetShift(int shiftId)
        {
            return await _dbContext.Shifts
                .Include(e => e.EmployeeDepartmentHistories)
                .FirstOrDefaultAsync(e => e.ShiftId == shiftId);
        }
        #endregion

        #region Department Object
        public async Task<DataSourceResult> ReadDepartmentsByQueryArg(DataSourceRequest queryAttrib)
        {
            queryAttrib.Filters.Add(new FilterDescriptor
            {
                Member = "DepartmentId"
            });

            var departments = await _dbContext.Departments
                .Where(e => e.DepartmentId > 1)
                .OrderBy(e => e.Name)
                .ToDataSourceResultAsync(queryAttrib);

            return departments;
        }
        #endregion

        #region JobCandidate Object
        public async Task<DataSourceResult> ReadJobCandidatesByQueryArg(DataSourceRequest queryAttrib)
        {
            queryAttrib.Filters.Add(new FilterDescriptor
            {
                Member = "JobCandidateId"
            });

            var candidates = await _dbContext.JobCandidates
                .Where(e => e.JobCandidateId > 1)
                .OrderBy(e => e.JobCandidateId)
                .ToDataSourceResultAsync(queryAttrib);

            return candidates;
        }

        public JobCandidate GetJobCandidate(int jobCandidateId)
        {
            var data = _dbContext.JobCandidates
                .Where(e => e.JobCandidateId == jobCandidateId)
                .FirstOrDefault();

            return data;
        }

        public async Task<List<JobCandidate>> GetJobCandidates()
        {
            return await _dbContext.JobCandidates.ToListAsync();
        }
        #endregion

        #region Person Object
        public async Task<List<Person>> GetPeople()
        {
            return await _dbContext.People.ToListAsync();
        }

        public async Task<List<Person>> GetPeople_CurrentEmployeesOnly()
        {
            return await _dbContext.People
                .Where(p => p.PersonType == "EM")
                .ToListAsync();
        }

        public async Task<int> GetPeople_EmployeesCount()
        {
            var people = await _dbContext.People
                .Where(p => p.PersonType == "EM")
                .CountAsync();

            return people;
        }

        public async Task<int> GetPeople_ReturnOneRandomPerson()
        {
            // Creating object of random class
            Random random = new Random();

            var randomEmployees = await _dbContext.People
                .Where(p => p.PersonType == "EM")
                .OrderBy(random => random.BusinessEntityId)
                .Select(o => new Person
                {
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    BusinessEntityId = o.BusinessEntityId
                }).ToListAsync();

            var randomEmployee = random.Next(randomEmployees.Count);

            return randomEmployee;
        }

        /// <summary>
        /// Generate Random person from employees list
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetRandomPerson()
        {
            // Creating object of random class
            Random random = new Random();

            var randomEmployees = await _dbContext.People
                .Where(p => p.PersonType == "EM")
                .Select(o => new Person
                {
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                    BusinessEntityId = o.BusinessEntityId
                }).ToListAsync();

            var randomEmployee = random.Next(randomEmployees.Count);

            string Name = "";

            Name = (string)randomEmployees[randomEmployee].FirstName + " " + (string)randomEmployees[randomEmployee].LastName;

            return Name;
        }

        public async Task<IEnumerable<Person>> GetFiveRandomEmployees()
        {
            // Creating object of random class
            Random random = new Random();

            // List of employees
            var employees = await _dbContext.People
                .Where(p => p.PersonType == "EM")
                .Select(o => new Person
                {
                    BusinessEntityId = o.BusinessEntityId,
                    FirstName = o.FirstName,
                    LastName = o.LastName,
                }).ToListAsync();

            // get 5 random emps from list
            var list = employees.OrderBy(x => random.Next()).Take(5);

            return list;
        }
        #endregion      
    }
}
