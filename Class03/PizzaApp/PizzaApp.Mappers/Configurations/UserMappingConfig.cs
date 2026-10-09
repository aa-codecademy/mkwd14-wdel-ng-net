using Mapster;
using PizzaApp.Domain.Entities;
using PizzaApp.Dtos.Users;

namespace PizzaApp.Mappers.Configurations;

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<User, UserDto>()
            .Ignore(dest => dest.Roles);
    }
}
