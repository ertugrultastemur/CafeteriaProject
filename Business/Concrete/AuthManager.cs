using Business.Abstract;
using Business.Constants;
using Core.Aspects.Autofac.Performance;
using Core.Dtos;
using Core.Entities.Concrete;
using Core.Utilities.Hashing;
using Core.Utilities.JWT;
using Core.Utilities.Results;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;

namespace Business.Concrete
{
    public class AuthManager : IAuthService
    {
        private IUserService _userService;
        private ITokenHelper _tokenHelper;
        private IRefreshTokenDal _refreshTokenDal;

        public AuthManager(IUserService userService, ITokenHelper tokenHelper, IRefreshTokenDal refreshTokenDal)
        {
            _userService = userService;
            _tokenHelper = tokenHelper;
            _refreshTokenDal = refreshTokenDal;
        }


        public IDataResult<AccessToken> CreateAccessToken(User user)
        {
            var claims = _userService.GetClaims(user);
            var accessToken = _tokenHelper.CreateAccessToken(user, claims.Data);
            return new SuccessDataResult<AccessToken>(accessToken, Messages.AccessTokenCreated);
        }

        public IDataResult<AccessToken> CreateRefreshToken(User user)
        {
            var refreshToken = _tokenHelper.CreateRefreshToken(user);
            return new SuccessDataResult<AccessToken>(refreshToken, Messages.RefreshTokenCreated);
        }

        [PerformanceAspect(3)]
        public IDataResult<TokenResponseDto> Login(SignInDto signInDto)
        {
            IDataResult<User> userCheck = _userService.GetByEmail(signInDto.Email);

            if (userCheck.Data == null)
            {
                return new ErrorDataResult<TokenResponseDto>(Messages.UserNotFound);
            }
            if (!HashingHelper.VerifyPasswordHash(signInDto.Password, userCheck.Data.PasswordHash, userCheck.Data.PasswordSalt))
            {
                return new ErrorDataResult<TokenResponseDto>(Messages.PasswordError);
            }

            var accessToken = CreateAccessToken(userCheck.Data);
            var refreshToken = CreateRefreshToken(userCheck.Data);
            if (accessToken == null || refreshToken == null || !accessToken.IsSuccess || !refreshToken.IsSuccess)
            {
                return new ErrorDataResult<TokenResponseDto>(Messages.AuthorizationDenied);
            }
            byte[] refreshTokenHash, refreshTokenSalt;
            HashingHelper.CreatePasswordHash(refreshToken.Data.Token, out refreshTokenHash, out refreshTokenSalt);
            RefreshToken token = new RefreshToken()
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = refreshToken.Data.Expiration,
                TokenHash = refreshTokenHash,
                TokenSalt = refreshTokenSalt,
                UserId = userCheck.Data.Id,
                IsDeleted = false
            };
            _refreshTokenDal.Add(token);
            return new SuccessDataResult<TokenResponseDto>(new TokenResponseDto { AccessToken=accessToken.Data.Token, RefreshToken=refreshToken.Data.Token}, Messages.UserSignInSuccessfully);
        }

        public IDataResult<TokenResponseDto> Register(SignUpDto signUpDto)
        {
            byte[] passwordHash, passwordSalt;
            HashingHelper.CreatePasswordHash(signUpDto.Password, out passwordHash, out passwordSalt);

            UserDto user = new UserDto
            {
                FirstName = signUpDto.FirstName,
                LastName = signUpDto.LastName,
                Email = signUpDto.Email,
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                DepartmentId = signUpDto.DepartmentId,
                Balance = 0,
            };

            _userService.Add(user);
            var accessToken = CreateAccessToken(UserDto.Generate(user));
            var refreshToken = CreateRefreshToken(UserDto.Generate(user));
            if (accessToken == null || refreshToken == null || !accessToken.IsSuccess || !refreshToken.IsSuccess) 
            {
                return new ErrorDataResult<TokenResponseDto>(Messages.AuthorizationDenied);
            }
            byte[] refreshTokenHash, refreshTokenSalt;
            HashingHelper.CreatePasswordHash(refreshToken.Data.Token, out refreshTokenHash, out refreshTokenSalt);

            RefreshToken token = new RefreshToken()
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = refreshToken.Data.Expiration,
                TokenHash = refreshTokenHash,
                TokenSalt = refreshTokenSalt,
                UserId = user.UserId,
                IsDeleted = false
            };
            return new SuccessDataResult<TokenResponseDto>(new TokenResponseDto()
            {
                AccessToken = accessToken.Data.Token,
                RefreshToken = refreshToken.Data.Token
            }, Messages.UserSignUpSuccessfully);
        }


        public IDataResult<TokenResponseDto> Refresh(TokenResponseDto tokenResponseDto)
        {
            var oldToken = _tokenHelper.GetUserIdFromToken(tokenResponseDto.RefreshToken);
            if (oldToken == null || !oldToken.IsSuccess)
            {
                return new ErrorDataResult<TokenResponseDto>(oldToken.Message??"401");
            }

            RefreshToken refToken = _refreshTokenDal.Get(r => r.UserId ==oldToken.Data);


            if (!HashingHelper.VerifyPasswordHash(tokenResponseDto.RefreshToken, refToken.TokenHash, refToken.TokenSalt))
            {
                return new ErrorDataResult<TokenResponseDto>(Messages.AuthorizationDenied);
            }
            User user = _userService.GetAllByIds(new List<int> { refToken.UserId }).Data[0];
            AccessToken token = CreateRefreshToken(user).Data;
            byte[] newRefreshTokenHash, newRefreshTokenSalt;
            HashingHelper.CreatePasswordHash(token.Token, out newRefreshTokenHash, out newRefreshTokenSalt);

            RefreshToken newToken = new RefreshToken()
            {
                CreatedDate = DateTime.UtcNow,
                ExpirationDate = token.Expiration,
                TokenHash = newRefreshTokenHash,
                TokenSalt = newRefreshTokenSalt,
                UserId = user.Id,
                IsDeleted = false
            };
            _refreshTokenDal.Add(newToken);
            AccessToken accessToken = CreateAccessToken(user).Data;
            return new SuccessDataResult<TokenResponseDto>(new TokenResponseDto()
            {
                AccessToken= accessToken.Token,
                RefreshToken = token.Token
            });
        }



        public IResult UserExists(string email)
        {
            if (_userService.GetByEmail(email).Data != null)
            {
                return new ErrorResult(Messages.UserAlreadyExists);
            }
            return new SuccessResult();
        }
    }
}
