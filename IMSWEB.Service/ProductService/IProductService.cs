using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface IProductService
    {
        void AddProduct(Product product);
        void UpdateProduct(Product product);
        void SaveProduct();
        Task<IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal, decimal>>>> GetAllProductAsync();
        IQueryable<ProductWisePurchaseModel> GetAllProductIQueryable();
        IQueryable<ProductWisePurchaseModel> GetAllProductIQueryableForIndex();
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, 
            Tuple<decimal?, int, decimal, decimal>>> GetAllProduct();

        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,
            Tuple<string, string, string, string, string, string, decimal, Tuple<EnumProductType>>>>> GetAllProductFromDetail();

        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int,
            string, string, Tuple<string, string, string, string, string>>>> GetAllDamageProductFromDetail();
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> SRWiseGetAllProductFromDetail(int EmployeeID);


        Product GetProductById(int id);
        void DeleteProduct(int id);

        //IEnumerable<Tuple<int, string, string,decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string>>> GetAllSalesProductFromDetailByCustomerID();
        //GetAllProductFromDetail
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<string>>>> 
            GetAllSalesProductFromDetailByCustomerID(int CustomerID);
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string, Tuple<decimal, string, string, string, string, string, decimal,Tuple<string> >>>> GetAllProductFromDetailForCredit();
        IEnumerable<Tuple<int,string, string, string, string>> GetProductDetails();

        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, string, decimal, int, int, string, string,Tuple<string>>>>
           GetAllSalesProductByCustomerID(int CustomerID);

        IEnumerable<Tuple<int, string, string, decimal,
   string, string, string, Tuple<decimal?, string, decimal, decimal, int, string, int>>> GetAllProductDetails();

        Product GetProductByConcernAndName(int ConcernID, string ProductName);
        IEnumerable<ProductDetailsModel> GetSalesDetailByCustomerID(int CustomerID, string IMEI);

        IQueryable<Product> GetAll(int ConcernID);
        IQueryable<Product> GetAllProducts();
        IEnumerable<ProductDetailsModel> GetCreditSalesDetailByCustomerID(int CustomerID, string IMEI);

        int GetProductTypeById(int productId);
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, int, decimal>>> GetAllUniqueProduct();
        IEnumerable<Tuple<int, string, string, decimal, string, string, string, Tuple<decimal?, int, decimal, decimal>>> GetAllECOMProduct();
        IQueryable<ProductWisePurchaseModel> GetDOProducts();
        IQueryable<ProductWisePurchaseModel> GetAllProductIQueryablePO();
        List<ProductWisePurchaseModel> GetAllProductIQueryableNew();
    }
}
