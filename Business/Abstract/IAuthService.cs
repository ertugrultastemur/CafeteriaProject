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

        IResult UserExists(string email);

        IDataResult<AccessToken> CreateAccessToken(User user);

        IDataResult<AccessToken> CreateRefreshToken(User user);

        IDataResult<TokenResponseDto> Refresh(TokenResponseDto tokenResponseDto);
    }
}
