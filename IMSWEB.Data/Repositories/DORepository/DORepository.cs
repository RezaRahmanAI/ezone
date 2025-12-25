using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Model;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using IMSWEB.Model.SPModel;
using System.Data.SqlTypes;
using System.Reflection;
using System.Web;
using Microsoft.AspNet.Identity;
namespace IMSWEB.Data
{
    public class DORepository : IDORepository
    {
        private IMSWEBContext _dbContext;

        #region Properties

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected IMSWEBContext DbContext
        {
            get { return _dbContext ?? (_dbContext = DbFactory.Init()); }
        }

        public DORepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        #endregion

        public Tuple<bool, int> ADDDOEntry(DO newDo, int DOID)
        {

            Tuple<bool, int> Result = new Tuple<bool, int>(false, 0);

            using (var trans = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (DOID > 0)//update 
                    {
                        var oldDO = DbContext.DOes.Where(i => i.DOID == DOID)
                                    .Include(i => i.DODetails).SingleOrDefault();
                        newDo.CreatedBy = oldDO.CreatedBy;
                        newDo.CreateDate = oldDO.CreateDate;
                        newDo.ConcernID = oldDO.ConcernID;

                        //old Customer Due update
                        if (oldDO.PaidAmt > 0 && oldDO.CustomerID > 0)
                        {
                            var oldCustomer = DbContext.Customers.Find(oldDO.CustomerID);
                            oldCustomer.TotalDue += oldDO.PaidAmt;
                        }
                        //old suppliers Due update
                        if (oldDO.PaidAmt > 0 && oldDO.SupplierID > 0)
                        {
                            var oldSupplier = DbContext.Suppliers.Find(oldDO.SupplierID);
                            oldSupplier.TotalDue += oldDO.PaidAmt;
                        }

                        //update Parent
                        DbContext.Entry(oldDO).CurrentValues.SetValues(newDo);

                        //delete children
                        foreach (var existingChild in oldDO.DODetails.ToList())
                        {
                            if (!newDo.DODetails.Any(c => c.DODID == existingChild.DODID))
                                DbContext.DODetails.Remove(existingChild);
                        }

                        //update and insert children
                        foreach (var item in newDo.DODetails)
                        {
                            var oldChild = DbContext.DODetails.FirstOrDefault(i => i.DODID == item.DODID);
                            //update
                            if (oldChild != null)
                                DbContext.Entry(oldChild).CurrentValues.SetValues(item);
                            else //insert
                            {
                                item.DOID = DOID;
                                DbContext.DODetails.Add(item);
                            }
                        }
                    }
                    else // New DO Add
                    {
                        DbContext.DOes.Add(newDo);
                    }

                    //New customer due update
                    if (newDo.PaidAmt > 0 && newDo.CustomerID > 0)
                    {
                        var newCustomer = DbContext.Customers.Find(newDo.CustomerID);
                        newCustomer.TotalDue -= newDo.PaidAmt;
                    }
                    //New Supplier Due Update
                    if (newDo.PaidAmt > 0 && newDo.SupplierID > 0)
                    {
                        var newSupplier = DbContext.Suppliers.Find(newDo.SupplierID);
                        newSupplier.TotalDue -= newDo.PaidAmt;
                    }
                    DbContext.SaveChanges();
                    trans.Commit();
                    Result = new Tuple<bool, int>(true, newDo.DOID);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    Result = new Tuple<bool, int>(false, 0);
                }
            };

            return Result;
        }



        public bool DeleteByID(int DOID, int UserID, DateTime dateTime)
        {

            bool Result = false;
            using (var trans = DbContext.Database.BeginTransaction())
            {
                try
                {
                    if (DOID > 0)
                    {
                        var OldFBEntry = DbContext.DOes.Find(DOID);
                        if (OldFBEntry != null)
                        {
                            OldFBEntry.ModifiedBy = UserID;
                            OldFBEntry.Status = EnumDOStatus.Return;
                        }
                        Result = DbContext.SaveChanges() > 0;
                        trans.Commit();
                    }
                }
                catch (Exception)
                {
                    trans.Rollback();
                    Result = false;
                }
            };
            return Result;
        }

    }
}
