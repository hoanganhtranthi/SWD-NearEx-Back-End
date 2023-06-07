using AutoMapper;
using AutoMapper.QueryableExtensions;
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
using System.Text;
using System.Threading.Tasks;

namespace NearExpiredProduct.Service.Service
{
    public interface IProductService
    {
        Task<PagedResults<ProductResponse>> GetProducts(ProductRequest request, PagingRequest paging);
        Task<ProductResponse> DeleteProduct(int id);
        //Task<ProductResponse> CreateProduct(ProductRequest request);
        Task<ProductResponse> GetProductByName(string name);
        Task<ProductResponse> GetProductById(int id);
        Task<ProductResponse> UpdateProduct(int id, ProductRequest request);
    }

    public class ProductService : IProductService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public ProductService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = configuration;
        }

        /* public async Task<ProductResponse> CreateProduct(ProductRequest product)
        {
            try
            {
                if (product == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Product Invalid!!!", "");
                }
                var listStoreId = _unitOfWork.Repository<Product>().GetAll().Where(a => a.StoreId == product.StoreId);
                if (listStoreId != null)
                {
                    var listName = _unitOfWork.Repository<Product>().GetAll().Where(a => a.ProductName == product.ProductName);
                    if (listName != null)
                    {
                        var list = _unitOfWork.Repository<Product>().GetAll().Where(a => a.== product.ProductName);
                        _unitOfWork.Repository<Product>().CreateAsync(product);
                        await _unitOfWork.CommitAsync();
                        return _mapper.Map<Product, ProductResponse>(p);
                    }
                }

            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Product Error!!!", ex.InnerException?.Message);
            }
        }
        */

        public async Task<ProductResponse> DeleteProduct(int id)
        {
            var product = await _unitOfWork.Repository<Product>().GetAsync(p => p.Id == id);
            try
            {
                if (id <= 0)
                {
                    throw new Exception();
                }
                _unitOfWork.Repository<Product>().Delete(product);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Product, ProductResponse>(product);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Product Error!!!", ex.InnerException?.Message);
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
                var response = await _unitOfWork.Repository<Product>().GetAsync(p => p.Id == id);

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found product with id", response.Id.ToString());
                }

                return _mapper.Map<ProductResponse>(response);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Product By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public Task<ProductResponse> GetProductByName(string name)
        {
            try
            {
                Product product = null;
                product = _unitOfWork.Repository<Product>().GetAll().Where(p => p.ProductName == name).FirstOrDefault();
                return Task.FromResult(_mapper.Map<Product, ProductResponse>(product));
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<PagedResults<ProductResponse>> GetProducts(ProductRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<ProductResponse>(request);
                var products = _unitOfWork.Repository<Product>().GetAll()
                                           .ProjectTo<ProductResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<ProductResponse>.Sorting(paging.SortType, products, paging.ColName);
                var result = PageHelper<ProductResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get product list error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<ProductResponse> UpdateProduct(int id, ProductRequest request)
        {
            try
            {
                Product product = null;
                product = _unitOfWork.Repository<Product>()
                    .Find(p => p.Id == id);

                if (product == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found product with id", id.ToString());
                }

                _mapper.Map<ProductRequest, Product>(request, product);

                await _unitOfWork.Repository<Product>().Update(product, id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Product, ProductResponse>(product);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update product error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
