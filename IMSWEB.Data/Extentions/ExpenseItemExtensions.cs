using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Data
{
    public static class ExpenseItemExtensions
    {
        public static async Task<IEnumerable<ExpenseItem>> GetAllExpenseItemAsync(this IBaseRepository<ExpenseItem> expenseItemRepository)
        {
            return await expenseItemRepository.All.ToListAsync();
        }
    }
}
