namespace Auto3D.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Auto3D.Data.Common.Models;

    public class Product : BaseDeletableModel<int>
    {
        public Product()
        {
            this.ShoppingCartProducts = new HashSet<ShoppingCartProduct>();
            this.OrderProducts = new HashSet<OrderProduct>();
            this.Pictures = new HashSet<ProductPicture>();
        }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public string PictureUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        [Required]
        public int CarId { get; set; }

        public Car Car { get; set; }

        public virtual ICollection<ShoppingCartProduct> ShoppingCartProducts { get; set; }

        public virtual ICollection<OrderProduct> OrderProducts { get; set; }

        public virtual ICollection<ProductPicture> Pictures { get; set; }
    }
}
