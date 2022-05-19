namespace Auto3D.Web.Areas.Administration.Controllers
{
    using Auto3D.Common;
    using Auto3D.Services.Data.Common;
    using Auto3D.Web.Controllers;

    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;

    [Authorize(Roles = GlobalConstants.AdministratorRoleName)]
    [Area("Administration")]
    public class AdministrationController : BaseController
    {
        protected bool IsImageTypeValid(string fileType)
        {
            return fileType == ServicesDataConstants.JpgFormat || fileType == ServicesDataConstants.PngFormat || fileType == ServicesDataConstants.JpegFormat;
        }
    }
}
