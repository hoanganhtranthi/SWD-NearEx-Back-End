using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace NearExpiredProduct.Service.DTO.Response
{
    public class SystemReportModel
    {
        public string StoreName { get; set; }
        public int TotalOrder { get; set; }
        public decimal TotalAmount { get; set; }
        public int ToTalCampaign { get; set; }
        public int TotalProduct { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalNewCampaign { get; set; }
        public int TotalAvaliableCampain { get; set; }
        public int TotalOutOfStockCampain { get; set; }
        public int TotalOrderNew { get; set; }
        public int TotalOrderPending { get; set; }
        public int TotalOrderAssign { get; set; }
        public int TotalOrderCancel { get; set; }
        public int TotalOrderStoreCancel { get; set; }
        public int TotalOrderFinish { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }

    public class StoreReportModel
    {
        public int TotalOrder { get; set; }
        public decimal TotalAmount { get; set; }
        public int ToTalCampaign { get; set; }
        public int TotalProduct { get; set; }
        public int TotalCustomer { get; set; }
        public int TotalProductInCampaign { get; set; }
        public int TotalOrderInCampain { get; set; }
        public int TotalCustomerBuyCampain { get; set; }

        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
    }
    public class SystemDayReportModel: DayReport<SystemReportModel>
    {

    }
    public class StoreDayReportModel : DayReport<StoreReportModel>
    {

    }
    public class DayReport<T>
    {
        public DateTime Date { get; set; }
        public T Data { get; set; }
    }

}
