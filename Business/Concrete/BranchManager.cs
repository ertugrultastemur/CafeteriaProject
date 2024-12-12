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
    public class BranchManager : IBranchService
    {

        IBranchDal _branchDal;
        IMunicipalityService _municipalityService;


        public BranchManager(IBranchDal branchDal, IMunicipalityService municipalityService)
        {
            _branchDal = branchDal;
            _municipalityService = municipalityService;
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IBranchService.Get")]
        public IResult Add(BranchRequestDto branchDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0) 
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Municipality municipality = _municipalityService.GetMunicipalityById(branchDto.MunicipalityId).Data;
            Branch branch = new Branch() { Name=branchDto.Name, Description=branchDto.Description, MunicipalityId=branchDto.MunicipalityId, IsDeleted=false };
            _branchDal.Add(branch);
            return new SuccessResult(Messages.BranchAdded);
        }

        public Branch AddBranch(BranchRequestDto branch)
        {
            Municipality municipality = _municipalityService.GetMunicipalityById(branch.MunicipalityId).Data;
            Branch branchD = new Branch() { Name = branch.Name, Description = branch.Description, MunicipalityId = branch.MunicipalityId, IsDeleted = false };
            _branchDal.Add(branchD);
            return branchD;
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IBranchService.Get")]
        public IResult Delete(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Branch branch = _branchDal.Get(b => b.Id.Equals(id));
            branch.IsDeleted = true;
            _branchDal.Update(branch);

            return new SuccessResult(Messages.BranchDeleted);
        }

        //[CacheAspect]
        public IDataResult<List<BranchResponseDto>> GetAll()
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<BranchResponseDto>>( result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<BranchResponseDto>>(_branchDal.GetAllAndDepends(includeProperties:"Departments,Municipality").ConvertAll(b => BranchResponseDto.Generate(b)), Messages.BranchesListed);
        }

        public IDataResult<List<Branch>> GetAllByIds(List<int> ids)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<Branch>>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<Branch>>(_branchDal.GetAll(b => ids.Contains(b.Id)), Messages.BranchListed);
        }

        public IDataResult<Branch> GetBranchById(int branchId)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<Branch>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<Branch>(_branchDal.Get(b => b.Id.Equals(branchId)), Messages.BranchListed);
        }

        public IDataResult<BranchResponseDto> GetById(int branchId)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<BranchResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<BranchResponseDto>(BranchResponseDto.Generate(_branchDal.Get(b => b.Id.Equals(branchId))), Messages.BranchListed);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IBranchService.Get")]
        public IDataResult<BranchResponseDto> Update(BranchRequestDto branchRequestDto)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<BranchResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Branch branch = _branchDal.Get(b => b.Id == branchRequestDto.BranchId);
            branch.Name = branchRequestDto.Name;
            branch.Description = branchRequestDto.Description;
            branch.MunicipalityId = _municipalityService.GetMunicipalityById(branchRequestDto.MunicipalityId).Data.Id;
            
            _branchDal.Update(branch);
            return new SuccessDataResult<BranchResponseDto>(BranchResponseDto.Generate(branch), Messages.BranchUpdated);
        }
    }
}
