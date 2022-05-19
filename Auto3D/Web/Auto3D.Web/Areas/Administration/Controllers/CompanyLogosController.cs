namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;
    using Auto3D.Web.ViewModels._AdministratorInputModels.CompanyLogos;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class CompanyLogosController : AdministrationController
    {
        private readonly ICompanyLogosService companyLogosServices;
        private readonly IWebHostEnvironment environment;

        public CompanyLogosController(
            ICompanyLogosService companyLogosServices,
            IWebHostEnvironment environment)
        {
            this.companyLogosServices = companyLogosServices;
            this.environment = environment;
        }

        public IActionResult Create()
        {
            var model = new CompanyLogoInputModel();
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(CompanyLogoInputModel input)
        {
            string imagePath = this.GetImagePath();

            await this.companyLogosServices.CreateAsync(input, imagePath);

            return this.RedirectToAction(nameof(this.View));
        }

        private string GetImagePath()
        {
            return $"{this.environment.WebRootPath}/companyLogos";
        }
    }
}
