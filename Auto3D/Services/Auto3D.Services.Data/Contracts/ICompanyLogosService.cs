namespace Auto3D.Services.Data.Contracts
{
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels._AdministratorInputModels.CompanyLogos;

    public interface ICompanyLogosService
    {
        Task<CompanyLogo> CreateAsync(CompanyLogoInputModel input, string imagePath);

        public string GetCurrentLogoPath();
    }
}
