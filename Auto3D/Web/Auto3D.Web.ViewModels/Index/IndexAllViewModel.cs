namespace Auto3D.Web.ViewModels.Index
{
    using System.Collections.Generic;

    using Auto3D.Web.ViewModels.CarBrands;
    using Auto3D.Web.ViewModels.PartnerLogos;
    using Auto3D.Web.ViewModels.Products;

    public class IndexAllViewModel
    {
        public IEnumerable<CarBrandViewModel> Brands { get; set; }

        public IEnumerable<ProductViewModel> Products { get; set; }

        public IEnumerable<PartnerLogoViewModel> Logos { get; set; }
    }
}
