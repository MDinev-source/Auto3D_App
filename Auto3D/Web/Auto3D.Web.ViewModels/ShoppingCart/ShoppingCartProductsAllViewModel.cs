namespace Auto3D.Web.ViewModels.ShoppingCart
{
    using System.Collections.Generic;

    public class ShoppingCartProductsAllViewModel
    {
        public string Coupon { get; set; }

        public IEnumerable<ShoppingCartProductViewModel> Products { get; set; }
    }
}
