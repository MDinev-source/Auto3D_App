namespace Auto3D.Web.ViewModels.Orders
{
    using System.Collections.Generic;

    using Auto3D.Web.ViewModels.Products;

    public class OrderDetailsViewModel : OrderViewModel
    {
        public int Id { get; set; }

        public decimal OrderTotalPrice { get; set; }

        public IEnumerable<OrderProductViewModel> Products { get; set; }
    }
}
