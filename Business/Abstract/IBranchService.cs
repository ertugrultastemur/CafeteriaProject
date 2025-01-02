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
    public interface IBranchService
    {
        IDataResult<List<BranchResponseDto>> GetAll();

        IDataResult<BranchResponseDto> Update(BranchRequestDto branchRequestDto);

        IDataResult<BranchResponseDto> GetById(int branchId);

        IDataResult<Branch> GetBranchById(int branchId);

        IDataResult<Branch> GetByUserId(int userId);

        IDataResult<List<Branch>> GetAllByIds(List<int> ids);

        IResult Add(BranchRequestDto branchDto);

        Branch AddBranch(BranchRequestDto branch);

        IResult Delete(int id);
    }
}
