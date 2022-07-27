namespace Auto3D.Web.Areas.Administration.Controllers
{
    using System.Threading.Tasks;

    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels.Inquiries;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;

    public class InquiriesController : AdministrationController
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

        public async Task<IActionResult> All()
        {
            var allViewModel = await this.inquiriesService.GetAllInquiriesAsync();

            return this.View(allViewModel);
        }

        public async Task<IActionResult> Details(int id)
        {
            string imagePath = this.GetImagePath();

            var order = await this.inquiriesService.GetDetailsViewModelByIdAsync(id, imagePath);

            return this.View(order);
        }

        [Authorize]
        public async Task<IActionResult> Delete(int id)
        {
            var inquiryToDelete = await this.inquiriesService.GetDeleteViewModelByIdAsync(id);
            return this.View(inquiryToDelete);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(InquiryDeleteViewModel inquiryDeleteViewModel)
        {
            string imagePath = this.GetImagePath();

            var id = inquiryDeleteViewModel.Id;
            await this.inquiriesService.DeleteByIdAsync(id, imagePath);
            return this.RedirectToAction(nameof(this.All));
        }

        private string GetImagePath()
        {
            return $"{this.environment.WebRootPath}/inquiriesPictures";
        }
    }
}
