using System;
using System.Linq;
using System.Windows.Controls;
using System.Globalization;

namespace easyBilling.Validators
{
    public class SelectedIndexVal : ValidationRule
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
                if (Convert.ToInt32(value) != -1)
                {
                    return new ValidationResult(true, null);
                }
                else
                {
                    return new ValidationResult(false, ErrorMsg);
                }
            }
            catch
            {
                return new ValidationResult(false, ErrorMsg);
            }
        }
    }
}
