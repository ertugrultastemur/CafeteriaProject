using Core.Utilities.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Utilities.Business
{
    public class BusinessRules
    {
        public static List<IResult> Check(params IResult[] logics)
        {
            List<IResult> result = new List<IResult>();
            foreach (var logic in logics)
            {
                if (!logic.IsSuccess) result.Add(logic);
            }
            return result;
        }
    }
}
