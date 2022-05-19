namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;
    using Auto3D.Web.ViewModels._AdministratorInputModels.CarBrands;
    using Auto3D.Web.ViewModels.CarBrands;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CarBrandsController : AdministrationController
    {
        private readonly ICarBrandsService carBrandsService;

        public CarBrandsController(
            ICarBrandsService carBrandsService)
        {
            this.carBrandsService = carBrandsService;
        }

        public IActionResult Create()
        {
            var model = new CarBrandCreateInputModel();
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(CarBrandCreateInputModel createInputModel)
        {
            var fileType = createInputModel.Logo.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(createInputModel);
            }

            await this.carBrandsService.CreateAsync(createInputModel);
            return this.RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var carBrandToDelete = await this.carBrandsService.GetDeleteViewModelByIdAsync(id);
            return this.View(carBrandToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CarBrandDeleteViewModel carBrandDeleteViewModel)
        {
            var id = carBrandDeleteViewModel.Id;
            await this.carBrandsService.DeleteByIdAsync(id);
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var carBrandToEdit = await this.carBrandsService.GetEditViewModelByIdAsync(id);

            return this.View(carBrandToEdit);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Edit(CarBrandEditViewModel carBrandEditViewModel)
        {
            if (carBrandEditViewModel.Logo != null)
            {
                var fileType = carBrandEditViewModel.Logo.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(carBrandEditViewModel);
                }
            }

            await this.carBrandsService.EditAsync(carBrandEditViewModel);

            return this.RedirectToAction("Index");
        }
    }
}
