namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels.Orders;

    public interface IOrdersService
    {
        Task<Order> CreateAsync(OrderCreateInputModel input, ApplicationUser user);

        Task<IEnumerable<OrdersAllViewModel>> GetAllOrdersAsync();

        Task<OrderDetailsViewModel> GetDetailsViewModelByIdAsync(int id);

        Task<OrderDeleteViewModel> GetDeleteViewModelByIdAsync(int id);

        Task DeleteByIdAsync(int id);
    }
}
