using Castle.DynamicProxy;
using Core.Utilities.Interceptors;
using System;
using System.Transactions;

namespace Business.BusinessAspects.Autofac
{
    public class TransactionalOperation : MethodInterception
    {
        public override void Intercept(IInvocation invocation)
        {
            using (TransactionScope transaction = new TransactionScope())
            {
                try
                {
                    invocation.Proceed();
                    transaction.Complete();
                }
                catch (Exception e)
                {
                    transaction.Dispose();
                    throw;
                }
            }
        }

    }
}
