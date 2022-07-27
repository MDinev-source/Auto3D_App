namespace Auto3D.Web.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Common;
    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Common;
    using Auto3D.Web.Helpers;
    using Auto3D.Web.ViewModels.ShoppingCart;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ShoppingCartController : BaseController
    {
        private readonly IShoppingCartsService shoppingCartService;

        public ShoppingCartController(IShoppingCartsService shoppingCartService)
        {
            this.shoppingCartService = shoppingCartService;
        }

        public async Task<IActionResult> Index(ShoppingCartProductsAllViewModel shoppingCartProductsAllViewModel)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                if (shoppingCartProductsAllViewModel.Coupon != null)
                {
                    await this.shoppingCartService.AddCouponToShoppingCart(username, shoppingCartProductsAllViewModel.Coupon);
                }

                var shoppingCart = await this.shoppingCartService.GetAllShoppingCartProductsAsync(username);
                shoppingCartProductsAllViewModel.Products = shoppingCart;
                return this.View(shoppingCartProductsAllViewModel);
            }

            var cart = this.GetShoppingCartFromSession();
            shoppingCartProductsAllViewModel.Products = cart;

            return this.View(shoppingCartProductsAllViewModel);
        }

        public async Task<IActionResult> Add(int id, int quantity)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                await this.shoppingCartService.AddProductToShoppingCartAsync(id, username, 1);
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                var shoppingCartProduct = await this.shoppingCartService.GetGuestShoppingCartProductToAdd(id, quantity);

                var cartAsList = cart.ToList();
                cartAsList.Add(shoppingCartProduct);
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cartAsList);
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id, int newQuantity)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                await this.shoppingCartService.EditShoppingCartProductAsync(id, username, newQuantity);
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                cart = this.shoppingCartService.EditGuestShoppingCartProduct(id, cart.ToArray(), newQuantity).ToArray();
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cart);
            }

            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(int id)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                await this.shoppingCartService.DeleteProductFromShoppingCart(id, username);
            }
            else
            {
                var cart = this.GetShoppingCartFromSession();
                cart = this.shoppingCartService.DeleteProductFromGuestShoppingCart(id, cart.ToArray()).ToArray();
                this.HttpContext.Session.SetObjectAsJson(WebConstants.ShoppingCartSessionKey, cart);
            }

            return this.RedirectToAction("Index");
        }

        private IEnumerable<ShoppingCartProductViewModel> GetShoppingCartFromSession()
        {
            return this.HttpContext.Session
                   .GetObjectFromJson<ShoppingCartProductViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                   new List<ShoppingCartProductViewModel>().ToArray();
        }
    }
}
