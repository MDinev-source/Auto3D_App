namespace Auto3D.Web.ViewModels.Cars
{
    using System.Collections.Generic;

    using Auto3D.Web.ViewModels.Enums;

    public class CarsAllViewModel
    {
        public int BrandId { get; set; }

        public int? PageNumber { get; set; }

        public int? PageSize { get; set; }

        public CarsSorter Sorter { get; set; }

        public IEnumerable<CarViewModel> Cars { get; set; }
    }
}
