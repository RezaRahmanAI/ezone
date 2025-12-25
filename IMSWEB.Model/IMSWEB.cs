//namespace IMSWEB.Model
//{
//    using System;
//    using System.Data.Entity;
//    using System.ComponentModel.DataAnnotations.Schema;
//    using System.Linq;

//    public partial class IMSWEB : DbContext
//    {
//        public IMSWEB()
//            : base("name=IMSWEB")
//        {
//        }

//        public virtual DbSet<CashCollection> CashCollections { get; set; }
//        public virtual DbSet<Category> Categorys { get; set; }
//        public virtual DbSet<Company> Companies { get; set; }
//        public virtual DbSet<CreditSaleProduct> CreditSaleProducts { get; set; }
//        public virtual DbSet<CreditSale> CreditSales { get; set; }
//        public virtual DbSet<CreditSalesDetail> CreditSalesDetails { get; set; }
//        public virtual DbSet<Customer> Customers { get; set; }
//        public virtual DbSet<DamageProduct> DamageProducts { get; set; }
//        public virtual DbSet<Designation> Designations { get; set; }
//        public virtual DbSet<Employee> Employees { get; set; }
//        public virtual DbSet<Expenditure> Expenditures { get; set; }
//        public virtual DbSet<ExpenseItem> ExpenseItems { get; set; }
//        public virtual DbSet<Model> Models { get; set; }
//        public virtual DbSet<OldUser> OldUsers { get; set; }
//        public virtual DbSet<POProductDetail> POProductDetails { get; set; }
//        public virtual DbSet<POrderDetail> POrderDetails { get; set; }
//        public virtual DbSet<POrder> POrders { get; set; }
//        public virtual DbSet<Product> Products { get; set; }
//        public virtual DbSet<Role> Roles { get; set; }
//        public virtual DbSet<SaleOffer> SaleOffers { get; set; }
//        public virtual DbSet<SisterConcern> SisterConcerns { get; set; }
//        public virtual DbSet<SOrderDetail> SOrderDetails { get; set; }
//        public virtual DbSet<SOrder> SOrders { get; set; }
//        public virtual DbSet<StockDetail> StockDetails { get; set; }
//        public virtual DbSet<Stock> Stocks { get; set; }
//        public virtual DbSet<Supplier> Suppliers { get; set; }
//        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
//        public virtual DbSet<SystemInformation> SystemInformations { get; set; }

//        protected override void OnModelCreating(DbModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<CashCollection>()
//                .Property(e => e.BankName)
//                .IsUnicode(false);

//            modelBuilder.Entity<CashCollection>()
//                .Property(e => e.BranchName)
//                .IsUnicode(false);

//            modelBuilder.Entity<CashCollection>()
//                .Property(e => e.Amount)
//                .HasPrecision(18, 0);

//            modelBuilder.Entity<CashCollection>()
//                .Property(e => e.AccountNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<CashCollection>()
//                .Property(e => e.MBAccountNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<CashCollection>()
//                .Property(e => e.BKashNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Category>()
//                .HasMany(e => e.Products)
//                .WithRequired(e => e.Category)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Company>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Company>()
//                .Property(e => e.Name)
//                .IsUnicode(false);

//            modelBuilder.Entity<Company>()
//                .HasMany(e => e.Products)
//                .WithRequired(e => e.Company)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.InvoiceNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.TSalesAmt)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.IntallmentPrinciple)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.UserName)
//                .IsUnicode(false);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.Remaining)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.DownPayment)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.WInterestAmt)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.FixedAmt)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.Discount)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .Property(e => e.NetAmount)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSale>()
//                .HasMany(e => e.CreditSaleProducts)
//                .WithRequired(e => e.CreditSale)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<CreditSale>()
//                .HasMany(e => e.CreditSalesDetails)
//                .WithRequired(e => e.CreditSale)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<CreditSalesDetail>()
//                .Property(e => e.Balance)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSalesDetail>()
//                .Property(e => e.InstallmentAmt)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSalesDetail>()
//                .Property(e => e.PaymentStatus)
//                .IsUnicode(false);

//            modelBuilder.Entity<CreditSalesDetail>()
//                .Property(e => e.InterestAmount)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<CreditSalesDetail>()
//                .Property(e => e.ClosingBalance)
//                .HasPrecision(19, 4);

//            modelBuilder.Entity<Customer>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Customer>()
//                .Property(e => e.ContactNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<Customer>()
//                .Property(e => e.EmailID)
//                .IsUnicode(false);

//            modelBuilder.Entity<Customer>()
//                .Property(e => e.NID)
//                .IsUnicode(false);

//            modelBuilder.Entity<Customer>()
//                .Property(e => e.PhotoPath)
//                .IsUnicode(false);

//            modelBuilder.Entity<Customer>()
//                .Property(e => e.RefContact)
//                .IsUnicode(false);

//            modelBuilder.Entity<Customer>()
//                .HasMany(e => e.CreditSales)
//                .WithRequired(e => e.Customer)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Customer>()
//                .HasMany(e => e.SOrders)
//                .WithRequired(e => e.Customer)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<DamageProduct>()
//                .Property(e => e.Qty)
//                .HasPrecision(18, 0);

