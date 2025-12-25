using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
namespace IMSWEB.Data
{
    public static class CommissionSetupExtensions
    {
        public static async Task<IEnumerable<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>>> GetAllAsync(this IBaseRepository<CommissionSetup> CommissionRepository,
            IBaseRepository<Employee> EmployeeRepository
            )
        {
            var result = await (from com in CommissionRepository.All
                                join emp in EmployeeRepository.All on com.EmployeeID equals emp.EmployeeID
                                select new
                                {
                                    com.CSID,
                                    com.CommissionMonth,
                                    com.AchievedPercentEnd,
                                    com.AchievedPercentStart,
                                    com.CommissionPercent,
                                    com.CommisssionAmt,
                                    com.Status,
                                    emp.Name
                                }).ToListAsync();

            return result.Select(x => new Tuple<int, DateTime, decimal, decimal, decimal, decimal, int,Tuple<string>>(
                x.CSID,
                x.CommissionMonth,
                x.AchievedPercentStart,
                x.AchievedPercentEnd,
                x.CommissionPercent,
                x.CommisssionAmt,
                x.Status, new Tuple<string>(x.Name)
                ));
        }
    }
}
