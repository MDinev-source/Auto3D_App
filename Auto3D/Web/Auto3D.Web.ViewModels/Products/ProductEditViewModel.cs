namespace Auto3D.Web.ViewModels.Products
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class ProductEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal Discount { get; set; }

        public string PictureUrl { get; set; }

        public IFormFile Picture { get; set; }
    }
}
