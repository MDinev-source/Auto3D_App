namespace Auto3D.Data.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Auto3D.Data.Common.Models;

    public class ProductPicture : BaseDeletableModel<string>
    {
        public ProductPicture()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Extension { get; set; }

        public string RemoteImageUrl { get; set; }

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; }
    }
}
