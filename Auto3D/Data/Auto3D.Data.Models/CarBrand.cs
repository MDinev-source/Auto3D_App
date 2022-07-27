namespace Auto3D.Data.Models
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Auto3D.Data.Common.Models;

    public class CarBrand : BaseDeletableModel<int>
    {
        public CarBrand()
        {
            this.Cars = new HashSet<Car>();
        }

        [Required]
        [MaxLength(50)]
        public string Brand { get; set; }

        [Required]
        public string LogoUrl { get; set; }

        public virtual ICollection<Car> Cars { get; set; }
    }
}
