namespace Auto3D.Data.Models
{
    using System;

    using Auto3D.Data.Common.Models;

    public class CompanyLogo : BaseDeletableModel<string>
    {
        public CompanyLogo()
        {
            this.Id = Guid.NewGuid().ToString();
        }

        public string Extension { get; set; }

        public string RemoteImageUrl { get; set; }
    }
}
