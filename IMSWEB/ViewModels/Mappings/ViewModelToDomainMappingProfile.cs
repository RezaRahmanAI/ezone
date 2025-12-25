using AutoMapper;
using IMSWEB.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class ViewModelToDomainMappingProfile : Profile
    {
        public ViewModelToDomainMappingProfile()
        {
            #region CreateProductViewModel, Product
            CreateMap<CreateProductViewModel, Product>()
            .ForMember(m => m.ProductID, map => map.MapFrom(vm => int.Parse(vm.ProductId)))
            .ForMember(m => m.PurchaseRate, map => map.MapFrom(vm => vm.MRP))
            .ForMember(m => m.ProductName, map => map.MapFrom(vm => vm.ProductName))
            .ForMember(m => m.PicturePath, map => map.MapFrom(vm => vm.PicturePath))
            .ForMember(m => m.CategoryID, map => map.MapFrom(vm => int.Parse(vm.CategoryId)))
            .ForMember(m => m.CompanyID, map => map.MapFrom(vm => int.Parse(vm.CompanyId)))
            .ForMember(m => m.UnitType, map => map.MapFrom(vm => vm.UnitType))
            .ForMember(m => m.PWDiscount, map => map.MapFrom(vm => int.Parse(vm.PWDiscount)))
            .ForMember(m => m.DisDurationFDate, map => map.MapFrom(vm => vm.DisDurationFDate))
            .ForMember(m => m.DisDurationToDate, map => map.MapFrom(vm => vm.DisDurationToDate))
            .ForMember(m => m.ProductType, map => map.MapFrom(vm => vm.ProductType))
            .ForMember(m => m.RP, map => map.MapFrom(vm => vm.ECOMRP))
            .ForMember(m => m.RP, map => map.MapFrom(vm => vm.RP))
            ;
            #endregion

            #region Tuple, CreateBankTransactionViewModel
            CreateMap<CreateBankTransactionViewModel, BankTransaction>()
            .ForMember(vm => vm.BankTranID, map => map.MapFrom(m => m.BankTranID))
            .ForMember(vm => vm.TranDate, map => map.MapFrom(m => m.TranDate))
            .ForMember(vm => vm.BankID, map => map.MapFrom(m => m.BankID))
            .ForMember(vm => vm.CustomerID, map => map.MapFrom(m => m.CustomerID))
            .ForMember(vm => vm.SupplierID, map => map.MapFrom(m => m.SupplierID))
            .ForMember(vm => vm.AnotherBankID, map => map.MapFrom(m => m.AnotherBankID))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => (int)m.TransactionType))
            .ForMember(vm => vm.TransactionNo, map => map.MapFrom(m => m.TransactionNo))
            .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Amount))
            .ForMember(vm => vm.ChecqueNo, map => map.MapFrom(m => m.ChecqueNo))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks))
            .ForMember(vm => vm.ExpenseItemID, map => map.MapFrom(m => m.ExpenseItemID))
            .ForMember(vm => vm.CreatedBy, map => map.MapFrom(m => m.CreatedBy))
            .ForMember(vm => vm.CreateDate, map => map.MapFrom(m => m.CreateDate))
            .ForMember(vm => vm.ModifiedBy, map => map.MapFrom(m => m.ModifiedBy))
            .ForMember(vm => vm.ModifiedDate, map => map.MapFrom(m => m.ModifiedDate));
            ;
            #endregion

            #region CreateUserViewModel, ApplicationUser
            CreateMap<CreateUserViewModel, ApplicationUser>()
            .ForMember(m => m.Id, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Email, map => map.MapFrom(vm => vm.Email))
            .ForMember(m => m.UserName, map => map.MapFrom(vm => vm.UserName))
            .ForMember(m => m.EmployeeID, map => map.MapFrom(vm => vm.EmployeeId))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)))
            .ForMember(m => m.PhoneNumber, map => map.MapFrom(vm => vm.PhoneNumber));
            #endregion

            #region GetCustomerViewModel, Customer
            CreateMap<GetCustomerViewModel, Customer>()
            .ForMember(m => m.CustomerID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.MemberID, map => map.MapFrom(vm => vm.MemberID))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.PhotoPath, map => map.MapFrom(vm => vm.PhotoPath))
            .ForMember(m => m.TotalDue, map => map.MapFrom(vm => decimal.Parse(vm.TotalDue)))
            .ForMember(m => m.CustomerType, map => map.MapFrom(vm => vm.CustomerType))
            .ForMember(m => m.CusDueLimit, map => map.MapFrom(vm => decimal.Parse(vm.CusDueLimit)));
            #endregion

            #region CreateCustomerViewModel, Customer
            CreateMap<CreateCustomerViewModel, Customer>()
            .ForMember(m => m.CustomerID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.MemberID, map => map.MapFrom(vm => vm.MemberID))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.PhotoPath, map => map.MapFrom(vm => vm.PhotoPath))
            .ForMember(m => m.TotalDue, map => map.MapFrom(vm => decimal.Parse(vm.TotalDue)))
            .ForMember(m => m.OpeningDue, map => map.MapFrom(vm => decimal.Parse(vm.OpeningDue)))
            .ForMember(m => m.CustomerType, map => map.MapFrom(vm => vm.CustomerType))
            .ForMember(m => m.CusDueLimit, map => map.MapFrom(vm => decimal.Parse(vm.CusDueLimit)))
            .ForMember(m => m.FName, map => map.MapFrom(vm => vm.FName))
            .ForMember(m => m.CompanyName, map => map.MapFrom(vm => vm.CompanyId))
            .ForMember(m => m.EmailID, map => map.MapFrom(vm => vm.EmailId))
            .ForMember(m => m.NID, map => map.MapFrom(vm => vm.NId))
            .ForMember(m => m.Address, map => map.MapFrom(vm => vm.Address))
            .ForMember(m => m.RefName, map => map.MapFrom(vm => vm.RefName))
            .ForMember(m => m.RefContact, map => map.MapFrom(vm => vm.RefContact))
            .ForMember(m => m.RefFName, map => map.MapFrom(vm => vm.RefFName))
            .ForMember(m => m.Profession, map => map.MapFrom(vm => vm.Profession))
            .ForMember(m => m.RefAddress, map => map.MapFrom(vm => vm.RefAddress))
            .ForMember(m => m.EmployeeID, map => map.MapFrom(vm => int.Parse(vm.EmployeeId)))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)));
            #endregion

            #region CreateCompanyViewModel, Company
            CreateMap<CreateCompanyViewModel, Company>()
            .ForMember(m => m.CompanyID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)));
            #endregion

            #region CreateGodownViewModel, Godown
            CreateMap<CreateGodownViewModel, Godown>()
            .ForMember(m => m.GodownID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code.Trim()))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name.Trim()))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)))
            .ForMember(m => m.ISCommon, map => map.MapFrom(vm => vm.ISCommon));
            #endregion

            #region CreateSisterConcernViewModel, SisterConcern
            CreateMap<CreateSisterConcernViewModel, SisterConcern>()
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Address, map => map.MapFrom(vm => vm.Address))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.ContactNo, map => map.MapFrom(vm => vm.ContactNo))
            .ForMember(m => m.ServiceCharge, map => map.MapFrom(vm => vm.ServiceCharge))
            .ForMember(m => m.SmsContactNo, map => map.MapFrom(vm => vm.SmsContactNo));
            #endregion

            #region CreateColorViewModel, Color
            CreateMap<CreateColorViewModel, Color>()
            .ForMember(m => m.ColorID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)));
            #endregion

            #region CreateCategoryViewModel, Category
            CreateMap<CreateCategoryViewModel, Category>()
            .ForMember(m => m.CategoryID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Description, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)));
            #endregion

            #region CreateBankViewModel, Bank
            CreateMap<CreateBankViewModel, Bank>()
            .ForMember(m => m.BankID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.BankName, map => map.MapFrom(vm => vm.BankName))
            .ForMember(m => m.AccountNo, map => map.MapFrom(vm => vm.AccountNo))
            .ForMember(m => m.AccountName, map => map.MapFrom(vm => vm.AccountName))
            .ForMember(m => m.OpeningBalance, map => map.MapFrom(vm => vm.OpeningBalance))
            .ForMember(m => m.TotalAmount, map => map.MapFrom(vm => vm.TotalAmount))

            ;
            #endregion



            #region CreateRoleViewModel, ApplicationRole
            CreateMap<CreateRoleViewModel, ApplicationRole>()
            .ForMember(m => m.Id, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name));
            #endregion

            #region CreateDesignationViewModel, Designation
            CreateMap<CreateDesignationViewModel, Designation>()
            .ForMember(m => m.DesignationID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Description, map => map.MapFrom(vm => vm.Name));
            #endregion

            #region CreateExpenseItemViewModel, ExpenseItem
            CreateMap<CreateExpenseItemViewModel, ExpenseItem>()
            .ForMember(m => m.ExpenseItemID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Description, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.Status, map => map.MapFrom(vm => vm.Status));
            #endregion

            #region GetSupplierViewModel, Supplier
            CreateMap<GetSupplierViewModel, Supplier>()
            .ForMember(m => m.SupplierID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.ContactNo, map => map.MapFrom(vm => vm.ContactNo))
            .ForMember(m => m.PhotoPath, map => map.MapFrom(vm => vm.PhotoPath))
            .ForMember(m => m.TotalDue, map => map.MapFrom(vm => decimal.Parse(vm.TotalDue)))
            .ForMember(m => m.OwnerName, map => map.MapFrom(vm => vm.OwnerName));
            #endregion

            #region CreateSupplierViewModel, Supplier
            CreateMap<CreateSupplierViewModel, Supplier>()
            .ForMember(m => m.SupplierID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.PhotoPath, map => map.MapFrom(vm => vm.PhotoPath))
            .ForMember(m => m.TotalDue, map => map.MapFrom(vm => decimal.Parse(vm.TotalDue)))
            .ForMember(m => m.OpeningDue, map => map.MapFrom(vm => decimal.Parse(vm.OpeningDue)))
            .ForMember(m => m.OwnerName, map => map.MapFrom(vm => vm.OwnerName))
            .ForMember(m => m.ContactNo, map => map.MapFrom(vm => vm.ContactNo))
            .ForMember(m => m.Address, map => map.MapFrom(vm => vm.Address))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)));
            #endregion

            #region GetEmployeeViewModel, Employee
            CreateMap<GetEmployeeViewModel, Employee>()
            .ForMember(m => m.EmployeeID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.ContactNo, map => map.MapFrom(vm => vm.ContactNo))
            .ForMember(m => m.PhotoPath, map => map.MapFrom(vm => vm.PhotoPath))
            .ForMember(m => m.JoiningDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.JoiningDate)))
            .ForMember(m => m.DesignationID, map => map.MapFrom(vm => int.Parse(vm.DesignationName)));
            #endregion

            #region CreateEmployeeViewModel, Employee
            CreateMap<CreateEmployeeViewModel, Employee>()
            .ForMember(m => m.EmployeeID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.PhotoPath, map => map.MapFrom(vm => vm.PhotoPath))
            .ForMember(m => m.ContactNo, map => map.MapFrom(vm => vm.ContactNo))
            .ForMember(m => m.JoiningDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.JoiningDate)))
            .ForMember(m => m.DesignationID, map => map.MapFrom(vm => int.Parse(vm.DesignationName)))
            .ForMember(m => m.FName, map => map.MapFrom(vm => vm.FName))
            .ForMember(m => m.MName, map => map.MapFrom(vm => vm.MName))
            .ForMember(m => m.EmailID, map => map.MapFrom(vm => vm.EmailId))
            .ForMember(m => m.NID, map => map.MapFrom(vm => vm.NId))
            .ForMember(m => m.PermanentAdd, map => map.MapFrom(vm => vm.PermanentAddress))
            .ForMember(m => m.PresentAdd, map => map.MapFrom(vm => vm.PresentAddress))
            .ForMember(m => m.BloodGroup, map => map.MapFrom(vm => vm.BloodGroup))
            .ForMember(m => m.GrossSalary, map => map.MapFrom(vm => decimal.Parse(vm.GrossSalary)))
            .ForMember(m => m.DOB, map => map.MapFrom(vm => Convert.ToDateTime(vm.DateOfBirth)))
            .ForMember(m => m.SRDueLimit, map => map.MapFrom(vm => decimal.Parse(vm.SRDueLimit)))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)));
            #endregion

            #region CreateSystemInformationViewModel, SystemInformation
            CreateMap<CreateSystemInformationViewModel, SystemInformation>()
            .ForMember(m => m.SystemInfoID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => vm.ConcernID))
            .ForMember(m => m.Address, map => map.MapFrom(vm => vm.Address))
            .ForMember(m => m.Name, map => map.MapFrom(vm => vm.Name))
            .ForMember(m => m.ConcernNameB, map => map.MapFrom(vm => vm.ConcernNameB))
            .ForMember(m => m.EmailAddress, map => map.MapFrom(vm => vm.EmailAddress))
            .ForMember(m => m.TelephoneNo, map => map.MapFrom(vm => vm.TelephoneNo))
            .ForMember(m => m.EmployeePhotoPath, map => map.MapFrom(vm => vm.EmployeePhotoPath))
            .ForMember(m => m.WebAddress, map => map.MapFrom(vm => vm.WebAddress))
            .ForMember(m => m.ProductPhotoPath, map => map.MapFrom(vm => vm.ProductPhotoPath))
            .ForMember(m => m.SupplierPhotoPath, map => map.MapFrom(vm => vm.SupplierPhotoPath))
            .ForMember(m => m.CustomerPhotoPath, map => map.MapFrom(vm => vm.CustomerPhotoPath))
            .ForMember(m => m.CustomerNIDPatht, map => map.MapFrom(vm => vm.CustomerNIDPatht))
            .ForMember(m => m.SupplierDocPath, map => map.MapFrom(vm => vm.SupplierDocPath))
            .ForMember(m => m.SystemStartDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.SystemStartDate)))
            .ForMember(m => m.CompanyTitle, map => map.MapFrom(vm => vm.CompanyTitle))
            .ForMember(m => m.IsCommonBank, map => map.MapFrom(vm => vm.IsCommonBank))
            ;

            #endregion

            #region CreateExpenditureViewModel, Expenditure
            CreateMap<CreateExpenditureViewModel, Expenditure>()
            .ForMember(m => m.ExpenditureID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.VoucherNo, map => map.MapFrom(vm => vm.VoucherNo))
            .ForMember(m => m.EntryDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.EntryDate)))
            .ForMember(m => m.Purpose, map => map.MapFrom(vm => vm.Purpose))
            .ForMember(m => m.ExpenseItemID, map => map.MapFrom(vm => int.Parse(vm.ExpenseItemID)))
            .ForMember(m => m.Amount, map => map.MapFrom(vm => decimal.Parse(vm.Amount)))
            .ForMember(m => m.CreatedBy, map => map.MapFrom(vm => int.Parse(vm.CreatedBy)))
            .ForMember(m => m.CreateDate, map => map.MapFrom(vm => vm.CreateDate))
            .ForMember(m => m.ModifiedBy, map => map.MapFrom(vm => int.Parse(vm.ModifiedBy)))
            .ForMember(m => m.ModifiedDate, map => map.MapFrom(vm => vm.ModifiedDate))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernID)));
            #endregion

            #region CreateSalesOrderViewModel,SOrder
            CreateMap<CreateSalesOrderViewModel, SOrder>()
            .ForMember(m => m.CustomerID, map => map.MapFrom(vm => vm.CustomerId))
            .ForMember(m => m.GrandTotal, map => map.MapFrom(vm => vm.GrandTotal))
            .ForMember(m => m.InvoiceNo, map => map.MapFrom(vm => vm.InvoiceNo))
            .ForMember(m => m.Remarks, map => map.MapFrom(vm => vm.Remarks))
            .ForMember(m => m.NetDiscount, map => map.MapFrom(vm => vm.NetDiscount))
            .ForMember(m => m.InvoiceDate, map => map.MapFrom(vm => vm.OrderDate))
            .ForMember(m => m.PaymentDue, map => map.MapFrom(vm => vm.PaymentDue))
            .ForMember(m => m.TDPercentage, map => map.MapFrom(vm => vm.TotalDiscountPercentage))
            .ForMember(m => m.TDAmount, map => map.MapFrom(vm => vm.TotalDiscountAmount))
            .ForMember(m => m.RecAmount, map => map.MapFrom(vm => vm.RecieveAmount))
            .ForMember(m => m.VATPercentage, map => map.MapFrom(vm => vm.VATPercentage))
            .ForMember(m => m.VATAmount, map => map.MapFrom(vm => vm.VATAmount))
            .ForMember(m => m.TotalAmount, map => map.MapFrom(vm => vm.TotalAmount))
            .ForMember(m => m.Status, map => map.MapFrom(vm => vm.Status))
            .ForMember(m => m.SOrderID, map => map.MapFrom(vm => vm.SalesOrderId))
            .ForMember(m => m.TotalDue, map => map.MapFrom(vm => vm.TotalDue))
            .ForMember(m => m.CreateDate, map => map.MapFrom(vm => vm.CreateDate))
            .ForMember(m => m.TSQty, map => map.MapFrom(vm => vm.TSQty));
            #endregion

            #region CreateSalesOrderViewModel,ReplaceOrder
            CreateMap<CreateSalesOrderViewModel, ReplaceOrder>()
            .ForMember(m => m.CustomerId, map => map.MapFrom(vm => Convert.ToInt32(vm.CustomerId)))
            .ForMember(m => m.GrandTotal, map => map.MapFrom(vm => vm.GrandTotal))
            .ForMember(m => m.InvoiceNo, map => map.MapFrom(vm => vm.InvoiceNo))
            .ForMember(m => m.NetDiscount, map => map.MapFrom(vm => vm.NetDiscount))
            .ForMember(m => m.OrderDate, map => map.MapFrom(vm => vm.OrderDate))
            .ForMember(m => m.PaymentDue, map => map.MapFrom(vm => vm.PaymentDue))
            .ForMember(m => m.TotalDiscountPercentage, map => map.MapFrom(vm => vm.TotalDiscountPercentage))
            .ForMember(m => m.TotalDiscountPercentage, map => map.MapFrom(vm => vm.TotalDiscountPercentage))
            .ForMember(m => m.RecieveAmount, map => map.MapFrom(vm => vm.RecieveAmount))
            .ForMember(m => m.VATPercentage, map => map.MapFrom(vm => vm.VATPercentage))
            .ForMember(m => m.VATAmount, map => map.MapFrom(vm => vm.VATAmount))
            .ForMember(m => m.TotalAmount, map => map.MapFrom(vm => vm.TotalAmount))
            .ForMember(m => m.Status, map => map.MapFrom(vm => vm.Status))
            .ForMember(m => m.SalesOrderId, map => map.MapFrom(vm => vm.SalesOrderId))
            .ForMember(m => m.TotalDue, map => map.MapFrom(vm => vm.TotalDue));
            #endregion

            #region CreateSalesOrderDetailViewModel,SOrderDetail
            CreateMap<CreateSalesOrderDetailViewModel, SOrderDetail>()
            .ForMember(m => m.SOrderDetailID, map => map.MapFrom(vm => vm.SODetailId))
            .ForMember(m => m.SOrderID, map => map.MapFrom(vm => vm.SalesOrderId))
            .ForMember(m => m.ProductID, map => map.MapFrom(vm => vm.ProductId))
            .ForMember(m => m.SDetailID, map => map.MapFrom(vm => vm.StockDetailId))
            .ForMember(m => m.Quantity, map => map.MapFrom(vm => vm.Quantity))
            .ForMember(m => m.UnitPrice, map => map.MapFrom(vm => vm.UnitPrice))
            .ForMember(m => m.MPRate, map => map.MapFrom(vm => vm.MRPRate))
            .ForMember(m => m.UTAmount, map => map.MapFrom(vm => vm.UTAmount))
            .ForMember(m => m.PPDPercentage, map => map.MapFrom(vm => vm.PPDPercentage))
            .ForMember(m => m.PPDAmount, map => map.MapFrom(vm => vm.PPDAmount))
            .ForMember(m => m.PPOffer, map => map.MapFrom(vm => vm.PPOffer))
            .ForMember(m => m.Motor, map => map.MapFrom(vm => vm.MotorWarrentyMonth))
            .ForMember(m => m.Panel, map => map.MapFrom(vm => vm.PanelWarrentyMonth))
            .ForMember(m => m.Compressor, map => map.MapFrom(vm => vm.CompressorWarrentyMonth))
            .ForMember(m => m.Spareparts, map => map.MapFrom(vm => vm.SparePartsWarrentyMonth))
            .ForMember(m => m.Service, map => map.MapFrom(vm => vm.ServiceWarrentyMonth))
            ;

            #endregion

            #region CreateSalesOrderDetailViewModel,SOrderDetail
            CreateMap<CreateSalesOrderDetailViewModel, ReplaceOrderDetail>()
            .ForMember(m => m.SOrderDetailID, map => map.MapFrom(vm => vm.SODetailId))
            .ForMember(m => m.SOrderID, map => map.MapFrom(vm => vm.SalesOrderId))
            .ForMember(m => m.ProductID, map => map.MapFrom(vm => vm.ProductId))
            .ForMember(m => m.SDetailID, map => map.MapFrom(vm => vm.StockDetailId))
            .ForMember(m => m.Quantity, map => map.MapFrom(vm => vm.Quantity))
            .ForMember(m => m.UnitPrice, map => map.MapFrom(vm => vm.UnitPrice))
            .ForMember(m => m.MPRate, map => map.MapFrom(vm => vm.MRPRate))
            .ForMember(m => m.UTAmount, map => map.MapFrom(vm => vm.UTAmount))
            .ForMember(m => m.PPDPercentage, map => map.MapFrom(vm => vm.PPDPercentage))
            .ForMember(m => m.PPDAmount, map => map.MapFrom(vm => vm.PPDAmount))
            .ForMember(m => m.PPOffer, map => map.MapFrom(vm => vm.PPOffer))
            .ForMember(m => m.DamageIMEINO, map => map.MapFrom(vm => vm.DamageIMEINO))
            .ForMember(m => m.ReplaceIMEINO, map => map.MapFrom(vm => vm.ReplaceIMEINO))
            .ForMember(m => m.DamageProductName, map => map.MapFrom(vm => vm.DamageProductName))
            .ForMember(m => m.DamageUnitPrice, map => map.MapFrom(vm => vm.DamageUnitPrice))
            .ForMember(m => m.ProductName, map => map.MapFrom(vm => vm.ProductName))


            ;

            #endregion

            #region GetCashCollectionViewModel, CashCollection
            CreateMap<GetCashCollectionViewModel, CashCollection>()
            .ForMember(m => m.CashCollectionID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.EntryDate, map => map.MapFrom(vm => vm.EntryDate))
            .ForMember(m => m.ReceiptNo, map => map.MapFrom(vm => vm.ReceiptNo))
            .ForMember(m => m.CustomerID, map => map.MapFrom(vm => vm.Name))
                //.ForMember(m => m.SupplierID, map => map.MapFrom(vm => vm.SupplierName))
            .ForMember(m => m.AccountNo, map => map.MapFrom(vm => vm.AccountNo))
            .ForMember(m => m.Amount, map => map.MapFrom(vm => decimal.Parse(vm.Amount)))
            .ForMember(m => m.TransactionType, map => map.MapFrom(vm => vm.TransactionType));
            #endregion

            #region CashCollectionViewModel, CashCollection
            CreateMap<CreateCashCollectionViewModel, CashCollection>()
            .ForMember(m => m.CashCollectionID, map => map.MapFrom(vm => int.Parse(vm.CashCollectionID)))
            .ForMember(m => m.PaymentType, map => map.MapFrom(vm => vm.PaymentType))
            .ForMember(m => m.BankName, map => map.MapFrom(vm => vm.BankName))
            .ForMember(m => m.BranchName, map => map.MapFrom(vm => vm.BranchName))
            .ForMember(m => m.EntryDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.EntryDate)))

            .ForMember(m => m.Amount, map => map.MapFrom(vm => decimal.Parse(vm.Amount)))
            .ForMember(m => m.AdjustAmt, map => map.MapFrom(vm => decimal.Parse(vm.AdjustAmt)))
            .ForMember(m => m.BalanceDue, map => map.MapFrom(vm => decimal.Parse(vm.BalanceDue)))
            .ForMember(m => m.ReceiptNo, map => map.MapFrom(vm => vm.ReceiptNo))

            .ForMember(m => m.AccountNo, map => map.MapFrom(vm => vm.AccountNo))
            .ForMember(m => m.MBAccountNo, map => map.MapFrom(vm => vm.MBAccountNo))
            .ForMember(m => m.BKashNo, map => map.MapFrom(vm => vm.BKashNo))
            .ForMember(m => m.TransactionType, map => map.MapFrom(vm => vm.TransactionType))
            .ForMember(m => m.CreatedBy, map => map.MapFrom(vm => int.Parse(vm.CreatedBy)))
            .ForMember(m => m.CreateDate, map => map.MapFrom(vm => vm.CreateDate))
            .ForMember(m => m.ModifiedBy, map => map.MapFrom(vm => int.Parse(vm.ModifiedBy)))
            .ForMember(m => m.ModifiedDate, map => map.MapFrom(vm => vm.ModifiedDate))
            .ForMember(m => m.CustomerID, map => map.MapFrom(vm => vm.CustomerID))
            .ForMember(m => m.SupplierID, map => map.MapFrom(vm => vm.SupplierID))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernID)));
            #endregion

            #region MenuViewModel, MenuItem
            CreateMap<MenuViewModel, MenuItem>()
            .ForMember(m => m.Id, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.Title, map => map.MapFrom(vm => vm.Title))
            .ForMember(m => m.Url, map => map.MapFrom(vm => vm.Url))
            .ForMember(m => m.Description, map => map.MapFrom(vm => vm.Description))
            .ForMember(m => m.WithoutView, map => map.MapFrom(vm => vm.WithoutView))
            .ForMember(m => m.ParentId, map => map.MapFrom(vm => int.Parse(vm.ParentId)));
            #endregion

            #region GetSaleOfferViewModel, SaleOffer
            CreateMap<GetSaleOfferViewModel, SaleOffer>()
            .ForMember(m => m.OfferID, map => map.MapFrom(vm => int.Parse(vm.Id)))
            .ForMember(m => m.OfferCode, map => map.MapFrom(vm => vm.OfferCode))
            .ForMember(m => m.ProductID, map => map.MapFrom(vm => vm.ProductName))
            .ForMember(m => m.FromDate, map => map.MapFrom(vm => vm.FromDate))
            .ForMember(m => m.ToDate, map => map.MapFrom(vm => vm.ToDate))
            .ForMember(m => m.Description, map => map.MapFrom(vm => vm.Description))
            .ForMember(m => m.OfferValue, map => map.MapFrom(vm => vm.OfferValue))
            .ForMember(m => m.OfferType, map => map.MapFrom(vm => vm.OfferType))
            .ForMember(m => m.ThresholdValue, map => map.MapFrom(vm => vm.ThresholdValue))
            .ForMember(m => m.Status, map => map.MapFrom(vm => vm.Status));
            #endregion

            #region CreateSaleOfferViewModel, SaleOffer
            CreateMap<CreateSaleOfferViewModel, SaleOffer>()
            .ForMember(m => m.OfferID, map => map.MapFrom(vm => int.Parse(vm.OfferID)))
            .ForMember(m => m.OfferCode, map => map.MapFrom(vm => vm.OfferCode))
            .ForMember(m => m.ProductID, map => map.MapFrom(vm => vm.ProductID))
            .ForMember(m => m.FromDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.FromDate)))
            .ForMember(m => m.ToDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.ToDate)))
            .ForMember(m => m.Description, map => map.MapFrom(vm => vm.Description))
            .ForMember(m => m.OfferValue, map => map.MapFrom(vm => decimal.Parse(vm.OfferValue)))
            .ForMember(m => m.OfferType, map => map.MapFrom(vm => vm.OfferType))
            .ForMember(m => m.ThresholdValue, map => map.MapFrom(vm => decimal.Parse(vm.ThresholdValue)))
            .ForMember(m => m.Status, map => map.MapFrom(vm => vm.Status))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernID)))
            .ForMember(m => m.CreatedBy, map => map.MapFrom(vm => int.Parse(vm.CreatedBy)))
            .ForMember(m => m.CreateDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.CreateDate)))
            .ForMember(m => m.ModifiedBy, map => map.MapFrom(vm => int.Parse(vm.ModifiedBy)))
            .ForMember(m => m.ModifiedDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.ModifiedDate)));
            #endregion

            #region CreateCreditSalesOrderViewModel,CreditSale
            CreateMap<CreateCreditSalesOrderViewModel, CreditSale>()
            .ForMember(vm => vm.CreditSalesID, map => map.MapFrom(m => m.SalesOrderId))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.InvoiceNo))
            .ForMember(vm => vm.SalesDate, map => map.MapFrom(m => m.OrderDate))
            .ForMember(vm => vm.IssueDate, map => map.MapFrom(m => m.InstallmentDate))
            .ForMember(vm => vm.NoOfInstallment, map => map.MapFrom(m => m.InstallmentNo))
            .ForMember(vm => vm.CustomerID, map => map.MapFrom(m => m.CustomerId))
            .ForMember(vm => vm.Discount, map => map.MapFrom(m => m.PPDiscountAmount))
            .ForMember(vm => vm.VATPercentage, map => map.MapFrom(m => m.VATPercentage))
            .ForMember(vm => vm.VATAmount, map => map.MapFrom(m => m.VATAmount))
            .ForMember(vm => vm.Discount, map => map.MapFrom(m => m.NetDiscount))
            .ForMember(vm => vm.NetAmount, map => map.MapFrom(m => m.TotalAmount))
            .ForMember(vm => vm.TSalesAmt, map => map.MapFrom(m => m.GrandTotal))
            .ForMember(vm => vm.DownPayment, map => map.MapFrom(m => m.RecieveAmount))
            .ForMember(vm => vm.Remaining, map => map.MapFrom(m => m.PaymentDue))
                //.ForMember(vm => vm.Remaining, map => map.MapFrom(m => m.CurrentPreviousDue))
            .ForMember(vm => vm.IsStatus, map => map.MapFrom(m => m.Status))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks))
            .ForMember(vm => vm.GuarName, map => map.MapFrom(m => m.GuarName))
            .ForMember(vm => vm.GuarContactNo, map => map.MapFrom(m => m.GuarContactNo))
            .ForMember(vm => vm.GuarAddress, map => map.MapFrom(m => m.GuarAddress))
            ;
            #endregion

            #region CreateCreditSalesOrderDetailViewModel,CreditSaleDetails
            CreateMap<CreateCreditSalesOrderDetailViewModel, CreditSaleDetails>()
            .ForMember(vm => vm.CreditSaleDetailsID, map => map.MapFrom(m => m.SODetailId))
            .ForMember(vm => vm.CreditSalesID, map => map.MapFrom(m => m.SalesOrderId))
            .ForMember(vm => vm.ProductID, map => map.MapFrom(m => m.ProductId))
            .ForMember(vm => vm.StockDetailID, map => map.MapFrom(m => m.StockDetailId))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => m.UnitPrice))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Quantity))
            .ForMember(vm => vm.MPRate, map => map.MapFrom(m => m.MRPRate))
            .ForMember(vm => vm.UTAmount, map => map.MapFrom(m => m.UTAmount))
            .ForMember(vm => vm.Compressor, map => map.MapFrom(m => m.CompressorWarrentyMonth))
            .ForMember(vm => vm.Panel, map => map.MapFrom(m => m.PanelWarrentyMonth))
            .ForMember(vm => vm.Motor, map => map.MapFrom(m => m.MotorWarrentyMonth))
            .ForMember(vm => vm.Spareparts, map => map.MapFrom(m => m.SparePartsWarrentyMonth))
            ;
            #endregion

            #region CreditSalesSchedules, CreateCreditSalesSchedules
            CreateMap<CreateCreditSalesSchedules, CreditSalesSchedule>()
            .ForMember(vm => vm.MonthDate, map => map.MapFrom(m => m.ScheduleDate))
            .ForMember(vm => vm.CreditSalesID, map => map.MapFrom(m => m.SalesOrderId))
            .ForMember(vm => vm.PaymentDate, map => map.MapFrom(m => m.PayDate))
            .ForMember(vm => vm.InstallmentAmt, map => map.MapFrom(m => m.InstallmentAmount))
            .ForMember(vm => vm.ClosingBalance, map => map.MapFrom(m => m.ClosingBalance))
            .ForMember(vm => vm.PaymentStatus, map => map.MapFrom(m => m.PaymentStatus))
            .ForMember(vm => vm.Balance, map => map.MapFrom(m => m.OpeningBalance))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks))
            .ForMember(vm => vm.ScheduleNo, map => map.MapFrom(m => m.ScheduleNo))
            .ForMember(vm => vm.CreatedBy, map => map.MapFrom(m => m.CreatedBy))
            ;

            #endregion

            #region CreatePurchaseOrderViewModel,Porder
            CreateMap<CreatePurchaseOrderViewModel, POrder>()
                .ForMember(m => m.ChallanNo, map => map.MapFrom(vm => vm.ChallanNo))
                .ForMember(m => m.Remarks, map => map.MapFrom(vm => vm.Remarks))
                .ForMember(m => m.OrderDate, map => map.MapFrom(vm => vm.OrderDate))
                .ForMember(m => m.SupplierID, map => map.MapFrom(vm => vm.SupplierId))
                .ForMember(m => m.GrandTotal, map => map.MapFrom(vm => vm.GrandTotal))
                .ForMember(m => m.TotalAmt, map => map.MapFrom(vm => vm.TotalAmount))
                .ForMember(m => m.NetDiscount, map => map.MapFrom(vm => vm.NetDiscount))
                .ForMember(m => m.TDiscount, map => map.MapFrom(vm => vm.TotalDiscountAmount)) //Flat Discount
                .ForMember(m => m.PaymentDue, map => map.MapFrom(vm => vm.PaymentDue))
                .ForMember(m => m.RecAmt, map => map.MapFrom(vm => vm.RecieveAmount))
                .ForMember(m => m.TotalDue, map => map.MapFrom(vm => vm.TotalDue))
                .ForMember(m => m.LaborCost, map => map.MapFrom(vm => vm.LabourCost))
                .ForMember(m => m.AdjAmount, map => map.MapFrom(vm => vm.AdjAmount))
                .ForMember(m => m.IsDamageOrder, map => map.MapFrom(vm => (vm.IsDamagePO ? 1 : 0)))

                ;
            #endregion


            #region CreatePurchaseOrderDetailViewModel,POrderDetail
            CreateMap<CreatePurchaseOrderDetailViewModel, POrderDetail>()
                .ForMember(m => m.ProductID, map => map.MapFrom(vm => vm.ProductId))
                .ForMember(m => m.Quantity, map => map.MapFrom(vm => vm.Quantity))
                .ForMember(m => m.UnitPrice, map => map.MapFrom(vm => vm.UnitPrice))
                .ForMember(m => m.TAmount, map => map.MapFrom(vm => vm.TAmount))
                .ForMember(m => m.PPDISAmt, map => map.MapFrom(vm => vm.PPDiscountAmount))
                .ForMember(m => m.PPDISPer, map => map.MapFrom(vm => vm.PPDisPercentage))
                .ForMember(m => m.MRPRate, map => map.MapFrom(vm => vm.MRPRate))
                .ForMember(m => m.SalesRate, map => map.MapFrom(vm => vm.SalesRate))
                .ForMember(m => m.GodownID, map => map.MapFrom(vm => vm.GodownID))
                .ForMember(m => m.CreditSalesRate, map => map.MapFrom(vm => vm.CreditSalesRate))
                ;
            #endregion

            #region AllowanceDeductionViewModel,AllowanceDeduction
            CreateMap<AllowanceDeductionViewModel, AllowanceDeduction>();
            #endregion

            #region GradeViewModel,Grade
            CreateMap<GradeViewModel, Grade>()
                ;
            #endregion

            #region EmpGradeSalaryAssignmentViewModel,EmpGradeSalaryAssignment
            CreateMap<EmpGradeSalaryAssignmentViewModel, EmpGradeSalaryAssignment>()
                ;
            #endregion

            #region ADParameterBasicCreate,ADParameterBasic
            CreateMap<ADParameterBasicCreate, ADParameterBasic>()
                ;
            #endregion
            #region ADParameterBasicCreate,ADParameterBasic
            CreateMap<ADParameterGradeCreate, ADParameterGrade>()
                ;
            #endregion

            #region DepartmentViewModel,Department
            CreateMap<DepartmentViewModel, Department>()
                 .ForMember(m => m.Status, map => map.MapFrom(vm => vm.Status))
                ;
            #endregion
            #region HolidayCalenderViewModel,HolidayCalender
            CreateMap<HolidayCalenderViewModel, HolidayCalender>()
                 .ForMember(m => m.Status, map => map.MapFrom(vm => vm.Status))
                ;
            #endregion
            #region EmployeeLeaveViewModel,EmployeeLeave
            CreateMap<EmployeeLeaveViewModel, EmployeeLeave>()
                .ForMember(vm => vm.PaidLeave, map => map.MapFrom(m => m.IsPaidLeave == true ? 1 : 0))
                ;
            #endregion
            #region AttendenceViewModel,Attendence
            CreateMap<AttendenceViewModel, Attendence>()
                ;
            #endregion
            #region AttendenceViewModel,Attendence
            CreateMap<AttendenceMonthViewModel, AttendenceMonth>()
                ;
            #endregion
            #region AdvanceSalaryViewModel,AdvanceSalary
            CreateMap<AdvanceSalaryViewModel, AdvanceSalary>()
                ;
            #endregion
            #region TargetSetupViewModel,TargetSetup
            CreateMap<TargetSetupViewModel, TargetSetup>()
                ;
            #endregion
            #region CommissionSetupViewModel,CommissionSetup
            CreateMap<CommissionSetupViewModel, CommissionSetup>()
                ;
            #endregion

            #region DesWiseCommissionViewModel,DesWiseCommission
            CreateMap<DesWiseCommissionViewModel, DesWiseCommission>()
                ;
            #endregion

            #region SMSBillPaymentViewModel, SMSBillPayment
            CreateMap<SMSBillPaymentViewModel, SMSBillPayment>();
            #endregion

            #region CardTypeViewModel,CardType
            CreateMap<CardTypeViewModel, CardType>()
                ;
            #endregion

            #region CardTypeSetupViewModel,CardTypeSetup
            CreateMap<CardTypeSetupViewModel, CardTypeSetup>()
                ;
            #endregion

            #region ShareInvestmentViewModel,ShareInvestment
            CreateMap<ShareInvestmentViewModel, ShareInvestment>()
                ;
            #endregion

            #region InvestmentheadViewModel,ShareInvestmentHead
            CreateMap<InvestmentheadViewModel, ShareInvestmentHead>()
                .ForMember(m => m.OpeningType, map => map.MapFrom(vm => vm.OpeningType.ToString()))
                ;
            #endregion

            #region ParentCategoryViewModel,ParentCategory
            CreateMap<ParentCategoryViewModel, ParentCategory>()
                ;
            #endregion

            #region MonthlyAttendenceVM,MonthlyAttendence
            CreateMap<MonthlyAttendenceVM, MonthlyAttendence>();
            ;
            #endregion
            #region DueSalaryViewModel,AdvanceSalary
            CreateMap<DueSalaryViewModel, AdvanceSalary>()
                ;
            #endregion

            #region CreditInterestPercentageViewModel, CreditInterestPercentage
            CreateMap<CreditInterestPercentageViewModel, CreditInterestPercentage>()
            .ForMember(m => m.IntPercentageID, map => map.MapFrom(vm => vm.IntPercentageID))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.Percentage, map => map.MapFrom(vm => vm.Percentage))
            .ForMember(m => m.ConcernID, map => map.MapFrom(vm => int.Parse(vm.ConcernId)))
            .ForMember(m => m.EffectDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.EffectDate)));
            #endregion

            #region CreateBankLoanVM, BankLoan
            CreateMap<CreateBankLoanVM, BankLoan>();
            #endregion

            #region CreateBankLoanDetailsVM, BankLoanDetails
            CreateMap<CreateBankLoanDetailsVM, BankLoanDetails>()
                .ForMember(m => m.ExpectedInstallmentAmount, map => map.MapFrom(vm => vm.InstallmentAmount));
            #endregion

            #region DODetailViewModel,DODetail
            CreateMap<DODetailViewModel, DODetail>()
            ;
            #endregion
        }
    }
}