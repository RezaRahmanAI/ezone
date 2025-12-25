using IMSWEB.Model;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IMSWEB.Service
{
    public interface ISalesOrderDetailService
    {
        //void AddSalesOrderDetail(SOrderDetail sorderDetail);
        //IEnumerable<Tuple<int, int, int, int, string, string, string,
        //    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, int>>>> GetSalesOrderDetailByOrderId(int id);

        void AddSalesOrderDetail(SOrderDetail sorderDetail);
        IEnumerable<Tuple<int, int, int, int, string, string, string,
            Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, int, int, decimal, string>>>> GetSalesOrderDetailByOrderId(int id);

        IEnumerable<Tuple<int, int, int, int, string, string, string,
    Tuple<decimal, decimal, decimal, decimal, decimal, decimal, int, Tuple<string, decimal, string, string, string, string>>>> GetSalesOrderDetailByOrderIdForInvoice(int id);

        void SaveSalesOrderDetail();
        void DeleteSalesOrderDetail(int id);
        IEnumerable<SOrderDetail> GetSOrderDetailsBySOrderID(int SOrderID);
    }
}
