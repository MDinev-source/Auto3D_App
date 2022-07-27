namespace Auto3D.Web.ViewModels.Orders
{
    using System.ComponentModel.DataAnnotations;

    public class OrderCreateInputModel
    {
        [Required]
        public string RecipientFullName { get; set; }

        [Required]
        public string RecipientPhoneNumber { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string DeliveryAddress { get; set; }

        [MaxLength(500)]
        public string Message { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CICNumber { get; set; }

        public string AccountablePerson { get; set; }
    }
}
