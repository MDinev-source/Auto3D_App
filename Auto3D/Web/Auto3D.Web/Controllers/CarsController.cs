namespace Auto3D.Web.Controllers
{
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels.Cars;
    using Microsoft.AspNetCore.Mvc;
    using X.PagedList;

    public class CarsController : BaseController
    {
        private readonly ICarsService carsService;

        public CarsController(
            ICarsService carsService)
        {
            this.carsService = carsService;
        }

        public async Task<IActionResult> All(CarsAllViewModel carsAllViewModel, int brandId)
        {
            var cars = await this.carsService.GetAllCarsByBrandIdAsync(brandId);

            cars = this.carsService.SortBy(cars.ToArray(), carsAllViewModel.Sorter).ToList();

            var pageNumber = carsAllViewModel.PageNumber ?? 1;
            var pageSize = carsAllViewModel.PageSize ?? 6;
            var pagecarsAllViewModel = cars.ToPagedList(pageNumber, pageSize);

            carsAllViewModel.Cars = pagecarsAllViewModel;

            //carsAllViewModel.DistrictName = this.districtsService.GetDistrict(carsAllViewModel.DistrictId).Name;

            return this.View(carsAllViewModel);
        }
    }
}
