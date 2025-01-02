using Business.Abstract;
using Business.BusinessAspects.Autofac;
using Business.Constants;
using Business.Utilities;
using Core.Aspects.Autofac.Caching.Caching;
using Core.Aspects.Autofac.Logging;
using Core.CrossCuttingConcerns.Logging.Log4Net.Loggers;
using Core.Dtos;
using Core.Entities.Concrete;
using Core.Utilities.Business;
using Core.Utilities.Hashing;
using Core.Utilities.JWT;
using Core.Utilities.Results;
using DataAccess.Abstract;
using Entity.Concrete;
using Entity.Dtos;
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

        private IBranchService _branchService;

        public UserManager(IUserDal userDal, IOperationClaimDal operationClaimDal, IRefreshTokenDal refreshTokenDal, IUserOperationClaimDal userOperationClaimDal, IBranchService branchService)
        {
            _userDal = userDal;
            _operationClaimDal = operationClaimDal;
            _refreshTokenDal = refreshTokenDal;
            _userOperationClaimDal = userOperationClaimDal;
            _branchService = branchService;
        }

        [LogAspect(typeof(DatabaseLogger))]
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

            User newUser = _userDal.AddAndReturn(user);
            
            return new SuccessDataResult<UserDto>(UserDto.Generate(newUser), Messages.UserAdded);
        }

        [LogAspect(typeof(DatabaseLogger))]
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
            User newUser = _userDal.AddAndReturn(_user);
            return new SuccessDataResult<User>(newUser, Messages.UserAdded);
        }

        [LogAspect(typeof(DatabaseLogger))]
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
            if(userDto.Password != "" && userDto.Password != null)
            {
                byte[] passwordHash, passwordSalt;
                HashingHelper.CreatePasswordHash(userDto.Password, out passwordHash, out passwordSalt);
                user.PasswordHash = passwordHash;
                user.PasswordSalt = passwordSalt;
            }
            _userDal.Update(user);

            return new SuccessDataResult<UserResponseDto>(UserResponseDto.Generate(user), Messages.UserUpdated);
        }

        [LogAspect(typeof(DatabaseLogger))]
        [TransactionalOperation]
        public IDataResult<UserResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<UserResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }

            User user = _userDal.Get(u => u.Id == resetPasswordDto.Id);
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(resetPasswordDto.Password, out passwordHash, out passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            _userDal.Update(user);
            return new SuccessDataResult<UserResponseDto>(Messages.ResetPassword);
        }

        [LogAspect(typeof(DatabaseLogger))]
        [TransactionalOperation]
        public IResult UndoDelete(int id)
        {
            var result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<UserResponseDto>(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }

            User user = _userDal.Get(u => u.Id.Equals(id));
            user.IsDeleted = false;
            _userDal.Update(user);
            return new SuccessResult(Messages.UserUndoDeleted);
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
            UserDetailDto _user = UserDetailGetter.GetDetails();
            Branch branch = _branchService.GetByUserId(_user.Id).Data;
            List<int> ids = new List<int>(); 
            ids= branch.Departments.Select(d=>d.Id).ToList();

            List<IResult> result = BusinessRules.Check();

            if (result.Count != 0)
            {
                return new ErrorDataResult<List<UserResponseDto>>( result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            return new SuccessDataResult<List<UserResponseDto>>( _userDal.GetAllAndDepends(u => ids.Contains(u.DepartmentId), includeProperties: "OperationClaims").ConvertAll(u => UserResponseDto.Generate(u)), Messages.UsersListed);
        }

        [LogAspect(typeof(DatabaseLogger))]
        [TransactionalOperation]
        public IResult UpdateBalance(decimal balance)
        {
            UserDetailDto _user = UserDetailGetter.GetDetails();
            List<IResult> result = BusinessRules.Check();
            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            User user = _userDal.Get(u => u.Id == _user.Id);
            user.Balance -= balance;
            _userDal.Update(user);
            return new SuccessResult(Messages.BalanceUpdated);
        }

        [LogAspect(typeof(DatabaseLogger))]
        [TransactionalOperation]
        public IResult UpdateBalanceByUserId(int userId, decimal balance)
        {
            List<IResult> result = BusinessRules.Check();
            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            User user = _userDal.Get(u => u.Id == userId);
            user.Balance -= balance;
            _userDal.Update(user);
            return new SuccessResult(Messages.BalanceUpdated);
        }

        [LogAspect(typeof(DatabaseLogger))]
        [TransactionalOperation]
        public IResult AddBalance(UserRequestDto userRequestDto)
        {

            List<IResult> result = BusinessRules.Check(CheckIfBalanceCouldNotBiggerThanZero(userRequestDto));
            if (result.Count != 0)
            {
                return new ErrorResult(result.Select(r => r.Message).Aggregate((current, next) => current + " && " + next));
            }
            User user = _userDal.Get(u => u.Id == userRequestDto.Id);
            user.Balance += userRequestDto.Balance;
            _userDal.Update(user);
            return new SuccessResult(Messages.BalanceUpdated);
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


        private IResult CheckIfBalanceCouldNotBiggerThanZero(UserRequestDto userRequestDto)
        {
            User user = _userDal.Get(u => u.Id==userRequestDto.Id);

            if (user.Balance > 0)
            {
                return new ErrorResult(Messages.BalanceBiggerThanZeroError);
            }
            return new SuccessResult();
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
