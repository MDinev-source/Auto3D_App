namespace Auto3D.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Data.Common.Repositories;
    using Auto3D.Data.Models;
    using Auto3D.Services.Data.Common;
    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels.ShoppingCart;
    using Microsoft.EntityFrameworkCore;

    public class ShoppingCartsService : IShoppingCartsService
    {
        private readonly IRepository<ApplicationUser> usersRepository;
        //private readonly IRepository<Product> productsRepository;
        private readonly IRepository<ShoppingCartProduct> shoppingCartProductsRepository;
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;
        private readonly ICouponsService couponsService;
        private readonly IProductsService productsService;

        public ShoppingCartsService(
            IRepository<ApplicationUser> usersRepository,
            //IRepository<Product> productsRepository,
            IRepository<ShoppingCartProduct> shoppingCartProductsRepository,
            IRepository<ShoppingCart> shoppingCartsRepository,
            ICouponsService couponsService,
            IProductsService productsService)
        {
            this.usersRepository = usersRepository;
            //this.productsRepository = productsRepository;
            this.shoppingCartProductsRepository = shoppingCartProductsRepository;
            this.shoppingCartsRepository = shoppingCartsRepository;
            this.couponsService = couponsService;
            this.productsService = productsService;
        }

        public async Task AddProductToShoppingCartAsync(int productId, string username, int quantity)
        {
            //var product = await this.productsRepository
            //     .All()
            //     .Where(p => p.Id == productId)
            //     .FirstOrDefaultAsync();

            var product = await this.productsService.GetProductByIdAsync(productId);

            if (product == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceProductId, productId));
            }

            var user = await this.usersRepository
                .All()
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            var shoppingCartProduct = new ShoppingCartProduct
            {
                ProductId = product.Id,
                ShoppingCartId = user.ShoppingCartId,
                Quantity = quantity,
            };

            await this.CalculateTotalPrice(quantity, shoppingCartProduct, user, product);

            this.shoppingCartProductsRepository.Add(shoppingCartProduct);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public async Task AddCouponToShoppingCart(string userName, string couponName)
        {
            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == userName);

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, userName));
            }

            decimal discount = 0;

            var coupon = this.couponsService.GetCouponByName(couponName);

            if (coupon != null)
            {
                discount = this.couponsService.CalculateCouponDiscount(coupon);
            }
            else
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCoupon, coupon.Name));
            }

            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .Where(s => s.Id == user.ShoppingCartId)
            .FirstOrDefaultAsync();

            if (shoppingCart == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartId, user.ShoppingCartId));
            }

            if (shoppingCart.CouponId == null)
            {
                shoppingCart.CouponId = coupon.Id;
                this.shoppingCartsRepository.Update(shoppingCart);
                await this.shoppingCartsRepository.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException(string.Format(ServicesDataConstants.CouponAlreadyAdded, shoppingCart.Id));
            }

            var shoppingCartProducts = await this.shoppingCartProductsRepository
                .All()
                .Where(p => p.ShoppingCartId == shoppingCart.Id)
                .ToArrayAsync();

            if (shoppingCartProducts.Length == 0)
            {
                throw new InvalidOperationException(string.Format(ServicesDataConstants.ShoppingCartNoProducts, shoppingCart.Id));
            }

            foreach (var cartProduct in shoppingCartProducts)
            {
                var currentProduct = await this.productsService.GetProductByIdAsync(cartProduct.ProductId);

                if (currentProduct.Discount == 0)
                {
                    cartProduct.TotalPrice = cartProduct.TotalPrice * discount;
                    this.shoppingCartProductsRepository.Update(cartProduct);
                    await this.shoppingCartProductsRepository.SaveChangesAsync();
                }
            }
        }

        public async Task AssignShoppingCartsUserId(ApplicationUser user)
        {
            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .Where(s => s.User.Id == user.Id)
             .FirstOrDefaultAsync();
            if (shoppingCart == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartForUser, user.Id, user.UserName));
            }

            shoppingCart.UserId = user.Id;
            this.shoppingCartsRepository.Update(shoppingCart);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public async Task ClearShoppingCart(string username)
        {
            var user = await this.usersRepository
            .All()
            .Where(u => u.UserName == username)
            .FirstOrDefaultAsync();

            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            var shoppingCartProducts = this.shoppingCartProductsRepository
                .All()
                .Where(s => s.ShoppingCart.User == user)
                .ToList();

            shoppingCartProducts.ForEach(s => s.IsDeleted = true);

            this.shoppingCartProductsRepository.UpdateRange(shoppingCartProducts);
            await this.shoppingCartProductsRepository.SaveChangesAsync();

            var shoppingCart = await this.shoppingCartsRepository
            .All()
            .Where(s => s.User.Id == user.Id)
            .FirstOrDefaultAsync();

            shoppingCart.CouponId = null;
            this.shoppingCartsRepository.Update(shoppingCart);
            await this.shoppingCartsRepository.SaveChangesAsync();
        }

        public IEnumerable<ShoppingCartProductViewModel> DeleteProductFromGuestShoppingCart(int productId, ShoppingCartProductViewModel[] cart)
        {
            var shoppingCartProductViewModel = cart.FirstOrDefault(s => s.ProductId == productId);

            if (shoppingCartProductViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceGuestShoppingProductId, productId));
            }

            var product = this.productsService.GetProductById(productId);

            if (product == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceProductId, productId));
            }

            var cartAsList = cart.ToList();
            cartAsList.Remove(shoppingCartProductViewModel);
            return cartAsList;
        }

        public async Task DeleteProductFromShoppingCart(int shoppingCartProductId, string username)
        {
            var shoppingCartProduct = await this.shoppingCartProductsRepository
                .All()
                .Where(s => s.Id == shoppingCartProductId)
               .FirstOrDefaultAsync();

            if (shoppingCartProduct == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartProductId, shoppingCartProductId));
            }

            var user = await this.usersRepository
                        .AllAsNoTracking()
                        .Where(u => u.UserName == username)
                        .FirstOrDefaultAsync();
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            this.shoppingCartProductsRepository.Delete(shoppingCartProduct);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public IEnumerable<ShoppingCartProductViewModel> EditGuestShoppingCartProduct(int shoppingCartProductId, ShoppingCartProductViewModel[] cart, int newQuantity)
        {
            if (newQuantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            var shoppingCartProductViewModel = cart
                .Where(c => c.ProductId == shoppingCartProductId)
                .FirstOrDefault();

            if (shoppingCartProductViewModel == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceGuestShoppingProductId, shoppingCartProductId));
            }

            var product = this.productsService.GetProductById(shoppingCartProductId);

            if (product == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceProductId, shoppingCartProductId));
            }

            shoppingCartProductViewModel.Quantity = newQuantity;

            for (var i = 0; i < cart.Length; i++)
            {
                if (cart[i].Id == shoppingCartProductId)
                {
                    cart[i] = shoppingCartProductViewModel;
                }
            }

            return cart;
        }

        public async Task EditShoppingCartProductAsync(int shoppingCartProductId, string username, int newQuantity)
        {
            var shoppingCartProduct = await this.shoppingCartProductsRepository
                .All()
                .Where(s => s.Id == shoppingCartProductId)
            .FirstOrDefaultAsync();

            if (shoppingCartProduct == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartProductId, shoppingCartProductId));
            }

            var user = await this.usersRepository.All().FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceUsername, username));
            }

            if (newQuantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            var product = await this.productsService.GetProductByIdAsync(shoppingCartProduct.ProductId);

            if (product == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceProductId, shoppingCartProduct.ProductId));
            }

            await this.CalculateTotalPrice(newQuantity, shoppingCartProduct, user, product);

            shoppingCartProduct.Quantity = newQuantity;
            this.shoppingCartProductsRepository.Update(shoppingCartProduct);
            await this.shoppingCartProductsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<ShoppingCartProductViewModel>> GetAllShoppingCartProductsAsync(string username)
        {
            var user = await this.usersRepository
                .All()
                .Where(u => u.UserName == username)
                .FirstOrDefaultAsync();

            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .Where(s => s.UserId == user.Id)
                .FirstOrDefaultAsync();

            decimal discount = 0;

            if (shoppingCart.CouponId != null)
            {
                var coupon = this.couponsService.GetCouponById(shoppingCart.CouponId);

                discount = this.couponsService.CalculateCouponDiscount(coupon);
            }

            var shoppingCartProducts = await this.shoppingCartProductsRepository
                .All()
                .Where(s => s.ShoppingCart.User.UserName == username)
                .Select(sp => new ShoppingCartProductViewModel
                {
                    Id = sp.Id,
                    ShoppingCartUserId = user.Id,
                    ProductId = sp.ProductId,
                    ProductName = sp.Product.Name,
                    CarBrand = sp.Product.Car.Brand.Brand,
                    CarModel = sp.Product.Car.Model,
                    CarModelYear = sp.Product.Car.Year,
                    ProductPrice = (sp.Product.Discount == 0 ? sp.Product.Price : sp.Product.Discount).ToString(),
                    ProductDiscount = sp.Product.Discount == 0
                                    && discount != 0
                                    ? (sp.Product.Price * discount).ToString("F2")
                                    : "No Discount",
                    Quantity = sp.Quantity,
                    ShoppingCartProductTotalPrice = sp.TotalPrice,
                })
                .ToArrayAsync();

            return shoppingCartProducts;
        }

        public async Task<ShoppingCartProductViewModel> GetGuestShoppingCartProductToAdd(int productId, int quantity)
        {
            var product = await this.productsService.GetProductByIdAsync(productId);

            if (product == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceProductId, productId));
            }

            if (quantity <= 0)
            {
                throw new InvalidOperationException(ServicesDataConstants.ZeroOrNegativeQuantity);
            }

            var shoppingCartProduct = new ShoppingCartProductViewModel
            {
                ProductId = product.Id,
                ProductName = product.Name,
                //CarBrand = "Super Car",
                //CarModel = "Super Car",
                //CarModelYear = "Super Car",
                ProductPrice = (product.Discount == 0 ? product.Price : product.Discount).ToString() + "test",
                Quantity = quantity,
            };
            return shoppingCartProduct;
        }

        //public async Task DeleteCouponsInShoppingCarts(int couponId)
        //{
        //    var carts = await this.shoppingCartsRepository
        //        .All()
        //        .Where(s => s.CouponId == couponId)
        //        .ToArrayAsync();

        //    foreach (var cart in carts)
        //    {
        //        cart.CouponId = null;
        //        this.shoppingCartsRepository.Update(cart);
        //        await this.shoppingCartsRepository.SaveChangesAsync();
        //    }
        //}

        public async Task<ShoppingCart> GetShoppingCartByUserId(string userId)
        {
            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .Where(sc => sc.UserId == userId)
                .FirstOrDefaultAsync();

            return shoppingCart;
        }

        public async Task<IEnumerable<ShoppingCartProduct>> GetAllShoppingCartProductsAsync(int id)
        {
            var shoppingCartProducts = await this.shoppingCartProductsRepository
                .AllAsNoTracking()
                .Where(x => x.ShoppingCartId == id)
                .ToArrayAsync();

            return shoppingCartProducts;
        }

        private async Task CalculateTotalPrice(int quantity, ShoppingCartProduct shoppingCartProduct, ApplicationUser user, Product product)
        {
            var shoppingCart = await this.shoppingCartsRepository
                .All()
                .Where(s => s.Id == user.ShoppingCartId)
                .FirstOrDefaultAsync();

            if (shoppingCart == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceShoppingCartId, user.ShoppingCartId));
            }

            //var product = await this.productsService.GetProductByIdAsync(shoppingCartProduct.ProductId);

            //if (product == null)
            //{
            //    throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceProductId, shoppingCartProduct.ProductId));
            //}

            if (shoppingCart.CouponId != null && product.Discount == 0)
            {
                var coupon = this.couponsService.GetCouponById(shoppingCart.CouponId);

                var discount = this.couponsService.CalculateCouponDiscount(coupon);

                shoppingCartProduct.TotalPrice = quantity * product.Price * discount;
            }
            else
            {
                if (product.Discount > 0)
                {
                    shoppingCartProduct.TotalPrice = quantity * product.Discount;
                }
                else
                {
                    shoppingCartProduct.TotalPrice = quantity * product.Price;
                }
            }
        }
    }
}
