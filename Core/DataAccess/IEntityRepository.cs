﻿using Core.Entities.Abstract;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.DataAccess
{
    public interface IEntityRepository<T> where T : class,IEntity,new()
    {
        List<T> GetAll(Expression<Func<T,bool>> filter=null);

        List<T> GetAllAndDepends(Expression<Func<T, bool>> filter = null, string includeProperties = "");

        T Get(Expression<Func<T, bool>> filter);

        T Get(Expression<Func<T,bool>> filter, string includeProperties);

        void Add(T entity);

        T AddAndReturn(T entity);

        void Update(T entity);

        void Delete(T entity);

    }
}
