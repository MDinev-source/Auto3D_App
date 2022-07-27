namespace Auto3D.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Data.Common.Repositories;
    using Auto3D.Data.Models;
    using Auto3D.Services.Data.Common;
    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels._AdministratorInputModels.PartnerLogos;
    using Auto3D.Web.ViewModels.PartnerLogos;
    using Microsoft.EntityFrameworkCore;

    public class PartnerLogosService : IPartnerLogosService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png", "jpeg", "svg" };

        private readonly IDeletableEntityRepository<Logo> partnerLogosRepository;

        public PartnerLogosService(
            IDeletableEntityRepository<Logo> partnerLogosRepository)
        {
            this.partnerLogosRepository = partnerLogosRepository;
        }

        public async Task<Logo> CreateAsync(PartnerLogoInputModel input, string imagePath)
        {
            Logo logo;
            if (input.Picture != null)
            {
                Directory.CreateDirectory($"{imagePath}/pictures/");

                var extension = Path.GetExtension(input.Picture.FileName).TrimStart('.');

                if (!this.allowedExtensions.Any(x => extension.ToLower().EndsWith(x)))
                {
                    throw new ArgumentException(ServicesDataConstants.InvalidLogoExtension);
                }

                logo = new Logo
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

            await this.partnerLogosRepository.AddAsync(logo);
            await this.partnerLogosRepository.SaveChangesAsync();

            return logo;
        }

        public async Task DeleteByIdAsync(string id, string imagePath)
        {
            var logo = await this.partnerLogosRepository
               .AllAsNoTracking()
               .Where(l => l.Id == id)
               .FirstOrDefaultAsync();

            var path = $"{imagePath}/pictures/{logo.Id}.{logo.Extension}";

            File.Delete(path);

            this.partnerLogosRepository.HardDelete(logo);
            await this.partnerLogosRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<PartnerLogoViewModel>> GetAllLogosAsync(string imagePath)
        {
            string basePath = GetPath(imagePath);

            var logos = await this.partnerLogosRepository.AllAsNoTracking()
             .Select(x => new PartnerLogoViewModel
             {
                 Id = x.Id,
                 Path = $"/{basePath}/pictures/{x.Id}.{x.Extension}",
             })
            .ToListAsync();
            return logos;
        }

        public async Task<PartnerLogoViewModel> GetDeleteViewModelByIdAsync(string id, string imagePath)
        {
            string basePath = GetPath(imagePath);

            var logo = await this.partnerLogosRepository
                .AllAsNoTracking()
                .Where(l => l.Id == id)
                .Select(l => new PartnerLogoViewModel
                {
                    Id = id,
                    Path = $"/{basePath}/pictures/{l.Id}.{l.Extension}",
                })
                .FirstOrDefaultAsync();

            return logo;
        }

        private static string GetPath(string imagePath)
        {
            var basePathArr = imagePath.Split("/");
            var basePath = basePathArr[basePathArr.Length - 1];
            return basePath;
        }
    }
}
