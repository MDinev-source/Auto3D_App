namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels.Inquiries;

    public interface IInquiriesService
    {
        Task<Inquiry> CreateAsync(InquiryCreateInputModel input, string imagePath);

        Task<IEnumerable<InquiriesAllViewModel>> GetAllInquiriesAsync();

        Task<InquiryDetailsViewModel> GetDetailsViewModelByIdAsync(int id, string imagePath);

        Task<InquiryDeleteViewModel> GetDeleteViewModelByIdAsync(int id);

        Task DeleteByIdAsync(int id, string imagePath);
    }
}
