using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Data.UnitOfWork;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Exceptions;
using NearExpiredProduct.Service.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using static NearExpiredProduct.Service.Helpers.Enum;

namespace NearExpiredProduct.Service.Service
{
    public interface IReportService
    {
        public Task<StoreReportModel> GetStoreDayReportInRange(DateFilterRequest filter);
        public Task<List<StoreDayReportModel>> GetStoreDayReportByDay(DateTime filter,int? storeId);
        Task<dynamic> GetOrdersReport();
    }
    public class ReportService:IReportService
    {
        private readonly IUnitOfWork _unitOfWork;
        public ReportService(IUnitOfWork unitOfWork )
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<StoreDayReportModel>> GetStoreDayReportByDay(DateTime filter,int? storeId)
        {
            if (filter == null)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Invalid day", "");
            }

            var from = filter.GetStartOfDate();
            var to = filter.GetEndOfDate();

            var listOrder = _unitOfWork.Repository<OrderOfCustomer>().GetAll().Where(x => x.OrderDate >= from && x.OrderDate <= to && x.Campaign.Product.StoreId == storeId).Include(a => a.Campaign).Include(c => c.Campaign.CampaignDetails).ToList();
            var listPro = _unitOfWork.Repository<Product>().GetAll().Where(a => a.StoreId == storeId).ToList();
            var listCampaign = _unitOfWork.Repository<Campaign>().GetAll().Include(c => c.Product).Include(c => c.OrderOfCustomers).Where(a => a.Status != (int)CampaginStatusEnum.OutOfStock && a.Product.StoreId == storeId && a.StartDate <= from).ToList();
            var order = listOrder.GroupBy(x =>
               new
               {
                   x.OrderDate.Date
               }).Select(x => new StoreDayReportModel
               {
                   Date = from,
                   Data = new StoreReportModel()
                   {
                       TotalOrder = listOrder.Count(),
                       ToTalCampaign = listCampaign.Count(),
                       TotalCustomer = listOrder.Select(a => a.CustomerId).Distinct().Count(),
                       TotalProduct = listPro.Count(),
                       TotalCustomerBuyCampain = listOrder.Where(a => a.Campaign.Status != (int)CampaginStatusEnum.OutOfStock).Select(a => a.CustomerId).Distinct().Count(),
                       TotalOrderInCampain = listCampaign.Sum(a => a.OrderOfCustomers.Count()),
                       TotalProductInCampaign = listCampaign.Sum(a => a.Quantity),
                       TotalAmount = (decimal)listOrder.Sum(o => (decimal.Parse(o.Campaign.CampaignDetails.LastOrDefault(x => x.DateApply <= o.OrderDate).Discount.ToString()) * o.Quantity)),
                   }
               });
            return order.ToList();
        }

