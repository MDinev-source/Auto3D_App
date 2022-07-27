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
        Task<Product> CreatAsync(ProductCreateinputModel input, string imagePath);

        Task<IEnumerable<ProductViewModel>> GetAllProductsByCarIdAsync(int carId);

        Task<IEnumerable<ProductViewModel>> ReviewAllProductsAsync();

        Task<IEnumerable<ProductViewModel>> ReviewLastAddedProductsAsync();

        IEnumerable<ProductViewModel> GetProductsFromSearch(string searchString, int carId);

        IEnumerable<ProductViewModel> SortBy(ProductViewModel[] cars, ProductsSorter sorter);

        Task<ProductEditViewModel> GetEditViewModelByIdAsync(int id);

        Task<int> EditAsync(ProductEditViewModel productEditViewModel);

        Task<ProductDetailsViewModel> GetDetailsViewModelByIdAsync(int id, string imagePath);

        Task<ProductDeleteViewModel> GetDeleteViewModelByIdAsync(int id);

        Task<int> DeleteByIdAsync(int id, string imagePath);

        Task<Product> GetProductByIdAsync(int id);

        Product GetProductById(int id);
    }
}
