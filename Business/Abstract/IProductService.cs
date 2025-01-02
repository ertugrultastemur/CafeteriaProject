using Core.Utilities.Results;
using Entity.Concrete;
using Entity.Dtos;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IProductService
    {
        IDataResult<List<ProductResponseDto>> GetAll();

        IDataResult<List<ProductResponseDto>> GetAllByCategoryId(int id);

        IDataResult<ProductResponseDto> Update(ProductRequestDto productRequestDto, IFormFile image);

        IDataResult<ProductResponseDto> GetById(int productId);

        IResult Add(ProductRequestDto entity, IFormFile image);

        IResult Delete(int id);

        IDataResult<List<ProductResponseDto>> GetByUnitPrice(decimal min, decimal max);

        IDataResult<List<Product>> GetAllByIds(List<int> ids);

    }
}
