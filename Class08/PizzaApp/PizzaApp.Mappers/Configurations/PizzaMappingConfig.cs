using Mapster;
using PizzaApp.Domain.Entities;
using PizzaApp.Dtos.Pizzas;

namespace PizzaApp.Mappers.Configurations;

public class PizzaMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Pizza, PizzaDto>();
        config.NewConfig<AddPizzaDto, Pizza>();
        config.NewConfig<UpdatePizzaDto, Pizza>();
    }
}
