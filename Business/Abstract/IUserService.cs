using Core.Dtos;
using Core.Entities.Concrete;
using Core.Utilities.JWT;
using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IUserService
    {
        IDataResult<List<OperationClaim>> GetClaims(User user);

        IDataResult<User> AddUser(User user);

        IDataResult<UserDto> Add(UserDto userDto);

        IDataResult<UserResponseDto> Update(UserRequestDto userDto);

        IDataResult<UserResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto);

        IResult UndoDelete(int id);

        IResult Delete(int id);

        IResult UpdateBalance(decimal balance);

        IResult UpdateBalanceByUserId(int userId, decimal balance);

        IResult AddBalance(UserRequestDto userRequestDto);

        IDataResult<UserResponseDto> GetById(int id);

        IDataResult<User> GetByEmail(string email);

        IDataResult<List<UserResponseDto>> GetAll();

        IDataResult<List<User>> GetAllByIds(List<int> ids);

        IDataResult<RefreshToken> GetByRefreshToken(string refreshToken);
    }
}
