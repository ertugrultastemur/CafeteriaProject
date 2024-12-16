using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Core.Dtos;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Hashing;
using Core.Utilities.JWT;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class UserManager : IUserService
    {
        private IUserDal _userDal;

        private IRefreshTokenDal _refreshTokenDal;

        private IOperationClaimDal _operationClaimDal;

        private IUserOperationClaimDal _userOperationClaimDal;

        public UserManager(IUserDal userDal, IOperationClaimDal operationClaimDal, IRefreshTokenDal refreshTokenDal, IUserOperationClaimDal userOperationClaimDal)
        {
            _userDal = userDal;
            _operationClaimDal = operationClaimDal;
            _refreshTokenDal = refreshTokenDal;
            _userOperationClaimDal = userOperationClaimDal;
        }

        [TransactionalOperation]
        public IDataResult<UserDto> Add(UserDto userDto)
        {
            List<IResult> result = BusinessRules.Check(CheckIfUserNameExistsOfUsersCorrect($"{userDto.Email}"));

            if (result.Count != 0)
            {
                return new ErrorDataResult<UserDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }

            User user = new User
            {
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Email = userDto.Email,
                PasswordHash = userDto.PasswordHash,
                PasswordSalt = userDto.PasswordSalt,
                DepartmentId = userDto.DepartmentId,
                Balance = 0,
                IsDeleted = false
            };

            _userDal.Add(user);
            
            return new SuccessDataResult<UserDto>( Messages.UserAdded);
        }

        [TransactionalOperation]
        public IDataResult<User> AddUser(User user)
        {
            List<IResult> result = BusinessRules.Check(CheckIfUserNameExistsOfUsersCorrect($"{user.Email}"));

            if (result.Count != 0)
            {
                return new ErrorDataResult<User>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            User _user = new User { 
                FirstName= user.FirstName,
                LastName= user.LastName,
                Email= user.Email,
                Balance= user.Balance,
                DepartmentId= user.DepartmentId        
            };
            _userDal.Add(_user);
            return new SuccessDataResult<User>( Messages.UserAdded);
        }

        [TransactionalOperation]
        public IDataResult<UserResponseDto> Update(UserRequestDto userDto)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<UserResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            List<OperationClaim> operationClaims = _operationClaimDal.GetAll(oc => userDto.OperationClaimIds.Contains(oc.Id));
            List<UserOperationClaim> userOperationClaims = new List<UserOperationClaim>();
            User user = _userDal.Get(u => u.Id == userDto.Id);
            user.Email = userDto.Email;
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Balance = userDto.Balance;
            user.DepartmentId = userDto.DepartmentId;
            operationClaims.ForEach(oc => userOperationClaims.Add(new UserOperationClaim { UserId = user.Id, OperationClaimId = oc.Id }));
            List<UserOperationClaim> newUserOperationClaims = new List<UserOperationClaim>();
            foreach (var userOperationClaim in userOperationClaims)
            {
                var exists = _userOperationClaimDal.Get(uoc =>
                    uoc.UserId == userOperationClaim.UserId &&
                    uoc.OperationClaimId == userOperationClaim.OperationClaimId);

                if (exists == null)
                {
                    newUserOperationClaims.Add(_userOperationClaimDal.AddAndReturn(userOperationClaim));
                }
                else
                {
                    newUserOperationClaims.Add(exists); 
                }
            }
            user.OperationClaims = newUserOperationClaims;
            _userDal.Update(user);

            return new SuccessDataResult<UserResponseDto>(UserResponseDto.Generate(user), Messages.UserUpdated);
        }



        public IDataResult<RefreshToken> GetByRefreshToken(string refreshToken)
        {
            var token = _refreshTokenDal.Get(r => HashingHelper.VerifyPasswordHash(refreshToken, r.TokenHash, r.TokenSalt));

            if (token == null || token.IsDeleted || token.ExpirationDate < DateTime.UtcNow)
            {
                return new ErrorDataResult<RefreshToken>(Messages.AuthorizationDenied);
            }

            return new SuccessDataResult<RefreshToken>(token, Messages.AuthorizationDenied);
        }


        public IDataResult<User> GetByEmail(string email)
        {
            List<IResult> result = BusinessRules.Check(CheckIfEmailNotFoundOfUsersCorrect(email));
            if (result.Count != 0)
            {
                return new ErrorDataResult<User>(_userDal.Get(u => u.Email == email), result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<User>(_userDal.Get(u => u.Email == email), Messages.UserListedByEmail);
        }

        public IDataResult<List<OperationClaim>> GetClaims(User user)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<OperationClaim>>(_userDal.GetClaims(user), result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<OperationClaim>>(_userDal.GetClaims(user), Messages.ClaimsListed);
        }

        public IDataResult<List<UserResponseDto>> GetAll()
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<UserResponseDto>>( result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<UserResponseDto>>(_userDal.GetAllAndDepends(includeProperties: "OperationClaims").ConvertAll(u => UserResponseDto.Generate(u)), Messages.UsersListed);
        }


        public IDataResult<UserResponseDto> GetById(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<UserResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            User user = _userDal.Get(u => u.Id.Equals(id), includeProperties: "OperationClaims,OperationClaims.OperationClaim");
            return new SuccessDataResult<UserResponseDto>(UserResponseDto.Generate(user), Messages.UsersListed);
        }

        public IDataResult<List<User>> GetAllByIds(List<int> ids)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<User>>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<User>>(_userDal.GetAll().FindAll(u => ids.Contains(u.Id)), Messages.UsersListed);
        }

        public IResult Delete(int id)
        {
            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            User user = _userDal.Get(u => u.Id.Equals(id));
            user.IsDeleted = true;
            _userDal.Update(user);

            return new SuccessResult(Messages.UserDeleted);
        }

        private IResult CheckIfUserNameExistsOfUsersCorrect(string username)
        {
            if (_userDal.GetAll(u => u.Email == username).Any())//hata burda
            {
                return new ErrorResult(Messages.EmailExistsError);
            }
            return new SuccessResult();
        }

        private IResult CheckIfEmailNotFoundOfUsersCorrect(string email)
        {
            if (!_userDal.GetAll(u => u.Email == email).Any())
            {
                return new ErrorResult(Messages.EmailExistsError);
            }
            return new SuccessResult();
        }

        private IResult CheckIfEmailExistsOfUsersCorrect(string email)
        {
            if (_userDal.GetAll(u => u.Email == email).Any())
            {
                return new ErrorResult(Messages.EmailExistsError);
            }
            return new SuccessResult();
        }


    }
}
