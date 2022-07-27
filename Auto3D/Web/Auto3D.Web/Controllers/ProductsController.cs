namespace Auto3D.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using X.PagedList;

    public class ProductsController : BaseController
    {
        private readonly IProductsService productsService;
        private readonly IWebHostEnvironment environment;

        public ProductsController(
            IProductsService productsService,
            IWebHostEnvironment environment)
        {
            this.productsService = productsService;
            this.environment = environment;
        }

        public async Task<IActionResult> All(ProductsAllViewModel productsAllViewModel, int carId)
        {
            var products = await this.productsService.GetAllProductsByCarIdAsync(carId);

            if (productsAllViewModel.SearchString != null)
            {
                products = this.productsService.GetProductsFromSearch(productsAllViewModel.SearchString, productsAllViewModel.CarId).ToList();
            }

            products = this.productsService.SortBy(products.ToArray(), productsAllViewModel.Sorter).ToList();

            var pageNumber = productsAllViewModel.PageNumber ?? 1;
            var pageSize = productsAllViewModel.PageSize ?? 6;
            var pageProductsAllViewModel = products.ToPagedList(pageNumber, pageSize);

            productsAllViewModel.Products = pageProductsAllViewModel;

            //carsAllViewModel.DistrictName = this.districtsService.GetDistrict(carsAllViewModel.DistrictId).Name;

            return this.View(productsAllViewModel);
        }

        public async Task<IActionResult> ReviewAll(ProductsAllViewModel productsAllViewModel)
        {
            var products = await this.productsService.ReviewAllProductsAsync();

            if (productsAllViewModel.SearchString != null)
            {
                products = this.productsService.GetProductsFromSearch(productsAllViewModel.SearchString, 0).ToList();
            }

            products = this.productsService.SortBy(products.ToArray(), productsAllViewModel.Sorter).ToList();

            var pageNumber = productsAllViewModel.PageNumber ?? 1;
            var pageSize = productsAllViewModel.PageSize ?? 6;
            var pageProductsAllViewModel = products.ToPagedList(pageNumber, pageSize);

            productsAllViewModel.Products = pageProductsAllViewModel;

            return this.View(productsAllViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            string imagePath = $"{this.environment.WebRootPath}/productsPictures";

            var product = await this.productsService.GetDetailsViewModelByIdAsync(id, imagePath);

            return this.View(product);
        }
    }
}
