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
    public interface IMunicipalityService
    {
        IDataResult<List<MunicipalityResponseDto>> GetAll();

        IDataResult<MunicipalityResponseDto> GetById(int municipalityId);

        IDataResult<Municipality> GetMunicipalityById(int id);

        IDataResult<MunicipalityResponseDto> Update(MunicipalityRequestDto municipalityRequestDto);

        IResult Add(MunicipalityRequestDto municipalityRequestDto);

        IResult Delete(int id);
    }
}
