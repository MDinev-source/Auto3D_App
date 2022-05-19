namespace Auto3D.Web.Controllers
{
    using System.Diagnostics;
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels;
    using Auto3D.Web.ViewModels.CarBrands;
    using Auto3D.Web.ViewModels.Index;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class HomeController : BaseController
    {
        private readonly ICarBrandsService carBrandsService;
        private readonly IProductsService productsService;
        private readonly IPartnerLogosService partnerLogosService;
        private readonly IWebHostEnvironment environment;

        public HomeController(
            ICarBrandsService carBrandsService,
            IProductsService productsService,
            IPartnerLogosService partnerLogosService,
            IWebHostEnvironment environment)
        {
            this.carBrandsService = carBrandsService;
            this.productsService = productsService;
            this.partnerLogosService = partnerLogosService;
            this.environment = environment;
        }

        public async Task<IActionResult> Index(IndexAllViewModel allIndexElementsViewModel)
        {
            var imagePath = $"{this.environment.WebRootPath}/partnerLogos";

            var allCarBrands = await this.carBrandsService.GetAllCarBrandsAsync();
            var reviewProducts = await this.productsService.ReviewProductsAsync();
            var partnerLogos = await this.partnerLogosService.GetAllLogosAsync(imagePath);

            allIndexElementsViewModel.Brands = allCarBrands;
            allIndexElementsViewModel.Products = reviewProducts;
            allIndexElementsViewModel.Logos = partnerLogos;

            return this.View(allIndexElementsViewModel);
        }

        public IActionResult Privacy()
        {
            return this.View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return this.View(
                new ErrorViewModel { RequestId = Activity.Current?.Id ?? this.HttpContext.TraceIdentifier });
        }
    }
}
