using System;
using System.Linq;
using System.Windows.Data;
using System.Globalization;

namespace easyBilling.ValueConverters
{
    public class CurrencyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            decimal v = 0;

            try
            {
                return System.Convert.ToDecimal(value);
            }
            catch
            {
                return 0.0;
            }

            //if (!string.IsNullOrEmpty(value.ToString()))
            //{
            //    v = value as decimal;
            //}

            ////Return String.Format("{0:c}", v).ToString()
            //return v;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
