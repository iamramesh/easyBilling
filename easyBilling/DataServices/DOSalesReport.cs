using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using easyBilling.Helper;

namespace easyBilling.DataServices
{
    public class DOSalesReport
    {
        public static List<DataServices.vw_SalesReport> GetSalesReportByFromTo(DateTime from, DateTime to)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.vw_SalesReports.Where(s => s.VoucherDate.Value.Date >= from && s.VoucherDate.Value.Date <= to).OrderBy(b => b.VoucherDate).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("GetSalesReportByFromTo", ex.Message);
                return null;
            }
        }
    }
}
