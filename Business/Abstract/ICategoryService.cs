using Core.Utilities.Results;
using Entity.Concrete;
using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface ICategoryService
    {
        IDataResult<List<CategoryResponseDto>> GetAll();

        IDataResult<CategoryResponseDto> GetById(int categoryId);

        IDataResult<CategoryResponseDto> Update(CategoryRequestDto categoryRequestDto);

        IResult Add(CategoryRequestDto categoryResponseDto);

        IResult Delete(int id);
    }
}
