using System;
using System.Linq;
using Telerik.Windows.Data;
using easyBilling.DataServices;

namespace easyBilling.ValueConverters
{
    public class QtyXRate : AggregateFunction<StockItem, decimal>
    {
        public QtyXRate()
        {
            this.AggregationExpression = stockItem => stockItem.Sum(s => s.Qty * s.Rate).Value ;
            this.ResultFormatString = "Total : {0:C}";
        }
    }
}
