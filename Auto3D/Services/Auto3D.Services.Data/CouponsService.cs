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
    using Auto3D.Web.ViewModels._AdministratorInputModels.Coupons;
    using Auto3D.Web.ViewModels.Coupons;
    using Microsoft.EntityFrameworkCore;

    public class CouponsService : ICouponsService
    {
        private readonly IDeletableEntityRepository<Coupon> couponsRepository;
        private readonly IRepository<ShoppingCart> shoppingCartsRepository;

        //private readonly IShoppingCartsService shoppingCartsService;

        public CouponsService(
            IDeletableEntityRepository<Coupon> couponsRepository,
            IRepository<ShoppingCart> shoppingCartsRepository
          /*  IShoppingCartsService shoppingCartsService*/)
        {
            this.couponsRepository = couponsRepository;
            this.shoppingCartsRepository = shoppingCartsRepository;
            //this.shoppingCartsService = shoppingCartsService;
        }

        public decimal CalculateCouponDiscount(Coupon coupon)
        {
            var discount = (100 - coupon.Discount) / 100;

            return discount;
        }

        public async Task<Coupon> CreatAsync(CouponCreateInputModel input)
        {
            var coupon = new Coupon
            {
                Name = input.Name,
                Discount = decimal.Parse(input.Discount),
            };

            await this.couponsRepository.AddAsync(coupon);
            await this.couponsRepository.SaveChangesAsync();

            return coupon;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var coupon = await this.couponsRepository
               .AllAsNoTracking()
               .FirstOrDefaultAsync(x => x.Id == id);

            if (coupon == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCouponId, id));
            }

            this.couponsRepository.Delete(coupon);
            await this.couponsRepository.SaveChangesAsync();

            var carts = await this.shoppingCartsRepository
                .All()
                .Where(s => s.CouponId == id)
                .ToArrayAsync();

            foreach (var cart in carts)
            {
                cart.CouponId = null;
                this.shoppingCartsRepository.Update(cart);
                await this.shoppingCartsRepository.SaveChangesAsync();
            }
        }

        public async Task EditAsync(CouponEditViewModel couponEditViewModel)
        {
            var coupon = this.couponsRepository
                  .AllAsNoTracking()
                  .Where(x => x.Id == couponEditViewModel.Id)
                  .FirstOrDefault();

            coupon.Name = couponEditViewModel.Name;
            coupon.Discount = couponEditViewModel.Discount;

            this.couponsRepository.Update(coupon);
            await this.couponsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CouponViewModel>> GetAllCouponsAsync()
        {
            var coupons = await this.couponsRepository
                .AllAsNoTracking()
                .Select(c => new CouponViewModel
                {
                    Id = c.Id,
                    Name = c.Name,
                    Discount = c.Discount,
                })
                .ToArrayAsync();

            return coupons;
        }

        public Coupon GetCouponById(int? id)
        {
            var coupon = this.couponsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            return coupon;
        }

        public Coupon GetCouponByName(string name)
        {
            var coupon = this.couponsRepository
                 .AllAsNoTracking()
                 .Where(x => x.Name == name)
                 .FirstOrDefault();

            return coupon;
        }

        public async Task<CouponDeleteViewModel> GetDeleteViewModelByIdAsync(int id)
        {
            var coupon = await this.couponsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CouponDeleteViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    Discount = x.Discount.ToString(),
                })
                .FirstOrDefaultAsync();

            if (coupon == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCouponId, id));
            }

            return coupon;
        }

        public async Task<CouponEditViewModel> GetEditViewModelByIdAsync(int id)
        {
            var coupon = await this.couponsRepository
                  .AllAsNoTracking()
                  .Where(x => x.Id == id)
                  .Select(x => new CouponEditViewModel
                  {
                      Id = id,
                      Name = x.Name,
                      Discount = x.Discount,
                  })
               .FirstOrDefaultAsync();

            return coupon;
        }
    }
}
