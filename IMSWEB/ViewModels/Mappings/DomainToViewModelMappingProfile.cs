using AutoMapper;
using IMSWEB.Model;
using IMSWEB.ViewModels;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Web;

namespace IMSWEB
{
    public class DomainToViewModelMappingProfile : Profile
    {
        public DomainToViewModelMappingProfile()
        {
            #region Product, CreateProductViewModel
            CreateMap<Product, CreateProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.ProductID))
            .ForMember(vm => vm.CategoryId, map => map.MapFrom(m => m.CategoryID))
            .ForMember(vm => vm.CompanyId, map => map.MapFrom(m => m.CompanyID))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.PWDiscount))
            .ForMember(vm => vm.DisDurationFDate, map => map.MapFrom(m => m.DisDurationFDate))
            .ForMember(vm => vm.DisDurationToDate, map => map.MapFrom(m => m.DisDurationToDate))
            ;
            #endregion

            #region Product, CreateProductViewModel
            CreateMap<ProductWisePurchaseModel, CreateProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.ProductID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.ProductCode))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.ProductName))
            //.ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.PicturePath))
            .ForMember(vm => vm.CategoryId, map => map.MapFrom(m => m.CategoryID))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.CategoryName))
            .ForMember(vm => vm.CompanyId, map => map.MapFrom(m => m.CompanyID))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.CompanyName))
            .ForMember(vm => vm.UnitType, map => map.MapFrom(m => m.UnitType))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.PWDiscount))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.PWDiscount))
            .ForMember(vm => vm.RP, map => map.MapFrom(m => m.RP))
            .ForMember(vm => vm.ECOMRP, map => map.MapFrom(m => m.RP))
            //.ForMember(vm => vm.DisDurationFDate, map => map.MapFrom(m => m.DisDurationFDate))
            //.ForMember(vm => vm.DisDurationToDate, map => map.MapFrom(m => m.DisDurationToDate))
            ;
            #endregion

            #region BankTransaction, CreateBankTransactionViewModel
            CreateMap<BankTransaction, CreateBankTransactionViewModel>()
               .ForMember(vm => vm.BankTranID, map => map.MapFrom(m => m.BankTranID))
               .ForMember(vm => vm.TranDate, map => map.MapFrom(m => m.TranDate))
               .ForMember(vm => vm.TransactionNo, map => map.MapFrom(m => m.TransactionNo))
               .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.TransactionType))
               .ForMember(vm => vm.BankID, map => map.MapFrom(m => m.BankID))
               .ForMember(vm => vm.CustomerID, map => map.MapFrom(m => m.CustomerID))
               .ForMember(vm => vm.SupplierID, map => map.MapFrom(m => m.SupplierID))
               .ForMember(vm => vm.AnotherBankID, map => map.MapFrom(m => m.AnotherBankID))
               .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Amount))
               .ForMember(vm => vm.ExpenseItemID, map => map.MapFrom(m => m.ExpenseItemID))
              
               .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks));


            #endregion

            #region ApplicationUser, CreateUserViewModel
            CreateMap<ApplicationUser, CreateUserViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
            .ForMember(vm => vm.Email, map => map.MapFrom(m => m.Email))
            .ForMember(vm => vm.UserName, map => map.MapFrom(m => m.UserName))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID))
            .ForMember(vm => vm.PhoneNumber, map => map.MapFrom(m => m.PhoneNumber))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.LockoutEnabled ? "Inactive" : "Active"));
            #endregion

            #region Customer, DetailCustomerViewModel
            CreateMap<Customer, GetCustomerViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.CustomerID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.MemberID, map => map.MapFrom(m => m.MemberID))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.PhotoPath))
            .ForMember(vm => vm.TotalDue, map => map.MapFrom(m => m.TotalDue))
            .ForMember(vm => vm.CustomerType, map => map.MapFrom(m => m.CustomerType))
            .ForMember(vm => vm.CusDueLimit, map => map.MapFrom(m => m.CusDueLimit))
            .ForMember(vm => vm.Address, map => map.MapFrom(m => m.Address));
            #endregion

            #region Customer, CreateCustomerViewModel
            CreateMap<Customer, CreateCustomerViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.CustomerID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.MemberID, map => map.MapFrom(m => m.MemberID))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.PhotoPath))
            .ForMember(vm => vm.TotalDue, map => map.MapFrom(m => m.TotalDue))
            .ForMember(vm => vm.OpeningDue, map => map.MapFrom(m => m.OpeningDue))
            .ForMember(vm => vm.CustomerType, map => map.MapFrom(m => m.CustomerType))
            .ForMember(vm => vm.CusDueLimit, map => map.MapFrom(m => m.CusDueLimit))
            .ForMember(vm => vm.FName, map => map.MapFrom(m => m.FName))
            .ForMember(vm => vm.CompanyId, map => map.MapFrom(m => m.CompanyName))
            .ForMember(vm => vm.EmailId, map => map.MapFrom(m => m.EmailID))
            .ForMember(vm => vm.NId, map => map.MapFrom(m => m.NID))
            .ForMember(vm => vm.Address, map => map.MapFrom(m => m.Address))
            .ForMember(vm => vm.RefName, map => map.MapFrom(m => m.RefName))
            .ForMember(vm => vm.RefContact, map => map.MapFrom(m => m.RefContact))
            .ForMember(vm => vm.RefFName, map => map.MapFrom(m => m.RefFName))
            .ForMember(vm => vm.Profession, map => map.MapFrom(m => m.Profession))
            .ForMember(vm => vm.RefAddress, map => map.MapFrom(m => m.RefAddress))
            .ForMember(vm => vm.EmployeeId, map => map.MapFrom(m => m.EmployeeID))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID));
            #endregion

            #region Company, CreateGodownViewModel
            CreateMap<Godown, CreateGodownViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.GodownID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID))
            .ForMember(vm => vm.ISCommon, map => map.MapFrom(m => m.ISCommon));
            #endregion

            #region Company, CreateCompanyViewModel
            CreateMap<Company, CreateCompanyViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.CompanyID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID));
            #endregion

            #region SisterConcern, CreateSisterConcernViewModel
            CreateMap<SisterConcern, CreateSisterConcernViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.ConcernID))
            .ForMember(vm => vm.Address, map => map.MapFrom(m => m.Address))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.ContactNo))
            .ForMember(vm => vm.ServiceCharge, map => map.MapFrom(m => m.ServiceCharge))
            .ForMember(vm => vm.SmsContactNo, map => map.MapFrom(m => m.SmsContactNo));
            #endregion

            #region Color, CreateColorViewModel
            CreateMap<Color, CreateColorViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.ColorID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID));
            #endregion

            #region Category, CreateCategoryViewModel
            CreateMap<Category, CreateCategoryViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.CategoryID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Description))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID));
            #endregion


            #region Bank, CreateBankViewModel
            CreateMap<Bank, CreateBankViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.BankID))
            .ForMember(m => m.Code, map => map.MapFrom(vm => vm.Code))
            .ForMember(m => m.BankName, map => map.MapFrom(vm => vm.BankName))
            .ForMember(m => m.AccountNo, map => map.MapFrom(vm => vm.AccountNo))
            .ForMember(m => m.AccountName, map => map.MapFrom(vm => vm.AccountName))
            .ForMember(m => m.OpeningBalance, map => map.MapFrom(vm => vm.OpeningBalance))
            .ForMember(m => m.TotalAmount, map => map.MapFrom(vm => vm.TotalAmount));

            #endregion

            #region ApplicationRole, CreateRoleViewModel
            CreateMap<ApplicationRole, CreateRoleViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Id))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name));
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string, decimal, string, string, string,
                Tuple<decimal?, string, decimal, decimal, int, string, int>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ModelName, map => map.MapFrom(m => m.Rest.Item2))
            //.ForMember(vm => vm.PKTSheet, map => map.MapFrom(m => m.Rest.Item3))
            //  .ForMember(vm => vm.SalesRate, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.GodownID, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Item6));
            // .ForMember(vm => vm.Packet, map => map.MapFrom(m => m.Rest.Item7));
            //.ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7));
            #endregion

            #region TransferHistory,TransferHistoryViewModel
            CreateMap<Tuple<int, int, string, DateTime, int, string, int, Tuple<string, decimal, string, string, string>>, GetTransferHistory>()
            .ForMember(vm => vm.TransferHID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.TransferDate, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ToGodown, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.ToGodownName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.GodownID, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.FromGodownName, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ModelName, map => map.MapFrom(m => m.Rest.Item5));
            #endregion


            #region Tuple, GetBankTransactionViewModel
            CreateMap<Tuple<int, string, string, string, string, string, string, Tuple<decimal, DateTime?, string, string, string, string,string>>, GetBankTransactionViewModel>()
            .ForMember(vm => vm.BankTranID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.BankName, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.SupplierName, map => map.MapFrom(m => m.Item4))
             .ForMember(vm => vm.AnotherBankName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.TransactionNo, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.TranDate, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.ChecqueNo, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.AccountName, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.AccountNo, map => map.MapFrom(m => m.Rest.Item6))
              .ForMember(vm => vm.WFStatus, map => map.MapFrom(m => m.Rest.Item7))
            ;
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7));
            #endregion
            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal, decimal>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.PRate, map => map.MapFrom(m => m.Rest.Item2))
            ;
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, int>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ProductType, map => map.MapFrom(m => m.Rest.Item2))
            ;
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ProductType, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.PRate, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.RP, map => map.MapFrom(m => m.Rest.Item4))
            ;
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            //.ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item1))
            //.ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            //.ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            //.ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            ;
            #endregion


            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Rest.Item1))
            //.ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item1))
            //.ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            //.ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            //.ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            ;
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string>>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            .ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            .ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            .ForMember(vm => vm.ServiceWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item5))
            ;
            #endregion


            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string>>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            .ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            .ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            .ForMember(vm => vm.ServiceWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item5))
           .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Rest.Item6))
            ;
            #endregion
            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            .ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            .ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            .ForMember(vm => vm.ServiceWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item5))
           .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Rest.Item6))
           .ForMember(vm => vm.PRate, map => map.MapFrom(m => m.Rest.Rest.Item7))
           .ForMember(vm => vm.ProductType, map => map.MapFrom(m => m.Rest.Rest.Rest.Item1))
            ;
            #endregion


            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<decimal, string, string, string, string, string>>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.CashSalesRate, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            .ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            .ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            .ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item5))
            .ForMember(vm => vm.ServiceWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item6))
            ;
            #endregion
            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal>>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.CashSalesRate, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            .ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            .ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            .ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item5))
            .ForMember(vm => vm.ServiceWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item6))
            .ForMember(vm => vm.MRPRate12, map => map.MapFrom(m => m.Rest.Rest.Item7))
            ;
            #endregion


            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal, Tuple<string>>>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.StockDetailsId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.OfferDescription, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.CashSalesRate, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.CompressorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item2))
            .ForMember(vm => vm.MotorWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item3))
            .ForMember(vm => vm.PanelWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item4))
            .ForMember(vm => vm.SparePartsWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item5))
            .ForMember(vm => vm.ServiceWarrentyMonth, map => map.MapFrom(m => m.Rest.Rest.Item6))
            .ForMember(vm => vm.MRPRate12, map => map.MapFrom(m => m.Rest.Rest.Item7))
            .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Rest.Rest.Item1))
            ;
            #endregion

            #region Designation, CreateDesignationViewModel
            CreateMap<Designation, CreateDesignationViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.DesignationID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Description));
            #endregion

            #region ExpenseItem, CreateExpenseItemViewModel
            CreateMap<ExpenseItem, CreateExpenseItemViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.ExpenseItemID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Description))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Status));
            #endregion

            #region Supplier, GetSupplierViewModel
            CreateMap<Supplier, GetSupplierViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.SupplierID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.ContactNo))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.PhotoPath))
            .ForMember(vm => vm.TotalDue, map => map.MapFrom(m => m.TotalDue))
            .ForMember(vm => vm.OwnerName, map => map.MapFrom(m => m.OwnerName));
            #endregion

            #region Supplier, CreateSupplierViewModel
            CreateMap<Supplier, CreateSupplierViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.SupplierID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.PhotoPath))
            .ForMember(vm => vm.TotalDue, map => map.MapFrom(m => m.TotalDue))
            .ForMember(vm => vm.OpeningDue, map => map.MapFrom(m => m.OpeningDue))
            .ForMember(vm => vm.OwnerName, map => map.MapFrom(m => m.OwnerName))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.ContactNo))
            .ForMember(vm => vm.Address, map => map.MapFrom(m => m.Address))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID));
            #endregion

            #region Tuple, GetEmployeeViewModel
            CreateMap<Tuple<int, string, string, string, string,
                DateTime, string>, GetEmployeeViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.JoiningDate, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item7));
            #endregion
            #region Tuple, GetEmployeeViewModel
            CreateMap<Tuple<int, string, string, string, string,
                DateTime, string, Tuple<int, EnumActiveInactive>>, GetEmployeeViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.JoiningDate, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.MachineEMPID, map => map.MapFrom(m => m.Rest.Item1))
             .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Rest.Item2));
            #endregion
            #region Tuple, GetEmployeeViewModel
            CreateMap<Tuple<int, string, string, string, string, DateTime, string, Tuple<string, string>>, GetEmployeeViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.JoiningDate, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.DepartmentName, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.GradeName, map => map.MapFrom(m => m.Rest.Item2))
            ;
            #endregion

            #region Employee, CreateEmployeeViewModel
            CreateMap<Employee, CreateEmployeeViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.EmployeeID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.PhotoPath, map => map.MapFrom(m => m.PhotoPath))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.ContactNo))
            .ForMember(vm => vm.JoiningDate, map => map.MapFrom(m => m.JoiningDate))
            .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.DesignationID))
            .ForMember(vm => vm.FName, map => map.MapFrom(m => m.FName))
            .ForMember(vm => vm.MName, map => map.MapFrom(m => m.MName))
            .ForMember(vm => vm.EmailId, map => map.MapFrom(m => m.EmailID))
            .ForMember(vm => vm.NId, map => map.MapFrom(m => m.NID))
            .ForMember(vm => vm.PermanentAddress, map => map.MapFrom(m => m.PermanentAdd))
            .ForMember(vm => vm.PresentAddress, map => map.MapFrom(m => m.PresentAdd))
            .ForMember(vm => vm.BloodGroup, map => map.MapFrom(m => m.BloodGroup))
            .ForMember(vm => vm.GrossSalary, map => map.MapFrom(m => m.GrossSalary))
            .ForMember(vm => vm.DateOfBirth, map => map.MapFrom(m => m.DOB))
            .ForMember(vm => vm.SRDueLimit, map => map.MapFrom(m => m.SRDueLimit))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID));
            #endregion

            #region SystemInformation, CreateSystemInformationViewModel
            CreateMap<SystemInformation, CreateSystemInformationViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.SystemInfoID))
            .ForMember(vm => vm.ConcernID, map => map.MapFrom(m => m.ConcernID))
            .ForMember(vm => vm.Address, map => map.MapFrom(m => m.Address))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
            .ForMember(vm => vm.ConcernNameB, map => map.MapFrom(m => m.ConcernNameB))
            .ForMember(vm => vm.EmailAddress, map => map.MapFrom(m => m.EmailAddress))
            .ForMember(vm => vm.TelephoneNo, map => map.MapFrom(m => m.TelephoneNo))
            .ForMember(vm => vm.EmployeePhotoPath, map => map.MapFrom(m => m.EmployeePhotoPath))
            .ForMember(vm => vm.WebAddress, map => map.MapFrom(m => m.WebAddress))
            .ForMember(vm => vm.ProductPhotoPath, map => map.MapFrom(m => m.ProductPhotoPath))
            .ForMember(vm => vm.SupplierPhotoPath, map => map.MapFrom(m => m.SupplierPhotoPath))
            .ForMember(vm => vm.CustomerPhotoPath, map => map.MapFrom(m => m.CustomerPhotoPath))
            .ForMember(vm => vm.CustomerNIDPatht, map => map.MapFrom(m => m.CustomerNIDPatht))
            .ForMember(vm => vm.SupplierDocPath, map => map.MapFrom(m => m.SupplierDocPath))
            .ForMember(vm => vm.SystemStartDate, map => map.MapFrom(m => Convert.ToDateTime(m.SystemStartDate)))
            .ForMember(vm => vm.CompanyTitle, map => map.MapFrom(m => m.CompanyTitle))
            .ForMember(vm => vm.IsCommonBank, map => map.MapFrom(m => m.IsCommonBank))
            ;


            #endregion

            #region CreateExpenditureViewModel, Expenditure
            CreateMap<Expenditure, CreateExpenditureViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.ExpenditureID))
            .ForMember(vm => vm.ExpenseItemName, map => map.MapFrom(m => m.ExpenseItem.Description))
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.EntryDate.ToString("dd MMM yyyy")))
            ;
            #endregion

            #region Tuple, GetPurchaseOrderViewModel
            CreateMap<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType, Tuple<int>>, GetPurchaseOrderViewModel>()
            .ForMember(vm => vm.PurchaseOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ChallanNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.SupplierName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7.ToString()))
            .ForMember(vm => vm.EditReqStatus, map => map.MapFrom(m => m.Rest.Item1.ToString()));

            #endregion


            #region Tuple, GetPurchaseOrderViewModel
            CreateMap<Tuple<int, string, DateTime, string, string, string, EnumPurchaseType>, GetPurchaseOrderViewModel > ()
            .ForMember(vm => vm.PurchaseOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ChallanNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.SupplierName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7.ToString()));
            

            #endregion

            #region Tuple, GetSalesOrderViewModel
            CreateMap<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>, GetSalesOrderViewModel>()
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.DueAmount, map => map.MapFrom(m => m.Item6.ToString("#,###")))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7.ToString()));
            #endregion

            #region Tuple, GetSalesOrderViewModelNew
            CreateMap<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int, decimal>>, GetSalesOrderViewModel>()
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CustomerCode, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.DueAmount, map => map.MapFrom(m => m.Item6.ToString("#,###")))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7.ToString()))
            .ForMember(vm => vm.EditReqStatus, map => map.MapFrom(m => m.Rest.Item2.ToString()))
            .ForMember(vm => vm.InvAmt, map => map.MapFrom(m => m.Rest.Item3.ToString("#,###")))
            ;
            #endregion
            #region Tuple, GetReplacementOrderListVM
            CreateMap<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>, GetReplacementOrderListVM>()
            .ForMember(vm => vm.SOrderID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.SalesDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.DueAmout, map => map.MapFrom(m => m.Item6.ToString("#,###")))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7.ToString()));
            #endregion

            #region Tuple, GetCreditSalesOrderViewModel
            CreateMap<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType>, GetCreditSalesOrderViewModel>()
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.DueAmount, map => map.MapFrom(m => m.Item6.ToString()))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => (m.Item7.ToString())));
            #endregion

            #region Tuple, GetCreditSalesOrderViewModelNew
            CreateMap<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, int>>, GetCreditSalesOrderViewModel>()
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))
             .ForMember(vm => vm.CustomerCode, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.DueAmount, map => map.MapFrom(m => m.Item6.ToString()))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => (m.Item7.ToString())))
            .ForMember(vm => vm.IsReturn, map => map.MapFrom(m => m.Rest.Item2))

            ;
            #endregion



            #region CreditSale, CreateCreditSalesOrderViewModel
            CreateMap<CreditSale, CreateCreditSalesOrderViewModel>()
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.CreditSalesID))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.InvoiceNo))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.SalesDate))
            .ForMember(vm => vm.InstallmentDate, map => map.MapFrom(m => m.IssueDate))
            .ForMember(vm => vm.InstallmentNo, map => map.MapFrom(m => m.NoOfInstallment))
            .ForMember(vm => vm.CustomerId, map => map.MapFrom(m => m.CustomerID))
            .ForMember(vm => vm.PPDiscountAmount, map => map.MapFrom(m => Math.Round(m.Discount, 2)))
            .ForMember(vm => vm.VATPercentage, map => map.MapFrom(m => Convert.ToInt32(m.VATPercentage)))
            .ForMember(vm => vm.VATAmount, map => map.MapFrom(m => Math.Round(m.VATAmount, 2)))
            .ForMember(vm => vm.NetDiscount, map => map.MapFrom(m => Math.Round(m.Discount, 2)))
            .ForMember(vm => vm.TotalAmount, map => map.MapFrom(m => Math.Round(m.NetAmount, 2)))
            .ForMember(vm => vm.GrandTotal, map => map.MapFrom(m => Math.Round(m.TSalesAmt, 2)))
            .ForMember(vm => vm.RecieveAmount, map => map.MapFrom(m => Math.Round(m.DownPayment, 2)))
            .ForMember(vm => vm.PaymentDue, map => map.MapFrom(m => Math.Round(m.Remaining, 2)))
            .ForMember(vm => vm.CurrentPreviousDue, map => map.MapFrom(m => Math.Round(m.Remaining, 2)))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.IsStatus.ToString()))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks))
            .ForMember(vm => vm.PayAdjustment, map => map.MapFrom(m => Math.Round(m.LastPayAdjAmt, 2)))
            .ForMember(vm => vm.TotalInterest, map => map.MapFrom(m => Math.Round(m.InterestAmount, 2)))
            .ForMember(vm => vm.IsWeekly, map => map.MapFrom(m => m.IsWeekly))
            .ForMember(vm => vm.GuarName, map => map.MapFrom(m => m.GuarName))
            .ForMember(vm => vm.GuarContactNo, map => map.MapFrom(m => m.GuarContactNo))
            .ForMember(vm => vm.GuarAddress, map => map.MapFrom(m => m.GuarAddress))
            ;
            #endregion

            #region CreditSaleDetails, CreateCreditSalesOrderDetailViewModel
            CreateMap<CreditSaleDetails, CreateCreditSalesOrderDetailViewModel>()
            .ForMember(vm => vm.SODetailId, map => map.MapFrom(m => m.CreditSaleDetailsID))
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.CreditSalesID))
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.ProductID))
            .ForMember(vm => vm.StockDetailId, map => map.MapFrom(m => m.StockDetailID))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.UnitPrice, 2)))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => Convert.ToInt32(m.Quantity)))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => Math.Round(m.MPRate, 2)))
            .ForMember(vm => vm.UTAmount, map => map.MapFrom(m => Math.Round(m.UTAmount, 2)))
            .ForMember(vm => vm.PPOffer, map => map.MapFrom(m => Math.Round(m.PPOffer, 2)))
            .ForMember(vm => vm.IntPercentage, map => map.MapFrom(m => Math.Round(m.IntPercentage, 2)))
            .ForMember(vm => vm.IntTotalAmt, map => map.MapFrom(m => Math.Round(m.IntTotalAmt, 2)));
            #endregion

            #region Tuple, CreateCreditSalesOrderDetailViewModel
            CreateMap<Tuple<int, int, int, int, decimal, decimal, decimal, Tuple<decimal,
                string, string, int, string, decimal>>, CreateCreditSalesOrderDetailViewModel>()
            .ForMember(vm => vm.SODetailId, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.StockDetailId, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.Item7, 2)))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => Convert.ToInt32(m.Item5)))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => Math.Round(m.Item6, 2)))
            .ForMember(vm => vm.UTAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item1, 2)))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.IntTotalAmt, map => map.MapFrom(m => m.Rest.Item6));
            #endregion

            #region CreditSalesSchedules, CreateCreditSalesSchedules
            CreateMap<CreditSalesSchedule, CreateCreditSalesSchedules>()
            .ForMember(vm => vm.ScheduleDate, map => map.MapFrom(m => m.MonthDate.ToString("dd MMM yyyy")))
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.CreditSalesID))
            .ForMember(vm => vm.PayDate, map => map.MapFrom(m => (m.PaymentStatus == "Due" ? string.Empty : m.PaymentDate.ToString("dd MMM yyyy"))))
            .ForMember(vm => vm.InstallmentAmount, map => map.MapFrom(m => Math.Round(m.InstallmentAmt, 2)))
            .ForMember(vm => vm.ClosingBalance, map => map.MapFrom(m => Math.Round(m.ClosingBalance, 2)))
            .ForMember(vm => vm.PaymentStatus, map => map.MapFrom(m => m.PaymentStatus))
            .ForMember(vm => vm.OpeningBalance, map => map.MapFrom(m => Math.Round(m.Balance, 2)))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks))
            .ForMember(vm => vm.ScheduleNo, map => map.MapFrom(m => m.ScheduleNo))
            .ForMember(vm => vm.CreatedBy, map => map.MapFrom(m => m.CreatedBy))
            ;
            #endregion

            #region Tuple, GetCashCollectionViewModel
            CreateMap<Tuple<int, DateTime, string, string, string,
                string, string>, GetCashCollectionViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.Item2.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.ReceiptNo, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item4))
            //.ForMember(vm => vm.SupplierName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.AccountNo, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.Item7));
            #endregion
            #region Tuple, GetCashCollectionViewModel
            CreateMap<Tuple<int, DateTime, string, string, string,
                string, string, Tuple<string>>, GetCashCollectionViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.Item2.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.ReceiptNo, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item4))
            //.ForMember(vm => vm.SupplierName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.AccountNo, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Rest.Item1))
            ;
            #endregion
            #region Tuple, GetCashCollectionViewModel
            CreateMap<Tuple<int, DateTime, string, string, string,
                string, string, Tuple<string, string>>, GetCashCollectionViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.Item2.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.ReceiptNo, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item4))
            //.ForMember(vm => vm.SupplierName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.AccountNo, map => map.MapFrom(m => m.Item5))
             .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Rest.Item1))
            ;
            #endregion

            #region CreateCashCollectionViewModel, CashCollection
            CreateMap<CashCollection, CreateCashCollectionViewModel>()
            .ForMember(vm => vm.CashCollectionID, map => map.MapFrom(m => m.CashCollectionID))
            .ForMember(vm => vm.PaymentType, map => map.MapFrom(m => m.PaymentType))
            .ForMember(vm => vm.BankName, map => map.MapFrom(m => m.BankName))
            .ForMember(vm => vm.BranchName, map => map.MapFrom(m => m.BranchName))
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => Convert.ToDateTime(m.EntryDate)))
            .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Amount))
            .ForMember(vm => vm.TempAmount, map => map.MapFrom(m => m.Amount))
            .ForMember(vm => vm.AccountNo, map => map.MapFrom(m => m.AccountNo))
            .ForMember(vm => vm.MBAccountNo, map => map.MapFrom(m => m.MBAccountNo))
            .ForMember(vm => vm.BKashNo, map => map.MapFrom(m => m.BKashNo))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.CreatedBy, map => map.MapFrom(m => m.CreatedBy))
            .ForMember(vm => vm.CreateDate, map => map.MapFrom(m => m.CreateDate))
            .ForMember(vm => vm.ModifiedBy, map => map.MapFrom(m => m.ModifiedBy.ToString()))
            .ForMember(vm => vm.ModifiedDate, map => map.MapFrom(m => m.ModifiedDate))
            .ForMember(vm => vm.CustomerID, map => map.MapFrom(m => m.CustomerID))
            .ForMember(vm => vm.SupplierID, map => map.MapFrom(m => m.SupplierID))
            .ForMember(vm => vm.ReceiptNo, map => map.MapFrom(m => m.ReceiptNo))
            .ForMember(vm => vm.AdjustAmt, map => map.MapFrom(m => m.AdjustAmt))
            .ForMember(vm => vm.TempAdjustAmt, map => map.MapFrom(m => m.AdjustAmt))
            .ForMember(vm => vm.BalanceDue, map => map.MapFrom(m => m.BalanceDue))
            .ForMember(vm => vm.CurrentDue, map => map.MapFrom(m => m.BalanceDue))
            .ForMember(vm => vm.ConcernID, map => map.MapFrom(m => m.ConcernID));

            #endregion

            #region CreateCashCollectionViewModel, CashCollection
            CreateMap<CashCollection, CreateCashCollectionViewModel>()
            .ForMember(vm => vm.TempAmount, map => map.MapFrom(m => m.Amount))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.Type, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.TempAdjustAmt, map => map.MapFrom(m => m.AdjustAmt))
            .ForMember(vm => vm.CurrentDue, map => map.MapFrom(m => m.BalanceDue))
            .ForMember(vm => vm.DeliveryType, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.EmobileType, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.EmobileDeliveryType, map => map.MapFrom(m => m.TransactionType))
            ;
            #endregion
            #region POrder, CreatePurchaseOrderViewModel
            CreateMap<POrder, CreatePurchaseOrderViewModel>()
            .ForMember(vm => vm.ChallanNo, map => map.MapFrom(m => m.ChallanNo))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks))
            .ForMember(vm => vm.IsDamagePO, map => map.MapFrom(m => m.IsDamageOrder == 1 ? true : false))
            .ForMember(vm => vm.GrandTotal, map => map.MapFrom(m => Math.Round(m.GrandTotal, 2)))
            .ForMember(vm => vm.LabourCost, map => map.MapFrom(m => Math.Round(m.LaborCost, 2)))
            .ForMember(vm => vm.NetDiscount, map => map.MapFrom(m => Math.Round(m.NetDiscount, 2)))
            .ForMember(vm => vm.tempNetDiscount, map => map.MapFrom(m => Math.Round(m.NetDiscount, 2)))
            .ForMember(vm => vm.tempFlatDiscountAmount, map => map.MapFrom(m => Math.Round(m.TDiscount, 2)))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.OrderDate))
            .ForMember(vm => vm.PaymentDue, map => map.MapFrom(m => Math.Round(m.PaymentDue, 2)))
            .ForMember(vm => vm.PPDiscountAmount, map => map.MapFrom(m => Math.Round(m.PPDisAmt, 2)))
            .ForMember(vm => vm.PurchaseOrderId, map => map.MapFrom(m => m.POrderID))
            .ForMember(vm => vm.RecieveAmount, map => map.MapFrom(m => Math.Round(m.RecAmt, 2)))
            .ForMember(vm => vm.SupplierId, map => map.MapFrom(m => m.SupplierID))
            .ForMember(vm => vm.TotalAmount, map => map.MapFrom(m => Math.Round(m.TotalAmt, 2)))
            .ForMember(vm => vm.AdjAmount, map => map.MapFrom(m => Math.Round(m.AdjAmount, 2)))
            .ForMember(vm => vm.TotalDiscountAmount, map => map.MapFrom(m => Math.Round(m.TDiscount, 2)))
            .ForMember(vm => vm.VATAmount, map => map.MapFrom(m => Math.Round(m.VATAmount, 2)))
            .ForMember(vm => vm.VATPercentage, map => map.MapFrom(m => Math.Round(m.VATPercentage, 2)))
            .ForMember(vm => vm.TotalDiscountPercentage, map => map.MapFrom(m => Math.Round(((100 * m.TDiscount) / (m.GrandTotal - (m.NetDiscount - m.TDiscount))), 2)))
            .ForMember(vm => vm.TotalDue, map => map.MapFrom(m => Math.Round(m.TotalDue, 2)));
            #endregion

            #region POrderDetail, CreatePurchaseOrderDetailViewModel
            CreateMap<POrderDetail, CreatePurchaseOrderDetailViewModel>()
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => Math.Round(m.MRPRate, 2)))
            .ForMember(vm => vm.PODetailId, map => map.MapFrom(m => m.POrderDetailID))
            .ForMember(vm => vm.PPDiscountAmount, map => map.MapFrom(m => Math.Round(m.PPDISAmt, 2)))
            .ForMember(vm => vm.PPDisPercentage, map => map.MapFrom(m => Math.Round(m.PPDISPer, 2)))
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.ProductID))
            .ForMember(vm => vm.PurchaseOrderId, map => map.MapFrom(m => m.POrderID))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => Math.Round(m.Quantity, 2)))
            .ForMember(vm => vm.TAmount, map => map.MapFrom(m => Math.Round(m.TAmount, 2)))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.UnitPrice, 2)))
            .ForMember(vm => vm.SalesRate, map => map.MapFrom(m => Math.Round(m.SalesRate, 2)))
            .ForMember(vm => vm.GodownID, map => map.MapFrom(m => m.GodownID))
            ;
            #endregion

            #region Tuple, CreatePurchaseOrderViewModel
            CreateMap<Tuple<decimal, int, decimal, decimal, int, int, decimal,
                Tuple<decimal, decimal, string, string, int, string, decimal, Tuple<int, string>>>, CreatePurchaseOrderDetailViewModel>()
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => Math.Round(m.Item1, 2)))
            .ForMember(vm => vm.PODetailId, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.PPDiscountAmount, map => map.MapFrom(m => Math.Round(m.Item3, 2)))
            .ForMember(vm => vm.PPDisPercentage, map => map.MapFrom(m => Math.Round(m.Item4, 2)))
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.PurchaseOrderId, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => Math.Round(m.Item7, 2)))
            .ForMember(vm => vm.TAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item1, 2)))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.Rest.Item2, 2)))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.SalesRate, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.GodownID, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Rest.Item2))

            ;
            #endregion

            #region SRVisit, CreateSRVisitViewModel
            CreateMap<SRVisit, CreateSRVisitViewModel>()
            .ForMember(vm => vm.ChallanNo, map => map.MapFrom(m => m.ChallanNo))
            .ForMember(vm => vm.VisitDate, map => map.MapFrom(m => m.VisitDate))
            .ForMember(vm => vm.SRVisitID, map => map.MapFrom(m => m.SRVisitID))
            .ForMember(vm => vm.EmployeeID, map => map.MapFrom(m => m.EmployeeID));
            #endregion

            #region SRVisitDetail, CreateSRVisitDetailViewModel
            CreateMap<SRVisitDetail, CreateSRVisitDetailViewModel>()
            .ForMember(vm => vm.SRVisitDID, map => map.MapFrom(m => m.SRVisitDID))
            .ForMember(vm => vm.ProductID, map => map.MapFrom(m => m.ProductID))
            .ForMember(vm => vm.SRVisitID, map => map.MapFrom(m => m.SRVisitID))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => Math.Round(m.Quantity, 2)));
            #endregion

            #region Tuple, GetSRVisitViewModel
            CreateMap<Tuple<int, string, DateTime, string, string, EnumSRVisitType>, GetSRVisitViewModel>()
            .ForMember(vm => vm.SRVisitID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ChallanNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.VisitDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.EmpName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item6.ToString()));
            #endregion

            #region Tuple, GetStockViewModel
            CreateMap<Tuple<int, string, string, string, decimal, decimal,
                decimal, Tuple<string, int, int, decimal>>, GetStockViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.LPPrice, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ProductID, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.ColorID, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.SalesRate, map => map.MapFrom(m => m.Rest.Item4))
            ;
            #endregion
            #region Tuple, GetStockViewModel
            CreateMap<Tuple<int, string, string, string, decimal, decimal,
                decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal>>, GetStockViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.LPPrice, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ProductID, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.ColorID, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.SalesRate, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.CreditSalesRate, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.CreditSalesRate3, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.CreditSalesRate12, map => map.MapFrom(m => m.Rest.Item7))
            ;
            #endregion

            #region Tuple, GetStockViewModel
            CreateMap<Tuple<int, string, string, string, decimal, decimal,
                decimal, Tuple<string, int, int, decimal, decimal, decimal, decimal, Tuple<string>>>, GetStockViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.LPPrice, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ProductID, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.ColorID, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.SalesRate, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.CreditSalesRate, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.CreditSalesRate3, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.CreditSalesRate12, map => map.MapFrom(m => m.Rest.Item7))
                .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Rest.Item1))
            ;
            #endregion


            #region Tuple, GetStockViewModel
            CreateMap<Tuple<int, string, string, string, string, string,
                string, Tuple<string>>, GetStockDetailViewModel>()
            .ForMember(vm => vm.SDetailID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.StockCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.IMENO, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Item7))
             .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Item1))
            ;

            #endregion

            #region SOrder, CreateSalesOrderViewModel
            CreateMap<SOrder, CreateSalesOrderViewModel>()

             .ForMember(vm => vm.CustomerId, map => map.MapFrom(m => m.CustomerID))
            .ForMember(vm => vm.GrandTotal, map => map.MapFrom(m => Math.Round(m.GrandTotal, 2)))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.InvoiceNo))
             .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Remarks))
             .ForMember(vm => vm.TramsAndCondition, map => map.MapFrom(m => m.TramsAndCondition))
            //.ForMember(vm => vm.NetDiscount, map => map.MapFrom(m => Math.Round(m.t, 2)))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.InvoiceDate))
            .ForMember(vm => vm.PaymentDue, map => map.MapFrom(m => Math.Round(m.PaymentDue, 2)))
            .ForMember(vm => vm.TotalDiscountPercentage, map => map.MapFrom(m => Math.Round(m.TDPercentage, 4)))
            .ForMember(vm => vm.TotalDiscountAmount, map => map.MapFrom(m => Math.Round(m.TDAmount, 2)))
            .ForMember(vm => vm.TempFlatDiscountAmount, map => map.MapFrom(m => Math.Round(m.TDAmount, 2)))
            .ForMember(vm => vm.NetDiscount, map => map.MapFrom(m => Math.Round(m.NetDiscount, 2)))
            .ForMember(vm => vm.PPDiscountAmount, map => map.MapFrom(m => Math.Round(m.NetDiscount - m.TDAmount, 2)))
            .ForMember(vm => vm.RecieveAmount, map => map.MapFrom(m => Math.Round(Convert.ToDecimal(m.RecAmount), 2)))
            .ForMember(vm => vm.VATPercentage, map => map.MapFrom(m => Math.Round(m.VATPercentage, 2)))
            .ForMember(vm => vm.VATAmount, map => map.MapFrom(m => Math.Round(m.VATAmount, 2)))
            .ForMember(vm => vm.TotalAmount, map => map.MapFrom(m => Math.Round(m.TotalAmount, 2)))
            .ForMember(vm => vm.AdjAmount, map => map.MapFrom(m => Math.Round(m.AdjAmount, 2)))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => (int)m.Status))
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.SOrderID))
            .ForMember(vm => vm.TotalDue, map => map.MapFrom(m => Math.Round(m.TotalDue, 2)))            
            .ForMember(vm => vm.CreateDate, map => map.MapFrom(m => m.CreateDate))
            .ForMember(vm => vm.TSQty, map => map.MapFrom(m => Math.Round(m.TSQty, 2)));
            #endregion

            CreateMap<Tuple<int, int, int, int, string, string, string,
                    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal, string>>>, CreateSalesOrderDetailViewModel>()
            .ForMember(vm => vm.SODetailId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.StockDetailId, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.Rest.Item2, 2)))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => Math.Round(m.Rest.Item3, 2)))
            .ForMember(vm => vm.UTAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item4, 2)))
            .ForMember(vm => vm.PPDPercentage, map => map.MapFrom(m => Math.Round(m.Rest.Item5, 2)))
            .ForMember(vm => vm.PPDAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item6, 2)))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.PPOffer, map => map.MapFrom(m => m.Rest.Rest.Item2))
            .ForMember(vm => vm.GodownID, map => map.MapFrom(m => m.Rest.Rest.Item3))
            .ForMember(vm => vm.ProductType, map => map.MapFrom(m => m.Rest.Rest.Item4))
            .ForMember(vm => vm.PRate, map => map.MapFrom(m => m.Rest.Rest.Item5))
            .ForMember(vm => vm.Warranty, map => map.MapFrom(m => m.Rest.Rest.Item6))
            .ForMember(vm => vm.Service, map => map.MapFrom(m => m.Rest.Rest.Item6))
            ;
            #region Tuple, CreateSalesOrderDetailViewModel
            CreateMap<Tuple<int, int, int, int, string, string, string,
                    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, int>>>, CreateSalesOrderDetailViewModel>()
            .ForMember(vm => vm.SODetailId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.StockDetailId, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.Rest.Item2, 2)))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => Math.Round(m.Rest.Item3, 2)))
            .ForMember(vm => vm.UTAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item4, 2)))
            .ForMember(vm => vm.PPDPercentage, map => map.MapFrom(m => Math.Round(m.Rest.Item5, 4)))
            .ForMember(vm => vm.PPDAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item6, 2)))
            .ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Rest.Item1))
            .ForMember(vm => vm.GodownID, map => map.MapFrom(m => m.Rest.Rest.Item2));
            //.ForMember(vm => vm.DamageIMEINO, map => map.MapFrom(m => m.Rest.Rest.Item2))
            //.ForMember(vm => vm.ReplaceIMEINO, map => map.MapFrom(m => m.Rest.Rest.Item3))
            //.ForMember(vm => vm.RepOrderID, map => map.MapFrom(m => m.Rest.Rest.Item4));

            #endregion


            //#region Tuple, CreateSalesOrderDetailViewModel
            //CreateMap<Tuple<int, int, int, int, string, string, string,
            //        Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, string, string, int>>>, CreateSalesOrderDetailViewModel>()
            //.ForMember(vm => vm.SODetailId, map => map.MapFrom(m => m.Item1))
            //.ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item2))
            //.ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item3))
            //.ForMember(vm => vm.StockDetailId, map => map.MapFrom(m => m.Item4))
            //.ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item5))
            //.ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item6))
            //.ForMember(vm => vm.IMENo, map => map.MapFrom(m => m.Item7))
            //.ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Rest.Item1))
            //.ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.Rest.Item2, 2)))
            //.ForMember(vm => vm.MRPRate, map => map.MapFrom(m => Math.Round(m.Rest.Item3, 2)))
            //.ForMember(vm => vm.UTAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item4, 2)))
            //.ForMember(vm => vm.PPDPercentage, map => map.MapFrom(m => Math.Round(m.Rest.Item5, 2)))
            //.ForMember(vm => vm.PPDAmount, map => map.MapFrom(m => Math.Round(m.Rest.Item6, 2)))
            //.ForMember(vm => vm.ColorId, map => map.MapFrom(m => m.Rest.Item7))
            //.ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Rest.Rest.Item1))
            //    //.ForMember(vm => vm.PPOffer, map => map.MapFrom(m => m.Rest.Rest.Item2));
            //.ForMember(vm => vm.DamageIMEINO, map => map.MapFrom(m => m.Rest.Rest.Item2))
            //.ForMember(vm => vm.ReplaceIMEINO, map => map.MapFrom(m => m.Rest.Rest.Item3))
            //.ForMember(vm => vm.RepOrderID, map => map.MapFrom(m => m.Rest.Rest.Item4));



            //#endregion

            #region MenuItem, MenuViewModel
            CreateMap<MenuItem, MenuViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(vm => vm.Id))
            .ForMember(vm => vm.Title, map => map.MapFrom(vm => vm.Title))
            .ForMember(vm => vm.Url, map => map.MapFrom(vm => vm.Url))
            .ForMember(vm => vm.Description, map => map.MapFrom(vm => vm.Description))
            .ForMember(vm => vm.WithoutView, map => map.MapFrom(vm => vm.WithoutView))
            .ForMember(vm => vm.ParentId, map => map.MapFrom(vm => vm.ParentId));
            #endregion

            #region Tuple, MenuViewModel
            CreateMap<Tuple<int, string, string, string, string>, MenuViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(vm => vm.Item1))
            .ForMember(vm => vm.Title, map => map.MapFrom(vm => vm.Item2))
            .ForMember(vm => vm.Url, map => map.MapFrom(vm => vm.Item4))
            .ForMember(vm => vm.Description, map => map.MapFrom(vm => vm.Item3))
            .ForMember(vm => vm.ParentId, map => map.MapFrom(vm => vm.Item5));
            #endregion

            #region Tuple, GetSaleOfferViewModel
            CreateMap<Tuple<int, string, string, DateTime, DateTime, string, string, Tuple<string, string, string>>, GetSaleOfferViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.OfferCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.FromDate, map => map.MapFrom(m => m.Item4.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.ToDate, map => map.MapFrom(m => m.Item5.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.Description, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.OfferValue, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.OfferType, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ThresholdValue, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Rest.Item3));
            #endregion

            #region SaleOffers, SaleOffersViewModel
            CreateMap<SaleOffer, CreateSaleOfferViewModel>()
            .ForMember(vm => vm.OfferID, map => map.MapFrom(vm => vm.OfferID))
            .ForMember(vm => vm.OfferCode, map => map.MapFrom(vm => vm.OfferCode))
            .ForMember(vm => vm.ProductID, map => map.MapFrom(vm => vm.ProductID))
            .ForMember(vm => vm.FromDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.FromDate)))
            .ForMember(vm => vm.ToDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.ToDate)))
            .ForMember(vm => vm.Description, map => map.MapFrom(vm => vm.Description))
            .ForMember(vm => vm.OfferValue, map => map.MapFrom(vm => vm.OfferValue))
            .ForMember(vm => vm.OfferType, map => map.MapFrom(vm => vm.OfferType))
            .ForMember(vm => vm.ThresholdValue, map => map.MapFrom(vm => vm.ThresholdValue))
            .ForMember(vm => vm.Status, map => map.MapFrom(vm => vm.Status))
            .ForMember(vm => vm.ConcernID, map => map.MapFrom(vm => vm.ConcernID))
            .ForMember(vm => vm.CreatedBy, map => map.MapFrom(vm => vm.CreatedBy))
            .ForMember(vm => vm.CreateDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.CreateDate)))
            .ForMember(vm => vm.ModifiedBy, map => map.MapFrom(vm => vm.ModifiedBy))
            .ForMember(vm => vm.ModifiedDate, map => map.MapFrom(vm => Convert.ToDateTime(vm.ModifiedDate)));
            #endregion

            #region ReturnOrder, CreateReturnOrderViewModel
            CreateMap<ROrder, CreateReturnOrderViewModel>()
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.InvoiceNo))
            .ForMember(vm => vm.ReturnDate, map => map.MapFrom(m => m.ReturnDate))
            .ForMember(vm => vm.GrandTotal, map => map.MapFrom(m => Math.Round(m.GrandTotal, 2)))
            .ForMember(vm => vm.CustomerID, map => map.MapFrom(m => m.CustomerID))
            .ForMember(vm => vm.PaidAmount, map => map.MapFrom(m => Math.Round(m.PaidAmount, 2)));
            #endregion

            #region ROrderDetail, CreateReturnOrderDetailViewModel
            CreateMap<ROrderDetail, CreateReturnOrderDetailViewModel>()
            .ForMember(vm => vm.ROrderID, map => map.MapFrom(m => m.ROrderID))
            .ForMember(vm => vm.ProductID, map => map.MapFrom(m => m.ProductID))
            .ForMember(vm => vm.Quantity, map => map.MapFrom(m => Math.Round(m.Quantity, 2)))
            .ForMember(vm => vm.UnitPrice, map => map.MapFrom(m => Math.Round(m.UnitPrice, 2)))
            .ForMember(vm => vm.UTAmount, map => map.MapFrom(m => Math.Round(m.UTAmount, 2)));
            #endregion

            #region SalaryMonthly, CreateSalaryMonthlyViewModel
            CreateMap<SalaryMonthly, CreateSalaryMonthlyViewModel>();
            #endregion

            #region AllowanceDeduction,AllowanceDeductionViewModel
            CreateMap<AllowanceDeduction, AllowanceDeductionViewModel>()
                ;
            #endregion
            #region Grade,GradeViewModel
            CreateMap<Grade, GradeViewModel>()
                ;
            #endregion

            #region EmpGradeSalaryAssignment,EmpGradeSalaryAssignmentViewModel
            CreateMap<EmpGradeSalaryAssignment, EmpGradeSalaryAssignmentViewModel>()
                ;
            #endregion

            #region Tuple<int, string, string, decimal?, decimal, string>,EmpGradeSalaryAssignmentViewModel
            CreateMap<Tuple<int, string, string, decimal?, decimal, string>, EmpGradeSalaryAssignmentViewModel>()
            .ForMember(vm => vm.EmpGradeSalaryID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.GradesalarytypeName, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.GradeName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.GrossSalary, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.BasicSalary, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.DateRange, map => map.MapFrom(m => m.Item6))

                ;
            #endregion


            #region Tuple<int, string, string, decimal?, decimal, string>,EmpGradeSalaryAssignmentViewModel
            CreateMap<Tuple<int, List<string>, string, string, string, string>, ADParameterBasicCreate>()
            .ForMember(vm => vm.ADParameterID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.GradeList, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.EntitleTypeName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PeriodicityName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.AllowDeductName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.IsTaxableName, map => map.MapFrom(m => m.Item6))

                ;
            #endregion

            #region ADParameterBasic,ADParameterBasicCreate
            CreateMap<ADParameterBasic, ADParameterBasicCreate>()
                ;
            #endregion

            #region Department,DepartmentViewModel
            CreateMap<Department, DepartmentViewModel>()
                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Status))
                ;
            #endregion

            #region HolidayCalender,HolidayCalenderViewModel
            CreateMap<HolidayCalender, HolidayCalenderViewModel>()
                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Status))
                ;
            #endregion
            #region EmployeeLeave,EmployeeLeaveViewModel
            CreateMap<EmployeeLeave, EmployeeLeaveViewModel>()
                .ForMember(vm => vm.IsPaidLeave, map => map.MapFrom(m => m.PaidLeave == 1 ? true : false))
                ;
            #endregion

            #region EmployeeLeave,EmployeeLeaveViewModel
            CreateMap<Tuple<int, DateTime, string, string, bool, string, Tuple<decimal, string, string, string, string>>, EmployeeLeaveViewModel>()
                .ForMember(vm => vm.EmployeeLeaveID, map => map.MapFrom(m => m.Item1))
                .ForMember(vm => vm.LeaveDate, map => map.MapFrom(m => m.Item2))
                .ForMember(vm => vm.LeaveType, map => map.MapFrom(m => m.Item3))
                .ForMember(vm => vm.Description, map => map.MapFrom(m => m.Item4))
                .ForMember(vm => vm.IsPaidLeave, map => map.MapFrom(m => m.Item5))
                .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item6))
                .ForMember(vm => vm.ShortLeaveHour, map => map.MapFrom(m => m.Item7.Item1))
                .ForMember(vm => vm.EmplyeeName, map => map.MapFrom(m => m.Item7.Item2))
                .ForMember(vm => vm.DepartmentName, map => map.MapFrom(m => m.Item7.Item3))
                .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item7.Item4))
                .ForMember(vm => vm.GradeName, map => map.MapFrom(m => m.Item7.Item5))
                ;
            #endregion
            #region Attendence,AttendenceViewModel
            CreateMap<Attendence, AttendenceViewModel>()
                ;
            #endregion

            #region Attendence,AttendenceViewModel
            CreateMap<AttendenceMonth, AttendenceMonthViewModel>()
                ;
            #endregion
            #region Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string>>,AdvanceSalaryViewModel
            CreateMap<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string, EnumSalaryType>>, AdvanceSalaryViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.Item1))
                .ForMember(vm => vm.EmployeeID, map => map.MapFrom(m => m.Item2))
                .ForMember(vm => vm.EmployeeCode, map => map.MapFrom(m => m.Item3))
                .ForMember(vm => vm.EmployeeName, map => map.MapFrom(m => m.Item4))
                .ForMember(vm => vm.DepartmentName, map => map.MapFrom(m => m.Item5))
                .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item6))
                .ForMember(vm => vm.GradeName, map => map.MapFrom(m => m.Item7))
                .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Rest.Item1))
                .ForMember(vm => vm.Date, map => map.MapFrom(m => m.Rest.Item2))
                .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Rest.Item3))
                .ForMember(vm => vm.SalaryType, map => map.MapFrom(m => m.Rest.Item4))

                ;
            #endregion
            #region Attendence,AttendenceViewModel
            CreateMap<AdvanceSalary, AdvanceSalaryViewModel>()
                ;
            #endregion
            #region Tuple<int, DateTime, int>,SalaryProcessViewModel
            CreateMap<Tuple<int, DateTime, int>, SalaryProcessViewModel>()
                    .ForMember(vm => vm.SalaryProcessID, map => map.MapFrom(m => m.Item1))
                .ForMember(vm => vm.SalaryProcessMonth, map => map.MapFrom(m => m.Item2))
                .ForMember(vm => vm.TotalEmployee, map => map.MapFrom(m => m.Item3))
                ;
            #endregion

            #region Tuple<int, DateTime, int>,SalaryProcessViewModel
            CreateMap<Employee, GetEmployeeViewModel>()
                 .ForMember(vm => vm.Id, map => map.MapFrom(m => m.EmployeeID))
                .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Name))
                .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
                ;
            #endregion
            #region TargetSetup,TargetSetupViewModel
            CreateMap<TargetSetup, TargetSetupViewModel>()
                ;
            #endregion
            #region Tuple<int, DateTime, int, decimal, decimal, string, string, Tuple<string>>,TargetSetupViewModel
            CreateMap<Tuple<int, DateTime, int, decimal, decimal, string, string, Tuple<string>>, TargetSetupViewModel>()
                 .ForMember(vm => vm.TID, map => map.MapFrom(m => m.Item1))
                 .ForMember(vm => vm.TargetMonth, map => map.MapFrom(m => m.Item2))
                 .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item3))
                 .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.Item4))
                 .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Item5))
                 .ForMember(vm => vm.EmployeeName, map => map.MapFrom(m => m.Item6))
                 .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item7))
                 .ForMember(vm => vm.DepartmentName, map => map.MapFrom(m => m.Rest.Item1))

                ;
            #endregion

            #region TargetSetup,TargetSetupViewModel
            CreateMap<ProductWisePurchaseModel, GetProductViewModel>()
                 .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.ProductID))
                 .ForMember(vm => vm.CategoryID, map => map.MapFrom(m => m.CategoryID))
                 .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.MRP))
                 .ForMember(vm => vm.RP, map => map.MapFrom(m => m.RP))
                ;
            #endregion
            #region CommissionSetup,CommissionSetupViewModel
            CreateMap<CommissionSetup, CommissionSetupViewModel>()
                ;
            #endregion


            CreateMap<Tuple<int, DateTime, decimal, decimal, decimal, decimal, int, Tuple<string>>, CommissionSetupViewModel>()
             .ForMember(vm => vm.CSID, map => map.MapFrom(m => m.Item1))
             .ForMember(vm => vm.CommissionMonth, map => map.MapFrom(m => m.Item2))
             .ForMember(vm => vm.AchievedPercentStart, map => map.MapFrom(m => m.Item3))
             .ForMember(vm => vm.AchievedPercentEnd, map => map.MapFrom(m => m.Item4))
             .ForMember(vm => vm.CommissionPercent, map => map.MapFrom(m => m.Item5))
             .ForMember(vm => vm.CommisssionAmt, map => map.MapFrom(m => m.Item6))
             .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7))
             .ForMember(vm => vm.EmployeeName, map => map.MapFrom(m => m.Rest.Item1));

            #region DesWiseCommission,DesWiseCommissionViewModel
            CreateMap<Tuple<int, Decimal, string>, DesWiseCommissionViewModel>()
             .ForMember(vm => vm.ID, map => map.MapFrom(m => m.Item1))
             .ForMember(vm => vm.CommissionPercent, map => map.MapFrom(m => m.Item2))
             .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item3))
                ;
            #endregion
            #region CommissionSetup,CommissionSetupViewModel
            CreateMap<DesWiseCommission, DesWiseCommissionViewModel>()
                ;
            #endregion


            CreateMap<Tuple<int, string, DateTime, decimal, decimal, decimal, int, Tuple<string, string>>, GetTransferViewModel>()
                      .ForMember(vm => vm.TransferID, map => map.MapFrom(m => m.Item1))
                      .ForMember(vm => vm.TransferNo, map => map.MapFrom(m => m.Item2))
                      .ForMember(vm => vm.TransferDate, map => map.MapFrom(m => m.Item3))
                      .ForMember(vm => vm.TotalAmount, map => map.MapFrom(m => m.Item4))
                      .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7))
                      .ForMember(vm => vm.ToConcernName, map => map.MapFrom(m => m.Rest.Item1))
                      .ForMember(vm => vm.FromConcern, map => map.MapFrom(m => m.Rest.Item2));

            #region Tuple, SMSStatusViewModel
            CreateMap<Tuple<DateTime, string, string, int, EnumSMSSendStatus, string, string, Tuple<string, string, string, string>>, SMSStatusViewModel>()
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.NoOfSMS, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.SendingStatus, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CustomerCode, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.Address, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ResponseMsg, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.SMSFormateDescription, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.SMS, map => map.MapFrom(m => m.Rest.Item4))
            ;
            #endregion

            #region SMSBillPayment, SMSBillPaymentViewModel
            CreateMap<SMSBillPayment, SMSBillPaymentViewModel>()
                ;
            #endregion

            #region CardType,CardTypeViewModel
            CreateMap<CardType, CardTypeViewModel>()
                ;
            #endregion

            #region CardTypeSetup,CardTypeSetupViewModel
            CreateMap<CardTypeSetup, CardTypeSetupViewModel>()
                ;
            #endregion

            #region CardTypeSetup,CardTypeSetupViewModel
            CreateMap<Tuple<int, string, decimal, string, string, string>, CardTypeSetupViewModel>()
             .ForMember(vm => vm.CardTypeSetupID, map => map.MapFrom(m => m.Item1))
             .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
             .ForMember(vm => vm.Percentage, map => map.MapFrom(m => m.Item3))
             .ForMember(vm => vm.BankName, map => map.MapFrom(m => m.Item4))
             .ForMember(vm => vm.AccountNo, map => map.MapFrom(m => m.Item5))
             .ForMember(vm => vm.CardTypeName, map => map.MapFrom(m => m.Item6))
                ;
            #endregion

            #region ShareInvestmentHead,InvestmentheadViewModel
            CreateMap<ShareInvestmentHead, InvestmentheadViewModel>()
                .ForMember(vm => vm.OpeningType, map => map.MapFrom(m => m.OpeningType))
                ;
            #endregion
            #region ShareInvestment,ShareInvestmentViewModel
            CreateMap<ShareInvestment, ShareInvestmentViewModel>()
                ;
            #endregion
            #region Tuple<int, string, string, string>,InvestmentheadViewModel
            CreateMap<Tuple<int, string, string, string>, InvestmentheadViewModel>()
             .ForMember(vm => vm.SIHID, map => map.MapFrom(m => m.Item1))
             .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Item2))
             .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item3))
             .ForMember(vm => vm.ParentName, map => map.MapFrom(m => m.Item4))
                ;
            #endregion

            #region ShareInvestment,ShareInvestmentViewModel
            CreateMap<ShareInvestment, ShareInvestmentViewModel>()
                ;
            #endregion


            #region Tuple<int, string, string, string>,InvestmentheadViewModel
            CreateMap<Tuple<int, DateTime, string, string, decimal>, ShareInvestmentViewModel>()
             .ForMember(vm => vm.SIID, map => map.MapFrom(m => m.Item1))
             .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.Item2))
             .ForMember(vm => vm.HeadName, map => map.MapFrom(m => m.Item3))
             .ForMember(vm => vm.Purpose, map => map.MapFrom(m => m.Item4))
             .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Item5))
                ;
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string, string, string, decimal, decimal,
                Tuple<decimal, decimal, decimal, decimal, string, List<string>,string>>
                , ProductDetailsModel>()
            .ForMember(vm => vm.StockID, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ColorName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.StockQty, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.MRPPrice, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.SalesPrice, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.CreditSalesPrice, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.CreditSalesPrice3, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.CreditSalesPrice12, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.GodownName, map => map.MapFrom(m => m.Rest.Item5))
            .ForMember(vm => vm.IMEIList, map => map.MapFrom(m => m.Rest.Item6))
            .ForMember(vm => vm.PCategoryName, map => map.MapFrom(m => m.Rest.Item7))
            .ForMember(vm => vm.ProMRP, map => map.MapFrom(m => m.Rest.Item4))
            ;
            #endregion

            #region SystemInformation, CreateSystemInformationViewModel
            CreateMap<SystemInformation, CreateSMSSettingViewModel>()
             .ForMember(vm => vm.Id, map => map.MapFrom(m => m.SystemInfoID))
           .ForMember(vm => vm.IsBanglaSmsEnable, map => map.MapFrom(m => m.IsBanglaSmsEnable))
             .ForMember(vm => vm.RetailSaleSmsService, map => map.MapFrom(m => m.IsRetailSMSEnable))
            .ForMember(vm => vm.HireSaleSmsService, map => map.MapFrom(m => m.IsHireSMSEnable))
            .ForMember(vm => vm.CashCollectionSmsService, map => map.MapFrom(m => m.IsCashcollSMSEnable))
            .ForMember(vm => vm.InstallmentSmsService, map => map.MapFrom(m => m.IsInstallmentSMSEnable))
            .ForMember(vm => vm.RemindDateSmsService, map => map.MapFrom(m => m.IsRemindSMSEnable));
            #endregion

            #region ParentCategory,ParentCategoryViewModel
            CreateMap<ParentCategory, ParentCategoryViewModel>();
            ;
            #endregion

            #region CreateCashCollectionViewModel, CashCollection
            CreateMap<CashCollection, CreateCashCollectionViewModel>()
            .ForMember(vm => vm.TempAmount, map => map.MapFrom(m => m.Amount))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.Type, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.TempAdjustAmt, map => map.MapFrom(m => m.AdjustAmt))
            .ForMember(vm => vm.CurrentDue, map => map.MapFrom(m => m.BalanceDue))
            .ForMember(vm => vm.DeliveryType, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.EmobileType, map => map.MapFrom(m => m.TransactionType))
            .ForMember(vm => vm.EmobileDeliveryType, map => map.MapFrom(m => m.TransactionType))
            ;

            #endregion

            #region Tuple, GetSalesOrderViewModelNew
            CreateMap<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType, Tuple<string, string>>, GetSalesOrderViewModel>()
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))

            .ForMember(vm => vm.DueAmount, map => map.MapFrom(m => m.Item6.ToString("#,###")))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => m.Item7.ToString()))

            .ForMember(vm => vm.UserName, map => map.MapFrom(m => (m.Rest.Item2.ToString())))

            ;
            #endregion

            #region Tuple, GetCreditSalesOrderViewModelNew
            CreateMap<Tuple<int, string, DateTime, string, string, decimal, EnumSalesType,
                Tuple<string, string>>, GetCreditSalesOrderViewModel>()
            .ForMember(vm => vm.SalesOrderId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.InvoiceNo, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.OrderDate, map => map.MapFrom(m => m.Item3.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.ContactNo, map => map.MapFrom(m => m.Item5))
            
            .ForMember(vm => vm.DueAmount, map => map.MapFrom(m => m.Item6.ToString()))
            .ForMember(vm => vm.Status, map => map.MapFrom(m => (m.Item7.ToString())))
          
            .ForMember(vm => vm.UserName, map => map.MapFrom(m => (m.Rest.Item2.ToString())))
           
            ;
            #endregion

            #region Tuple, GetCashCollectionViewModel
            CreateMap<Tuple<int, DateTime, string, string, string,
                string, string, Tuple<string, string, string>>, GetCashCollectionViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.Item2.ToString("dd-MMM-yyyy")))
            .ForMember(vm => vm.ReceiptNo, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.Name, map => map.MapFrom(m => m.Item4))
            //.ForMember(vm => vm.SupplierName, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.AccountNo, map => map.MapFrom(m => m.Item5))
             .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.TransactionType, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Rest.Item1))           
            .ForMember(vm => vm.UserName, map => map.MapFrom(m => m.Rest.Item3))
            ;
            #endregion

            #region CreateExpenditureViewModel, Expenditure
            CreateMap<Expenditure, CreateExpenditureViewModel>()
            .ForMember(vm => vm.Id, map => map.MapFrom(m => m.ExpenditureID))
            .ForMember(vm => vm.ExpenseItemName, map => map.MapFrom(m => m.ExpenseItem.Description))
            .ForMember(vm => vm.EntryDate, map => map.MapFrom(m => m.EntryDate.ToString("dd MMM yyyy")))
            .ForMember(vm => vm.CreatedBy, map => map.MapFrom(m => m.User.UserName))
            ;
            #endregion

            #region SOrderDetail,CreateSalesOrderDetailViewModel
            CreateMap<SOrderDetail, CreateSalesOrderDetailViewModel>()
            .ForMember(vm => vm.StockDetailId, map => map.MapFrom(m => m.SDetailID))
             .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.MPRate))

                ;
            #endregion

            #region MonthlyAttendence,MonthlyAttendenceVM
            CreateMap<MonthlyAttendence, MonthlyAttendenceVM>();
            ;
            #endregion


            #region ProductDetailsModel, GetStockDetailViewModel
            CreateMap<ProductDetailsModel, GetStockDetailViewModel>()
                      .ForMember(vm => vm.StockCode, map => map.MapFrom(m => m.ProductCode));
            #endregion

            #region ProductDetailsModel, GetStockDetailViewModel
            CreateMap<ProductDetailsModel, GetStockViewModel>()
             .ForMember(vm => vm.Code, map => map.MapFrom(m => m.ProductCode))
                        .ForMember(vm => vm.Quantity, map => map.MapFrom(m => m.StockQty))
                          .ForMember(vm => vm.SalesRate, map => map.MapFrom(m => m.SalesPrice));
            #endregion
            #region Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string>>,DueSalaryViewModel
            CreateMap<Tuple<int, int, string, string, string, string, string, Tuple<decimal, DateTime, string>>, DueSalaryViewModel>()
                .ForMember(vm => vm.ID, map => map.MapFrom(m => m.Item1))
                .ForMember(vm => vm.EmployeeID, map => map.MapFrom(m => m.Item2))
                .ForMember(vm => vm.EmployeeCode, map => map.MapFrom(m => m.Item3))
                .ForMember(vm => vm.EmployeeName, map => map.MapFrom(m => m.Item4))
                .ForMember(vm => vm.DepartmentName, map => map.MapFrom(m => m.Item5))
                .ForMember(vm => vm.DesignationName, map => map.MapFrom(m => m.Item6))
                .ForMember(vm => vm.GradeName, map => map.MapFrom(m => m.Item7))
                .ForMember(vm => vm.Amount, map => map.MapFrom(m => m.Rest.Item1))
                .ForMember(vm => vm.Date, map => map.MapFrom(m => m.Rest.Item2))
                .ForMember(vm => vm.Remarks, map => map.MapFrom(m => m.Rest.Item3))

                ;
            #endregion
            #region AdvanceSalary,DueSalaryViewModel
            CreateMap<AdvanceSalary, DueSalaryViewModel>()
                ;
            #endregion

            #region CreditInterestPercentage, CreditInterestPercentageViewModel
            CreateMap<CreditInterestPercentage, CreditInterestPercentageViewModel>()
            .ForMember(vm => vm.IntPercentageID, map => map.MapFrom(m => m.IntPercentageID))
            .ForMember(vm => vm.Code, map => map.MapFrom(m => m.Code))
            .ForMember(vm => vm.Percentage, map => map.MapFrom(m => m.Percentage))
            .ForMember(vm => vm.ConcernId, map => map.MapFrom(m => m.ConcernID))
            .ForMember(vm => vm.EffectDate, map => map.MapFrom(m => Convert.ToDateTime(m.EffectDate)));
            #endregion

            #region BankLoan, CreateBankLoanVM
            CreateMap<BankLoan, CreateBankLoanVM>();
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, int, decimal>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.RP, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ProductType, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            ;
            #endregion

            #region Tuple, GetProductViewModel
            CreateMap<Tuple<int, string, string,
            decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>, GetProductViewModel>()
            .ForMember(vm => vm.ProductId, map => map.MapFrom(m => m.Item1))
            .ForMember(vm => vm.ProductCode, map => map.MapFrom(m => m.Item2))
            .ForMember(vm => vm.ProductName, map => map.MapFrom(m => m.Item3))
            .ForMember(vm => vm.PWDiscount, map => map.MapFrom(m => m.Item4))
            .ForMember(vm => vm.PicturePath, map => map.MapFrom(m => m.Item5))
            .ForMember(vm => vm.CategoryName, map => map.MapFrom(m => m.Item6))
            .ForMember(vm => vm.CompanyName, map => map.MapFrom(m => m.Item7))
            .ForMember(vm => vm.PreStock, map => map.MapFrom(m => m.Rest.Item1))
            .ForMember(vm => vm.ProductType, map => map.MapFrom(m => m.Rest.Item2))
            .ForMember(vm => vm.MRPRate, map => map.MapFrom(m => m.Rest.Item3))
            .ForMember(vm => vm.RP, map => map.MapFrom(m => m.Rest.Item4))
            .ForMember(vm => vm.PRate, map => map.MapFrom(m => m.Rest.Item4))
            ;
            #endregion

            #region DO,DOViewModel
            CreateMap<DO, DOViewModel>()
              .ForMember(vm => vm.CustomerName, map => map.MapFrom(m => m.Customer.Name))
              .ForMember(vm => vm.PaymentDue, map => map.MapFrom(m => (m.TotalAmt - m.PaidAmt)))
              .ForMember(vm => vm.TotalDiscountPercentage, map => map.MapFrom(m => (m.FlatDiscountPer)))
              .ForMember(vm => vm.TotalDiscountAmount, map => map.MapFrom(m => (m.FlatDiscount)))

                ;
            #endregion

            #region DODetail,DODetailViewModel
            CreateMap<DODetail, DODetailViewModel>()
                ;
            #endregion
        }
    }
}