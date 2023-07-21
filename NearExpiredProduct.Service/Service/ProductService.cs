using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NearExpiredProduct.Data.Entity;
using NearExpiredProduct.Data.UnitOfWork;
using NearExpiredProduct.Service.DTO.Request;
using NearExpiredProduct.Service.DTO.Response;
using NearExpiredProduct.Service.Exceptions;
using NearExpiredProduct.Service.Helpers;
using NearExpiredProduct.Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Twilio.Http;
using static NearExpiredProduct.Service.Helpers.Enum;

namespace NearExpiredProduct.Service.Service
{
    public interface IProductService
    {
        Task<PagedResults<ProductResponse>> GetProducts(ProductRequest request, PagingRequest paging);
        Task<IEnumerable<dynamic>> GetProductNames(int storeId);
        Task<ProductResponse> CreateProduct(CreateProductRequest request);
        Task<ProductResponse> GetProductById(int id);
        Task<ProductResponse> UpdateStatusProduct(int id, ProductStatusEnum status);
        Task<ProductResponse> UpdateProduct(int id, UpdateProductRequest request);
      
    }

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

         public async Task<ProductResponse> CreateProduct(CreateProductRequest product)
        {
            try
            {
                if (product == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Product Invalid!!!", "");
                }
                if(_unitOfWork.Repository<Product>().GetAll().Where(a=>a.Code.Equals(product.Code)).SingleOrDefault() != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Product Code has already insert!!!", "");
                }
                if (_unitOfWork.Repository<Product>().GetAll().Include(a=>a.Store).Where(a => a.ProductName.Equals(product.ProductName) && a.StoreId==product.StoreId && a.Status==(int)ProductStatusEnum.Avaliable).SingleOrDefault() != null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Product has already insert!!!", "");
                }
                var p = _mapper.Map<Product>(product);
                p.Status=(int)ProductStatusEnum.Avaliable;
                await _unitOfWork.Repository<Product>().CreateAsync(p);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Product, ProductResponse>(p);

            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Product Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<ProductResponse> GetProductById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Product Invalid", "");
                }
                var response = await _unitOfWork.Repository<Product>().GetAll().Where(p => p.Id == id).SingleOrDefaultAsync();

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found product with id{id.ToString()}", "");
                }
                return _mapper.Map<ProductResponse>(response);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Product By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<IEnumerable<dynamic>> GetProductNames(int storeId)
        {
            try
            {
                var products = await _unitOfWork.Repository<Product>().GetAll().Where(a => a.StoreId == storeId).Select(a => new
                {
                    Id = a.Id,
                    Name = a.ProductName,
                    Price=a.Price
                }).ToListAsync();
                return products;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get list of product name error!!!!!", ex.Message);
            }
        }

        public async Task<PagedResults<ProductResponse>> GetProducts(ProductRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<ProductResponse>(request);
                var  products = await _unitOfWork.Repository<Product>().GetAll()
                                           .ProjectTo<ProductResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToListAsync();
                    var sort = PageHelper<ProductResponse>.Sorting(paging.SortType, products, paging.ColName);
                var result = PageHelper<ProductResponse>.Paging(sort, paging.Page, paging.PageSize);
                return result;
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get product list error!!!!!", ex.Message);
            }
        }
        
        public async Task<ProductResponse> UpdateProduct(int id, UpdateProductRequest request)
        {
            try
            {
               Product product = _unitOfWork.Repository<Product>()
                    .Find(p => p.Id == id);
                var products = _unitOfWork.Repository<Product>().GetAll();

                if (products != null)
                {
                    var pro = products.SingleOrDefault(a => a.ProductName.Equals(request.ProductName) && a.StoreId == request.StoreId && a.Status == 1);

                    if (pro != null)
                    {
                        if (pro.Id != id)
                        {
                            throw new CrudException(HttpStatusCode.BadRequest, "Product has already !!!", "");
                        }
                    }
                }
                if (product == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, $"Not found product with id{id.ToString()}", "");
                }
      
                _mapper.Map<UpdateProductRequest, Product>(request, product);

                await _unitOfWork.Repository<Product>().Update(product, id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Product, ProductResponse>(product);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Product By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<ProductResponse> UpdateStatusProduct(int id, ProductStatusEnum status)
        {
            try
            {
                var product = await _unitOfWork.Repository<Product>().GetAll()
                            .Where(x => x.Id == id)
                            .FirstOrDefaultAsync();
                product.Status = (int)status;

                await _unitOfWork.Repository<Product>().Update(product,product.Id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<ProductResponse>(product);
            }
            catch (CrudException ex)
            {
                throw ex;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update product status error", e.Message);
            }
        }
       
    }
}
