namespace Auto3D.Services.Data.Contracts
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Auto3D.Data.Models;
    using Auto3D.Web.ViewModels._AdministratorInputModels.Coupons;
    using Auto3D.Web.ViewModels.Coupons;

    public interface ICouponsService
    {
        Task<Coupon> CreatAsync(CouponCreateInputModel input);

        Task<IEnumerable<CouponViewModel>> GetAllCouponsAsync();

        Task<CouponEditViewModel> GetEditViewModelByIdAsync(int id);

        Task EditAsync(CouponEditViewModel couponEditViewModel);

        Task<CouponDeleteViewModel> GetDeleteViewModelByIdAsync(int id);

        Task DeleteByIdAsync(int id);

        Coupon GetCouponByName(string name);

        Coupon GetCouponById(int? id);

        decimal CalculateCouponDiscount(Coupon coupon);
    }
}
