using Core.EntityFramework;
using DataAccess.Abstract;
using Entity.Concrete;
using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfProductDal : EfEntityRepositoryBase<Product, DbContextImpl>, IProductDal
    {
        public List<ProductDetailDto> GetProductDetails()
        {
            using (DbContextImpl context = new DbContextImpl())
            {
                IQueryable<ProductDetailDto> result = from p in context.Products
                                                      join c in context.Categories
                                                      on p.Category.Id equals c.Id
                                                      select new ProductDetailDto
                                                      {
                                                          ProductId = p.Id,
                                                          ProductName = p.Name,
                                                          CategoryName = c.CategoryName,
                                                          UnitsInStock = p.UnitsInStock,
                                                      };
                return result.ToList();
            }

        }
    }
}
