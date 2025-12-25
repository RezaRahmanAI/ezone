using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IMSWEB.Service;
using IMSWEB.Model;
using System.Data;
using Microsoft.Reporting.WebForms;
using IMSWEB.Report.DataSets;

namespace IMSWEB.Report
{
    public class BasicReport : IBasicReport
    {
        DataSet _dataSet = null;
        ReportParameter _reportParameter = null;
        List<ReportParameter> _reportParameters = null;
        IEmployeeService _employeeService;
        ISystemInformationService _systemInformationService;
        ICustomerService _customerService;
        ISupplierService _SupplierService;
        ICreditSalesOrderService _creditSalesOrderService;
        private readonly ISalesOrderService _salesOrderService;
        public BasicReport(IEmployeeService employeeService, ICustomerService customerService, ISupplierService supplierService, ISystemInformationService systemInformationService, ICreditSalesOrderService creditSalesOrderService, ISalesOrderService salesOrderService)
        {
            _SupplierService = supplierService;
            _customerService = customerService;
            _employeeService = employeeService;
            _systemInformationService = systemInformationService;
            _creditSalesOrderService = creditSalesOrderService;
            _salesOrderService = salesOrderService;
        }

        public async Task<byte[]> EmployeeInformationReport(string userName, int concernID)
        {
            try
            {
                var empInfos = await _employeeService.GetAllEmployeeAsync();
                DataRow row = null;
                BasicDataSet.dtEmployeesInfoDataTable dtEmployeesInfo = new BasicDataSet.dtEmployeesInfoDataTable();

                foreach (var item in empInfos)
                {
                    row = dtEmployeesInfo.NewRow();
                    row["EmpCode"] = item.Item2;
                    row["Name"] = item.Item3;
                    row["Designation"] = item.Item7;
                    row["ContactNo"] = item.Item4;
                    row["JoiningDate"] = item.Item6;

                    dtEmployeesInfo.Rows.Add(row);
                }

                dtEmployeesInfo.TableName = "dtEmployeesInfo";
                _dataSet = new DataSet();
                _dataSet.Tables.Add(dtEmployeesInfo);
                GetCommonParameters(userName, concernID);

                return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "EmployeeInformation.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] CustomerCategoryWiseDueRpt(string userName, int concernID, int CustomerId, int reportType, int DueType)
        {
            try
            {
                #region old data
                //var customerDueInfo = _customerService.CustomerCategoryWiseDueRpt(concernID, CustomerId, reportType, DueType);

                //BasicDataSet.dtCustomerDataTable dt = new BasicDataSet.dtCustomerDataTable();
                //_dataSet = new DataSet();
                //DataRow row = null;
                //ProductDetailsModel product = null;

                //foreach (var grd in customerDueInfo)
                //{
                //    row = dt.NewRow();
                //    row["CCode"] = grd.Item1 + ", " + grd.Item2 + ", " + grd.Item6 + ", " + grd.Item5;
                //    row["CName"] = grd.Item2;

                //    product = _creditSalesOrderService.GetLastSalesOrderByCustomerID(grd.Rest.Item3);
                //    if (product != null)
                //    {
                //        row["LastRec"] = Math.Round(product.RecAmount, 2);
                //        row["LastCollDate"] = product.CollectionDate.ToShortDateString();
                //        row["LastSalesDate"] = product.Date.ToShortDateString();
                //    }

                //    row["CompanyName"] = grd.Item3;
                //    row["CusType"] = grd.Item4;
                //    row["ContactNo"] = grd.Item5;
                //    row["Address"] = grd.Item6;
                //    row["TotalDue"] = grd.Rest.Item2;
                //    row["SalesDue"] = grd.Item7;
                //    row["HireDue"] = grd.Rest.Item1;
                //    dt.Rows.Add(row);
                //}

                //dt.TableName = "dtCustomer";
                //_dataSet.Tables.Add(dt);
                #endregion

                var customerDueInfo = _salesOrderService.GetCustomerWiseTotalDue(CustomerId, concernID);

                if (customerDueInfo.Any())
                {
                    if (reportType > 0)
                    {
                        switch (reportType)
                        {
                            case (int)EnumCustomerType.Retail:
                                customerDueInfo = customerDueInfo.Where(c => c.CustomerType.Equals(EnumCustomerType.Retail.ToString())).ToList();
                                break;
                            case (int)EnumCustomerType.Dealer:
                                customerDueInfo = customerDueInfo.Where(c => c.CustomerType.Equals(EnumCustomerType.Dealer.ToString())).ToList();
                                break;
                            case (int)EnumCustomerType.Hire:
                                customerDueInfo = customerDueInfo.Where(c => c.CustomerType.Equals(EnumCustomerType.Hire.ToString())).ToList();
                                break;
                            case (int)EnumCustomerType.Branch:
                                customerDueInfo = customerDueInfo.Where(c => c.CustomerType.Equals(EnumCustomerType.Branch.ToString())).ToList();
                                break;

                            default:
                                break;  
                        }
                    }
                }

                BasicDataSet.dtCustomerDataTable dt = new BasicDataSet.dtCustomerDataTable();
                _dataSet = new DataSet();
                DataRow row = null;
                ProductDetailsModel product = null;

                foreach (var grd in customerDueInfo)
                {
                    row = dt.NewRow();
                    row["CCode"] = grd.CustomerAddress;
                    row["CName"] = grd.CustomerName;

                    product = _creditSalesOrderService.GetLastSalesOrderByCustomerID(grd.CustomerID);
                    if (product != null)
                    {
                        row["LastRec"] = Math.Round(product.RecAmount, 2);
                        //row["InterestAmt"] = Math.Round(Ca.RecAmount, 2);

                        row["LastCollDate"] = product.CollectionDate.ToShortDateString();
                        row["LastSalesDate"] = product.Date.ToShortDateString();
                    }

                    row["CompanyName"] = grd.CustomerName;
                    row["CusType"] = grd.CustomerType;
                    row["ContactNo"] = grd.ContactNo;
                    row["Address"] = grd.Address;
                    row["TotalDue"] = grd.TotalDue;
                    row["SalesDue"] = grd.SalesDue;
                    row["HireDue"] = grd.HireDue;
                    row["TInterestAmount"] = grd.TInterestAmount;
                    row["CrInterestAmount"] = grd.CrInterestAmount;
                    dt.Rows.Add(row);
                    var total = customerDueInfo.Sum(d => d.TotalDue);
                }

                dt.TableName = "dtCustomer";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptCustomer.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ConcernWiseSupplierDueRpt_Old(string userName, int UserconcernID,
            int SupplierId, int concerID, bool IsAdminReport)
        {
            try
            {
                var supplierDueInfo = _SupplierService.ConcernWiseSupplierDueRpt(concerID, SupplierId, IsAdminReport);

                BasicDataSet.dtSupplierDataTable dt = new BasicDataSet.dtSupplierDataTable();
                _dataSet = new DataSet();

                foreach (var grd in supplierDueInfo)
                {
                    dt.Rows.Add(grd.Item1, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7);
                }

                dt.TableName = "dtSupplier";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserconcernID);
                if (IsAdminReport)
                    return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptAdminSupplier.rdlc");
                else
                    return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptSupplier.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public byte[] ConcernWiseSupplierDueRpt(string userName, int UserconcernID,
            int SupplierId, int concerID, bool IsAdminReport)
        {
            try
            {
                //var supplierDueInfo = _SupplierService.ConcernWiseSupplierDueRpt(concerID, SupplierId, IsAdminReport);
                DateTime asOnDate = Convert.ToDateTime(GetClientDateTime());


                var supplierDueInfo = _SupplierService.GetCustomerDateWiseTotalDue(SupplierId, concerID, asOnDate, asOnDate, false, UserconcernID); 

                BasicDataSet.dtSupplierDataTable dt = new BasicDataSet.dtSupplierDataTable();
                _dataSet = new DataSet();
                DataRow row = null;

                if (supplierDueInfo != null && supplierDueInfo.Any())
                {
                    foreach (var loan in supplierDueInfo)
                    {
                        row = dt.NewRow();
                        row["SCode"] = loan.Code;
                        row["SName"] = loan.Name;
                        row["OwnerName"] = loan.OwnerName;
                        row["ContactNo"] = loan.ContactNo;
                        row["Address"] = loan.Address;
                        row["TotalDue"] = loan.TotalDue;
                        row["ConcernName"] = loan.ConcernName;
                        dt.Rows.Add(row);
                    }
                }


                //foreach (var grd in supplierDueInfo)
                //{
                //    dt.Rows.Add(grd.Item1, grd.Item2, grd.Item3, grd.Item4, grd.Item5, grd.Item6, grd.Item7);
                //}

                dt.TableName = "dtSupplier";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, UserconcernID);
                if (IsAdminReport)
                    return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptAdminSupplier.rdlc");
                else
                    return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptSupplier.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void GetCommonParameters(string userName, int concernID)
        {
            string logoPath = string.Empty;
            SystemInformation currentSystemInfo = _systemInformationService.GetSystemInformationByConcernId(concernID);
            _reportParameters = new List<ReportParameter>();

            _reportParameter = new ReportParameter("Logo", logoPath);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("CompanyName", currentSystemInfo.Name);
            _reportParameters.Add(_reportParameter);

            _reportParameter = new ReportParameter("Phone", currentSystemInfo.TelephoneNo);
            _reportParameters.Add(_reportParameter);


            _reportParameter = new ReportParameter("Address", currentSystemInfo.Address);
            _reportParameters.Add(_reportParameter);
            _reportParameter = new ReportParameter("PrintedBy", userName);
            _reportParameters.Add(_reportParameter);
        }


        public byte[] GetCustomerDetails(string userName, int concernID, int CustomerID)
        {
            try
            {
                var Customer = _customerService.GetCustomerById(CustomerID);
                DataRow row = null;

                BasicDataSet.dtCustomerDetailsDataTable dt = new BasicDataSet.dtCustomerDetailsDataTable();
                _dataSet = new DataSet();

                row = dt.NewRow();
                row["Code"] = Customer.Code;
                row["Name"] = Customer.Name;
                row["Address"] = Customer.Address;
                row["CompanyName"] = Customer.CompanyName;
                row["ContactNo"] = Customer.ContactNo;
                row["CreditDue"] = Customer.CreditDue;
                row["TotalDue"] = Customer.TotalDue;
                row["CustomerType"] = Customer.CustomerType.ToString();
                row["EmailID"] = Customer.EmailID;
                row["FName"] = Customer.FName;
                row["NID"] = Customer.NID;
                row["OpeningDue"] = Customer.OpeningDue;
                row["RefAddress"] = Customer.RefAddress;
                row["RefContactNo"] = Customer.RefContact;
                row["RefFName"] = Customer.RefFName;
                row["RefName"] = Customer.RefName;
                row["Remarks"] = Customer.Remarks;
                row["Profession"] = Customer.Profession;
                dt.Rows.Add(row);

                dt.TableName = "dtCustomerDetails";
                _dataSet.Tables.Add(dt);

                GetCommonParameters(userName, concernID);
                return ReportBase.GenerateBasicReport(_dataSet, _reportParameters, "rptCustomerDetails.rdlc");
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public string GetClientDateTime()
        {
            DateTime utcTime = DateTime.UtcNow;
            TimeZoneInfo BdZone = TimeZoneInfo.FindSystemTimeZoneById("Bangladesh Standard Time");
            DateTime localDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcTime, BdZone);
            return localDateTime.ToString("dd MMM yyyy HH:mm:ss");
        }
    }
}
