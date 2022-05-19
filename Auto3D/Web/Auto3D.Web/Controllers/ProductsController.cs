namespace Auto3D.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Mvc;
    using X.PagedList;

    public class ProductsController : BaseController
    {
        private readonly IProductsService productsService;

        public ProductsController(
            IProductsService productsService)
        {
            this.productsService = productsService;
        }

        public async Task<IActionResult> All(ProductsAllViewModel productsAllViewModel, int carId)
        {
            var products = await this.productsService.GetAllProductsByCarIdAsync(carId);

            products = this.productsService.SortBy(products.ToArray(), productsAllViewModel.Sorter).ToList();

            var pageNumber = productsAllViewModel.PageNumber ?? 1;
            var pageSize = productsAllViewModel.PageSize ?? 6;
            var pageProductsAllViewModel = products.ToPagedList(pageNumber, pageSize);

            productsAllViewModel.Products = pageProductsAllViewModel;

            //carsAllViewModel.DistrictName = this.districtsService.GetDistrict(carsAllViewModel.DistrictId).Name;

            return this.View(productsAllViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            var product = await this.productsService.GetDetailsViewModelByIdAsync(id);

            return this.View(product);
        }
    }
}
