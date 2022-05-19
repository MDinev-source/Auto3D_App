namespace Auto3D.Services.Data
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Data.Common.Repositories;
    using Auto3D.Data.Models;
    using Auto3D.Services.Data.Common;
    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels._AdministratorInputModels.CompanyLogos;
    using Microsoft.EntityFrameworkCore;

    public class CompanyLogosService : ICompanyLogosService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png", "jpeg", "svg" };

        private readonly IDeletableEntityRepository<CompanyLogo> companyLogoRepository;

        public CompanyLogosService(
            IDeletableEntityRepository<CompanyLogo> companyLogoRepository)
        {
            this.companyLogoRepository = companyLogoRepository;
        }

        public async Task<CompanyLogo> CreateAsync(CompanyLogoInputModel input, string imagePath)
        {
            var currentLogo = await this.companyLogoRepository
            .AllAsNoTracking()
            .FirstOrDefaultAsync();

            if (currentLogo != null)
            {
                await this.DeleteCurrentLogo(imagePath, currentLogo);
            }

            CompanyLogo logo;
            if (input.Picture != null)
            {
                Directory.CreateDirectory($"{imagePath}/pictures/");

                var extension = Path.GetExtension(input.Picture.FileName).TrimStart('.');

                if (!this.allowedExtensions.Any(x => extension.ToLower().EndsWith(x)))
                {
                    throw new ArgumentException(ServicesDataConstants.InvalidLogoExtension);
                }

                logo = new CompanyLogo
                {
                    Extension = extension,
                };

                var physicalPath = $"{imagePath}/pictures/{logo.Id}.{extension}";

                logo.RemoteImageUrl = physicalPath;

                using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
                await input.Picture.CopyToAsync(fileStream);
            }
            else
            {
                throw new ArgumentException(ServicesDataConstants.NoLogoSelected);
            }

            await this.companyLogoRepository.AddAsync(logo);
            await this.companyLogoRepository.SaveChangesAsync();

            return logo;
        }

        public string GetCurrentLogoPath()
        {
            var logo = this.companyLogoRepository
                .AllAsNoTracking()
                .FirstOrDefault();

            var path = string.Empty;
            if (logo != null)
            {
                string basePath = GetPath(logo.RemoteImageUrl);
                path = $"/{basePath}/pictures/{logo.Id}.{logo.Extension}";
            }

            return path;
        }

        private async Task DeleteCurrentLogo(string imagePath, CompanyLogo currentLogo)
        {
            var path = $"{imagePath}/pictures/{currentLogo.Id}.{currentLogo.Extension}";

            File.Delete(path);

            this.companyLogoRepository.HardDelete(currentLogo);
            await this.companyLogoRepository.SaveChangesAsync();
        }

        private static string GetPath(string imagePath)
        {
            var basePathArr = imagePath.Split("/");
            var basePath = basePathArr[basePathArr.Length - 3];
            return basePath;
        }
    }
}
