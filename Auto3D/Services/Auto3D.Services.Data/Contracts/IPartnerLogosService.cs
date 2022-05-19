namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels._AdministratorInputModels.PartnerLogos;
    using Auto3D.Web.ViewModels.PartnerLogos;

    public interface IPartnerLogosService
    {
        Task<Logo> CreateAsync(PartnerLogoInputModel input, string imagePath);

        Task<IEnumerable<PartnerLogoViewModel>> GetAllLogosAsync(string imagePath);

        Task<PartnerLogoViewModel> GetDeleteViewModelByIdAsync(string id, string imagePath);

        Task DeleteByIdAsync(string id, string imagePath);
    }
}
