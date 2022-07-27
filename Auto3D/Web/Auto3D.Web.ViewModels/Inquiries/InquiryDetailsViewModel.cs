namespace Auto3D.Web.ViewModels.Inquiries
{
    using System.Collections.Generic;

    public class InquiryDetailsViewModel : InquiryViewModel
    {
        public int Id { get; set; }

        public IEnumerable<InquiryPicturesViewModel> Pictures { get; set; }
    }
}
