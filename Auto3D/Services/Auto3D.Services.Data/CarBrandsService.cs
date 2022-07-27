namespace Auto3D.Services.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Auto3D.Data.Common.Repositories;
    using Auto3D.Data.Models;
    using Auto3D.Services.Data.Common;
    using Auto3D.Services.Data.Contracts;
    using Auto3D.Web.ViewModels._AdministratorInputModels.CarBrands;
    using Auto3D.Web.ViewModels.CarBrands;
    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;

    public class CarBrandsService : ICarBrandsService
    {
        private readonly IDeletableEntityRepository<CarBrand> carBrandsRepository;
        private readonly Cloudinary cloudinary;

        public CarBrandsService(
            IDeletableEntityRepository<CarBrand> carBrandRepository,
            Cloudinary cloudinary)
        {
            this.carBrandsRepository = carBrandRepository;
            this.cloudinary = cloudinary;
        }

        public async Task<CarBrand> CreateAsync(CarBrandCreateInputModel input)
        {
            var logo = input.Logo;

            var logoUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, logo, input.Brand);

            var carBrand = new CarBrand
            {
                Brand = input.Brand,
                LogoUrl = logoUrl,
            };

            await this.carBrandsRepository.AddAsync(carBrand);
            await this.carBrandsRepository.SaveChangesAsync();

            return carBrand;
        }

        public async Task DeleteByIdAsync(int id)
        {
            var carBrand = await this.carBrandsRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (carBrand == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCarBrandId, id));
            }

            this.carBrandsRepository.Delete(carBrand);
            await this.carBrandsRepository.SaveChangesAsync();
        }

        public async Task<IEnumerable<CarBrandViewModel>> GetAllCarBrandsAsync()
        {
            var carBrands = await this.carBrandsRepository.AllAsNoTracking()
                .Select(x => new CarBrandViewModel
                {
                    Id = x.Id,
                    Brand = x.Brand,
                    Logo = x.LogoUrl,
                })
                .ToListAsync();

            return carBrands;
        }

        public async Task<CarBrandDeleteViewModel> GetDeleteViewModelByIdAsync(int id)
        {
            var carBrand = await this.carBrandsRepository.AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CarBrandDeleteViewModel
                {
                    Id = x.Id,
                    Brand = x.Brand,
                    Logo = x.LogoUrl,
                })
                .FirstOrDefaultAsync();

            if (carBrand == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCarBrandId, id));
            }

            return carBrand;
        }

        public async Task<CarBrandEditViewModel> GetEditViewModelByIdAsync(int id)
        {
            var brand = await this.carBrandsRepository
                  .AllAsNoTracking()
                  .Where(x => x.Id == id)
                  .Select(x => new CarBrandEditViewModel
                  {
                      Id = id,
                      Brand = x.Brand,
                      LogoUrl = x.LogoUrl,
                  })
               .FirstOrDefaultAsync();

            return brand;
        }

        public async Task EditAsync(CarBrandEditViewModel carBrandEditModel)
        {
            var brand = this.carBrandsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == carBrandEditModel.Id)
                .FirstOrDefault();

            var logoUrl = string.Empty;

            if (carBrandEditModel.Logo != null)
            {
                var logo = carBrandEditModel.Logo;

                logoUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, logo, carBrandEditModel.Brand);

                brand.LogoUrl = logoUrl;
            }

            brand.Brand = carBrandEditModel.Brand;

            this.carBrandsRepository.Update(brand);
            await this.carBrandsRepository.SaveChangesAsync();
        }

        public async Task<CarBrand> GetBrandAsync(int id)
        {
            var brand = await this.carBrandsRepository
                 .AllAsNoTracking()
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();

            return brand;
        }

        public CarBrand GetBrand(int id)
        {
            var brand = this.carBrandsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .FirstOrDefault();

            return brand;
        }
    }
}
