using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class ExpenseItemService : IExpenseItemService
    {
        private readonly IBaseRepository<ExpenseItem> _expenseItemRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ExpenseItemService(IBaseRepository<ExpenseItem> expenseItemRepository, IUnitOfWork unitOfWork)
        {
            _expenseItemRepository = expenseItemRepository;
            _unitOfWork = unitOfWork;
        }

        public void AddExpenseItem(ExpenseItem ExpenseItem)
        {
            _expenseItemRepository.Add(ExpenseItem);
        }

        public void UpdateExpenseItem(ExpenseItem ExpenseItem)
        {
            _expenseItemRepository.Update(ExpenseItem);
        }

        public void SaveExpenseItem()
        {
            _unitOfWork.Commit();
        }

        public IEnumerable<ExpenseItem> GetAllExpenseItem()
        {
            return _expenseItemRepository.All.ToList();
        }

        public async Task<IEnumerable<ExpenseItem>> GetAllExpenseItemAsync()
        {
            return await _expenseItemRepository.GetAllExpenseItemAsync();
        }

        public ExpenseItem GetExpenseItemById(int id)
        {
            return _expenseItemRepository.FindBy(x=>x.ExpenseItemID == id).First();
        }

        public void DeleteExpenseItem(int id)
        {
            _expenseItemRepository.Delete(x => x.ExpenseItemID == id);
        }

        public IQueryable<ExpenseItem> GetAll()
        {
            return _expenseItemRepository.All;
        }
        public IQueryable<ExpenseItem> GetAll(int concernId)
        {
            return _expenseItemRepository.GetAll().Where(e => e.ConcernID == concernId);
        }
    }
}
