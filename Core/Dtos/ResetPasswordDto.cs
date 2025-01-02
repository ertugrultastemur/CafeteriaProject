using Core.Entities.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Dtos
{
    public class ResetPasswordDto: IDto
    {
        public int Id { get; set; }

        public string Password { get; set; }
    }
}
