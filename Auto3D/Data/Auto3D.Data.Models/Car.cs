namespace Auto3D.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Auto3D.Data.Common.Models;

    public class Car : BaseDeletableModel<int>
    {
        public Car()
        {
            this.Products = new HashSet<Product>();
        }

        [Required]
        public string Model { get; set; }

        [Required]
        public int BrandId { get; set; }

        public CarBrand Brand { get; set; }

        [Required]
        public string Year { get; set; }

        [Required]
        public string PictureUrl { get; set; }

        public virtual ICollection<Product> Products { get; set; }
    }
}
