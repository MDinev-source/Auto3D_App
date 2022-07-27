namespace Auto3D.Web.ViewModels.Orders
{
    public class OrderProductViewModel
    {
        public string CarBrand { get; set; }

        public string CarModel { get; set; }

        public string CarYear { get; set; }

        public string ProductName { get; set; }

        public string ProductPictureUrl { get; set; }

        public string ProductPrice { get; set; }

        public string ProductTotalPrice { get; set; }

        public int Quantity { get; set; }

        public decimal TotalProductPrice { get; set; }
    }
}
