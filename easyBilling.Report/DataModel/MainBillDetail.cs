using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easyBilling.Report.DataModel
{
    public class MainBillDetail
    {
        public String InvoiceNo
        {
            get;
            set;
        }

        public DateTime InvoiceDate
        {
            get;
            set;
        }

        public Decimal NetQty
        {
            get;
            set;
        }

        public Decimal NetRate
        {
            get;
            set;
        }

        public Decimal NetAmt
        {
            get;
            set;
        }
    }
}
