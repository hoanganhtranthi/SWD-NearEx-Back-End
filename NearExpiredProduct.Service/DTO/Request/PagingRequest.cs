﻿

using static NearExpiredProduct.Service.Helpers.SortType;

namespace NearExpiredProduct.Service.DTO.Request
{
    public class PagingRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public SortOrder SortType { get; set; }
        public string ColName { get; set; } = "Id";
    }
}