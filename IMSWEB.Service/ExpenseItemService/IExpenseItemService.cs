using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IExpenseItemService
    {
        void AddExpenseItem(ExpenseItem ExpenseItem);
        void UpdateExpenseItem(ExpenseItem ExpenseItem);
        void SaveExpenseItem();
        IEnumerable<ExpenseItem> GetAllExpenseItem();
        Task<IEnumerable<ExpenseItem>> GetAllExpenseItemAsync();
        ExpenseItem GetExpenseItemById(int id);
        void DeleteExpenseItem(int id);

        IQueryable<ExpenseItem> GetAll();
        IQueryable<ExpenseItem> GetAll(int concernId);
    }
}
