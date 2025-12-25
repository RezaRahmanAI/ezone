using IMSWEB.Data;
using IMSWEB.Model;
using IMSWEB.Model.TO;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly IBaseRepository<Supplier> _supplierRepository;
        private readonly IBaseRepository<StockDetail> _stockDetailRepository;
        private readonly IBaseRepository<POrderDetail> _pOrderDetailRepository;
        private readonly IBaseRepository<POrder> _pOrderRepository;
        private readonly IBaseRepository<SisterConcern> _SisterConcernRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISisterConcernService _SisterConcernService;

        public SupplierService(IBaseRepository<Supplier> supplierRepository, IBaseRepository<StockDetail> stockDetailRepository, IBaseRepository<POrderDetail> pOrderDetailRepository, IBaseRepository<POrder> pOrderRepository, IUnitOfWork unitOfWork,
            IBaseRepository<SisterConcern> concernRepository, ISisterConcernService SisterConcernService)
        {
            _supplierRepository = supplierRepository;
            _stockDetailRepository = stockDetailRepository;
            _pOrderDetailRepository = pOrderDetailRepository;
            _pOrderRepository = pOrderRepository;
            _unitOfWork = unitOfWork;
            _SisterConcernRepository = concernRepository;
            _SisterConcernService = SisterConcernService;
        }

        public void AddSupplier(Supplier Supplier)
        {
            _supplierRepository.Add(Supplier);
        }

        public void UpdateSupplier(Supplier Supplier)
        {
            _supplierRepository.Update(Supplier);
        }

        public void SaveSupplier()
        {
            _unitOfWork.Commit();
        }

        public IQueryable<Supplier> GetAllSupplier()
        {
            return _supplierRepository.All;
        }

        public async Task<IEnumerable<Supplier>> GetAllSupplierAsync()
        {
            return await _supplierRepository.GetAllSupplierAsync();
        }

        public Supplier GetSupplierById(int id)
        {
            return _supplierRepository.FindBy(x=>x.SupplierID == id).First();
        }

        public IEnumerable<Tuple<string, string, string, string, string,decimal, string>>
        ConcernWiseSupplierDueRpt(int concernId, int nSupplierId, bool IsAdminReport)
        {
            return _supplierRepository.ConcernWiseSupplierDueRpt(_SisterConcernRepository,concernId,nSupplierId,IsAdminReport);
        }

        public void DeleteSupplier(int id)
        {
            _supplierRepository.Delete(x => x.SupplierID == id);
        }

        public List<SupplierDueTO> GetCustomerDateWiseTotalDue(int supplierId, int concernId, DateTime fromDate, DateTime toDate, bool isTrialBalance, int UserConcernID)
        {
            List<SupplierDueTO> prevSupplierDue = new List<SupplierDueTO>();
            List<SupplierDueTO> prevSupplierDues = new List<SupplierDueTO>();
            List<SupplierDueTO> currentSupplierDue = new List<SupplierDueTO>();
            DateTime asOnDate = fromDate.AddDays(-1);
            #region sp call

            if (isTrialBalance)
            {
                prevSupplierDue = _supplierRepository.ExecSP<SupplierDueTO>("GetDatewiseSupplierDue @concernId, @SupplierId, @asOnDate, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                       new SqlParameter("SupplierId", SqlDbType.Int) { Value = supplierId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();

                currentSupplierDue = _supplierRepository.ExecSP<SupplierDueTO>("GetDatewiseSupplierDue @concernId, @SupplierId, @asOnDate, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                       new SqlParameter("SupplierId", SqlDbType.Int) { Value = supplierId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = DBNull.Value },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();
            }
            else
            {
                asOnDate = DateTime.Now;
                if (concernId > 0)
                {
                    prevSupplierDue = _supplierRepository.ExecSP<SupplierDueTO>("GetDatewiseSupplierDue @concernId, @SupplierId, @asOnDate, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = concernId },
                       new SqlParameter("SupplierId", SqlDbType.Int) { Value = supplierId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();
                    currentSupplierDue = null;
                }
                else
                {

                    //var concernData = _SisterConcernRepository.GetAll().ToList(); 
                    var concernData = _SisterConcernService.GetFamilyTree(UserConcernID);
                    foreach (var concern in concernData)
                    {
                        var concernID = concern.ConcernID;
                        prevSupplierDues = _supplierRepository.ExecSP<SupplierDueTO>("GetDatewiseSupplierDue @concernId, @SupplierId, @asOnDate, @fromDate, @toDate",
                       new SqlParameter("concernId", SqlDbType.Int) { Value = concernID },
                       new SqlParameter("SupplierId", SqlDbType.Int) { Value = supplierId },
                       new SqlParameter("asOnDate", SqlDbType.DateTime) { Value = asOnDate },
                       new SqlParameter("fromDate", SqlDbType.DateTime) { Value = fromDate },
                       new SqlParameter("toDate", SqlDbType.DateTime) { Value = toDate }
                        ).ToList();
                        prevSupplierDue.AddRange(prevSupplierDues);
                    }


                        
                    currentSupplierDue = null;
                }
    
            }
            #endregion
            //List<SupplierDueTO> supplierInfoes;
            List<SupplierDueTO> supplierInfoes = new List<SupplierDueTO>();
            if (concernId > 0)
            {
                #region supplier info query
                string query = @"SELECT s.SupplierID, s.Code, s.Name, s.OwnerName, s.ContactNo, s.Address, s.TotalDue, s.OpeningDue, sc.Name AS ConcernName
                      FROM dbo.Suppliers s
                      JOIN SisterConcerns sc ON s.ConcernID = sc.ConcernID
                      WHERE (@supplierId = 0 OR s.SupplierID = @supplierId) AND s.ConcernID = @concernId";

                supplierInfoes = _supplierRepository.ExecSP<SupplierDueTO>(query,
                    new SqlParameter("supplierId", SqlDbType.Int) { Value = supplierId },
                    new SqlParameter("concernId", SqlDbType.Int) { Value = concernId }
                ).ToList();
                #endregion
            }
            else
            {
                #region supplier info query
                var concernData = _SisterConcernRepository.GetAll().ToList();
                foreach (var concern in concernData)
                {
                    var concernID = concern.ConcernID;
                    string query = @"SELECT s.SupplierID, s.Code, s.Name, s.OwnerName, s.ContactNo, s.Address, s.TotalDue, s.OpeningDue, sc.Name AS ConcernName
                      FROM dbo.Suppliers s
                      JOIN SisterConcerns sc ON s.ConcernID = sc.ConcernID
                      WHERE (@supplierId = 0 OR s.SupplierID = @supplierId) AND s.ConcernID = @concernID";
                    List<SupplierDueTO> supplierInfoesForConcern = _supplierRepository.ExecSP<SupplierDueTO>(query,
                        new SqlParameter("supplierId", SqlDbType.Int) { Value = supplierId },
                        new SqlParameter("concernId", SqlDbType.Int) { Value = concernID }
                    ).ToList();
                    supplierInfoes.AddRange(supplierInfoesForConcern);
                }
                #endregion
            }

            #region combined final result
            if (prevSupplierDue.Any())
            {
                foreach (var item in prevSupplierDue)
                {
                    //if (item.CustomerID == 196794)
                    //{
                    //    string msg = string.Empty;
                    //}
                    SupplierDueTO supplier = supplierInfoes.Where(c => c.SupplierID == item.SupplierID).FirstOrDefault();
                    item.Code = supplier.Code;
                    item.Name = supplier.Name;
                    item.ContactNo = supplier.ContactNo;
                    item.OwnerName = supplier.OwnerName;
                    item.Address = supplier.Address;
                    item.ConcernName = supplier.ConcernName;
                    item.TotalDue = item.TotalDue;
                }
            }
            #endregion

            return prevSupplierDue;
        }

        public int GetSupplierIdBySDetailId(int id)
        {
           return _supplierRepository.GetSupplierIdBySDetailId(_stockDetailRepository,_pOrderRepository,_pOrderDetailRepository, id);
        }
    
    }
}
