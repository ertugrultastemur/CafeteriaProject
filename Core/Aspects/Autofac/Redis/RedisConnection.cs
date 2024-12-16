using Autofac.Core;
using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using Core.Utilities.IoC;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;
using Microsoft.Extensions.DependencyInjection;

namespace Core.Aspects.Autofac.Redis
{
    public class RedisConnection : MethodInterception
    {
        IDatabase _database;
        private static ConnectionMultiplexer _connection;

        public RedisConnection()
        {
            _database = ServiceTool.ServiceProvider.GetService<IDatabase>();
        }

        protected override void OnBefore(IInvocation invocation)
        {
            if (_connection == null || !_connection.IsConnected)
            {
                _connection = ConnectionMultiplexer.Connect("localhost:6379");
            }
            _database = _connection.GetDatabase();
        }
        public override void Intercept(IInvocation invocation)
        {
                try 
                { 


                    invocation.SetArgumentValue(0, _database);
                    invocation.Proceed();

                }
                catch (Exception e)
                {
                    throw;
                }
        }
    }
}
