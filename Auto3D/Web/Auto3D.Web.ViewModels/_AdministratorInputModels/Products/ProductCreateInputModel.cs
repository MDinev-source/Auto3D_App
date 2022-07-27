namespace Auto3D.Web.ViewModels._AdministratorInputModels.Products
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class ProductCreateinputModel
    {
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public IFormFile PictureUrl { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
        public decimal Discount { get; set; }

        [Required]
        public int CarId { get; set; }

        public ICollection<IFormFile> Pictures { get; set; }
    }
}