//            modelBuilder.Entity<Designation>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Designation>()
//                .HasMany(e => e.Employees)
//                .WithRequired(e => e.Designation)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Employee>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Employee>()
//                .Property(e => e.ContactNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<Employee>()
//                .Property(e => e.EmailID)
//                .IsUnicode(false);

//            modelBuilder.Entity<Employee>()
//                .Property(e => e.NID)
//                .IsUnicode(false);

//            modelBuilder.Entity<Employee>()
//                .Property(e => e.BloodGroup)
//                .IsUnicode(false);

//            modelBuilder.Entity<Employee>()
//                .Property(e => e.PhotoPath)
//                .IsUnicode(false);

//            modelBuilder.Entity<Employee>()
//                .HasMany(e => e.Customers)
//                .WithRequired(e => e.Employee)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<ExpenseItem>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<ExpenseItem>()
//                .HasMany(e => e.Expenditures)
//                .WithRequired(e => e.ExpenseItem)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Model>()
//                .Property(e => e.Name)
//                .IsUnicode(false);

//            modelBuilder.Entity<Model>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Model>()
//                .HasMany(e => e.Products)
//                .WithRequired(e => e.Model)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<OldUser>()
//                .Property(e => e.UserName)
//                .IsUnicode(false);

//            modelBuilder.Entity<OldUser>()
//                .Property(e => e.UserPassword)
//                .IsUnicode(false);

//            modelBuilder.Entity<OldUser>()
//                .Property(e => e.ContactNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<OldUser>()
//                .Property(e => e.EmailAddress)
//                .IsUnicode(false);

//            modelBuilder.Entity<POrderDetail>()
//                .HasMany(e => e.POProductDetails)
//                .WithRequired(e => e.POrderDetail)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<POrder>()
//                .Property(e => e.ChallanNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<POrder>()
//                .HasMany(e => e.POrderDetails)
//                .WithRequired(e => e.POrder)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Product>()
//                .Property(e => e.PicturePath)
//                .IsUnicode(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.CreditSaleProducts)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.DamageProducts)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.POProductDetails)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.POrderDetails)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.SaleOffers)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.SOrderDetails)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.StockDetails)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Product>()
//                .HasMany(e => e.Stocks)
//                .WithRequired(e => e.Product)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SaleOffer>()
//                .Property(e => e.Description)
//                .IsUnicode(false);

//            modelBuilder.Entity<SisterConcern>()
//                .Property(e => e.Address)
//                .IsUnicode(false);

//            modelBuilder.Entity<SisterConcern>()
//                .Property(e => e.ContactNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.CashCollections)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Categorys)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Companies)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.CreditSales)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Customers)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.DamageProducts)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Employees)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Expenditures)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Models)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.OldUsers)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.POrders)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.SOrders)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Stocks)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.Suppliers)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SisterConcern>()
//                .HasMany(e => e.SystemInformations)
//                .WithRequired(e => e.SisterConcern)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SOrder>()
//                .Property(e => e.InvoiceNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<SOrder>()
//                .Property(e => e.Remarks)
//                .IsUnicode(false);

//            modelBuilder.Entity<SOrder>()
//                .HasMany(e => e.SOrderDetails)
//                .WithRequired(e => e.SOrder)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<StockDetail>()
//                .Property(e => e.StockCode)
//                .IsUnicode(false);

//            modelBuilder.Entity<StockDetail>()
//                .HasMany(e => e.CreditSaleProducts)
//                .WithRequired(e => e.StockDetail)
//                .HasForeignKey(e => e.StockDetailID)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<StockDetail>()
//                .HasMany(e => e.SOrderDetails)
//                .WithRequired(e => e.StockDetail)
//                .HasForeignKey(e => e.StockDetailID)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Stock>()
//                .Property(e => e.StockCode)
//                .IsUnicode(false);

//            modelBuilder.Entity<Stock>()
//                .HasMany(e => e.StockDetails)
//                .WithRequired(e => e.Stock)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<Supplier>()
//                .Property(e => e.Code)
//                .IsUnicode(false);

//            modelBuilder.Entity<Supplier>()
//                .Property(e => e.ContactNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<Supplier>()
//                .Property(e => e.PhotoPath)
//                .IsUnicode(false);

//            modelBuilder.Entity<Supplier>()
//                .HasMany(e => e.CashCollections)
//                .WithOptional(e => e.Supplier)
//                .HasForeignKey(e => e.CompanyID);

//            modelBuilder.Entity<Supplier>()
//                .HasMany(e => e.POrders)
//                .WithRequired(e => e.Supplier)
//                .WillCascadeOnDelete(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.Address)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.TelephoneNo)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.EmailAddress)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.WebAddress)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.ProductPhotoPath)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.SupplierPhotoPath)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.CustomerPhotoPath)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.CustomerNIDPatht)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.SupplierDocPath)
//                .IsUnicode(false);

//            modelBuilder.Entity<SystemInformation>()
//                .Property(e => e.EmployeePhotoPath)
//                .IsUnicode(false);
//        }
//    }
//}
