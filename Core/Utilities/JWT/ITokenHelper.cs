using Core.Entities.Concrete;
using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.JWT
{
    public interface ITokenHelper
    {
        AccessToken CreateAccessToken(User user, List<OperationClaim> operationClaims);

        AccessToken CreateRefreshToken(User user);

        IDataResult<int?> GetUserIdFromToken(string token);

    }
}
