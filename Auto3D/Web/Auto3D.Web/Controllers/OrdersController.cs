namespace Auto3D.Web.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;
    using Auto3D.Web.ViewModels.Orders;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;

    public class OrdersController : BaseController
    {
        private readonly IOrdersService ordersService;
        private readonly UserManager<ApplicationUser> userManager;

        public OrdersController(
            IOrdersService ordersService,
            UserManager<ApplicationUser> userManager)
        {
            this.ordersService = ordersService;
            this.userManager = userManager;
        }

        public IActionResult Create()
        {
            var model = new OrderCreateInputModel();
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(OrderCreateInputModel createInputModel)
        {
            var user = await this.userManager.GetUserAsync(this.User);

            await this.ordersService.CreateAsync(createInputModel, user);
            return this.RedirectToAction("Index");
        }
    }
}
