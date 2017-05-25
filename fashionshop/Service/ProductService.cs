using Common;
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

        Product GetById(int id);

        IEnumerable<Product> GetLastest(int top);

        IEnumerable<Product> GetTrend(int top);

        IEnumerable<Product> GetHotProduct(int top);

        IEnumerable<Product> GetListProductByCategoryIdPaging(int categoryId, int page, int pageSize, string sort, out int totalRow);

        IEnumerable<Product> Search(string keyword, int page, int pageSize, string sort, out int totalRow);

        IEnumerable<Product> GetListProduct(string keyword);

        IEnumerable<string> GetListProductByName(string name);

        IEnumerable<Product> GetRelatedProduct(int id, int top);

        IEnumerable<Product> GetListProductByTag(string tagId, int page, int pageSize, out int totalRow);

        Tag GetTags(string tagId);

        void IncreaseView(int id);

        IEnumerable<Tag> GetListTagByProductId(int id);

        bool SellProduct(int productId, int quantity);

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

        public Product GetById(int id)
        {
            return _productRepository.GetSingleById(id);
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
            return _productRepository.GetMulti(x => x.Status).OrderByDescending(x => x.CreatedDate).Take(top);
        }

        public IEnumerable<Product> GetHotProduct(int top)
        {
            return _productRepository.GetMulti(x => x.Status && x.HotFlag == true).OrderByDescending(x => x.CreatedDate).Take(top);
        }

        public IEnumerable<Product> GetListProductByCategoryIdPaging(int categoryId, int page, int pageSize, string sort, out int totalRow)
        {
            var productCategories = _productCategoryRepository.GetAll();
            var listPro = new List<Product>();
            var checkId = productCategories.Where(x => x.ParentID == categoryId);
            if (checkId.Count() > 0)
            {
                foreach (var item in productCategories.Where(x => x.ParentID == categoryId))
                {
                    var child = productCategories.Where(x => x.ParentID == item.ID);
                    if (child.Count() > 0)
                    {
                        foreach (var item1 in child)
                        {
                            var child1 = productCategories.Where(x => x.ParentID == item1.ID);
                            if (child1.Count() > 0)
                            {
                                foreach (var item2 in child1)
                                {
                                    var query2 = _productRepository.GetMulti(x => x.CategoryID == item2.ID && x.Status);
                                    listPro.AddRange(query2);
                                }
                            }
                            var query1 = _productRepository.GetMulti(x => x.CategoryID == item1.ID && x.Status);
                            listPro.AddRange(query1);
                        }
                    }
                    var query = _productRepository.GetMulti(x => x.CategoryID == item.ID && x.Status);
                    listPro.AddRange(query);
                }
            }
            else
            {
                var query0 = _productRepository.GetMulti(x => x.CategoryID == categoryId && x.Status);
                listPro.AddRange(query0);
            }


            totalRow = listPro.Count();
            var pagination = listPro.Skip((page - 1) * pageSize).Take(pageSize);
            switch (sort)
            {
                case "popular":
                    pagination = pagination.OrderByDescending(x => x.ViewCount);
                    break;
                case "de_price":
                    pagination = pagination.OrderByDescending(m => m.Price);
                    break;
                case "in_price":
                    pagination = pagination.OrderBy(m => m.Price);
                    break;
                case "name":
                    pagination = pagination.OrderBy(m => m.Name);
                    break;
                default:
                    pagination = pagination.OrderByDescending(m => m.CreatedDate);
                    break;
            }
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
            var pagination= query.Skip((page - 1) * pageSize).Take(pageSize);
            switch (sort)
            {
                case "popular":
                    pagination = pagination.OrderByDescending(x => x.ViewCount);
                    break;
                case "discount":
                    pagination = pagination.OrderByDescending(m => m.PromotionPrice.HasValue);
                    break;
                case "price":
                    pagination = pagination.OrderBy(m => m.Price);
                    break;
                default:
                    pagination = pagination.OrderByDescending(m => m.CreatedDate);
                    break;
            }
            return pagination;
        }

        public IEnumerable<Product> GetRelatedProduct(int id, int top)
        {
            var product = _productRepository.GetSingleById(id);
            return _productRepository.GetMulti(x => x.Status && x.ID != id && x.CategoryID == product.CategoryID).OrderByDescending(x => x.CreatedDate).Take(top);
        }

        public IEnumerable<Product> GetListProductByTag(string tagId, int page, int pageSize, out int totalRow)
        {
            var model = _productRepository.GetListProductByTag(tagId, page, pageSize, out totalRow);
            return model;
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

        //Selling product
        public bool SellProduct(int productId, int quantity)
        {
            var product = _productRepository.GetSingleById(productId);
            if (product.Quantity < quantity)
                return false;
            product.Quantity -= quantity;
            return true;
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

        public IEnumerable<Product> GetTrend(int top)
        {
            return _productRepository.GetMulti(x => x.Status && x.TrendFlag == true).OrderByDescending(x => x.CreatedDate).Take(top);
        }
    }
}
