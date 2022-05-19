namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;
    using Auto3D.Web.ViewModels._AdministratorInputModels.PartnerLogos;
    using Auto3D.Web.ViewModels.PartnerLogos;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class PartnerLogosController : AdministrationController
    {
        private readonly IPartnerLogosService partnerLogosService;
        private readonly IWebHostEnvironment environment;

        public PartnerLogosController(
            IPartnerLogosService partnerLogosService,
            IWebHostEnvironment environment)
        {
            this.partnerLogosService = partnerLogosService;
            this.environment = environment;
        }

        public IActionResult Create()
        {
            var model = new PartnerLogoInputModel();
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(PartnerLogoInputModel input)
        {
            string imagePath = this.GetImagePath();

            await this.partnerLogosService.CreateAsync(input, imagePath);

            return this.RedirectToAction(nameof(this.View));
        }

        public async Task<IActionResult> All()
        {
            string imagePath = this.GetImagePath();

            var logosAllViewModel = await this.partnerLogosService.GetAllLogosAsync(imagePath);

            return this.View(logosAllViewModel);
        }

        [Authorize]
        public async Task<IActionResult> Delete(string id)
        {
            string imagePath = this.GetImagePath();

            var logoToDelete = await this.partnerLogosService.GetDeleteViewModelByIdAsync(id, imagePath);
            return this.View(logoToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(PartnerLogoViewModel partnerLogoViewModel)
        {
            string imagePath = this.GetImagePath();

            var id = partnerLogoViewModel.Id;
            await this.partnerLogosService.DeleteByIdAsync(id, imagePath);
            return this.RedirectToAction("Index");
        }

        private string GetImagePath()
        {
            return $"{this.environment.WebRootPath}/partnerLogos";
        }
    }
}
