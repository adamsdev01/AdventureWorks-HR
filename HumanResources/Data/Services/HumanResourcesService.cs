using HumanResources.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;

namespace HumanResources.Data.Services
{
    public class HumanResourcesService
    {
        private readonly AdventureContext _dbContext;

        public HumanResourcesService(AdventureContext dbContext)
        {
            _dbContext = dbContext;
        }

        #region Employee Object
        public async Task<IEnumerable<Employee>> GetEmployees()
        {
            return await _dbContext.Employees.ToListAsync();
        }

        public async Task<Employee> GetEmployee(int businessEntityId)
        {
            return await _dbContext.Employees
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

    }
}
