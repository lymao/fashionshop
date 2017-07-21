﻿using Common;
using Data.Infrastructure;
using Data.Repositories;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IProductService
    {
        Product Add(Product Product);

        void Update(Product Product);

        Product Delete(int id);

        IEnumerable<Product> GetAll();

        IEnumerable<Product> GetAll(string keyword);

        Product GetDetail(int id);

        IEnumerable<Product> GetLastest(int top);

        IEnumerable<Product> GetHotProduct(int top);

        IEnumerable<Product> GetTrendProduct(int top);

        IEnumerable<Product> GetViewProduct(int top);

        IEnumerable<Product> GetListProductByCategoryIdPaging(int categoryId, int page, int pageSize, string sort, out int totalRow);

        IEnumerable<Product> Search(string keyword, int page, int pageSize, string sort, out int totalRow);

        IEnumerable<Product> GetListProduct(string keyword);

        IEnumerable<string> GetListProductByName(string name);

        IEnumerable<Product> GetRelatedProduct(int id, int top);

        IEnumerable<Product> GetListProductByTag(string tagId, int page, int pageSize, string sort, out int totalRow);

        Tag GetTags(string tagId);

        void IncreaseView(int id);

        IEnumerable<Tag> GetListTagByProductId(int id);

        void Save();
    }

    public class ProductService : IProductService
    {
        private IProductRepository _productRepository;
        private ITagRepository _tagRepository;
        private IProductTagRepository _productTagRepository;
        private IProductCategoryRepository _productCategoryRepository;

        private IUnitOfWork _unitOfWork;

        public ProductService(IProductCategoryRepository productCategoryRepository, IProductRepository productRepository, IProductTagRepository productTagRepository,
            ITagRepository tagRepository, IUnitOfWork unitOfWork)
        {
            this._productCategoryRepository = productCategoryRepository;
            this._productRepository = productRepository;
            this._productTagRepository = productTagRepository;
            this._tagRepository = tagRepository;
            this._unitOfWork = unitOfWork;
        }

        public Product Add(Product Product)
        {
            var product = _productRepository.Add(Product);
            _unitOfWork.Commit();
            if (!string.IsNullOrEmpty(Product.Tags))
            {
                string[] tags = Product.Tags.Split(',');
                for (var i = 0; i < tags.Length; i++)
                {
                    var tagId = StringHelper.ToUnsignString(tags[i]);
                    if (_tagRepository.Count(x => x.ID == tagId) == 0)
                    {
                        Tag tag = new Tag();
                        tag.ID = tagId;
                        tag.Name = tags[i];
                        tag.Type = CommonConstants.ProductTag;
                        _tagRepository.Add(tag);
                    }

                    ProductTag productTag = new ProductTag();
                    productTag.ProductID = Product.ID;
                    productTag.TagID = tagId;
                    _productTagRepository.Add(productTag);
                }
            }
            return product;
        }

        public Product Delete(int id)
        {
            return _productRepository.Delete(id);
        }

        public IEnumerable<Product> GetAll()
        {
            return _productRepository.GetAll();
        }

        public IEnumerable<Product> GetAll(string keyword)
        {
            if (!string.IsNullOrEmpty(keyword))
                return _productRepository.GetMulti(x => x.Name.Contains(keyword) || x.Description.Contains(keyword));
            else
                return _productRepository.GetAll();
        }

        public Product GetDetail(int id)
        {
            return _productRepository.GetSingleByCondition(x => x.ID == id, new string[] { "ProductSizes.Size" });
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public void Update(Product Product)
        {
            _productRepository.Update(Product);
            if (!string.IsNullOrEmpty(Product.Tags))
            {
                string[] tags = Product.Tags.Split(',');
                for (var i = 0; i < tags.Length; i++)
                {
                    var tagId = StringHelper.ToUnsignString(tags[i]);
                    if (_tagRepository.Count(x => x.ID == tagId) == 0)
                    {
                        Tag tag = new Tag();
                        tag.ID = tagId;
                        tag.Name = tags[i];
                        tag.Type = CommonConstants.ProductTag;
                        _tagRepository.Add(tag);
                    }
                    _productTagRepository.DeleteMulti(x => x.ProductID == Product.ID);
                    ProductTag productTag = new ProductTag();
                    productTag.ProductID = Product.ID;
                    productTag.TagID = tagId;
                    _productTagRepository.Add(productTag);
                }

            }
        }

        public IEnumerable<Product> GetLastest(int top)
        {
            return _productRepository.GetMulti(x => x.Status, new string[] { "ProductSizes.Size" }).OrderByDescending(x => x.CreatedDate).Take(top);
        }

        public IEnumerable<Product> GetHotProduct(int top)
        {
            return _productRepository.GetMulti(x => x.Status && x.HotFlag == true).OrderByDescending(x => x.CreatedDate).Take(top);
        }

        public List<int> GetIdRecursive(IEnumerable<ProductCategory> productCategories, int id)
        {
            var arr = new List<int>();
            if ((productCategories.Where(x => x.ParentID == id).Count()) > 0)
            {
                foreach (var item in productCategories.Where(x => x.ParentID == id))
                {
                    var child = productCategories.Where(x => x.ParentID == item.ID);
                    if (child.Count() > 0)
                    {
                        var arrsub = GetIdRecursive(child, item.ID);
                        arr.AddRange(arrsub);
                    }
                    arr.Add(item.ID);
                }
            }
            else
            {
                arr.Add(id);
            }
            return arr;
        }

        public IEnumerable<Product> GetListProductByCategoryIdPaging(int categoryId, int page, int pageSize, string sort, out int totalRow)
        {
            var productCategories = _productCategoryRepository.GetAll();
            var listId = GetIdRecursive(productCategories, categoryId);
            var query = _productRepository.GetAll().Where(x => listId.Contains(x.CategoryID));
            totalRow = query.Count();

            switch (sort)
            {
                case "popular":
                    query = query.OrderByDescending(x => x.ViewCount);
                    break;
                case "de_price":
                    query = query.OrderByDescending(m => m.Price);
                    break;
                case "in_price":
                    query = query.OrderBy(m => m.Price);
                    break;
                case "name":
                    query = query.OrderBy(m => m.Name);
                    break;
                default:
                    query = query.OrderByDescending(m => m.CreatedDate);
                    break;
            }
            var pagination = query.Skip((page - 1) * pageSize).Take(pageSize);
            return pagination;
        }

        public IEnumerable<string> GetListProductByName(string name)
        {
            return _productRepository.GetMulti(x => x.Status && x.Name.Contains(name)).Select(y => y.Name);
        }

        public IEnumerable<Product> Search(string keyword, int page, int pageSize, string sort, out int totalRow)
        {
            var query = _productRepository.GetMulti(x => x.Name.Contains(keyword) && x.Status);
            totalRow = query.Count();
            switch (sort)
            {
                case "popular":
                    query = query.OrderByDescending(x => x.ViewCount);
                    break;
                case "de_price":
                    query = query.OrderByDescending(m => m.Price);
                    break;
                case "in_price":
                    query = query.OrderBy(m => m.Price);
                    break;
                case "name":
                    query = query.OrderBy(m => m.Name);
                    break;
                default:
                    query = query.OrderByDescending(m => m.CreatedDate);
                    break;
            }
            var pagination = query.Skip((page - 1) * pageSize).Take(pageSize);
            return pagination;
        }

        public IEnumerable<Product> GetRelatedProduct(int id, int top)
        {
            var product = _productRepository.GetSingleById(id);
            return _productRepository.GetMulti(x => x.Status && x.ID != id && x.CategoryID == product.CategoryID, new string[] { "ProductSizes.Size" }).OrderByDescending(x => x.CreatedDate).Take(top);
        }

        public IEnumerable<Product> GetListProductByTag(string tagId, int page, int pageSize, string sort, out int totalRow)
        {
            var query = _productRepository.GetListProductByTag(tagId);
            totalRow = query.Count();
            switch (sort)
            {
                case "popular":
                    query = query.OrderByDescending(x => x.ViewCount);
                    break;
                case "de_price":
                    query = query.OrderByDescending(m => m.Price);
                    break;
                case "in_price":
                    query = query.OrderBy(m => m.Price);
                    break;
                case "name":
                    query = query.OrderBy(m => m.Name);
                    break;
                default:
                    query = query.OrderByDescending(m => m.CreatedDate);
                    break;
            }
            var pagination = query.Skip((page - 1) * pageSize).Take(pageSize);
            return pagination;
        }

        public Tag GetTags(string tagId)
        {
            return _tagRepository.GetSingleByCondition(x => x.ID == tagId);
        }

        public void IncreaseView(int id)
        {
            var product = _productRepository.GetSingleById(id);

            if (product.ViewCount.HasValue)
                product.ViewCount += 1;
            else
                product.ViewCount = 1;
        }

        public IEnumerable<Tag> GetListTagByProductId(int id)
        {
            return _productTagRepository.GetMulti(x => x.ProductID == id, new string[] { "Tag" }).Select(y => y.Tag);
        }

        public IEnumerable<Product> GetListProduct(string keyword)
        {
            IEnumerable<Product> query;
            if (!string.IsNullOrEmpty(keyword))
                query = _productRepository.GetMulti(x => x.Name.Contains(keyword));
            else
                query = _productRepository.GetAll();
            return query;
        }

        public IEnumerable<Product> GetViewProduct(int top)
        {
            return _productRepository.GetMulti(x => x.Status).OrderByDescending(x => x.ViewCount).Take(top);
        }

        public IEnumerable<Product> GetTrendProduct(int top)
        {
            return _productRepository.GetMulti(x => x.Status && x.TrendFlag == true).OrderByDescending(x => x.CreatedDate).Take(top);
        }
    }
}
