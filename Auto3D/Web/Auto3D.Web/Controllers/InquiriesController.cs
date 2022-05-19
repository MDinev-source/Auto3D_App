namespace Auto3D.Web.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.Filters;
    using Auto3D.Web.ViewModels.Inquiries;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class InquiriesController : BaseController
    {
        private readonly IInquiriesService inquiriesService;
        private readonly IWebHostEnvironment environment;

        public InquiriesController(
            IInquiriesService inquiriesService,
            IWebHostEnvironment environment)
        {
            this.inquiriesService = inquiriesService;
            this.environment = environment;
        }

        public IActionResult Create()
        {
            var model = new InquiryCreateInputModel();
            return this.View(model);
        }

        [HttpPost]
        [ModelStateValidationActionFilter]
        public async Task<IActionResult> Create(InquiryCreateInputModel input)
        {
            var imagePath = $"{this.environment.WebRootPath}/inquiriesPictures";

            await this.inquiriesService.CreateAsync(input, imagePath);

            return this.RedirectToAction(nameof(this.View));
        }
    }
}
