namespace Auto3D.Web.ViewModels.ShoppingCart
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class ShoppingCartProductViewModel
    {
        public int Id { get; set; }

        public string ShoppingCartUserId { get; set; }

        public int ProductId { get; set; }

        public string ProductPictureUrl { get; set; }

        public string ProductName { get; set; }

        public string CarBrand { get; set; }

        public string CarModel { get; set; }

        public string CarModelYear { get; set; }

        public string ProductPrice { get; set; }

        public string ProductDiscount { get; set; }

        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        public decimal? ShoppingCartProductTotalPrice { get; set; }
    }
}
