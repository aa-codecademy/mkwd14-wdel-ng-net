using Mapster;
using PizzaApp.Domain.Entities;
using PizzaApp.Dtos.Orders;

namespace PizzaApp.Mappers.Configurations;

public class OrderMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.UserName, src => src.User != null ? src.User.UserName : null);

        config.NewConfig<AddOrderDto, Order>();
        config.NewConfig<UpdateOrderDto, Order>();
    }
}
