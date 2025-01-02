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
    public interface IAuthService
    {
        IDataResult<TokenResponseDto> Register(SignUpDto signUpDto);

        IDataResult<TokenResponseDto> Login(SignInDto signInDto);

        IResult AddOperationClaim(OperationClaimDto operationClaimDto);

        IDataResult<UserResponseDto> UpdateUserRoles(OperationClaimDto operationClaimDto);

        IDataResult<UserResponseDto> ResetPassword(ResetPasswordDto resetPasswordDto);

        IDataResult<List<OperationClaimResponseDto>> GetAllOperationClaims();

        IResult CheckIfUserExists(string email);

        IResult CheckIfOperationClaimAlreadyExists(OperationClaimDto operationClaimDto);

        IDataResult<AccessToken> CreateAccessToken(User user);

        IDataResult<AccessToken> CreateRefreshToken(User user);

        IDataResult<TokenResponseDto> Refresh(TokenResponseDto tokenResponseDto);
    }
}
