using PizzaApp.Domain.Enums;

namespace PizzaApp.Services.Rules.Order;

public static class OrderStatusRules
{
    public static bool CanChange(OrderStatus currentStatus, OrderStatus newStatus, bool isAdmin)
    {
        // TODO: Think of ways to improve this method. 

        if (!isAdmin)
        {
            return currentStatus == OrderStatus.Pending && newStatus == OrderStatus.Cancelled;
        }

        if (currentStatus == OrderStatus.Pending && newStatus == OrderStatus.Preparing)
        {
            return true;
        }

        if (currentStatus == OrderStatus.Preparing && newStatus == OrderStatus.Delivered)
        {
            return true;
        }

        // MISTAKE : You cannot cancel an order that has already been delivered
        //if (currentStatus == OrderStatus.Delivered && newStatus == OrderStatus.Cancelled)
        //{
        //    return true;
        //}

        if ((currentStatus == OrderStatus.Pending || currentStatus == OrderStatus.Preparing)
            && newStatus == OrderStatus.Cancelled)
        {
            return true;
        }

        return false;
    }
}
