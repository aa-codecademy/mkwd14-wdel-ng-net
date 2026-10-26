using PizzaApp.Domain.Enums;

namespace PizzaApp.Services.Rules.Order;

public static class OrderStatusRules
{
    public static bool CanChange(OrderStatus currentStatus, OrderStatus newStatus, bool isAdmin)
    {
        if (!isAdmin)
        {
            return currentStatus == OrderStatus.Pending && newStatus == OrderStatus.Cancelled;
        }

        //Tuple<OrderStatus, OrderStatus> tuple = new(currentStatus, newStatus);

        return (currentStatus, newStatus) switch
        {
            (OrderStatus.Pending, OrderStatus.Preparing) => true,
            (OrderStatus.Preparing, OrderStatus.Delivered) => true,
            (OrderStatus.Preparing or OrderStatus.Pending, OrderStatus.Cancelled) => true,
            (OrderStatus.Delivered, OrderStatus.Returned) => true,
            _ => false
        };
    }
}
