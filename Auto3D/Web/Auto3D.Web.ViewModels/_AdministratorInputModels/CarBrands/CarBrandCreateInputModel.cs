namespace Auto3D.Web.ViewModels._AdministratorInputModels.CarBrands
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class CarBrandCreateInputModel
    {
        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile Logo { get; set; }
    }
}
