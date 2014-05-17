using System;
using System.Collections.Generic;
using System.Linq;
using easyBilling.DataServices;
using easyBilling.Helper;

namespace easyBilling.DataServices
{
    public class DataOperations
    {
        public static string GetVoucherNo(long fId)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();

                var setg = context.Settings.Where(s => s.FormId == fId).SingleOrDefault();

                string sql1 = null;

                if (fId == 1)
                {
                    sql1 = "Select Max(Cast(right(VoucherNo,Len(VoucherNo)-" + setg.Prefix.Length.ToString() + ") as int )) as VoucherNo from dbo.BillHeader where VoucherNo like '" + setg.Prefix + "%'";
                }
                else if (fId == 2)
                {
                    sql1 = "Select Max(Cast(right(VoucherNo,Len(VoucherNo)-" + setg.Prefix.Length.ToString() + ") as int )) as VoucherNo from dbo.Customer where VoucherNo like '" + setg.Prefix + "%'";
                }
                else if (fId == 3)
                {
                    sql1 = "Select Max(Cast(right(VoucherNo,Len(VoucherNo)-" + setg.Prefix.Length.ToString() + ") as int )) as VoucherNo from dbo.StockItem where VoucherNo like '" + setg.Prefix + "%'";
                }

                string maxId = null;

                try
                {
                    foreach (var fs in context.ExecuteQuery<int>(sql1).ToList())
                    {
                        maxId = fs.ToString();
                    }
                }
                catch
                {
                    //return setg.Prefix + System.StrDup(Convert.ToInt32(setg.Length) - setg.Prefix.Length - Strings.Len(Strings.Trim(Conversion.Str(0 + 1))), "0") + "1";

                    return setg.Prefix + "1".PadLeft(Convert.ToInt32(setg.Length) - setg.Prefix.Length, '0');
                }

