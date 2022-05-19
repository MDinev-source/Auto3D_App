namespace Auto3D.Data.Models
{
    using System.Collections.Generic;

    using Auto3D.Data.Common.Models;

    public class Coupon : BaseDeletableModel<int>
    {
        public Coupon()
        {
            this.ShoppingCarts = new HashSet<ShoppingCart>();
        }

        public string Name { get; set; }

        public decimal Discount { get; set; }

        public IEnumerable<ShoppingCart> ShoppingCarts { get; set; }
    }
}
