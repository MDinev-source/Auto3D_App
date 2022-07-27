namespace Auto3D.Data.Models
{
    using Auto3D.Data.Common.Models;

    public class OrderProduct : BaseDeletableModel<int>
    {
        public int OrderId { get; set; }

        public virtual Order Order { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int ProductQuantity { get; set; }

        public decimal CouponDiscount { get; set; }
    }
}
