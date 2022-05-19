namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels._AdministratorInputModels.CarBrands;
    using Auto3D.Web.ViewModels.CarBrands;

    public interface ICarBrandsService
    {
        Task<CarBrand> CreateAsync(CarBrandCreateInputModel input);

        Task<IEnumerable<CarBrandViewModel>> GetAllCarBrandsAsync();

        Task<CarBrandEditViewModel> GetEditViewModelByIdAsync(int id);

        Task EditAsync(CarBrandEditViewModel carBrandEditViewModel);

        Task<CarBrandDeleteViewModel> GetDeleteViewModelByIdAsync(int id);

        Task DeleteByIdAsync(int id);

        Task<CarBrand> GetBrandAsync(int id);
    }
}
