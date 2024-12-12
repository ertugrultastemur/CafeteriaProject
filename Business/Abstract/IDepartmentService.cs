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
    public interface IDepartmentService
    {
        IDataResult<List<DepartmentResponseDto>> GetAll();

        IDataResult<DepartmentResponseDto> Update(DepartmentRequestDto departmentRequestDto);

        IDataResult<DepartmentResponseDto> GetById(int departmentId);

        IResult Add(DepartmentRequestDto departmentDto);

        IResult Delete(int id);
    }
}
