using System.Collections.Generic;

namespace Auto3D.Web.ViewModels.Inquiries
{
    public class InquiryDetailsViewModel : InquiryViewModel
    {
        public int Id { get; set; }

        public IEnumerable<InquiryPicturesViewModel> Pictures { get; set; }
    }
}
