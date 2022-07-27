namespace Auto3D.Data.Models
{
    using System.Collections.Generic;

    using Auto3D.Data.Common.Models;

    public class Order : BaseDeletableModel<int>
    {
        public Order()
        {
            this.Products = new HashSet<OrderProduct>();
        }

        public string DeliveryAddress { get; set; }

        public string RecipientPhoneNumber { get; set; }

        public string RecipientFullName { get; set; }

        public string RecipientEmail { get; set; }

        public string Message { get; set; }

        public string CompanyName { get; set; }

        public string CompanyAddress { get; set; }

        public string CICNumber { get; set; }

        public string AccountablePerson { get; set; }

        public string UserId { get; set; }

        public virtual ApplicationUser User { get; set; }

        public virtual ICollection<OrderProduct> Products { get; set; }
    }
}
