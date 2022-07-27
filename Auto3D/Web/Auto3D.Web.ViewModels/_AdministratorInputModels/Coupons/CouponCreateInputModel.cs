namespace Auto3D.Web.ViewModels._AdministratorInputModels.Coupons
{
    using System.ComponentModel.DataAnnotations;

    public class CouponCreateInputModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [RegularExpression(@"^\d{1,3}$")]
        public string Discount { get; set; }
    }
}
