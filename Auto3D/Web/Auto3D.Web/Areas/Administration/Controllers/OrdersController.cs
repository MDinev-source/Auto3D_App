namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels.Orders;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class OrdersController : AdministrationController
    {
        private readonly IOrdersService ordersService;

        public OrdersController(
            IOrdersService ordersService)
        {
            this.ordersService = ordersService;
        }

        public async Task<IActionResult> All()
        {
            var allViewModel = await this.ordersService.GetAllOrdersAsync();

            return this.View(allViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await this.ordersService.GetDetailsViewModelByIdAsync(id);

            return this.View(order);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var orderToDelete = await this.ordersService.GetDeleteViewModelByIdAsync(id);
            return this.View(orderToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(OrderDeleteViewModel orderDeleteViewModel)
        {
            var id = orderDeleteViewModel.Id;
            await this.ordersService.DeleteByIdAsync(id);
            return this.RedirectToAction(nameof(this.All));
        }
    }
}
