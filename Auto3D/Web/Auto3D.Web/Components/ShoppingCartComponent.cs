namespace Auto3D.Web.Components
{
    using System.Collections.Generic;

    using Auto3D.Common;
    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Common;
    using Auto3D.Web.Helpers;
    using Auto3D.Web.ViewModels.ShoppingCart;
    using Microsoft.AspNetCore.Mvc;

    public class ShoppingCartComponent : ViewComponent
    {
        private readonly IShoppingCartsService shoppingCartsService;

        public ShoppingCartComponent(IShoppingCartsService shoppingCartsService)
        {
            this.shoppingCartsService = shoppingCartsService;
        }

        public IViewComponentResult Invoke(ShoppingCartProductsAllViewModel shoppingCartProductsAllViewModel)
        {
            if (this.User.IsInRole(GlobalConstants.UserRoleName))
            {
                var username = this.User.Identity.Name;
                var shoppingCartProducts = this.shoppingCartsService.GetAllShoppingCartProductsAsync(username)
                    .GetAwaiter()
                    .GetResult();

                return this.View(shoppingCartProducts);
            }

            var cart = this.HttpContext.Session
                       .GetObjectFromJson<ShoppingCartProductViewModel[]>(WebConstants.ShoppingCartSessionKey) ??
                       new List<ShoppingCartProductViewModel>().ToArray();

            return this.View(cart);
        }
    }
}
