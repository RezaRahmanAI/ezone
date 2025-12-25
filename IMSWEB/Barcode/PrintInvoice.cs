using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Printing;
using System.Data;
using System.Windows.Forms;
using GenCode128;
using IMSWEB.Model;
using IMSWEB.Service;

namespace IMSWEB
{
    public class PrintInvoice
    {
        #region Print Invoice

        SOrder _SOrder = null;
        PrintDocument pdoc = null;
        DataTable _SalesDT = null;

        string _CashierName = "";
        string _InvoiceNo;
        DateTime _InvoiceDate;
        decimal _TotalDis = 0;
        decimal _TotalAmt = 0;
        decimal _PrvDue = 0;
        decimal _GrandTotal = 0;
        decimal _Paid = 0;
        decimal _DueAmt = 0;
        decimal _ChangeAmt = 0;
        decimal _VATAmt = 0;
        int nNumberOfCopies = 1;

        #endregion

        #region Barcode Purpose
        string sBarcode = string.Empty;
        string sUnitPrice = string.Empty;
        string sProName = string.Empty;
        string sBrand = string.Empty;
        string sColor = string.Empty;
        string sProSize = string.Empty;

        int nBarCodeWeight = 1;
        PrintDocument pdocBarCode = null;
        SalesOrderViewModel _SOrderViewModel = null;
        List<SisterConcern> _SisterConcernList = null;
        SisterConcern _SisterCocern = null;
        ISisterConcernService _SC = null;

        #endregion

        public PrintInvoice()
        {

        }
        public void print1(SOrder oSOrder)
        {
            _CashierName = "Mr. Arian";//Global.CurrentUser.UserName;
            _SOrder = oSOrder;
            _InvoiceNo = oSOrder.InvoiceNo;
            _InvoiceDate = (DateTime)oSOrder.InvoiceDate;
            _TotalDis = (decimal)oSOrder.TDAmount;
            _TotalAmt = (decimal)oSOrder.TotalAmount;
            _PrvDue = (decimal)(oSOrder.Customer.TotalDue - oSOrder.PaymentDue);
            _GrandTotal = (decimal)(oSOrder.GrandTotal + (oSOrder.Customer.TotalDue - oSOrder.PaymentDue));
            _Paid = (decimal)oSOrder.RecAmount;
            _DueAmt = oSOrder.Customer.TotalDue;
            _ChangeAmt = 0;// (decimal)oSOrder.BackAmount;
            _VATAmt = (decimal)oSOrder.VATAmount;

            PrintDialog pd = new PrintDialog();
            pdoc = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            Font font = new Font("Courier New", 8);


            PaperSize psize = new PaperSize("Custom", 100, 200);
            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            pdoc.DefaultPageSettings.PaperSize.Height = 620;
            pdoc.DefaultPageSettings.PaperSize.Width = 350;
            //pd.PrinterSettings.Copies = 3;

            //pd.AllowSelection = true;
            //pd.AllowSomePages = true;

            #region For Auto Print

            //for (int i = 1; i <= nNumberOfCopies; i++)
            //{
            //    pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
            //    pdoc.Print();
            //}

            #endregion

            #region Using by Command
            pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
            DialogResult result = pd.ShowDialog();
            if (result == DialogResult.OK)
            {
                PrintPreviewDialog pp = new PrintPreviewDialog();
                pp.Document = pdoc;
                result = pp.ShowDialog();
                if (result == DialogResult.OK)
                {
                    pdoc.Print();
                }
            }

            #endregion

        }

