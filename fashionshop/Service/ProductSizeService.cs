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
    public interface IProductSizeService
    {
        List<ProductSize> GetAllSizeByProductId(int productId);
        bool CheckExist(int productId, int sizeId);
        void Add(ProductSize productSize);
        IEnumerable<ProductSize> GetAll();
        void Delete(int productId, int sizeId, int quantity);
        bool SellProduct(int productId, int sizeId, int quantity);
        ProductSize GetBySize(int productId, int sizeId);
        void Save();
    }
    public class ProductSizeService : IProductSizeService
    {
        IProductSizeRepository _productSizeRepository;
        IProductRepository _productRepository;
        IUnitOfWork _unitOfWork;
        public ProductSizeService(IProductRepository productRepository, IProductSizeRepository productSizeRepository, IUnitOfWork unitOfWork, IProductService productService)
        {
            this._productSizeRepository = productSizeRepository;
            this._unitOfWork = unitOfWork;
            this._productRepository = productRepository;
        }

        public void Add(ProductSize productSize)
        {
            var product = _productRepository.GetSingleByCondition(x => x.ID == productSize.ProductId, new string[] { "ProductSizes.Size" });
            product.Quantity += productSize.Quantity;
            _productSizeRepository.Add(productSize);
        }

        public bool CheckExist(int productId, int sizeId)
        {
            return _productSizeRepository.CheckContains(x => x.ProductId == productId && x.SizeId == sizeId);
        }

        public void Delete(int productId, int sizeId, int quantity)
        {
            var product = _productRepository.GetSingleByCondition(x => x.ID == productId, new string[] { "ProductSizes.Size" });
            product.Quantity -= quantity;
            var query = _productSizeRepository.GetSingleByCondition(x => x.ProductId == productId && x.SizeId == sizeId);
            _productSizeRepository.Delete(query);
        }

        public IEnumerable<ProductSize> GetAll()
        {
            return _productSizeRepository.GetAll(new string[] { "Product", "Size" });
        }

        public List<ProductSize> GetAllSizeByProductId(int productId)
        {
            var query = _productSizeRepository.GetMulti(x => x.ProductId == productId, new string[] { "Size" });
            return query.ToList();
        }

        public ProductSize GetBySize(int productId, int sizeId)
        {
            return _productSizeRepository.GetSingleByCondition(x => x.ProductId == productId && x.SizeId == sizeId);
        }

        public void Save()
        {
            _unitOfWork.Commit();
        }

        public bool SellProduct(int productId, int sizeId, int quantity)
        {
            var productSize = _productSizeRepository.GetSingleByCondition(x => x.ProductId == productId && x.SizeId == sizeId);
            var product = _productRepository.GetSingleById(productId);
            if (productSize.Quantity < quantity)
                return false;
            productSize.Quantity -= quantity;
            product.Quantity -= quantity;
            return true;
        }
    }
}
