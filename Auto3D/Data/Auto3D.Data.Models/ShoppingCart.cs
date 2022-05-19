namespace Auto3D.Data.Models
{
    using System.Collections.Generic;

    using Auto3D.Data.Common.Models;

    public class ShoppingCart : BaseModel<int>
    {
        public ShoppingCart()
        {
            this.ShoppingCartProducts = new HashSet<ShoppingCartProduct>();
        }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public int? CouponId { get; set; }

        public Coupon Coupon { get; set; }

        public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; }
    }
}
