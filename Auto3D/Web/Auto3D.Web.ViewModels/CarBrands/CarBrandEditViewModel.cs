namespace Auto3D.Web.ViewModels.CarBrands
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class CarBrandEditViewModel
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        public string LogoUrl { get; set; }

        public IFormFile Logo { get; set; }
    }
}
