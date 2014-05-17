using System;
using System.Linq;
using System.Windows.Controls;
using System.Globalization;

namespace easyBilling.Validators
{
    public class NumberVal : ValidationRule
    {
        private string errorMsg;

        public string ErrorMsg
        {
            get { return errorMsg; }
            set { errorMsg = value; }
        }

        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            try
            {
                value = Convert.ToInt64(value);

                return new ValidationResult(true, null);
            }
            catch
            {
                return new ValidationResult(false, ErrorMsg);
            }
        }
    }
}
