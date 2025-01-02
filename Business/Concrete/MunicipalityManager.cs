using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.Utilities;
using Core.Aspects.Autofac.Caching.Caching;
using Core.Dtos;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Results;
using DataAccess.Abstract;
using DataAccess.Concrete.EntityFramework;
using Entity.Concrete;
using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class MunicipalityManager : IMunicipalityService
    {

        IMunicipalityDal _municipalityDal;



        public MunicipalityManager(IMunicipalityDal municipalityDal)
        {
            _municipalityDal = municipalityDal;
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IMunicipalityService.Get")]
        public IResult Add(MunicipalityRequestDto municipalityDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Municipality municipality = new Municipality() { 
                Name=municipalityDto.Name, 
                Description=municipalityDto.Description,
                IsDeleted=false };
            _municipalityDal.Add(municipality);
            return new SuccessResult(Messages.MunicipalityAdded);
        }


        [TransactionalOperation]
        [CacheRemoveAspect("IMunicipalityService.Get")]
        public IResult Delete(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Municipality municipality = _municipalityDal.Get(u => u.Id.Equals(id));
            municipality.IsDeleted = true;
            _municipalityDal.Update(municipality);

            return new SuccessResult(Messages.MunicipalityDeleted);
        }

        [CacheAspect]
        [TransactionalOperation]
        public IDataResult<List<MunicipalityResponseDto>> GetAll()
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<MunicipalityResponseDto>>( result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            List<Municipality> municipalities = _municipalityDal.GetAllAndDepends( includeProperties: "Branches,Branches.Departments");
            return new SuccessDataResult<List<MunicipalityResponseDto>>(municipalities.ConvertAll(m => MunicipalityResponseDto.Generate(m)), Messages.MunicipalitiesListed);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IMunicipalityService.Get")]
        public IDataResult<MunicipalityResponseDto> Update(MunicipalityRequestDto municipalityRequestDto)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<MunicipalityResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Municipality municipality = _municipalityDal.Get(m => m.Id == municipalityRequestDto.MunicipalityId);
            municipality.Name = municipalityRequestDto.Name;
            municipality.Description = municipalityRequestDto.Description;
            municipality.Branches.Clear();
            _municipalityDal.Update(municipality);
            return new SuccessDataResult<MunicipalityResponseDto>(MunicipalityResponseDto.Generate(municipality), Messages.MunicipalityUpdated);
        }

        public IDataResult<MunicipalityResponseDto> GetById(int municipalityId)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<MunicipalityResponseDto>(MunicipalityResponseDto.Generate(_municipalityDal.Get(m => m.Id.Equals(municipalityId))), result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<MunicipalityResponseDto>(MunicipalityResponseDto.Generate(_municipalityDal.Get(m => m.Id.Equals(municipalityId))), Messages.MunicipalityListed);
        }

        public IDataResult<Municipality> GetMunicipalityById(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<Municipality>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<Municipality>(_municipalityDal.Get(m => m.Id.Equals(id)), Messages.MunicipalityListed);
        }
    }
}
