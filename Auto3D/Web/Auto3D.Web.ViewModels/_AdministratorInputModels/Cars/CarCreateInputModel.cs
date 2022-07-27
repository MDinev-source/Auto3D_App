namespace Auto3D.Web.ViewModels._AdministratorInputModels.Cars
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class CarCreateInputModel
    {
        [Required]
        [MaxLength(50)]
        public string Model { get; set; }

        [Required]
        public int BrandId { get; set; }

        [Required]
        [RegularExpression(@"^[1-9]\d*(-\d{4})?$")]
        public string Year { get; set; }

        [Required]
        [DataType(DataType.Upload)]
        public IFormFile PictureUrl { get; set; }
    }
}
