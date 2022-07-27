namespace Auto3D.Web.ViewModels.Inquiries
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class InquiryCreateInputModel
    {
        [Required]
        public string RecipientName { get; set; }

        [Required]
        [EmailAddress]
        public string RecipientEmail { get; set; }

        public string RecipientPhoneNumber { get; set; }

        [Required]
        [MaxLength(50)]
        public string Subject { get; set; }

        [Required]
        [MaxLength(500)]
        public string Message { get; set; }

        public ICollection<IFormFile> Pictures { get; set; }
    }
}
