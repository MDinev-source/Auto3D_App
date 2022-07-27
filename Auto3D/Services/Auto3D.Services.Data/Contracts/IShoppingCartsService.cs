namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels.ShoppingCart;

    public interface IShoppingCartsService
    {
        Task AssignShoppingCartsUserId(ApplicationUser user);

        Task<IEnumerable<ShoppingCartProductViewModel>> GetAllShoppingCartProductsAsync(string username);

        Task AddCouponToShoppingCart(string userName, string couponName);

        Task AddProductToShoppingCartAsync(int productId, string username, int quantity);

        Task<ShoppingCartProductViewModel> GetGuestShoppingCartProductToAdd(int productId, int quantity);

        Task EditShoppingCartProductAsync(int shoppingCartProductId, string username, int newQuantity);

        IEnumerable<ShoppingCartProductViewModel> EditGuestShoppingCartProduct(int shoppingCartProductId, ShoppingCartProductViewModel[] cart, int newQuantity);

        Task DeleteProductFromShoppingCart(int shoppingCartProductId, string username);

        IEnumerable<ShoppingCartProductViewModel> DeleteProductFromGuestShoppingCart(int productId, ShoppingCartProductViewModel[] cart);

        Task ClearShoppingCart(string username);

        Task<IEnumerable<ShoppingCartProduct>> GetAllShoppingCartProductsAsync(int id);

        Task<ShoppingCart> GetShoppingCartByUserId(string userId);
    }
}
