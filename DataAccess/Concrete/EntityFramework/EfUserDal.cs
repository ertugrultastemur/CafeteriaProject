﻿using Core.Entities.Concrete;
using Core.EntityFramework;
using DataAccess.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataAccess.Concrete.EntityFramework
{
    public class EfUserDal : EfEntityRepositoryBase<User, DbContextImpl>, IUserDal
    {
        public List<OperationClaim> GetClaims(User user)
        {
            using (var context = new DbContextImpl())
            {
                var result = from operationClaim in context.OperationClaims
                             join userOperationClaim in context.UserOperationClaims
                             on operationClaim.Id equals userOperationClaim.OperationClaimId
                             where userOperationClaim.UserId == user.Id
                             select new OperationClaim { Id = userOperationClaim.OperationClaimId, Name = operationClaim.Name };
                return result.ToList();
            }
        }
    }
}
