using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Reporting.WebForms;
using System.Data;
using System.IO;

namespace IMSWEB.Report
{
    public class ReportBase
    {
        public static byte[] GenerateBasicReport(DataSet dataSet, List<ReportParameter> reportParameters,
            string reportName)
        {
            try
            {
                ReportViewer rptViewer = new ReportViewer();
                rptViewer.ShowPrintButton = true;
                rptViewer.LocalReport.DataSources.Clear();
                dataSet.DataSetName = "BasicDataSet";

                foreach (DataTable dt in dataSet.Tables)
                    rptViewer.LocalReport.DataSources.Add(new ReportDataSource(dt.DataSet.DataSetName + "_" + dt.TableName, dt));

                rptViewer.LocalReport.EnableExternalImages = true;
                var appDomain = AppDomain.CurrentDomain;
                var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
                string path = Path.Combine(basePath, "Rdlcs\\Basic", reportName);
                rptViewer.LocalReport.ReportPath = path;
                rptViewer.LocalReport.SetParameters(reportParameters);

                return RenderReport(rptViewer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] GenerateTransactionalReport(DataSet dataSet, List<ReportParameter> reportParameters,
            string reportName)
        {
            try
            {
                ReportViewer rptViewer = new ReportViewer();
                rptViewer.ShowPrintButton = true;
                rptViewer.LocalReport.DataSources.Clear();
                dataSet.DataSetName = "TransactionalDataSet";

                foreach (DataTable dt in dataSet.Tables)
                    rptViewer.LocalReport.DataSources.Add(new ReportDataSource(dt.DataSet.DataSetName + "_" + dt.TableName, dt));

                rptViewer.LocalReport.EnableExternalImages = true;
                var appDomain = AppDomain.CurrentDomain;
                var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
                string path = Path.Combine(basePath, "Rdlcs\\Transactional", reportName);
                rptViewer.LocalReport.ReportPath = path;
                rptViewer.LocalReport.SetParameters(reportParameters);

                return RenderReport(rptViewer);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static byte[] GenerateTransactionalReportByFileType(DataSet dataSet, List<ReportParameter> reportParameters,
            string reportName, int FileType)
        {
            try
            {
                ReportViewer rptViewer = new ReportViewer();
                rptViewer.ShowPrintButton = true;
                rptViewer.LocalReport.DataSources.Clear();
                dataSet.DataSetName = "TransactionalDataSet";

                foreach (DataTable dt in dataSet.Tables)
                    rptViewer.LocalReport.DataSources.Add(new ReportDataSource(dt.DataSet.DataSetName + "_" + dt.TableName, dt));

                rptViewer.LocalReport.EnableExternalImages = true;
                var appDomain = AppDomain.CurrentDomain;
                var basePath = appDomain.RelativeSearchPath ?? appDomain.BaseDirectory;
                string path = Path.Combine(basePath, "Rdlcs\\Transactional", reportName);
                rptViewer.LocalReport.ReportPath = path;
                rptViewer.LocalReport.SetParameters(reportParameters);
                if (FileType == 1)
                {
                    return RenderReportinExcel(rptViewer);
                }
                else
                {
                    return RenderReport(rptViewer);

                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static byte[] RenderReportinExcel(ReportViewer rptViewer)
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;





                byte[] bytes = rptViewer.LocalReport.Render("EXCEL", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                return bytes;




            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        private static byte[] RenderReport(ReportViewer rptViewer)
        {
            try
            {
                Warning[] warnings;
                string[] streamids;
                string mimeType;
                string encoding;
                string extension;

                byte[] bytes = rptViewer.LocalReport.Render("PDF", null, out mimeType, out encoding, out extension, out streamids, out warnings);
                return bytes;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
