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
    using Auto3D.Web.ViewModels._AdministratorInputModels.Cars;
    using Auto3D.Web.ViewModels.Cars;
    using Auto3D.Web.ViewModels.Enums;
    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;

    public class CarsService : ICarsService
    {
        private readonly IDeletableEntityRepository<Car> carsRepository;
        private readonly Cloudinary cloudinary;

        public CarsService(
            IDeletableEntityRepository<Car> carsRepository,
            Cloudinary cloudinary)
        {
            this.carsRepository = carsRepository;
            this.cloudinary = cloudinary;
        }

        public async Task<Car> CreateAsync(CarCreateInputModel input)
        {
            var picture = input.PictureUrl;

            var pictureUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, picture, input.Model);

            var car = new Car
            {
                Model = input.Model,
                BrandId = input.BrandId,
                Year = input.Year,
                PictureUrl = pictureUrl,
            };

            await this.carsRepository.AddAsync(car);
            await this.carsRepository.SaveChangesAsync();

            return car;
        }

        public async Task<IEnumerable<CarViewModel>> GetAllCarsByBrandIdAsync(int carBrandId)
        {
            var cars = await this.carsRepository
                 .AllAsNoTracking()
                 .Where(x => x.Brand.Id == carBrandId)
                  .Select(x => new CarViewModel
                  {
                      Id = x.Id,
                      Model = x.Model,
                      BrandId = x.Brand.Id,
                      Brand = x.Brand.Brand,
                      Year = x.Year,
                      PictureUrl = x.PictureUrl,
                  })
                 .ToListAsync();

            return cars;
        }

        public IEnumerable<CarViewModel> SortBy(CarViewModel[] cars, CarsSorter sorter)
        {
            switch (sorter)
            {
                case CarsSorter.Model:
                    return cars.OrderBy(c => c.Model).ThenBy(y => y.Year).ToList();
                case CarsSorter.Year:
                    return cars.OrderByDescending(c => c.Year).ThenBy(d => d.Model).ToList();
                default:
                    return cars.OrderBy(c => c.Model).ThenBy(y => y.Year).ToList();
            }
        }

        public async Task<CarEditViewModel> GetEditViewModelByIdAsync(int id)
        {
            var car = await this.carsRepository
                  .AllAsNoTracking()
                  .Where(x => x.Id == id)
                  .Select(x => new CarEditViewModel
                  {
                      Id = id,
                      Model = x.Model,
                      PictureUrl = x.PictureUrl,
                      Year = x.Year,
                  })
               .FirstOrDefaultAsync();

            return car;
        }

        public async Task EditAsync(CarEditViewModel carEditModel)
        {
            var car = this.carsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == carEditModel.Id)
                .FirstOrDefault();

            var picture = string.Empty;

            if (carEditModel.Picture != null)
            {
                var logo = carEditModel.Picture;

                picture = await ApplicationCloudinary.UploadImage(this.cloudinary, logo, carEditModel.Model);

                car.PictureUrl = picture;
            }

            car.Model = carEditModel.Model;
            car.Year = carEditModel.Year;

            this.carsRepository.Update(car);
            await this.carsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var car = await this.carsRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (car == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCarId, id));
            }

            this.carsRepository.Delete(car);
            await this.carsRepository.SaveChangesAsync();
        }

        public async Task<CarDeleteViewModel> GetDeleteViewModelByIdAsync(int id)
        {
            var car = await this.carsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new CarDeleteViewModel
                {
                    Id = x.Id,
                    Model = x.Model,
                    Year = x.Year,
                    PictureUrl = x.PictureUrl,
                })
                .FirstOrDefaultAsync();

            if (car == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCarBrandId, id));
            }

            return car;
        }

        public async Task<Car> GetCarAsync(int id)
        {
            var car = await this.carsRepository
                 .AllAsNoTracking()
                 .Where(x => x.Id == id)
                 .FirstOrDefaultAsync();

            return car;
        }
    }
}
