using Core.Utilities.Results;
using Entity.Concrete;
using Entity.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Abstract
{
    public interface IOrderService
    {
        IDataResult<List<OrderResponseDto>> GetAll();

        IDataResult<OrderResponseDto> GetById(int orderId);

        IDataResult<List<Order>> GetAllByIds(List<int> ids);

        IDataResult<OrderResponseDto> Update(OrderRequestDto orderRequestDto);

        IResult Add(OrderRequestDto order);

        IResult Delete(int id);
    }
}
