using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using easyBilling.Helper;

namespace easyBilling.DataServices
{
    public class DOCustomer
    {
        public static List<DataServices.Local> GetAllLocal()
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.Locals.OrderBy(c => c.Name).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("GetAllLocal", ex.Message);
                return null;
            }
        }

        public static List<DataServices.Customer> GetAllCustomer()
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.Customers.Where(c => c.Deleted != true).OrderBy(c => c.Name).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("GetAllCustomer", ex.Message);
                return null;
            }
        }

        public static DataServices.Customer GetCustomerByCid(long cId)
        {
            eBillingDCDataContext context = new eBillingDCDataContext();
            return context.Customers.Where(c => c.CustomerId == cId).SingleOrDefault();
        }

        public static DataServices.Customer GetCustomerByName(String name)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.Customers.Where(c => c.Name == name).First();
            }
            catch(Exception ex)
            {
                LogEntry.WriteEntry("GetCustomerByName", ex.Message);
                return null;
            }
        }

        public static Boolean AddCustomer(DataServices.Customer newCustomer)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();

                newCustomer.VoucherNo = DataOperations.GetVoucherNo(2);
                newCustomer.CreatedDttm = System.DateTime.Now;
                newCustomer.Deleted = false;

                context.Customers.InsertOnSubmit(newCustomer);
                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("AddCustomer", ex.Message);
                return false;
            }
        }

        public static Boolean UpdateCustomer(DataServices.Customer upCustomer)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                var custToUpdate = context.Customers.Where(c => c.CustomerId == upCustomer.CustomerId).SingleOrDefault();

                custToUpdate.VoucherNo = upCustomer.VoucherNo;
                custToUpdate.Name = upCustomer.Name;
                custToUpdate.Address = upCustomer.Address;
                custToUpdate.ContactNo = upCustomer.ContactNo;
                custToUpdate.Remarks = upCustomer.Remarks;

                custToUpdate.ModifiedDttm = System.DateTime.Now;

                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("UpdateCustomer", ex.Message);
                return false;
            }
        }

        public static Boolean DeleteCustomer(long cId)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                var custToDelete = context.Customers.Where(c => c.CustomerId == cId).SingleOrDefault();

                custToDelete.Deleted = true;
                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("DeleteCustomer", ex.Message);
                return false;
            }
        }

        public static long AddLocal(DataServices.Local newLocal)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();

                context.Locals.InsertOnSubmit(newLocal);
                context.SubmitChanges();

                return Convert.ToInt64(context.Locals.Max(l => l.LocalId));
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("AddLocal", ex.Message);
                
                return 0;
            }
        }
    }
}
