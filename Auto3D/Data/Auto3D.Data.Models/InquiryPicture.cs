namespace Auto3D.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Auto3D.Data.Common.Models;

    public class InquiryPicture : BaseDeletableModel<string>
    {
        public InquiryPicture()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Extension { get; set; }

        public string RemoteImageUrl { get; set; }

        [Required]
        public int InquiryId { get; set; }

        public Inquiry Inquiry { get; set; }
    }
}
