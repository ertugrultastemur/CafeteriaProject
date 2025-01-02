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
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class DepartmentManager : IDepartmentService
    {

        private IDepartmentDal _departmentDal;
        private IBranchService _branchService;
        private IUserService _userService;

        public DepartmentManager(IDepartmentDal departmentDal, IUserService userService, IBranchService branchService)
        {
            _departmentDal = departmentDal;
            _userService = userService;
            _branchService = branchService;
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IDepartmentService.Get")]
        public IResult Add(DepartmentRequestDto departmentDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Department department = new Department() {
                Name = departmentDto.Name,
                Description = departmentDto.Description,
                Floor = departmentDto.Floor,
                Branch = _branchService.GetBranchById(departmentDto.BranchId).Data,
                IsDeleted = false };
            _departmentDal.Add(department);
            return new SuccessResult(Messages.DepartmentAdded);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IDepartmentService.Get")]
        public IResult Delete(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Department department = _departmentDal.Get(b => b.Id.Equals(id));
            department.IsDeleted = true;
            _departmentDal.Update(department);

            return new SuccessResult(Messages.DepartmentDeleted);
        }

        [CacheAspect]
        public IDataResult<List<DepartmentResponseDto>> GetAll()
        {
            UserDetailDto _user = UserDetailGetter.GetDetails();
            Branch branch = _branchService.GetByUserId(_user.Id).Data;

            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<DepartmentResponseDto>>( result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            List<Department> departments = _departmentDal.GetAllAndDepends(d => d.Branch.Id == branch.Id, includeProperties: "Branch,Users");

            return new SuccessDataResult<List<DepartmentResponseDto>>(departments
                .ConvertAll(d => DepartmentResponseDto.Generate(d)), Messages.BranchesListed);
        }

        public IDataResult<DepartmentResponseDto> GetById(int departmentId)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<DepartmentResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<DepartmentResponseDto>(DepartmentResponseDto.Generate(_departmentDal.Get(d => d.Id.Equals(departmentId))), Messages.BranchesListed);
        }

        [TransactionalOperation]
        [CacheRemoveAspect("IDepartmentService.Get")]
        public IDataResult<DepartmentResponseDto> Update(DepartmentRequestDto departmentRequestDto)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<DepartmentResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            Department department = _departmentDal.Get(d => d.Id == departmentRequestDto.DepartmentId);
            department.Name = departmentRequestDto.Name;
            department.Description = departmentRequestDto.Description;
            department.Floor = departmentRequestDto.Floor;
            List<User> users = _userService.GetAllByIds(departmentRequestDto.UserIds).Data;
            department.Users.Clear();
            department.Users = users;
            _departmentDal.Update(department);
            return new SuccessDataResult<DepartmentResponseDto>(DepartmentResponseDto.Generate(department), Messages.DepartmentUpdated);
        }
    }
}
