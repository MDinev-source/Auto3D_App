namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;
    using Auto3D.Web.ViewModels._AdministratorInputModels.Products;
    using Auto3D.Web.ViewModels.Products;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class ProductsController : AdministrationController
    {
        private readonly IProductsService productsService;

        public ProductsController(
            IProductsService productsService)
        {
            this.productsService = productsService;
        }

        public IActionResult Create(int carId)
        {
            var model = new ProductCreateinputModel();
            model.CarId = carId;
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(ProductCreateinputModel createInputModel)
        {
            var fileType = createInputModel.PictureUrl.ContentType.Split('/')[1];
            if (!this.IsImageTypeValid(fileType))
            {
                return this.View(createInputModel);
            }

            await this.productsService.CreatAsync(createInputModel);
            return this.RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(int id)
        {
            var productToEdit = await this.productsService.GetEditViewModelByIdAsync(id);

            return this.View(productToEdit);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Edit(ProductEditViewModel productEditViewModel)
        {
            if (productEditViewModel.Picture != null)
            {
                var fileType = productEditViewModel.Picture.ContentType.Split('/')[1];
                if (!this.IsImageTypeValid(fileType))
                {
                    return this.View(productEditViewModel);
                }
            }

            await this.productsService.EditAsync(productEditViewModel);

            return this.RedirectToAction("Index");
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var productToDelete = await this.productsService.GetDeleteViewModelByIdAsync(id);
            return this.View(productToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(ProductDeleteViewModel productDeleteViewModel)
        {
            var id = productDeleteViewModel.Id;
            await this.productsService.DeleteByIdAsync(id);
            return this.RedirectToAction("Index");
        }
    }
}
