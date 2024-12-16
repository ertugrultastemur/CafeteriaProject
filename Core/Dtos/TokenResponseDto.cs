using Core.Entities.Abstract;
using Core.Utilities.JWT;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class TokenResponseDto : IDto
    {
        public string AccessToken { get; set; }

        public string RefreshToken { get; set; }
    }
}
