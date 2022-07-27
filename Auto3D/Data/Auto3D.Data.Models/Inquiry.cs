namespace Auto3D.Data.Models
{
    using System.Collections.Generic;

    using Auto3D.Data.Common.Models;

    public class Inquiry : BaseDeletableModel<int>
    {
        public Inquiry()
        {
            this.Pictures = new HashSet<InquiryPicture>();
        }

        public string RecipientName { get; set; }

        public string RecipientEmail { get; set; }

        public string RecipientPhoneNumber { get; set; }

        public string Subject { get; set; }

        public string Message { get; set; }

        public virtual ICollection<InquiryPicture> Pictures { get; set; }
    }
}
