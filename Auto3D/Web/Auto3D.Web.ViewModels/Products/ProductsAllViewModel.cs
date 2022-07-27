namespace Auto3D.Web.ViewModels.Products
{
    using System.Collections.Generic;

    using Auto3D.Web.ViewModels.Enums;

    public class ProductsAllViewModel
    {
        public int CarId { get; set; }

        public string SearchString { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public ProductsSorter Sorter { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }
    }
}