        public async Task<StoreReportModel> GetStoreDayReportInRange(DateFilterRequest filter)
        {
            var from = filter?.FromDate;
            var to = filter?.ToDate;
            var listOrder = new List<OrderOfCustomer>();
            var listCampaign = new List<Campaign>();
            if (from == null && to == null)
            {
                listOrder = _unitOfWork.Repository<OrderOfCustomer>().GetAll().Where(x => x.Campaign.Product.StoreId == filter.StoreId).Include(a => a.Campaign).Include(c => c.Campaign.CampaignDetails).ToList();
                listCampaign = _unitOfWork.Repository<Campaign>().GetAll().Include(c => c.Product).Include(c => c.OrderOfCustomers).Where(a => a.Status != (int)CampaginStatusEnum.OutOfStock && a.Product.StoreId == (int)filter.StoreId).ToList();
            }
            else
            {
                listOrder = _unitOfWork.Repository<OrderOfCustomer>().GetAll().Where(x => x.OrderDate >= from && x.OrderDate <= to && x.Campaign.Product.StoreId == filter.StoreId).Include(a => a.Campaign).Include(c => c.Campaign.CampaignDetails).ToList();
                listCampaign = _unitOfWork.Repository<Campaign>().GetAll().Include(c => c.Product).Include(c => c.OrderOfCustomers).Where(a => a.Status != (int)CampaginStatusEnum.OutOfStock && a.Product.StoreId == (int)filter.StoreId && a.StartDate <= to).ToList();

                if (DateTime.Compare((DateTime)from, (DateTime)to) > 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Invalid day", "");
                }
            }
            var listPro = _unitOfWork.Repository<Product>().GetAll().Where(a => a.StoreId == (int)filter.StoreId).ToList();
            StoreReportModel report = new StoreReportModel()
            {
                TotalOrder = listOrder.Count(),
                ToTalCampaign = listCampaign.Count(),
                TotalCustomer = listOrder.Select(a => a.CustomerId).Distinct().Count(),
                TotalProduct = listPro.Count(),
                TotalCustomerBuyCampain = listOrder.Where(a => a.Campaign.Status != (int)CampaginStatusEnum.OutOfStock).Select(a => a.CustomerId).Distinct().Count(),
                TotalOrderInCampain = listCampaign.Sum(a => a.OrderOfCustomers.Count()),
                TotalProductInCampaign = listCampaign.Sum(a => a.Quantity),               
                TotalAmount = (decimal)listOrder.Sum(o => (decimal.Parse(o.Campaign.CampaignDetails.LastOrDefault(x => x.DateApply <= o.OrderDate).Discount.ToString()) * o.Quantity)),              
            };
            return report;
        }
       public async Task<dynamic> GetOrdersReport()
        {
            var date = DateTime.Today;
            var orders =  _unitOfWork.Repository<OrderOfCustomer>().GetAll().Include(c => c.Campaign).Include(c => c.Campaign.CampaignDetails).Include(c=>c.Campaign.Product).Include(c=>c.Campaign.Product.Category)
                .ToList();
            var cates = new List<OrderAmoutByCateRequest>();
            foreach (var order in orders)
            {
                var cate = new OrderAmoutByCateRequest();
                var c = cates.Where(a => a.CateName.Equals(order.Campaign.Product.Category.CategoryName)).SingleOrDefault();
                var amout = decimal.Parse(order.Campaign.CampaignDetails.LastOrDefault(x => x.DateApply <= order.OrderDate).Discount.ToString()) * order.Quantity;
                if (c != null)
                {
                    c.Total += amout;
                    continue;
                }
                cate.CateName = order.Campaign.Product.Category.CategoryName;
                cate.Total += amout;
                cates.Add(cate);
            }
            var ordersCate = PageHelper<OrderAmoutByCateRequest>.Paging(cates.OrderByDescending(c => c.Total).ToList(), 1, 4);
            var ordersTotal = _unitOfWork.Repository<OrderOfCustomer>()
                .FindAll(p =>
                    p.OrderDate <= date)
                .AsNoTracking().Count();
            var ordersCOD = _unitOfWork.Repository<Payment>()
                .FindAll(p =>
                     p.Time <= date && p.Method.Equals("COD")).Include(a => a.Order).Include(x => x.Order.Campaign)
                .AsNoTracking().Sum(a=> (decimal.Parse(a.Order.Campaign.CampaignDetails.LastOrDefault(x => x.DateApply <= a.Order.OrderDate).Discount.ToString()) * a.Order.Quantity));
            var ordersOnline = _unitOfWork.Repository<Payment>()
               .FindAll(p =>
                    p.Time <= date && p.Method != "COD").Include(a=>a.Order).Include(x=>x.Order.Campaign)
               .AsNoTracking().Sum(a => (decimal.Parse(a.Order.Campaign.CampaignDetails.LastOrDefault(x => x.DateApply <= a.Order.OrderDate).Discount.ToString()) * a.Order.Quantity));
            
            return new
            {
                Date = date,
                TotalOrder = ordersTotal,
                TotalOrderCOD=ordersCOD,
                TotalOrderOnline=ordersOnline,
                OrdersByCate=ordersCate
            };
        }
    }
}
