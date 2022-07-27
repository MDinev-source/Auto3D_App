namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;

    using Auto3D.Web.ViewModels._AdministratorInputModels.Cars;
    using Auto3D.Web.ViewModels.Cars;
    using Auto3D.Web.ViewModels.Enums;

    public interface ICarsService
    {
        Task<Car> CreateAsync(CarCreateInputModel input);

        Task<IEnumerable<CarViewModel>> GetAllCarsByBrandIdAsync(int carBrandId);

        IEnumerable<CarViewModel> GetProductsFromSearch(string searchString, int brandId);

        IEnumerable<CarViewModel> SortBy(CarViewModel[] cars, CarsSorter sorter);

        Task<CarEditViewModel> GetEditViewModelByIdAsync(int id);

        Task<int> EditAsync(CarEditViewModel carEditViewModel);

        Task<CarDeleteViewModel> GetDeleteViewModelByIdAsync(int id);

        Task<int> DeleteByIdAsync(int id);

        Task<Car> GetCarAsync(int id);

        Car GetCar(int id);
    }
}
