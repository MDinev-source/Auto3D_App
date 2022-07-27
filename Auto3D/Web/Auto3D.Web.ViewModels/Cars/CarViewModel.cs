namespace Auto3D.Web.ViewModels.Cars
{
    using System.ComponentModel.DataAnnotations;

    public class CarViewModel
    {
        public int Id { get; set; }

        public string Model { get; set; }

        public int BrandId { get; set; }

        public string Brand { get; set; }

        public string Year { get; set; }

        public string PictureUrl { get; set; }
    }
}