                if (string.IsNullOrEmpty(maxId))
                {
                    return setg.Prefix + "1".PadLeft(Convert.ToInt32(setg.Length) - setg.Prefix.Length, '0');
                    //return setg.Prefix + Strings.StrDup(Convert.ToInt32(setg.Lenth) - Strings.Len(setg.Prefix) - Strings.Len(Strings.Trim(Conversion.Str(0 + 1))), "0") + "1";
                }
                else
                {
                    maxId = (Convert.ToInt32(maxId) + 1).ToString();

                    //return setg.Prefix + maxId.PadLeft(Convert.ToInt32(setg.Length) - setg.Prefix.Length - maxId.Length, '0');
                    return setg.Prefix + maxId.PadLeft(Convert.ToInt32(setg.Length) - setg.Prefix.Length, '0');

                    //return setg.Prefix + Strings.StrDup(Convert.ToInt32(setg.Lenth) - Strings.Len(setg.Prefix) - Strings.Len(Strings.Trim(Conversion.Str(Convert.ToInt32(maxId) + 1))), "0") + Strings.Trim(Conversion.Str(Convert.ToInt32(maxId) + 1));
                }
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("GetVoucherId", ex.Message);
                return "NULL";
            }
        }


        //StockItem Table Operations ------------------------------------------------------------------------
        public static List<DataServices.StockItem> GetAllStockItem()
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.StockItems.Where(s => s.Deleted != true).OrderBy(s => s.ItemName).ToList();
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("FetchAllStockItems", ex.Message);
                return null;
            }
        }

        public static DataServices.StockItem GetStockItemBySid(long sId)
        {
            eBillingDCDataContext context = new eBillingDCDataContext();
            return context.StockItems.Where(s => s.StockItemId == sId).SingleOrDefault();
        }

        public static Boolean AddStockItem(DataServices.StockItem newStockItem)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();

                newStockItem.CreatedDttm = System.DateTime.Now;
                newStockItem.Deleted = false;

                context.StockItems.InsertOnSubmit(newStockItem);
                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("AddStockItem", ex.Message);
                return false;
            }
        }

        public static Boolean UpdateStockItem(DataServices.StockItem upStockItem)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                var stockToUpdate = context.StockItems.Where(s => s.StockItemId == upStockItem.StockItemId).SingleOrDefault();

                stockToUpdate.VoucherNo = upStockItem.VoucherNo;
                stockToUpdate.ItemName = upStockItem.ItemName;
                stockToUpdate.Qty = upStockItem.Qty;
                stockToUpdate.Rate = upStockItem.Rate;
                stockToUpdate.Remarks = upStockItem.Remarks;

                stockToUpdate.ModifiedDttm = System.DateTime.Now;

                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("UpdateStockItem", ex.Message);
                return false;
            }
        }

        public static Boolean DeleteStockItem(long sId)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                var stockToDelete = context.StockItems.Where(s => s.StockItemId == sId).SingleOrDefault();

                stockToDelete.Deleted = true;
                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("DeleteStockItem", ex.Message);
                return false;
            }
        }


        //BillHeader Table Operations ------------------------------------------------------------------------
        public static List<easyBilling.Report.DataModel.BillView> GetBillView(long BHid)
        {
            try
            {
                string sqlquery = "SELECT dbo.BillHeader.BillHeaderId, dbo.BillHeader.VoucherNo AS BillNo, dbo.BillHeader.VoucherDate AS BillDate, dbo.BillHeader.NetQty, "
                            + "dbo.BillHeader.Discount, dbo.BillHeader.NetRate, dbo.BillHeader.NetAmt, dbo.BillHeader.Remarks, dbo.StockItem.ItemName, dbo.BillDetail.Qty, "
                            + "dbo.BillDetail.Rate, dbo.BillDetail.Amount FROM dbo.BillDetail INNER JOIN "
                            + "dbo.BillHeader ON dbo.BillDetail.BillHeaderId = dbo.BillHeader.BillHeaderId INNER JOIN "
                            + "dbo.StockItem ON dbo.BillDetail.StockItemId = dbo.StockItem.StockItemId "
                            + "WHERE (dbo.BillHeader.Deleted = 0) AND (dbo.BillDetail.Deleted = 0) AND (dbo.BillHeader.BillHeaderId = " + BHid + ")"
                            + " ORDER BY BillNo";

                eBillingDCDataContext context = new eBillingDCDataContext();

                return context.ExecuteQuery<easyBilling.Report.DataModel.BillView>(sqlquery).ToList();
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("GetBillView", ex.Message);
                return null;
            }
        }

        public static List<DataServices.BillHeader> GetAllBillHeader()
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.BillHeaders.Where(b => b.Deleted != true).OrderByDescending(b => b.VoucherDate).ToList();
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("GetAllBillHeader", ex.Message);
                return null;
            }
        }

        public static List<DataServices.BillHeader> GetAllBillHeaderByFromTo(DateTime from, DateTime to)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.BillHeaders.Where(b => b.Deleted != true && b.VoucherDate.Value.Date >= from && b.VoucherDate.Value.Date <= to).OrderBy(b => b.VoucherDate).ToList();
            }
            catch (Exception ex)
            {
                Helper.LogEntry.WriteEntry("GetAllBillHeaderByFromTo", ex.Message);
                return null;
            }
        }

        public static BillHeader GetBillHeaderByBHId(long BHid)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.BillHeaders.Where(b => b.BillHeaderId == BHid & b.Deleted != true).SingleOrDefault();
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("GetBillHeaderByBHId", ex.Message);
                return null;
            }
        }
        
        public static long AddBillHeader(BillHeader newBH, List<BillDetail> listBD)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();

                newBH.CreatedDttm = System.DateTime.Now;
                newBH.Deleted = false;

                context.BillHeaders.InsertOnSubmit(newBH);

                context.SubmitChanges();

                long BHid = Convert.ToInt64(context.BillHeaders.Max(b => b.BillHeaderId));

                AddBillDetail(listBD, BHid, context);

                context.SubmitChanges();

                return BHid;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("AddBillHeader", ex.Message);
                return 0;
            }
        }

        public static long AddBillHeader(BillHeader newBH, List<BillDetail> listBD, Local newLocal)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();

                newBH.CustomerId = DOCustomer.AddLocal(newLocal);
                newBH.CreatedDttm = System.DateTime.Now;
                newBH.Deleted = false;

                context.BillHeaders.InsertOnSubmit(newBH);

                context.SubmitChanges();

                long BHid = Convert.ToInt64(context.BillHeaders.Max(b => b.BillHeaderId));

                AddBillDetail(listBD, BHid, context);

                context.SubmitChanges();

                return BHid;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("AddBillHeader", ex.Message);
                return 0;
            }
        }

        public static long UpdateBillHeader(BillHeader upBH, List<BillDetail> listBD)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                var bhToUpdate = context.BillHeaders.Where(b => b.BillHeaderId == upBH.BillHeaderId).SingleOrDefault();

                bhToUpdate.CustomerId = upBH.CustomerId;
                bhToUpdate.IsLocal = upBH.IsLocal;
                bhToUpdate.VoucherDate = upBH.VoucherDate;
                bhToUpdate.NetQty = upBH.NetQty;
                bhToUpdate.NetRate = upBH.NetRate;
                bhToUpdate.NetAmt = upBH.NetAmt;
                bhToUpdate.Discount = upBH.Discount;
                bhToUpdate.Remarks = upBH.Remarks;

                bhToUpdate.ModifiedDttm = System.DateTime.Now;

                AddBillDetail(listBD, upBH.BillHeaderId, context);

                context.SubmitChanges();

                return upBH.BillHeaderId;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("UpdateBillHeader", ex.Message);
                return 0;
            }
        }

        public static Boolean UpdateBillHeader(BillHeader upBH, List<BillDetail> listBD, Local newLocal)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                var bhToUpdate = context.BillHeaders.Where(b => b.BillHeaderId == upBH.BillHeaderId).SingleOrDefault();

                bhToUpdate.CustomerId = DOCustomer.AddLocal(newLocal);
                bhToUpdate.IsLocal = upBH.IsLocal;
                bhToUpdate.VoucherDate = upBH.VoucherDate;
                bhToUpdate.NetQty = upBH.NetQty;
                bhToUpdate.NetRate = upBH.NetRate;
                bhToUpdate.NetAmt = upBH.NetAmt;
                bhToUpdate.Discount = upBH.Discount;
                bhToUpdate.Remarks = upBH.Remarks;

                bhToUpdate.ModifiedDttm = System.DateTime.Now;

                AddBillDetail(listBD, upBH.BillHeaderId, context);

                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("UpdateBillHeader", ex.Message);
                return false;
            }
        }

        public static Boolean DeleteBillHeader(long BHid)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                var bhToDelete = context.BillHeaders.Where(b => b.BillHeaderId == BHid).SingleOrDefault();

                bhToDelete.Deleted = true;

                DeleteBillDetail(BHid, context);

                context.SubmitChanges();

                return true;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("DeleteBillHeader", ex.Message);
                return false;
            }
        }

        //BillDetail Table Operations ------------------------------------------------------------------------

        public static List<VW_BillDetail_Update> GetVW_BillDetail_UpdateByBHid(long BHid)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();
                return context.VW_BillDetail_Updates.Where(b => b.BillHeaderId == BHid).ToList();
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("GetVW_BillDetail_UpdateByBHid", ex.Message);
                return null;
            }
        }

        public static Boolean AddBillDetail(List<BillDetail> listBD, long BHid, eBillingDCDataContext context)
        {
            try
            {
                foreach (var bd in listBD)
                {
                    if (UpdateBillDetail(bd, context) == false)
                    {
                        bd.BillHeaderId = BHid;

                        if (bd.Deleted == true)
                        {
                        }
                        else
                        {
                            bd.Deleted = false;
                        }

                        context.BillDetails.InsertOnSubmit(bd);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("AddBillDetail", ex.Message);
                return false;
            }
        }

        public static Boolean UpdateBillDetail(BillDetail upBD, eBillingDCDataContext context)
        {
            try
            {
                var bdToUp = context.BillDetails.Where(b => b.BillDetailId == upBD.BillDetailId).SingleOrDefault();

                bdToUp.StockItemId = upBD.StockItemId;
                bdToUp.Qty = upBD.Qty;
                bdToUp.Rate = upBD.Rate;
                bdToUp.Amount= upBD.Amount;
                bdToUp.Deleted = upBD.Deleted;

                return true;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("UpdateBillDetail", ex.Message);
                return false;
            }
        }

        public static Boolean DeleteBillDetail(long BHid, eBillingDCDataContext context)
        {
            try
            {
                var query = context.BillDetails.Where(b => b.BillHeaderId == BHid).ToList();

                foreach (var bd in query)
                {
                    var bdToUp = context.BillDetails.Where(b => b.BillDetailId == bd.BillDetailId).SingleOrDefault();
                    bdToUp.Deleted = true;
                }

                return true;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("DeleteBillDetail", ex.Message);
                return false;
            }
        }


        public static long AddAppUse(DataServices.AppUse newApp)
        {
            try
            {
                eBillingDCDataContext context = new eBillingDCDataContext();

                context.AppUses.InsertOnSubmit(newApp);
                context.SubmitChanges();

                return newApp.aId;
            }
            catch (Exception ex)
            {
                LogEntry.WriteEntry("AddAppUse", ex.Message);
                return 0;
            }
        }
    }
}
