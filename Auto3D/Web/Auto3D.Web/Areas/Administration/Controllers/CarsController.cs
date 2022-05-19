namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;

    using Auto3D.Web.ViewModels._AdministratorInputModels.Cars;
    using Auto3D.Web.ViewModels.Cars;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CarsController : AdministrationController
    {
        private readonly ICarsService carsService;

        public CarsController(
            ICarsService carsService)
        {
            this.carsService = carsService;
        }

        public IActionResult Create(int brandId)
        {
            var model = new CarCreateInputModel();
            model.BrandId = brandId;
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(CarCreateInputModel createInputModel)
        {
            var fileType = createInputModel.PictureUrl.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(createInputModel);
            }

            await this.carsService.CreateAsync(createInputModel);
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var carToEdit = await this.carsService.GetEditViewModelByIdAsync(id);

            return this.View(carToEdit);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Edit(CarEditViewModel carEditViewModel)
        {
            if (carEditViewModel.Picture != null)
            {
                var fileType = carEditViewModel.Picture.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(carEditViewModel);
                }
            }

            await this.carsService.EditAsync(carEditViewModel);

            return this.RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var carBrandToDelete = await this.carsService.GetDeleteViewModelByIdAsync(id);
            return this.View(carBrandToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CarDeleteViewModel carDeleteViewModel)
        {
            var id = carDeleteViewModel.Id;
            await this.carsService.DeleteByIdAsync(id);
            return this.RedirectToAction("Index");
        }
    }
}
