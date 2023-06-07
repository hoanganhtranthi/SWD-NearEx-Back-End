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
    interface ICategoryService
    {
        Task<PagedResults<CategoryResponse>> GetCategorys(CategoryRequest request, PagingRequest paging);
        Task<CategoryResponse> DeleteCategory(int id);
        Task<CategoryResponse> GetCategoryByName(string name);
        Task<CategoryResponse> GetCategoryById(int id);
        Task<CategoryResponse> UpdateCategory(int id, CategoryRequest request);
        Task<CategoryResponse> InsertCategory(CategoryRequest category);
    }
    public class CategoryService : ICategoryService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;

        public CategoryService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _config = configuration;
        }

        public async Task<CategoryResponse> DeleteCategory(int id)
        {
            var category = await _unitOfWork.Repository<Category>().GetAsync(c => c.Id == id);
            try
            {
                if (id <= 0)
                {
                    throw new Exception();
                }
                _unitOfWork.Repository<Category>().Delete(category);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Category, CategoryResponse>(category);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Category Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<CategoryResponse> GetCategoryById(int id)
        {
            try
            {
                if (id <= 0)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Id Category Invalid", "");
                }
                var response = await _unitOfWork.Repository<Category>().GetAsync(c => c.Id == id);

                if (response == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found category with id", response.Id.ToString());
                }

                return _mapper.Map<CategoryResponse>(response);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Category By ID Error!!!", ex.InnerException?.Message);
            }
        }

        public Task<CategoryResponse> GetCategoryByName(string name)
        {
            try
            {
                Category category = null;
                category = _unitOfWork.Repository<Category>().GetAll().Where(c => c.CateName == name).FirstOrDefault();
                return Task.FromResult(_mapper.Map<Category, CategoryResponse>(category));
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Task<PagedResults<CategoryResponse>> GetCategorys(CategoryRequest request, PagingRequest paging)
        {
            try
            {
                var filter = _mapper.Map<CategoryResponse>(request);
                var categorys = _unitOfWork.Repository<Category>().GetAll()
                                           .ProjectTo<CategoryResponse>(_mapper.ConfigurationProvider)
                                           .DynamicFilter(filter)
                                           .ToList();
                var sort = PageHelper<CategoryResponse>.Sorting(paging.SortType, categorys, paging.ColName);
                var result = PageHelper<CategoryResponse>.Paging(sort, paging.Page, paging.PageSize);
                return Task.FromResult(result);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get category list error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public async Task<CategoryResponse> InsertCategory(CategoryRequest category)
        {
            try
            {
                if (category == null)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Category Invalid!!!", "");
                }
                var cate = await _unitOfWork.Repository<Category>().GetAsync(c => c.Id == category.Id);
                _unitOfWork.Repository<Category>().CreateAsync(cate);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Category, CategoryResponse>(cate);
            }
            catch (Exception ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Category Error!!!", ex.InnerException?.Message);
            }
        }

        public async Task<CategoryResponse> UpdateCategory(int id, CategoryRequest request)
        {
            try
            {
                Category category = null;
                category = _unitOfWork.Repository<Category>()
                    .Find(c => c.Id == id);

                if (category == null)
                {
                    throw new CrudException(HttpStatusCode.NotFound, "Not found category with id", id.ToString());
                }

                _mapper.Map<CategoryRequest, Category>(request, category);

                await _unitOfWork.Repository<Category>().Update(category, id);
                await _unitOfWork.CommitAsync();
                return _mapper.Map<Category, CategoryResponse>(category);
            }
            catch (CrudException ex)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update category error!!!!!", ex.Message);
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
