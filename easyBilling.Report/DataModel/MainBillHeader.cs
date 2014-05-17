using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace easyBilling.Report.DataModel
{
    public class MainBillHeader
    {
        public String BillNo
        {
            get;
            set;
        }

        public DateTime BillDate
        {
            get;
            set;
        }

        public DateTime BillFrom
        {
            get;
            set;
        }

        public DateTime BillTo
        {
            get;
            set;
        }

        public String CustomerName
        {
            get;
            set;
        }

        public String ContactNo
        {
            get;
            set;
        }

        public String Address
        {
            get;
            set;
        }

        public Decimal GrandTotal
        {
            get;
            set;
        }

        public String TotalInWords
        {
            get;
            set;
        }
    }
}
