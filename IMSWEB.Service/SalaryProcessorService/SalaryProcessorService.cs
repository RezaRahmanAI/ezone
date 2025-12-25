using IMSWEB.Data;
using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace IMSWEB.Service
{
    public class SalaryProcessorService : ISalaryProcessorService
    {
        private readonly IBaseRepository<ADParameterBasic> _ADParamBasicRepository;
        private readonly IBaseRepository<ADParameterGrade> _ADParamGradeRepository;
        private readonly IBaseRepository<ADParameterEmployee> _ADParameterEmployeeRepository;
        private readonly IBaseRepository<AllowanceDeduction> _AllowanceDeductionRepository;
        private readonly IBaseRepository<AttendenceMonth> _AttendenceMonthRepository;
        private readonly IBaseRepository<Attendence> _AttendenceRepository;
        private readonly IBaseRepository<HolidayCalender> _holidayCalenderRepisitory;
        private readonly IBaseRepository<AdvanceSalary> _advanceSalaryRepository;
        private readonly IBaseRepository<EmployeeLeave> _employeeLeaveRepository;
        private readonly IBaseRepository<SalaryProcess> _BaseSalaryProcessRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ISalaryProcessRepository _SalaryProcessRepository;
        private readonly IBaseRepository<SOrder> _SOrderRepository;
        private readonly IBaseRepository<SOrderDetail> _SOrderDetailRepository;
        private readonly IBaseRepository<CommissionSetup> _CommissionSetupRepository;
        private readonly IBaseRepository<Customer> _CustomerRepository;
        private readonly IUserService _UserService;
        private readonly IExpenditureService _ExpenditureService;
        private readonly ICashCollectionService _CashCollectionService;
        private readonly IBaseRepository<ManualAttendence> _manualAttendence;
        private readonly IBaseRepository<SalaryMonthly> _SalaryMonthlyRepository;
        private readonly IBaseRepository<MonthlyAttendence> _MonthlyAttRepository;

        public SalaryProcessorService(
            IBaseRepository<ADParameterBasic> ADParamBasicRepository, IBaseRepository<AllowanceDeduction> AllowanceDeductionRepository,
            IBaseRepository<Attendence> AttendenceRepository,
            IBaseRepository<HolidayCalender> holidayCalenderRepisitory, IBaseRepository<AdvanceSalary> advanceSalaryRepository,
            IBaseRepository<EmployeeLeave> employeeLeaveRepository, IBaseRepository<SalaryProcess> baseSalaryProcessRepository,
            IBaseRepository<ADParameterGrade> ADParamGradeRepository, ISalaryProcessRepository SalaryProcessRepository,
            IBaseRepository<SOrder> SOrderRepository, IBaseRepository<SOrderDetail> SOrderDetailRepository,
            IBaseRepository<CommissionSetup> CommissionSetupRepository, IBaseRepository<Customer> CustomerRepository,
            IBaseRepository<AttendenceMonth> AttendenceMonthRepository, IBaseRepository<ADParameterEmployee> ADParameterEmployeeRepository,
            IUnitOfWork unitOfWork, IUserService UserService,
            IExpenditureService ExpenditureService,
            ICashCollectionService CashCollectionService,
            IBaseRepository<ManualAttendence> manualAttendenc,
            IBaseRepository<SalaryMonthly> SalaryMonthlyDetailRepository,
            IBaseRepository<MonthlyAttendence> MonthlyAttRepository
            )
        {
            _ADParamBasicRepository = ADParamBasicRepository;
            _AllowanceDeductionRepository = AllowanceDeductionRepository;
            _AttendenceRepository = AttendenceRepository;
            _holidayCalenderRepisitory = holidayCalenderRepisitory;
            _advanceSalaryRepository = advanceSalaryRepository;
            _employeeLeaveRepository = employeeLeaveRepository;
            _ADParamGradeRepository = ADParamGradeRepository;
            _BaseSalaryProcessRepository = baseSalaryProcessRepository;
            _unitOfWork = unitOfWork;
            _SalaryProcessRepository = SalaryProcessRepository;
            _SOrderRepository = SOrderRepository;
            _SOrderDetailRepository = SOrderDetailRepository;
            _CommissionSetupRepository = CommissionSetupRepository;
            _CustomerRepository = CustomerRepository;
            _AttendenceMonthRepository = AttendenceMonthRepository;
            _ADParameterEmployeeRepository = ADParameterEmployeeRepository;
            _UserService = UserService;
            _ExpenditureService = ExpenditureService;
            _CashCollectionService = CashCollectionService;
            _manualAttendence = manualAttendenc;
            _SalaryMonthlyRepository = SalaryMonthlyDetailRepository;
            _MonthlyAttRepository = MonthlyAttRepository;
        }


        public SalaryMonthly MonthlySalaryProcessByEmployeeID(Employee oEmployee, bool IsFestBonusApply, EmpGradeSalaryAssignment EMPGradeSalary,
            SystemInformation sysInfo, TargetSetup EmployeeTarget, bool IsShowroomCommission, decimal TotalCollectionAmount,
            decimal VoltageStabilizerComm, decimal TotalExtraCommission, decimal DSGNCommissionPercent)
        {
            SalaryMonthly salaryMonthly = new SalaryMonthly();
            var AllHolidays = _holidayCalenderRepisitory.GetAllHolidaysBySalaryProcessMonth((DateTime)sysInfo.NextPayProcessDate);
            var AllAttendence = _AttendenceRepository.GetAllEntriesByAccountNo(_AttendenceMonthRepository, oEmployee.MachineEMPID, (DateTime)sysInfo.NextPayProcessDate);
            var SpecialHolidays = _holidayCalenderRepisitory.GetSpecialHolidaysBySalaryProcessMonth((DateTime)sysInfo.NextPayProcessDate);
            var Leaves = _employeeLeaveRepository.All;
            var DateRange = ConstantData.GetFirstAndLastDateOfMonth(sysInfo.NextPayProcessDate);
            var ManualAttendenc = _manualAttendence.All
                                .Where(i => i.Date >= DateRange.Item1
                                && i.Date <= DateRange.Item2 && i.EmployeeID == oEmployee.EmployeeID);
            var empMonthlyAttend = _MonthlyAttRepository.All
                                .FirstOrDefault(i => i.Month >= DateRange.Item1
                                && i.Month <= DateRange.Item2 && i.EmployeeID == oEmployee.EmployeeID);

            #region Get Allowance deduction

            var ADParamGrades = (from adbasic in _ADParamBasicRepository.All
                                 join adg in _ADParamGradeRepository.All on adbasic.ADParameterID equals adg.ADParameterID
                                 join AD in _AllowanceDeductionRepository.All on adbasic.AllowDeductID equals AD.AllowDeductID
                                 where adbasic.IsCurrentlyActive == (int)EnumActiveInactive.Active && adg.GradeID == EMPGradeSalary.GradeID && adbasic.Status == (int)EnumActiveInactive.Active
                                 select new
                                 {
                                     adbasic.AllowOrDeduct,
                                     adbasic.FlatAmount,
                                     adbasic.PercentOfBasic,
                                     adbasic.PercentOfGross,
                                     adbasic.AllowDeductID,
                                     AD.Name,
                                     AD.Code
                                 }).ToList();


            var ADParamsEmployee = (from adbasic in _ADParamBasicRepository.All
                                    join ademp in _ADParameterEmployeeRepository.All on adbasic.ADParameterID equals ademp.ADParameterID
                                    join AD in _AllowanceDeductionRepository.All on adbasic.AllowDeductID equals AD.AllowDeductID
                                    where adbasic.IsCurrentlyActive == (int)EnumActiveInactive.Active && ademp.EmployeeID == oEmployee.EmployeeID && adbasic.Status == (int)EnumActiveInactive.Active
                                    select new
                                    {
                                        adbasic.AllowOrDeduct,
                                        adbasic.FlatAmount,
                                        adbasic.PercentOfBasic,
                                        adbasic.PercentOfGross,
                                        adbasic.AllowDeductID,
                                        AD.Name,
                                        AD.Code
                                    }).ToList();

            ADParamGrades.AddRange(ADParamsEmployee);
            #endregion

            salaryMonthly.DepartmentID = (int)oEmployee.DepartmentID;
            salaryMonthly.DesignationID = oEmployee.DesignationID;
            salaryMonthly.LocationID = 0;
            salaryMonthly.GradeID = EMPGradeSalary.GradeID;
            salaryMonthly.ThisMonthBasic = EMPGradeSalary.BasicSalary;
            salaryMonthly.ThisMonthGross = (decimal)EMPGradeSalary.GrossSalary;
            salaryMonthly.SalaryMonth = sysInfo != null ? DateRange.Item2 : ConstantData.GetFirstAndLastDateOfMonth(DateTime.Now).Item2;
            salaryMonthly.EmployeeID = oEmployee.EmployeeID;
            salaryMonthly.ConcernID = sysInfo.ConcernID;
            SalaryMonthlyDetail salaryMonthlyDetail;

            #region Gross Salary
            salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Gross_Salary;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Gross_Salary;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.OtherItem;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Gross_Salary.ToString();
            salaryMonthlyDetail.CalculatedAmount = (decimal)EMPGradeSalary.GrossSalary;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 2;
            salaryMonthly.SalaryMonthlyDetails.Add(salaryMonthlyDetail);
            #endregion

            #region Attendence Salary and Working days calculations
            decimal WorkingDays = 0m;
            var Attendence = AttendenceCalculation(oEmployee, sysInfo, EMPGradeSalary,
                AllHolidays, AllAttendence, Leaves, out WorkingDays, ManualAttendenc, empMonthlyAttend);
            salaryMonthly.SalaryMonthlyDetails.Add(Attendence);
            salaryMonthly.WorkinDays = WorkingDays;
            if (WorkingDays == 0m)
            {
                var NetAmountSMD = CalculateNetAmount(salaryMonthly);
                salaryMonthly.SalaryMonthlyDetails.Add(NetAmountSMD);
                salaryMonthly.SalaryMonthlyDetails = salaryMonthly.SalaryMonthlyDetails.OrderBy(i => i.Position).ToList();
                return salaryMonthly;
            }
            #endregion

            #region OT
            decimal OTHours;
            var OTSMD = OTCalculation(oEmployee, sysInfo, EMPGradeSalary, AllHolidays,
                AllAttendence, out OTHours, empMonthlyAttend);
            salaryMonthly.SalaryMonthlyDetails.Add(OTSMD);
            salaryMonthly.OTHours = OTHours;
            #endregion

            #region Attendence Bonus
            //var AttendencBonus = AttendenceBonusCalculation(Employee, sysInfo, AllHolidays, SpecialHolidays, Leaves);
            //salaryMonthly.SalaryMonthlyDetails.Add(AttendencBonus);
            #endregion

            #region Festival Bonus
            if (IsFestBonusApply)
            {
                var Bonus = FestivalBonusCalculation(oEmployee, sysInfo, EMPGradeSalary);
                salaryMonthly.SalaryMonthlyDetails.Add(Bonus);
            }
            #endregion

            #region Allowanc Or Deduction
            foreach (var item in ADParamGrades)
            {
                salaryMonthlyDetail = new SalaryMonthlyDetail();
                salaryMonthlyDetail.ItemID = item.AllowDeductID;
                salaryMonthlyDetail.ItemCode = Convert.ToInt32(item.Code);
                salaryMonthlyDetail.ItemGroup = item.AllowOrDeduct == (int)EnumAllowOrDeduct.Allowance ? (int)EnumSalaryGroup.Allowance : (int)EnumSalaryGroup.Deductions;
                salaryMonthlyDetail.Description = item.Name;
                salaryMonthlyDetail.CalculatedAmount = item.PercentOfBasic != 0 ? ((item.PercentOfBasic / 100) * EMPGradeSalary.BasicSalary) : item.PercentOfGross != 0 ? ((item.PercentOfGross / 100) * (decimal)EMPGradeSalary.GrossSalary) : (decimal)item.FlatAmount;
                salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
                salaryMonthlyDetail.Position = 6;
                salaryMonthly.SalaryMonthlyDetails.Add(salaryMonthlyDetail);
            }
            #endregion

            #region Advance Calculation
            var Advances = AdvanceSalaryCalculation(oEmployee.EmployeeID, sysInfo.NextPayProcessDate);
            salaryMonthly.SalaryMonthlyDetails.Add(Advances);
            #endregion
        
            #region Due Calculation
            var DueSalary = DueSalaryCalculation(oEmployee.EmployeeID, sysInfo.NextPayProcessDate);
            salaryMonthly.SalaryMonthlyDetails.Add(DueSalary);
            #endregion

            #region Due Salary Pay Calculation
            var DueSalaryPay = DueSalaryPayCalculation(oEmployee.EmployeeID, sysInfo.NextPayProcessDate);
            salaryMonthly.SalaryMonthlyDetails.Add(DueSalaryPay);
            #endregion



            #region Commissions Calculation

            var EmployeeCommission = GetEmployeeCommission(oEmployee, DSGNCommissionPercent);
            salaryMonthly.AchievedComm = DSGNCommissionPercent;
            salaryMonthly.SalaryMonthlyDetails.Add(EmployeeCommission);


            #region old commission
            //decimal AchievedComm = 0m, TargetFailedDeductions = 0m;
            //if (IsShowroomCommission) //SSDC,Hawra and Haven
            //{
            //    var EmployeeCommission = CalculateShowroomCommissions(oEmployee, EmployeeTarget, sysInfo, TotalCollectionAmount, out AchievedComm, out TargetFailedDeductions, DSGNCommissionPercent);
            //    salaryMonthly.AchievedComm = AchievedComm;
            //    salaryMonthly.SalaryMonthlyDetails.Add(EmployeeCommission);

            //    var VoltageSComm = CalculateExtraCommission(oEmployee, VoltageStabilizerComm, EnumSalaryItemCode.Voltage_StabilizerComm, DSGNCommissionPercent);
            //    salaryMonthly.SalaryMonthlyDetails.Add(VoltageSComm);
            //    if ((sysInfo.ConcernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID || sysInfo.ConcernID == (int)EnumSisterConcern.HAVEN_ENTERPRISE_CONCERNID) && AchievedComm >= 70)
            //    {
            //        var ExtraComm = CalculateExtraCommission(oEmployee, TotalExtraCommission, EnumSalaryItemCode.Extra_Commission, DSGNCommissionPercent);
            //        ExtraComm.Position = 10;
            //        salaryMonthly.SalaryMonthlyDetails.Add(ExtraComm);
            //    }
            //    else if (sysInfo.ConcernID == (int)EnumSisterConcern.SAMSUNG_ELECTRA_CONCERNID)
            //    {
            //        var ExtraComm = CalculateExtraCommission(oEmployee, TotalExtraCommission, EnumSalaryItemCode.Extra_Commission, DSGNCommissionPercent);
            //        ExtraComm.Position = 10;
            //        salaryMonthly.SalaryMonthlyDetails.Add(ExtraComm);
            //    }


            //}
            //else
            //{
            //    if (_CashCollectionService.IsCommissionApplicable(DateRange.Item1, DateRange.Item2, oEmployee.EmployeeID))
            //    {
            //        var EmployeeCommission = CalculateEmployeeCommissions(oEmployee, EmployeeTarget, sysInfo, out AchievedComm);
            //        salaryMonthly.AchievedComm = AchievedComm;
            //        salaryMonthly.SalaryMonthlyDetails.Add(EmployeeCommission);
            //    }

            //}
            #endregion

            #endregion

            #region Target Failed Deduction
            //if (salaryMonthly.AchievedComm < 50m && IsShowroomCommission == false)
            //{
            //    salaryMonthlyDetail = new SalaryMonthlyDetail();
            //    salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Target_Failed_Deduct;
            //    salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Target_Failed_Deduct;
            //    salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Deductions;
            //    salaryMonthlyDetail.Description = EnumSalaryItemCode.Target_Failed_Deduct.ToString();
            //    salaryMonthlyDetail.CalculatedAmount = (decimal)EMPGradeSalary.GrossSalary * .2m;
            //    salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            //    salaryMonthlyDetail.Position = 11;
            //    salaryMonthly.SalaryMonthlyDetails.Add(salaryMonthlyDetail);
            //}
            //else
            //if (salaryMonthly.AchievedComm < 90m && IsShowroomCommission == true)
            //{
            //    salaryMonthlyDetail = new SalaryMonthlyDetail();
            //    salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Target_Failed_Deduct;
            //    salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Target_Failed_Deduct;
            //    salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Deductions;
            //    salaryMonthlyDetail.Description = EnumSalaryItemCode.Target_Failed_Deduct.ToString();
            //    salaryMonthlyDetail.CalculatedAmount = TargetFailedDeductions;
            //    salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            //    salaryMonthlyDetail.Position = 11;
            //    salaryMonthly.SalaryMonthlyDetails.Add(salaryMonthlyDetail);
            //}
            #endregion

            #region Over Expense than conveyance deduction
            if (ADParamsEmployee.Count() > 0)
            {
                var EmployeeAllowance = ADParamsEmployee.FirstOrDefault();
                decimal TotalExpense = GetMonthlyExpense(oEmployee, DateRange.Item1, DateRange.Item2);
                if (TotalExpense > EmployeeAllowance.FlatAmount)
                {
                    salaryMonthlyDetail = new SalaryMonthlyDetail();
                    salaryMonthlyDetail.ItemID = EmployeeAllowance.AllowDeductID;
                    salaryMonthlyDetail.ItemCode = Convert.ToInt32(EmployeeAllowance.Code);
                    salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Deductions;
                    salaryMonthlyDetail.Description = EmployeeAllowance.Name;
                    salaryMonthlyDetail.CalculatedAmount = TotalExpense - EmployeeAllowance.FlatAmount;
                    salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
                    salaryMonthlyDetail.Position = 11;
                    salaryMonthly.SalaryMonthlyDetails.Add(salaryMonthlyDetail);
                }
            }

            #endregion

            #region NetAmount
            var NetAmountSMonthlyDetails = CalculateNetAmount(salaryMonthly);
            salaryMonthly.SalaryMonthlyDetails.Add(NetAmountSMonthlyDetails);
            #endregion

            #region Basic Salary
            salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Basic_Salary;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Basic_Salary;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.OtherItem;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Basic_Salary.ToString();
            salaryMonthlyDetail.CalculatedAmount = (decimal)EMPGradeSalary.GrossSalary - salaryMonthly.SalaryMonthlyDetails.Where(i => i.ItemGroup == (int)EnumSalaryGroup.Allowance).Sum(i => i.CalculatedAmount);
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 1;
            salaryMonthly.SalaryMonthlyDetails.Add(salaryMonthlyDetail);
            #endregion

            salaryMonthly.SalaryMonthlyDetails = salaryMonthly.SalaryMonthlyDetails.OrderBy(i => i.Position).ToList();

            return salaryMonthly;

        }

        private static SalaryMonthlyDetail CalculateNetAmount(SalaryMonthly salaryMonthly)
        {
            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Net_Payable;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Net_Payable;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.OtherItem;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Net_Payable.ToString();
            salaryMonthlyDetail.CalculatedAmount = salaryMonthly.SalaryMonthlyDetails.Where(i => i.ItemGroup == (int)EnumSalaryGroup.Gross).Sum(i => i.CalculatedAmount) - salaryMonthly.SalaryMonthlyDetails.Where(i => i.ItemGroup == (int)EnumSalaryGroup.Deductions).Sum(i => i.CalculatedAmount);
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 12;
            return salaryMonthlyDetail;
        }

        private decimal GetMonthlyExpense(Employee Employee, DateTime fromDate, DateTime toDate)
        {
            int UserID = _UserService.GetUserIDByEmployeeID(Employee.EmployeeID);
            decimal ExpenseAmount = _ExpenditureService.GetExpenditureAmountByUserID(UserID, fromDate, toDate);
            return ExpenseAmount;
        }

        #region Festival Bonus calculation
        private SalaryMonthlyDetail FestivalBonusCalculation(Employee employee, SystemInformation sysInfo, EmpGradeSalaryAssignment LastGradesalary)
        {
            decimal BonusAmt = 0, Percentage = 0;
            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            if (employee != null && sysInfo != null && LastGradesalary != null)
            {
                Int64 Months = ((sysInfo.NextPayProcessDate.Year - employee.JoiningDate.Value.Year) * 12) + (sysInfo.NextPayProcessDate.Month - employee.JoiningDate.Value.Month);
                #region Formula
                //ex.: 6-11,25:12-I,50
                // 6 to 11 months get 25% bonus of Gross salary.
                // 12 to Infinity months get 50% bonus of Gross salary.
                #endregion
                var formulas = sysInfo.BonusFormula.Split(':');
                if (formulas.Length > 0)
                {
                    for (int i = 0; i < formulas.Length; i++)
                    {
                        var Formula = formulas[i].Split(',');
                        if (Formula.Length == 2)
                            Percentage = Convert.ToDecimal(Formula[1]);
                        var range = Formula[0].Split('-');
                        if (range[1] == "I")
                        {
                            if (Months >= Convert.ToInt64(range[0]))
                            {
                                BonusAmt = (decimal)((decimal)(Percentage / 100) * LastGradesalary.GrossSalary);
                            }
                        }
                        else if (Months >= Convert.ToInt64(range[0]) && Months <= Convert.ToInt64(range[1]))
                        {
                            BonusAmt = (decimal)((decimal)(Percentage / 100) * LastGradesalary.GrossSalary);
                        }

                    }
                }

            }

            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Bonus;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Bonus;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Bonus.ToString();
            salaryMonthlyDetail.CalculatedAmount = BonusAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 5;

            return salaryMonthlyDetail;
        }
        #endregion

        #region OT Calculations
        private decimal CalculateWorkingDaysOTHour(List<Attendence> workingDaysAttendences, int DesignationID)
        {
            var totalOTHour = from at in workingDaysAttendences
                              select new
                              {
                                  OThour = at.OTTime == "" ? 0m : Convert.ToDecimal(at.OTTime.Split(':')[0]),
                                  Latehour = at.Late == "" ? 0m : Convert.ToDecimal(at.Late.Split(':')[0]),
                                  //WorkerOT = (at.OTTime == "" ? 0m : Convert.ToDecimal(at.OTTime.Split(':')[0])) > (at.Late == "" ? 0m : Convert.ToDecimal(at.Late.Split(':')[0]))?(at.OTTime == "" ? 0m : Convert.ToDecimal(at.OTTime.Split(':')[0])) - (at.Late == "" ? 0m : Convert.ToDecimal(at.Late.Split(':')[0])):0m
                              };
            if (DesignationID == (int)EnumDesignation.Worker)
                return totalOTHour.Sum(i => ((i.OThour >= i.Latehour) ? (i.OThour - i.Latehour) : 0m));


            return totalOTHour.Sum(i => (i.OThour));
        }
        private decimal CalculateHolidaysOTHour(List<Attendence> HolidayAttendences)
        {
            var totalOTHour = from at in HolidayAttendences
                              select new
                              {
                                  Workhour = at.WorkTime == "" ? 0m : (Convert.ToDecimal(at.WorkTime.Split(':')[0])) > 0 ? (Convert.ToDecimal(at.WorkTime.Split(':')[0]) - 1) : 0m,
                                  //OThour = at.OTTime == "" ? 0m : Convert.ToDecimal(at.OTTime.Split(':')[0]),
                                  OThour = (at.OTTime == "" ? 0m : Convert.ToDecimal(at.OTTime.Split(':')[0])) > (at.Late == "" ? 0m : Convert.ToDecimal(at.Late.Split(':')[0])) ? (at.OTTime == "" ? 0m : Convert.ToDecimal(at.OTTime.Split(':')[0])) - (at.Late == "" ? 0m : Convert.ToDecimal(at.Late.Split(':')[0])) : 0m

                              };

            return totalOTHour.Sum(i => (i.Workhour + i.OThour));
        }
        private SalaryMonthlyDetail OTCalculation(Employee employee, SystemInformation sysInfo, EmpGradeSalaryAssignment LastGradesalary,
            IQueryable<HolidayCalender> AllHolidays, IQueryable<Attendence> AllEntries, out decimal OTHours,
            MonthlyAttendence empMonthlyAttend)
        {
            decimal OTAmount = 0, OThours = 0, OTDays = 0;

            //remove holidays and working day absent
            var WorkingDaysAttendences = AllEntries.Where(i => (!AllHolidays.Select(j => j.Date).Contains(i.Date)) && i.Absent == 0).ToList();

            var holidayattendences = AllEntries.Where(i => (AllHolidays.Select(j => j.Date).Contains(i.Date)) && i.Absent == 0).ToList();
            //OThours = CalculateWorkingDaysOTHour(WorkingDaysAttendences, employee.DesignationID);
            //OThours += CalculateHolidaysOTHour(holidayattendences);
            OTDays = holidayattendences.Count();
            if (empMonthlyAttend != null)
                OTDays += empMonthlyAttend.OTDays;

            if (employee != null)
            {
                #region desgination wise
                //if (employee.DesignationID == (int)EnumDesignation.Staff)
                //{
                //    decimal PerHourAmt = Math.Round((decimal)(LastGradesalary.GrossSalary / (30m * 8m)), 2);
                //    OTAmount = Math.Round(PerHourAmt * OThours, 2);
                //}
                //else if (employee.DesignationID == (int)EnumDesignation.Worker)
                //{
                //    decimal PerHourAmt = Math.Round((decimal)(LastGradesalary.GrossSalary / (26m * 8m)), 2);
                //    OTAmount = Math.Round(PerHourAmt * OThours, 2);
                //}

                decimal PerDaySalary = Math.Round((decimal)(LastGradesalary.GrossSalary / 30m), 2);
                OTAmount = Math.Round(PerDaySalary * OTDays, 2);
                #endregion
            }
            //return new Tuple<decimal, decimal>(OThours, OTAmount);
            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Over_Time_Amount;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Over_Time_Amount;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Over_Time_Amount.ToString();
            salaryMonthlyDetail.CalculatedAmount = OTAmount;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            OTHours = OTDays;
            salaryMonthlyDetail.Position = 4;

            return salaryMonthlyDetail;
        }
        #endregion

        #region Attendence Bonus Calculation
        private SalaryMonthlyDetail AttendenceBonusCalculation(Employee employee, SystemInformation sysInfo, IQueryable<HolidayCalender> AllHolidays,
            IQueryable<HolidayCalender> SpecialHolidays, IQueryable<Attendence> AllEntries,
            IQueryable<EmployeeLeave> leaves)
        {
            decimal ActualWorkingDays = 0m, BonusAmt = 0, empWorkedDays = 0;


            var WorkingDaysAttendences = AllEntries.Where(i => (!AllHolidays.Select(j => j.Date).Contains(i.Date)) && i.Absent == 0).ToList();
            var daterange = ConstantData.GetFirstAndLastDateOfMonth(sysInfo.NextPayProcessDate);
            var EmpPaidApprLeaves = leaves.Where(i => (i.LeaveDate >= daterange.Item1 && i.LeaveDate <= daterange.Item2) && i.PaidLeave == 1 && i.EmployeeID == employee.EmployeeID && i.Status == (int)EnumEmployeeLeaveStatus.Approved);
            var EMPShortLeaves = EmpPaidApprLeaves.Where(i => i.LeaveType == (int)EnumEmployeeLeaveType.ShortLeave);

            ActualWorkingDays = sysInfo.WorkingDays - SpecialHolidays.Count();
            if (employee.DesignationID == (int)EnumDesignation.Worker)
                empWorkedDays = CalculateWorkingDays(WorkingDaysAttendences, EMPShortLeaves.ToList()) - CalculateFineDays(WorkingDaysAttendences);
            else
                empWorkedDays = WorkingDaysAttendences.Count() - CalculateFineDays(WorkingDaysAttendences);

            if (empWorkedDays >= ActualWorkingDays)
            {
                if (employee.DepartmentID == (int)EnumDepartment.Management)
                    BonusAmt = 300;
                else if (employee.DepartmentID == (int)EnumDepartment.Sales)
                    BonusAmt = 200;
            }
            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Tot_Attend_Days_Bonus;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Tot_Attend_Days_Bonus;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Tot_Attend_Days_Bonus.ToString();
            salaryMonthlyDetail.CalculatedAmount = BonusAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 4;

            return salaryMonthlyDetail;
        }

        #endregion

        #region Attendence Calculation
        private decimal CalculateWorkingDays(List<Attendence> workingDaysAttendences, List<EmployeeLeave> shortLeaves)
        {
            decimal NetWorkingDays = 0;
            var workHours = (from at in workingDaysAttendences
                             join sl in shortLeaves on at.Date equals sl.LeaveDate into ps
                             from sl in ps.DefaultIfEmpty()
                             select new
                             {
                                 WorkHours = at.WorkTime == "" ? 0m : Convert.ToDecimal(at.WorkTime.Split(':')[0]),
                                 ShortLeaveHour = sl == null ? 0m : sl.ShortLeaveHour
                             }).ToList();

            var NetWorkingHours = from wh in workHours
                                  select new
                                  {
                                      NetWorkHour = ((wh.WorkHours - 1) + wh.ShortLeaveHour)
                                  };

            NetWorkingDays = NetWorkingHours.Sum(i => i.NetWorkHour) / 8;

            return NetWorkingDays;
        }

        // if an employee concurrently attends after 8:05am then one day absent will be considered.
        private int CalculateFineDays(List<Attendence> WorkingDaysAttendences)
        {
            int fineDays = 0, counter = 0;

            foreach (var item in WorkingDaysAttendences)
            {
                if (!string.IsNullOrEmpty(item.Late)) //if late
                {
                    counter++;
                }
                else
                    counter = 0;

                if (counter == 3)
                {
                    fineDays++;
                    counter = 0;
                }
            }
            return fineDays;
        }

        // if an employee doesn't check out then one day absent will be considered.
        private int CalculateNotCheckOutFineDays(List<Attendence> WorkingDaysAttendences)
        {
            int fineDays = 0;
            fineDays = WorkingDaysAttendences.Where(i => string.IsNullOrEmpty(i.ClockOut)).Count();
            return fineDays;
        }
        private SalaryMonthlyDetail AttendenceCalculation(Employee employee, SystemInformation sysInfo,
            EmpGradeSalaryAssignment LastGradesalary,
            IQueryable<HolidayCalender> AllHolidays, IQueryable<Attendence> AllEntries,
            IQueryable<EmployeeLeave> leaves,
            out decimal ActualWorkingDays, IQueryable<ManualAttendence> manualAttendences,
            MonthlyAttendence empMonthlyAtt)
        {
            decimal AttendenceSalary = 0, PaidDayLeavesNumber = 0;
            int fineDays = 0, leaveAttendCounter = 0;

            var WorkingDaysAttendences = AllEntries.Where(i => (!AllHolidays.Select(j => j.Date)
            .Contains(i.Date)) && i.Absent == 0).ToList().OrderBy(i => i.Date).ToList();

            var daterange = ConstantData.GetFirstAndLastDateOfMonth(sysInfo.NextPayProcessDate);
            var EmpPaidApprLeaves = leaves.Where(i => (i.LeaveDate >= daterange.Item1 && i.LeaveDate <= daterange.Item2) && i.PaidLeave == 1 && i.EmployeeID == employee.EmployeeID && i.Status == (int)EnumEmployeeLeaveStatus.Approved);
            var EMPShortLeaves = EmpPaidApprLeaves.Where(i => i.LeaveType == (int)EnumEmployeeLeaveType.ShortLeave);
            var PaidDayLeaves = EmpPaidApprLeaves.Where(i => i.LeaveType == (int)EnumEmployeeLeaveType.DayLeave);

            foreach (var item in PaidDayLeaves)
            {
                if (WorkingDaysAttendences.Any(i => i.Date == item.LeaveDate))
                {
                    leaveAttendCounter++;
                }
            }

            PaidDayLeavesNumber = PaidDayLeaves.Count() - leaveAttendCounter;

            //if (employee.DesignationID == (int)EnumDesignation.Worker)
            //    ActualWorkingDays = CalculateWorkingDays(WorkingDaysAttendences, EMPShortLeaves.ToList()) + PaidDayLeavesNumber;
            //else
            ActualWorkingDays = WorkingDaysAttendences.Count() + PaidDayLeavesNumber;

            //fine days 
            fineDays = CalculateNotCheckOutFineDays(WorkingDaysAttendences);

            ActualWorkingDays = ActualWorkingDays - fineDays;
            if(empMonthlyAtt.Days>0)
                ActualWorkingDays += AllHolidays.Where(i => i.Type == (int)EnumHolidayType.WeeklyHoliday).Count();

            //Manual Attendence Calculations
            if (manualAttendences != null)
                ActualWorkingDays += manualAttendences.Count();
            //Monthly attendence calculation
            if (empMonthlyAtt != null)
                ActualWorkingDays += empMonthlyAtt.Days;

            if (employee != null)
            {
                decimal PerDay = Math.Round((decimal)(LastGradesalary.GrossSalary / (30m)), 2);
                AttendenceSalary = Math.Round(PerDay * ActualWorkingDays, 2);
            }

            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Tot_Attend_Days_Amount;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Tot_Attend_Days_Amount;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Tot_Attend_Days_Amount.ToString();
            salaryMonthlyDetail.CalculatedAmount = AttendenceSalary;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 3;
            return salaryMonthlyDetail;
        }

        #endregion

        #region Advance Salary Calculation
        private SalaryMonthlyDetail AdvanceSalaryCalculation(int EmployeeID, DateTime SalaryProcessMonth)
        {
            var dateRange = ConstantData.GetFirstAndLastDateOfMonth(SalaryProcessMonth);
            decimal AdvanceAmt = 0;
            var advance = _advanceSalaryRepository.All.Where(i => i.EmployeeID == EmployeeID && i.Date >= dateRange.Item1 && i.Date <= dateRange.Item2 && i.SalaryType == EnumSalaryType.AdvanceSalary);
            if (advance.Count() != 0)
            {
                AdvanceAmt = advance.Sum(i => i.Amount);
            }

            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Advance_Deduction;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Advance_Deduction;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Deductions;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Advance_Deduction.ToString();
            salaryMonthlyDetail.CalculatedAmount = AdvanceAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 7;

            return salaryMonthlyDetail;
        }

        #endregion

        #region Due Salary Calculation
        private SalaryMonthlyDetail DueSalaryCalculation(int EmployeeID, DateTime SalaryProcessMonth)
        {
            var dateRange = ConstantData.GetFirstAndLastDateOfMonth(SalaryProcessMonth);
            decimal DueAmt = 0;
            var due = _advanceSalaryRepository.All.Where(i => i.EmployeeID == EmployeeID && i.Date >= dateRange.Item1 && i.Date <= dateRange.Item2 && i.SalaryType == EnumSalaryType.DueSalary);
            if (due.Count() != 0)
            {
                DueAmt = due.Sum(i => i.Amount);
            }

            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Due_Salary;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Due_Salary;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Deductions;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Due_Salary.ToString();
            salaryMonthlyDetail.CalculatedAmount = DueAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 7;

            return salaryMonthlyDetail;
        }

        #endregion

        #region Due Salary Pay Calculation
        private SalaryMonthlyDetail DueSalaryPayCalculation(int EmployeeID, DateTime SalaryProcessMonth)
        {
            var dateRange = ConstantData.GetFirstAndLastDateOfMonth(SalaryProcessMonth);
            decimal DueAmt = 0;
            var due = _advanceSalaryRepository.All.Where(i => i.EmployeeID == EmployeeID && i.Date >= dateRange.Item1 && i.Date <= dateRange.Item2 && i.SalaryType == EnumSalaryType.DueSalayPay);
            if (due.Count() != 0)
            {
                DueAmt = due.Sum(i => i.Amount);
            }

            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Due_SalaryPay;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Due_SalaryPay;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Due_SalaryPay.ToString();
            salaryMonthlyDetail.CalculatedAmount = DueAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 6;

            return salaryMonthlyDetail;
        }

        #endregion

        #region Calculate Employee Commissions
        private SalaryMonthlyDetail CalculateEmployeeCommissions(Employee employee, TargetSetup Target, SystemInformation SysInfo, out decimal OutAchievedPercent)
        {
            Decimal CommissionAmt = 0m, AchievedPercent = 0m;
            var DateRange = ConstantData.GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
            var Commissions = _CommissionSetupRepository.All.Where(i => i.EmployeeID == employee.EmployeeID && i.CommissionMonth >= DateRange.Item1 && i.CommissionMonth <= DateRange.Item2);
            if (Target != null)
            {
                var SORderDetails = GetAllSalesProduct(employee.EmployeeID, Target.TargetMonth);
                if (SORderDetails.Count() > 0)
                {
                    if (Target.Amount > 0m) //When Target is flat amount
                    {
                        decimal AchievedAmt = SORderDetails.Sum(i => i.UTAmount);
                        decimal TargetAmount = Target.Amount;
                        AchievedPercent = Math.Round(((AchievedAmt * 100) / TargetAmount), 2);
                        var commission = Commissions.FirstOrDefault(i => AchievedPercent >= i.AchievedPercentStart && AchievedPercent <= i.AchievedPercentEnd);
                        if (commission != null)
                        {
                            CommissionAmt = AchievedAmt * commission.CommissionPercent;
                        }

                    }
                    else //when target is product Qty
                    {
                        decimal AchievedQty = SORderDetails.Where(i => Target.TargetSetupDetails.Select(j => j.ProductID).Contains(i.ProductID)).Sum(i => i.Quantity);
                        decimal TargetQty = Target.TargetSetupDetails.Sum(i => i.Quantity);

                        AchievedPercent = TargetQty != 0 ? Math.Round(((AchievedQty * 100) / TargetQty), 2) : 0m;
                        //var commission = Commissions.FirstOrDefault(i => AchievedPercent >= i.AchievedPercentStart && AchievedPercent <= i.AchievedPercentEnd);

                        var commission = Commissions.FirstOrDefault(i => AchievedQty >= i.AchievedPercentStart && AchievedQty <= i.AchievedPercentEnd);
                        if (commission != null)
                        {
                            CommissionAmt = AchievedQty * commission.CommisssionAmt;
                        }
                    }

                }
            }


            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Commission;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Commission;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Commission.ToString();
            salaryMonthlyDetail.CalculatedAmount = CommissionAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 8;
            OutAchievedPercent = AchievedPercent;
            return salaryMonthlyDetail;
        }

        private List<SOrderDetail> GetAllSalesProduct(int EmployeeID, DateTime TargetMonth)
        {
            var DateRange = ConstantData.GetFirstAndLastDateOfMonth(TargetMonth);
            var sales = from so in _SOrderRepository.All
                        join sod in _SOrderDetailRepository.All on so.SOrderID equals sod.SOrderID
                        join c in _CustomerRepository.All on so.CustomerID equals c.CustomerID
                        where (so.Status == 1 && c.EmployeeID == EmployeeID && (so.InvoiceDate >= DateRange.Item1 && so.InvoiceDate <= DateRange.Item2))
                        select sod;

            return sales.ToList();
        }

        /// <summary>
        /// For Showrooms(SSDC,Hawra,Haven) commissions calculation will be on 
        /// Totalcollections(Receive amount,Cashcollection,downpayment,Installment collections)
        /// </summary>
        private SalaryMonthlyDetail CalculateShowroomCommissions(Employee employee, TargetSetup Target, SystemInformation SysInfo,
            decimal TotalCollection, out decimal OutAchievedPercent, out decimal outTargetFailedDeduction, decimal DSGNCommissionPercent)
        {
            Decimal TotalCommissionAmt = 0m, EmployeeCommissionAmt = 0m, AchievedPercent = 0m, TargetFailedDeduction = 0m;
            var DateRange = ConstantData.GetFirstAndLastDateOfMonth(SysInfo.NextPayProcessDate);
            var Commissions = _CommissionSetupRepository.All.Where(i => i.CommissionMonth >= DateRange.Item1 && i.CommissionMonth <= DateRange.Item2);

            if (Target != null)
            {
                decimal AchievedAmt = TotalCollection;
                decimal TargetAmount = Target.Amount;
                AchievedPercent = TargetAmount > 0 ? Math.Round(((AchievedAmt * 100) / TargetAmount), 2) : 0m;
                var commission = Commissions.FirstOrDefault(i => AchievedPercent >= i.AchievedPercentStart && AchievedPercent <= i.AchievedPercentEnd);
                if (commission != null)
                {
                    TotalCommissionAmt = AchievedAmt * commission.CommissionPercent;
                    EmployeeCommissionAmt = Math.Round((decimal)(TotalCommissionAmt * DSGNCommissionPercent), 2);
                    if (AchievedPercent < 90)
                    {
                        TotalCommissionAmt = TotalCommissionAmt - ((20m * TotalCommissionAmt) / 100m);
                        decimal empcomDeduct = Math.Round((decimal)(TotalCommissionAmt * DSGNCommissionPercent), 2);
                        TargetFailedDeduction = EmployeeCommissionAmt - empcomDeduct;
                    }
                }
            }
            outTargetFailedDeduction = TargetFailedDeduction;
            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Commission;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Commission;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Commission.ToString();
            salaryMonthlyDetail.CalculatedAmount = EmployeeCommissionAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 8;
            OutAchievedPercent = AchievedPercent;

            return salaryMonthlyDetail;
        }


        private SalaryMonthlyDetail CalculateExtraCommission(Employee employee, decimal TotalExtraCommission, EnumSalaryItemCode SalaryItemCode, decimal DSGNCommissionPercent)
        {

            decimal EmployeeCommissionAmt = 0m;

            if (TotalExtraCommission > 0m)
            {
                EmployeeCommissionAmt = Math.Round((decimal)(TotalExtraCommission * DSGNCommissionPercent), 2);
            }

            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)SalaryItemCode;
            salaryMonthlyDetail.ItemCode = (int)SalaryItemCode;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = SalaryItemCode.ToString();
            salaryMonthlyDetail.CalculatedAmount = EmployeeCommissionAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 9;

            return salaryMonthlyDetail;
        }

        private SalaryMonthlyDetail GetEmployeeCommission(Employee employee, decimal EmployeeCommissionAmt)
        {
            var salaryMonthlyDetail = new SalaryMonthlyDetail();
            salaryMonthlyDetail.ItemID = (int)EnumSalaryItemCode.Commission;
            salaryMonthlyDetail.ItemCode = (int)EnumSalaryItemCode.Commission;
            salaryMonthlyDetail.ItemGroup = (int)EnumSalaryGroup.Gross;
            salaryMonthlyDetail.Description = EnumSalaryItemCode.Commission.ToString();
            salaryMonthlyDetail.CalculatedAmount = EmployeeCommissionAmt;
            salaryMonthlyDetail.ChangedAmount = salaryMonthlyDetail.CalculatedAmount;
            salaryMonthlyDetail.Position = 9;

            return salaryMonthlyDetail;
        }
        #endregion



        #region CRUD Operation
        public IQueryable<SalaryProcess> GetAllIQueryable()
        {
            return _BaseSalaryProcessRepository.All;
        }

        public void Add(SalaryProcess SalaryProcess)
        {
            _BaseSalaryProcessRepository.Add(SalaryProcess);
        }

        public void Update(SalaryProcess SalaryProcess)
        {
            _BaseSalaryProcessRepository.Add(SalaryProcess);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }
        public IEnumerable<Tuple<int, DateTime, int>> GetUndoAbleSalaryProcess()
        {
            return _BaseSalaryProcessRepository.GetUndoAbleSalaryProcess();
        }
        public SalaryProcess GetById(int id)
        {
            return _BaseSalaryProcessRepository.All.FirstOrDefault(i => i.SalaryProcessID == id);
        }
        public void Delete(int id)
        {
            _BaseSalaryProcessRepository.Delete(x => x.SalaryProcessID == id);
        }

        public bool UndoSalaryProcessUsingSP(int SalaryProcessID)
        {
            return _SalaryProcessRepository.UndoSalaryProcessUsingSP(SalaryProcessID);
        }

        public bool FinalizeSalaryMonthUsingSP(DateTime fromDate, DateTime toDate, int ConcernID, int FinalizedBy, DateTime MonthEndDate, DateTime NextMonth, DataTable dtWeeklyHolidays)
        {
            return _SalaryProcessRepository.FinalizeSalaryMonthUsingSP(fromDate, toDate, ConcernID, FinalizedBy, MonthEndDate, NextMonth, dtWeeklyHolidays);
        }

        public bool IsSalaryProcessed(int EmployeeID, Tuple<DateTime, DateTime> salayMonth)
        {
            var d = from s in _BaseSalaryProcessRepository.All
                    join ds in _SalaryMonthlyRepository.All on s.SalaryProcessID equals ds.SalaryProcessID
                    where ds.EmployeeID == EmployeeID
                    && (s.SalaryMonth >= salayMonth.Item1 && s.SalaryMonth <= salayMonth.Item2)
                    select ds;

            if (d.Any())
                return true;

            return false;

        }

        #endregion
    }
}
