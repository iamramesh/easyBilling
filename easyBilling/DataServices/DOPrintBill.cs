using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easyBilling.DataServices
{
    public class DOPrintBill
    {
        public static List<easyBilling.Report.DataModel.MainBillDetail> GetMainBillDetail(long cid, DateTime from, DateTime to)
        {
            try
            {
                string sqlquery = "SELECT VoucherNo AS InvoiceNo, VoucherDate AS InvoiceDate, NetQty, NetRate, NetAmt "
                                    + "FROM dbo.BillHeader WHERE (Deleted <> 1) AND "
                                    + "(VoucherDate >= '" + from.Date.ToString() + "' And VoucherDate <= '" + to.Date.ToString() + "') AND (CustomerId = " + cid + ")"
                                    + "ORDER BY InvoiceDate";

                eBillingDCDataContext context = new eBillingDCDataContext();

                return context.ExecuteQuery<easyBilling.Report.DataModel.MainBillDetail>(sqlquery).ToList();
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("GetMainBillDetail", ex.Message);
                return null;
            }
        }
    }
}
