namespace Auto3D.Web.ViewModels.Products
{
    using System.Collections.Generic;

    public class ProductDetailsViewModel
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public string CarBrand { get; set; }

        public string CarModel { get; set; }

        public string CarYear { get; set; }

        public string PictureUrl { get; set; }

        public int CarouselIndex { get; set; }

        public IEnumerable<ProductPicturesViewModel> Pictures { get; set; }
    }
}
