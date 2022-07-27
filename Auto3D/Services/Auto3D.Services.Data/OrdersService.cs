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
    using Auto3D.Web.ViewModels.Orders;
    using Microsoft.EntityFrameworkCore;

    public class OrdersService : IOrdersService
    {
        private readonly IDeletableEntityRepository<Order> ordersRepository;
        private readonly IDeletableEntityRepository<OrderProduct> orderProductsRepository;
        private readonly IShoppingCartsService shoppingCartsService;
        private readonly IProductsService productsService;
        private readonly ICarsService carsService;
        private readonly ICarBrandsService carBrandsService;
        private readonly ICouponsService couponsService;

        public OrdersService(
            IDeletableEntityRepository<Order> ordersRepository,
            IDeletableEntityRepository<OrderProduct> orderProductsRepository,
            IShoppingCartsService shoppingCartsService,
            IProductsService productsService,
            ICarsService carsService,
            ICarBrandsService carBrandsService,
            ICouponsService couponsService)
        {
            this.ordersRepository = ordersRepository;
            this.orderProductsRepository = orderProductsRepository;
            this.shoppingCartsService = shoppingCartsService;
            this.productsService = productsService;
            this.carsService = carsService;
            this.carBrandsService = carBrandsService;
            this.couponsService = couponsService;
        }

        public async Task<Order> CreateAsync(OrderCreateInputModel input, ApplicationUser user)
        {
            var isAnyInvoiceFieldFilled = input.CompanyName != null
                || input.CompanyAddress != null
                || input.CICNumber != null
                || input.AccountablePerson != null;

            var isAllInvoiceFieldFilled = input.CompanyName != null
                && input.CompanyAddress != null
                && input.CICNumber != null
                && input.AccountablePerson != null;

            var order = new Order
            {
                DeliveryAddress = input.DeliveryAddress,
                RecipientPhoneNumber = input.RecipientPhoneNumber,
                RecipientFullName = input.RecipientFullName,
                Message = input.Message,
                RecipientEmail = input.Email != null ? input.Email : user.Email,
                UserId = user.Id,
            };

            if (isAnyInvoiceFieldFilled)
            {
                if (isAllInvoiceFieldFilled)
                {
                    order.CompanyName = input.CompanyName;
                    order.CompanyAddress = input.CompanyAddress;
                    order.CICNumber = input.CICNumber;
                    order.AccountablePerson = input.AccountablePerson;
                }
                else
                {
                    throw new ArgumentException(ServicesDataConstants.BlankInvoiceData);
                }
            }

            await this.ordersRepository.AddAsync(order);
            await this.ordersRepository.SaveChangesAsync();

            var shoppingCart = await this.shoppingCartsService.GetShoppingCartByUserId(user.Id);
            var shoppingCartProducts = await this.shoppingCartsService.GetAllShoppingCartProductsAsync(shoppingCart.Id);

            if (shoppingCart == null)
            {
                throw new ArgumentNullException(string.Format(ServicesDataConstants.NullReferenceShoppingCartId, shoppingCart.Id));
            }

            decimal discount = 0;
            if (shoppingCart.CouponId != null)
            {
                var coupon = this.couponsService.GetCouponById(shoppingCart.CouponId);

                discount = this.couponsService.CalculateCouponDiscount(coupon);
            }

            if (shoppingCartProducts.Any())
            {
                foreach (var cartProduct in shoppingCartProducts)
                {
                    var currentProduct = await this.productsService.GetProductByIdAsync(cartProduct.ProductId);

                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.Id,
                        ProductId = currentProduct.Id,
                        ProductQuantity = cartProduct.Quantity,
                    };

                    if (discount > 0)
                    {
                        orderProduct.CouponDiscount = discount;
                    }

                    await this.orderProductsRepository.AddAsync(orderProduct);
                    await this.orderProductsRepository.SaveChangesAsync();
                }

                await this.shoppingCartsService.ClearShoppingCart(user.UserName);
            }
            else
            {
                throw new InvalidOperationException(string.Format(ServicesDataConstants.ShoppingCartNoProducts, shoppingCart.Id));
            }

            return order;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var order = await this.ordersRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (order == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceInquiryId, id));
            }

            this.ordersRepository.Delete(order);
            await this.ordersRepository.SaveChangesAsync();

            var orderProducts = await this.orderProductsRepository
                .AllAsNoTracking()
                .Where(s => s.OrderId == order.Id)
                .ToArrayAsync();

            foreach (var product in orderProducts)
            {
                this.orderProductsRepository.Delete(product);
                await this.orderProductsRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<OrdersAllViewModel>> GetAllOrdersAsync()
        {
            var orders = await this.ordersRepository
                .AllAsNoTracking()
                .OrderByDescending(o => o.CreatedOn)
                .Select(o => new OrdersAllViewModel
                {
                    Id = o.Id,
                    UserFullName = o.RecipientFullName,
                    CorrespondenceEmail = o.RecipientEmail,
                    CreateDate = o.CreatedOn.ToString("dd MMM yy HH:mm:ss"),
                })
                .OrderBy(o => o.UserFullName)
                .ToListAsync();

            return orders;
        }

        public async Task<OrderDeleteViewModel> GetDeleteViewModelByIdAsync(int id)
        {
            var order = await this.ordersRepository
                .AllAsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new OrderDeleteViewModel
                {
                    Id = o.Id,
                    RecipientFullName = o.RecipientFullName,
                    RecipientPhoneNumber = o.RecipientPhoneNumber,
                    Email = o.RecipientEmail,
                    DeliveryAddress = o.DeliveryAddress,
                    Message = o.Message,
                    CompanyName = o.CompanyName,
                    CompanyAddress = o.CompanyAddress,
                    CICNumber = o.CICNumber,
                    AccountablePerson = o.AccountablePerson,
                })
                .FirstOrDefaultAsync();

            return order;
        }

        public async Task<OrderDetailsViewModel> GetDetailsViewModelByIdAsync(int id)
        {
            var order = await this.ordersRepository
                .AllAsNoTracking()
                .Where(o => o.Id == id)
                .Select(o => new OrderDetailsViewModel
                {
                    Id = o.Id,
                    RecipientFullName = o.RecipientFullName,
                    RecipientPhoneNumber = o.RecipientPhoneNumber,
                    Email = o.RecipientEmail,
                    DeliveryAddress = o.DeliveryAddress,
                    Message = o.Message,
                    CompanyName = o.CompanyName,
                    CompanyAddress = o.CompanyAddress,
                    CICNumber = o.CICNumber,
                    AccountablePerson = o.AccountablePerson,
                })
                .FirstOrDefaultAsync();

            var orderProducts = await this.orderProductsRepository
               .AllAsNoTracking()
               .Where(op => op.OrderId == id)
               .ToListAsync();

            var productsList = new List<OrderProductViewModel>();

            foreach (var product in orderProducts)
            {
                var currentProduct = await this.productsService.GetProductByIdAsync(product.ProductId);

                var currentCar = await this.carsService.GetCarAsync(currentProduct.CarId);

                var currentBrand = await this.carBrandsService.GetBrandAsync(currentCar.BrandId);

                var productToAdd = new OrderProductViewModel
                {
                    CarBrand = currentBrand.Brand,
                    CarModel = currentCar.Model,
                    CarYear = currentCar.Year,
                    ProductName = currentProduct.Name,
                    Quantity = product.ProductQuantity,
                };

                if (currentProduct.Discount > 0)
                {
                    productToAdd.ProductPrice = $"Promotion {currentProduct.Price}$/{currentProduct.Discount}$";
                    productToAdd.TotalProductPrice = product.ProductQuantity * currentProduct.Discount;
                }
                else if (currentProduct.Discount == 0 && product.CouponDiscount == 0)
                {
                    productToAdd.ProductPrice = $"Original Price {currentProduct.Price}$";
                    productToAdd.TotalProductPrice = product.ProductQuantity * currentProduct.Price;
                }
                else
                {
                    productToAdd.ProductPrice = $"Coupon Discount {currentProduct.Price}$/{currentProduct.Price * product.CouponDiscount:f2}$";
                    productToAdd.TotalProductPrice = Convert.ToDecimal((product.ProductQuantity * (currentProduct.Price * product.CouponDiscount)).ToString("F2"));
                }

                order.OrderTotalPrice += productToAdd.TotalProductPrice;

                productsList.Add(productToAdd);
            }

            order.Products = productsList;

            return order;
        }
    }
}
