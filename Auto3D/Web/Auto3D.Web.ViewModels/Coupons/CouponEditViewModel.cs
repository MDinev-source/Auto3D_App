namespace Auto3D.Web.ViewModels.Coupons
{
    using System.ComponentModel.DataAnnotations;

    public class CouponEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        public decimal Discount { get; set; }
    }
}
