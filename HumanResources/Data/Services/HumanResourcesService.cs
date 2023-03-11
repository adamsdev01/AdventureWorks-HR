using HumanResources.Data.Models;
using HumanResources.Data.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
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
