using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.Helpers
{
    public class Enum
    {
        //public enum AreaEnum
        //{
        // "Khu A - Khu vực trống đồng" = 1
        // "Khu B - Khu vực tiện lợi 7Element" = 2
        // "Khu C - Khu vực thư viện" = 3
        // "Khu D - Khu vực Passio" = 4
        //}

        //6h30 - 7h 
        //9h15-9h45
        //12h 12h30
        //2h45- 3h15

        public enum OrderTypeEnum
        {
            AtStore = 1,
            Delivery = 2
        }

        public enum ProductStatusEnum
        {
            New = 0,
            Avaliable = 1,
            OutOfStock = 2
        }

        public enum OrderStatusEnum
        {
            StoreCancel = 0,
            Cancel = 1,
            Pending = 2,
            Assign = 3,
            Finish = 4
        }
    }
}
