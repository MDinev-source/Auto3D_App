namespace Auto3D.Data.Models
{
    using Auto3D.Data.Common.Models;

    public class ShoppingCartProduct : BaseDeletableModel<int>
    {
        public int ShoppingCartId { get; set; }

        public virtual ShoppingCart ShoppingCart { get; set; }

        public int ProductId { get; set; }

        public virtual Product Product { get; set; }

        public int Quantity { get; set; }

        public decimal TotalPrice { get; set; }
    }
}
