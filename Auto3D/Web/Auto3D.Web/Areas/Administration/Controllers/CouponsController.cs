namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;
    using Auto3D.Web.ViewModels._AdministratorInputModels.Coupons;
    using Auto3D.Web.ViewModels.Coupons;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    public class CouponsController : AdministrationController
    {
        private readonly ICouponsService couponsService;

        public CouponsController(
            ICouponsService couponsService)
        {
            this.couponsService = couponsService;
        }

        public IActionResult Create()
        {
            var model = new CouponCreateInputModel();
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(CouponCreateInputModel createInputModel)
        {
            await this.couponsService.CreatAsync(createInputModel);
            return this.RedirectToAction(nameof(this.All));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var couponToEdit = await this.couponsService.GetEditViewModelByIdAsync(id);

            return this.View(couponToEdit);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Edit(CouponEditViewModel couponEditViewModel)
        {
            await this.couponsService.EditAsync(couponEditViewModel);

            return this.RedirectToAction(nameof(this.All));
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var couponToDelete = await this.couponsService.GetDeleteViewModelByIdAsync(id);
            return this.View(couponToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(CouponDeleteViewModel couponDeleteViewModel)
        {
            var id = couponDeleteViewModel.Id;
            await this.couponsService.DeleteByIdAsync(id);
            return this.RedirectToAction(nameof(this.All));
        }

        public async Task<IActionResult> All()
        {
            var allViewModel = await this.couponsService.GetAllCouponsAsync();

            return this.View(allViewModel);
        }
    }
}
