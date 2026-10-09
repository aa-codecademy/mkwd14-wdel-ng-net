using MapsterMapper;
using PizzaApp.DataAccess.Repositories.Abstractions;
using PizzaApp.Domain.Entities;
using PizzaApp.Dtos.Pizzas;
using PizzaApp.Services.Abstractions;
using PizzaApp.Shared.Exceptions;

namespace PizzaApp.Services.Implementations;

public class PizzaService : IPizzaService
{
    private readonly IPizzaRepository _pizzaRepository;
    private readonly IMapper _mapper;
    private readonly ICurrentUser _currentUser;

    public PizzaService(
        IPizzaRepository pizzaRepository,
        IMapper mapper,
        ICurrentUser currentUser)
    {
        _pizzaRepository = pizzaRepository;
        _mapper = mapper;
        _currentUser = currentUser;
    }

    public async Task<List<PizzaDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        // Customers see their own saved pizzas, Admins see everybody's
        string? userId = _currentUser.IsAdmin ? null : _currentUser.Id;

        List<Pizza> pizzas = await _pizzaRepository.GetSavedPizzasAsync(userId, cancellationToken);

        List<PizzaDto> result = _mapper.Map<List<PizzaDto>>(pizzas);

        return result;
    }
    
    public async Task<PizzaDto> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var pizza = await FindPizzaAsync(id, cancellationToken);

        // TODO: Make this check reusable for anywhere we need to check if the user is allowed to access a resource (TIP: extension methods)
        if (!_currentUser.IsAdmin && _currentUser.Id != pizza.UserId)
        {
            throw new ForbiddenException();
        }

        return _mapper.Map<PizzaDto>(pizza);
    }

    // TODO: Implement the CreateAsync, UpdateAsync, and DeleteAsync methods
    public Task<PizzaDto> CreateAsync(AddPizzaDto request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task<PizzaDto> UpdateAsync(int id, UpdatePizzaDto request, CancellationToken cancellationToken = default) => throw new NotImplementedException();
    public Task DeleteAsync(int id, CancellationToken cancellationToken = default) => throw new NotImplementedException();

    private async Task<Pizza> FindPizzaAsync(int id, CancellationToken cancellationToken)
    {
        return await _pizzaRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new NotFoundException(nameof(Pizza), id);
    }
}
