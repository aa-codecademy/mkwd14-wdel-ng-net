using System.ComponentModel.DataAnnotations;

namespace PizzaApp.Dtos.Orders;

/// <summary>
/// Changes the delivery details of an order that is still Pending.
/// </summary>
public class UpdateOrderDto
{
    [Required]
    [StringLength(200)]
    public string AddressTo { get; set; } = string.Empty;

    [StringLength(500)]
    public string? Description { get; set; }
}
