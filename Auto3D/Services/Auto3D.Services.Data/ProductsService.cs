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
    using Auto3D.Web.ViewModels._AdministratorInputModels.Products;
    using Auto3D.Web.ViewModels.Enums;
    using Auto3D.Web.ViewModels.Products;
    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;

    public class ProductsService : IProductsService
    {
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly Cloudinary cloudinary;

        public ProductsService(
            IDeletableEntityRepository<Product> productsRepository,
            Cloudinary cloudinary)
        {
            this.productsRepository = productsRepository;
            this.cloudinary = cloudinary;
        }

        public async Task<Product> CreatAsync(ProductCreateinputModel input)
        {
            var picture = input.PictureUrl;

            var pictureUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, picture, input.Name);

            var product = new Product
            {
                Name = input.Name,
                Description = input.Description,
                PictureUrl = pictureUrl,
                Price = input.Price,
                Discount = input.Discount,
                CarId = input.CarId,
            };

            await this.productsRepository.AddAsync(product);
            await this.productsRepository.SaveChangesAsync();

            return product;
        }

        public async Task<IEnumerable<ProductViewModel>> GetAllProductsByCarIdAsync(int carId)
        {
            var products = await this.productsRepository
                 .AllAsNoTracking()
                 .Where(x => x.CarId == carId)
                  .Select(x => new ProductViewModel
                  {
                      Id = x.Id,
                      Name = x.Name,
                      Description = x.Description,
                      CarId = x.CarId,
                      CarBrand = x.Car.Brand.Brand,
                      CarModel = x.Car.Model,
                      CarYear = x.Car.Year,
                      Price = x.Price,
                      Discount = x.Discount,
                      PictureUrl = x.PictureUrl,
                  })
                 .ToListAsync();

            return products;
        }

        public IEnumerable<ProductViewModel> SortBy(ProductViewModel[] cars, ProductsSorter sorter)
        {
            switch (sorter)
            {
                case ProductsSorter.Name:
                    return cars.OrderBy(p => p.Name).ThenBy(c => c.CarBrand).ThenBy(c => c.CarModel).ToList();
                case ProductsSorter.CarBrand:
                    return cars.OrderBy(p => p.CarBrand).ThenBy(c => c.Name).ToList();
                case ProductsSorter.Price:
                    return cars.OrderBy(p => p.Price).ThenBy(c => c.Name).ToList();
                case ProductsSorter.PriceDesc:
                    return cars.OrderByDescending(p => p.Price).ThenBy(c => c.Name).ToList();
                case ProductsSorter.Discount:
                    return cars.OrderBy(p => p.Discount).ThenBy(c => c.Name).ToList();
                case ProductsSorter.DiscountDesc:
                    return cars.OrderByDescending(p => p.Discount).ThenBy(c => c.Name).ToList();
                default:
                    return cars.OrderBy(p => p.Name).ThenBy(c => c.CarBrand).ThenBy(c => c.CarModel).ToList();
            }
        }

        public async Task<ProductEditViewModel> GetEditViewModelByIdAsync(int id)
        {
            var product = await this.productsRepository
                  .AllAsNoTracking()
                  .Where(x => x.Id == id)
                  .Select(x => new ProductEditViewModel
                  {
                      Id = id,
                      Name = x.Name,
                      PictureUrl = x.PictureUrl,
                      Price = x.Price,
                      Description = x.Description,
                      Discount = x.Discount,
                  })
               .FirstOrDefaultAsync();

            return product;
        }

        public async Task EditAsync(ProductEditViewModel productEditModel)
        {
            var product = this.productsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == productEditModel.Id)
                .FirstOrDefault();

            var picture = string.Empty;

            if (productEditModel.Picture != null)
            {
                var logo = productEditModel.Picture;

                picture = await ApplicationCloudinary.UploadImage(this.cloudinary, logo, productEditModel.Name);

                product.PictureUrl = picture;
            }

            product.Name = productEditModel.Name;
            product.Description = productEditModel.Description;
            product.Price = productEditModel.Price;
            product.Discount = productEditModel.Discount;

            this.productsRepository.Update(product);
            await this.productsRepository.SaveChangesAsync();
        }

        public async Task DeleteByIdAsync(int id)
        {
            var product = await this.productsRepository
                .AllAsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id);

            if (product == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceProductId, id));
            }

            this.productsRepository.Delete(product);
            await this.productsRepository.SaveChangesAsync();
        }

        public async Task<ProductDeleteViewModel> GetDeleteViewModelByIdAsync(int id)
        {
            var car = await this.productsRepository
                .AllAsNoTracking()
                .Where(x => x.Id == id)
                .Select(x => new ProductDeleteViewModel
                {
                    Id = x.Id,
                    Name = x.Name,
                    CarBrand = x.Car.Brand.Brand,
                    CarModel = x.Car.Model,
                    CarYear = x.Car.Year,
                    PictureUrl = x.PictureUrl,
                })
                .FirstOrDefaultAsync();

            if (car == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceCarBrandId, id));
            }

            return car;
        }

        public async Task<ProductDetailsViewModel> GetDetailsViewModelByIdAsync(int id)
        {
            var product = await this.productsRepository
            .AllAsNoTracking()
            .Where(p => p.Id == id)
            .Select(x => new ProductDetailsViewModel
            {
                Id = id,
                Name = x.Name,
                Description = x.Description,
                Price = x.Price,
                Discount = x.Discount,
                CarBrand = x.Car.Brand.Brand,
                CarModel = x.Car.Model,
                CarYear = x.Car.Year,
                PictureUrl = x.PictureUrl,
            })
            .FirstOrDefaultAsync();

            return product;
        }

        public async Task<Product> GetProductByIdAsync(int id)
        {
            var product = await this.productsRepository
                .AllAsNoTracking()
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            return product;
        }

        public Product GetProductById(int id)
        {
            var product = this.productsRepository
                 .All()
                 .Where(p => p.Id == id)
                 .FirstOrDefault();

            return product;
        }

        public async Task<IEnumerable<ProductViewModel>> ReviewProductsAsync()
        {
            var products = await this.productsRepository
            .AllAsNoTracking()
           .Select(x => new ProductViewModel
           {
               Id = x.Id,
               Name = x.Name,
               Description = x.Description,
               CarId = x.CarId,
               CarBrand = x.Car.Brand.Brand,
               CarModel = x.Car.Model,
               CarYear = x.Car.Year,
               Price = x.Price,
               Discount = x.Discount,
               PictureUrl = x.PictureUrl,
               CreateDate = x.CreatedOn.ToString(),
           })
           .OrderByDescending(p => p.CreateDate)
           .Take(3)
          .ToListAsync();

            return products;
        }
    }
}
