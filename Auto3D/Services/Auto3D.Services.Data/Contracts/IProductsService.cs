namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels._AdministratorInputModels.Products;
    using Auto3D.Web.ViewModels.Enums;
    using Auto3D.Web.ViewModels.Products;

    public interface IProductsService
    {
        Task<Product> CreatAsync(ProductCreateinputModel input);

        Task<IEnumerable<ProductViewModel>> GetAllProductsByCarIdAsync(int carId);

        Task<IEnumerable<ProductViewModel>> ReviewProductsAsync();

        IEnumerable<ProductViewModel> SortBy(ProductViewModel[] cars, ProductsSorter sorter);

        Task<ProductEditViewModel> GetEditViewModelByIdAsync(int id);

        Task EditAsync(ProductEditViewModel productEditViewModel);

        Task<ProductDetailsViewModel> GetDetailsViewModelByIdAsync(int id);

        Task<ProductDeleteViewModel> GetDeleteViewModelByIdAsync(int id);

        Task DeleteByIdAsync(int id);

        Task<Product> GetProductByIdAsync(int id);

        Product GetProductById(int id);
    }
}
