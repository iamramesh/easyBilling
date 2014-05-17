using System;
using System.Linq;

namespace easyBilling.Report.DataModel
{
    public class BillView
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

        public String Remarks
        {
            get;
            set;
        }

        public String ItemName
        {
            get;
            set;
        }

        public Decimal Qty
        {
            get;
            set;
        }

        public Decimal Rate
        {
            get;
            set;
        }

        public Decimal Amount
        {
            get;
            set;
        }
    }
}