        public void print(SalesOrderViewModel oSOrder,ISisterConcernService SC)
        {
            _SC = SC;
            
            _SisterCocern = _SC.GetSisterConcernById(System.Web.HttpContext.Current.User.Identity.GetConcernId());
            _CashierName = System.Web.HttpContext.Current.User.Identity.Name;//Global.CurrentUser.UserName;
            _SOrderViewModel = oSOrder;
            _InvoiceNo = oSOrder.SalesOrder.InvoiceNo;
            _InvoiceDate = Convert.ToDateTime(oSOrder.SalesOrder.OrderDate);
            _TotalDis = Convert.ToDecimal(oSOrder.SalesOrder.NetDiscount);
            _TotalAmt = Convert.ToDecimal(oSOrder.SalesOrder.TotalAmount);
            _PrvDue = 0;//(decimal)(oSOrder.SalesOrder.TotalDue - oSOrder.PaymentDue);
            _GrandTotal = Convert.ToDecimal(oSOrder.SalesOrder.GrandTotal);
            _Paid = Convert.ToDecimal(oSOrder.SalesOrder.RecieveAmount);
            _DueAmt = Convert.ToDecimal(oSOrder.SalesOrder.PaymentDue);
            _ChangeAmt = 0;// (decimal)oSOrder.BackAmount;
            _VATAmt = Convert.ToDecimal(oSOrder.SalesOrder.VATAmount);

            PrintDialog pd = new PrintDialog();
            pdoc = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            Font font = new Font("Courier New", 8);


            PaperSize psize = new PaperSize("Custom", 100, 200);
            pd.Document = pdoc;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            pdoc.DefaultPageSettings.PaperSize.Height = 620;
            pdoc.DefaultPageSettings.PaperSize.Width = 350;
            //pd.PrinterSettings.Copies = 3;

            //pd.AllowSelection = true;
            //pd.AllowSomePages = true;

            #region For Auto Print

            for (int i = 1; i <= nNumberOfCopies; i++)
            {
                pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
                pdoc.Print();
            }

            #endregion

            #region Using by Command
            //pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
            //DialogResult result = pd.ShowDialog();
            //if (result == DialogResult.OK)
            //{
            //    PrintPreviewDialog pp = new PrintPreviewDialog();
            //    pp.Document = pdoc;
            //    result = pp.ShowDialog();
            //    if (result == DialogResult.OK)
            //    {
            //        pdoc.Print();
            //    }
            //}

            #endregion

        }
        void pdoc_PrintPage(object sender, PrintPageEventArgs e)
        {


            Graphics graphics = e.Graphics;
            Font font = new Font("Times New Roman", 10);
            float fontHeight = font.GetHeight();
            int startX = 2;
            int startY = 2;
            int Offset = 30;
            graphics.DrawString("          " + _SisterCocern.Name, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
            Offset = Offset + 27;
            graphics.DrawString("                         " + _SisterCocern.Address, new Font("Times New Roman", 8, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX - 20, startY + Offset);
            Offset = Offset + 20;

            //graphics.DrawString("                    " + _SisterCocern.Address, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX + 3, startY + Offset);
            //Offset = Offset + 20;

            graphics.DrawString("                     Contact :" + _SisterCocern.ContactNo, new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
            Offset = Offset + 20;

            String underLine1 = "-------------------------------------------------------------------";
            graphics.DrawString(underLine1, new Font("Times New Roman", 11), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
            Offset = Offset + 20;

            graphics.DrawString(" Cashier Name:" + _CashierName, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
            Offset = Offset + 20;

            graphics.DrawString(" Invoice No:" + _InvoiceNo, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
            Offset = Offset + 20;
            graphics.DrawString(" Date & Time:" + _InvoiceDate, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
            Offset = Offset + 15;
            String underLine = "-------------------------------------------------------------------";
            graphics.DrawString(underLine, new Font("Times New Roman", 11), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            Offset = Offset + 15;                     //-------
            graphics.DrawString(" Item " + "              " + "     " + "           " + " Qty" + "          " + "Rate" + "          " + "Total", new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            Offset = Offset + 15;
            underLine = "-------------------------------------------------------------------";
            graphics.DrawString(underLine, new Font("Times New Roman", 11), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            int nTempProId = 0;
            decimal oTempSoldQty = 0;

            foreach (var oItem in _SOrderViewModel.SODetails)
            {
                if (nTempProId != Convert.ToInt16(oItem.ProductId))
                {
                    oTempSoldQty = _SOrderViewModel.SODetails.Where(pd => Convert.ToInt16(pd.ProductId) == Convert.ToInt16(oItem.ProductId)).ToList().Count;
                    Offset = Offset + 15;
                    graphics.DrawString(" " + oItem.ProductName + "[" + oItem.ProductCode + "]", new Font("Times New Roman", 9, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
                    Offset = Offset + 15;             //---------------------
                    graphics.DrawString("                                          " + Math.Round(oTempSoldQty, 0) + "           " + Math.Round(Convert.ToDecimal(oItem.UnitPrice), 2) + "          " + Math.Round(Convert.ToDecimal(oItem.UTAmount) * oTempSoldQty, 2), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
                    nTempProId = Convert.ToInt16(oItem.ProductId);
                }
            }

            Offset = Offset + 15;
            underLine = "-------------------------------------------------------------------";
            graphics.DrawString(underLine, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            //Offset = Offset + 15;
            //graphics.DrawString("Total Amount.                       " + Math.Round(_TotalAmt, 2), new Font("Courier New", 8, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);


            Offset = Offset + 15;
            graphics.DrawString("Total Dis. (-)                                                         " + Math.Round(_TotalDis, 2), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            //Offset = Offset + 15;
            //graphics.DrawString("Prv. Due" + "                            " + Math.Round(_PrvDue,2), new Font("Courier New", 8, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 15;
            graphics.DrawString("Total VAT (+)" + "                                                    " + Math.Round(_VATAmt, 2), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            Offset = Offset + 15;
            underLine = "-------------------------------------------------------------------";
            graphics.DrawString(underLine, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);
 
            Offset = Offset + 15;
            graphics.DrawString("Net Amount (TK)" + "                                              " + Math.Round(_GrandTotal, 2), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);


            Offset = Offset + 15;
            graphics.DrawString("Paid Amount :                                                    " + Math.Round(_Paid, 2), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            //Offset = Offset + 15;
            //underLine = "--------------------------------------------";
            //graphics.DrawString(underLine, new Font("Courier New", 8, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);


            //Offset = Offset + 15;
            //                     //---------------------------------------
            //graphics.DrawString("Total Due :                        " + Math.Round(_DueAmt,2), new Font("Courier New", 8, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);

            Offset = Offset + 15;
            //---------------------------------------
            graphics.DrawString("Changed Amount :                                              " + Math.Round(_ChangeAmt, 2), new Font("Times New Roman", 10, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);


            Offset = Offset + 15;
            underLine = "-------------------------------------------------------------------";
            graphics.DrawString(underLine, new Font("Times New Roman", 11, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);


            Offset = Offset + 175;//85
            graphics.DrawString("   Developed By: Object Canvas Technology", new Font("Times New Roman", 12, FontStyle.Bold), new SolidBrush(System.Drawing.Color.Black), startX, startY + Offset);

            //Offset = Offset + 15;//85
            //graphics.DrawString("       www.objectcanvas.com", new Font("Courier New", 9, FontStyle.Bold), new SolidBrush(Color.Black), startX, startY + Offset);


        }

        public void PrintBarcode(POrder oPOrder)
        {
            #region Barcode Purpose

            PrintDialog pd = new PrintDialog();
            pdocBarCode = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            Font font = new Font("Times New Roman", 7);

            PaperSize psize = new PaperSize("Custom", 96,144);
            pd.Document = pdocBarCode;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            pdocBarCode.DefaultPageSettings.PaperSize.Height = 96;
            pdocBarCode.DefaultPageSettings.PaperSize.Width = 250;

            foreach (POrderDetail oPODetail in oPOrder.POrderDetails.ToList())
            {
               
               //if(oPODetail.Product.ProductType==2)
                foreach (POProductDetail oPOPItem in oPODetail.POProductDetails.ToList())
                {
                    sBarcode = oPOPItem.IMENO;
                    sProName = oPODetail.Product.ProductName;
                    sBrand = oPODetail.Product.Company.Name;//Brand
                    sColor = oPODetail.Product.Category.Description;//Color
                    //sProSize = oPODetail.Product.Country.Description;//Size
                    sUnitPrice = oPODetail.MRPRate.ToString();

                    #region For Auto Print
                    pdocBarCode.PrintPage += new PrintPageEventHandler(pdoc_PrintPageBarcode);
                    pdocBarCode.Print();
                    #endregion

                    #region Using by Command
                    pdoc.PrintPage += new PrintPageEventHandler(pdoc_PrintPage);
                    DialogResult result = pd.ShowDialog();
                    if (result == DialogResult.OK)
                    {
                        PrintPreviewDialog pp = new PrintPreviewDialog();
                        pp.Document = pdoc;
                        result = pp.ShowDialog();
                        if (result == DialogResult.OK)
                        {
                            pdoc.Print();
                        }
                    }
                    //pdocBarCode.PrintPage += new PrintPageEventHandler(pdoc_PrintPageBarcode);
                    //DialogResult result = pd.ShowDialog();
                    //if (result == DialogResult.OK)
                    //{
                    //    PrintPreviewDialog pp = new PrintPreviewDialog();
                    //    pp.Document = pdocBarCode;
                    //    result = pp.ShowDialog();
                    //    if (result == DialogResult.OK)PrintPageEventArgs
                    //    {
                    //        pdoc.Print();
                    //    }
                    //}

                    #endregion
                }
            }

            #endregion
        }


        public void PrintBarcodeWeb(PurchaseOrderViewModel purchaseOrder)
        {
            #region Barcode Purpose

            PrintDialog pd = new PrintDialog();
            pdocBarCode = new PrintDocument();
            PrinterSettings ps = new PrinterSettings();
            Font font = new Font("Times New Roman", 7);

            PaperSize psize = new PaperSize("Custom", 96, 144);
            pd.Document = pdocBarCode;
            pd.Document.DefaultPageSettings.PaperSize = psize;
            pdocBarCode.DefaultPageSettings.PaperSize.Height = 96;
            pdocBarCode.DefaultPageSettings.PaperSize.Width = 250;

            foreach (var oPODetail in purchaseOrder.PODetails)
            {
                
                //if (oPODetail.ProductType == 2)
                    foreach (var oPOPItem in oPODetail.POProductDetails.ToList())
                    {
                        sBarcode = oPOPItem.IMENO;
                        sProName = oPODetail.ProductName;
                        sBrand ="Test Brand";//Brand
                        sColor = oPODetail.ColorName;//Color

                        sUnitPrice = oPODetail.MRPRate.ToString();

                        #region For Auto Print
                        pdocBarCode.PrintPage += new PrintPageEventHandler(pdoc_PrintPageBarcode);
                        pdocBarCode.Print();
                        #endregion

                        #region Using by Command
                        //pdocBarCode.PrintPage += new PrintPageEventHandler(pdoc_PrintPageBarcode);
                        //DialogResult result = pd.ShowDialog();
                        //if (result == DialogResult.OK)
                        //{
                        //    PrintPreviewDialog pp = new PrintPreviewDialog();
                        //    pp.Document = pdocBarCode;
                        //    result = pp.ShowDialog();
                        //    if (result == DialogResult.OK)
                        //    {
                        //        pdoc.Print();
                        //    }
                        //}

                        #endregion
                    }
            }

            #endregion
        }
        private void pdoc_PrintPageBarcode(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            Graphics g = e.Graphics;
            Font font = new Font("Times New Roman", 7);

            float fontHeight = font.GetHeight();
            int startX = 5;
            int startY = 2;
            int Offset = 5;


            string sBSN = string.Format("{0}", "   Shapla Computer and Consumers");
            g.DrawString(sBSN, new Font("Times New Roman", 7, FontStyle.Bold), System.Drawing.Brushes.Black, startX + 35, startY);// + 33
            Offset = Offset + 10;

            string sProduct = string.Format("{0}",""+sProName);
            g.DrawString(sProName, new Font("Times New Roman", 7, FontStyle.Bold), System.Drawing.Brushes.Black, startX + 45, startY + Offset);//+ 33
            Offset = Offset + 15;

            Image myimg = Code128Rendering.MakeBarcodeImage(sBarcode, nBarCodeWeight, true);
            g.DrawImage(myimg, startX +40, startY + Offset);
            Offset = Offset + 25;

            //pictBarcode.Image = myimg;

            string sBarcodeCaption = string.Format("   {0}", sBarcode);
            g.DrawString(sBarcodeCaption, new Font("Times New Roman",7, FontStyle.Bold), System.Drawing.Brushes.Black, startX + 75, startY + Offset);// + 15
            Offset = Offset + 20;//myimg.Height;

            //string sProBrand = string.Format("   {0}", sBrand);
            //g.DrawString(sProBrand, new Font("Arial", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startX + 15, startY + Offset);
            //Offset = Offset +  15;

            //string sColorSize = string.Format("   {0}", "Color: "+ sColor + ", Size:   "+ sProSize);
            //g.DrawString(sColorSize, new Font("Times New Roman", 10, FontStyle.Bold), System.Drawing.Brushes.Black, startX, startY + Offset - 3);// + 15
            //Offset = Offset+ 15;

            string caption = string.Format("  Price Tk:   {0}", Convert.ToString(sUnitPrice));
            g.DrawString(caption, new Font("Times New Roman",7, FontStyle.Bold), System.Drawing.Brushes.Black, startX +45, startY + Offset);// + 15

        }

    }
}
