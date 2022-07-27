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
    using Auto3D.Web.ViewModels._AdministratorInputModels.Products;
    using Auto3D.Web.ViewModels.Enums;
    using Auto3D.Web.ViewModels.Products;
    using CloudinaryDotNet;
    using Microsoft.EntityFrameworkCore;

    public class ProductsService : IProductsService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png", "jpeg" };
        private readonly IDeletableEntityRepository<Product> productsRepository;
        private readonly Cloudinary cloudinary;
        private readonly ICarsService carsService;
        private readonly ICarBrandsService carBrandsService;
        private readonly IDeletableEntityRepository<ProductPicture> productPicturesRepository;

        public ProductsService(
            IDeletableEntityRepository<Product> productsRepository,
            Cloudinary cloudinary,
            ICarsService carsService,
            ICarBrandsService carBrandsService,
            IDeletableEntityRepository<ProductPicture> productPicturesRepository)
        {
            this.productsRepository = productsRepository;
            this.cloudinary = cloudinary;
            this.carsService = carsService;
            this.carBrandsService = carBrandsService;
            this.productPicturesRepository = productPicturesRepository;
        }

        public async Task<Product> CreatAsync(ProductCreateinputModel input, string imagePath)
        {
            var mainPicture = input.PictureUrl;

            var pictureUrl = await ApplicationCloudinary.UploadImage(this.cloudinary, mainPicture, input.Name);

            var product = new Product
            {
                Name = input.Name,
                Description = input.Description,
                PictureUrl = pictureUrl,
                Price = input.Price,
                Discount = input.Discount,
                CarId = input.CarId,
            };

            if (input.Pictures != null)
            {
                Directory.CreateDirectory($"{imagePath}/pictures/");
                foreach (var picture in input.Pictures)
                {
                    var extension = Path.GetExtension(picture.FileName).TrimStart('.');

                    if (!this.allowedExtensions.Any(x => extension.ToLower().EndsWith(x)))
                    {
                        throw new ArgumentException(ServicesDataConstants.InvalidImageExtension);
                    }

                    var productPicture = new ProductPicture
                    {
                        Extension = extension,
                        ProductId = product.Id,
                    };

                    product.Pictures.Add(productPicture);

                    var physicalPath = $"{imagePath}/pictures/{productPicture.Id}.{extension}";

                    productPicture.RemoteImageUrl = physicalPath;

                    using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
                    await picture.CopyToAsync(fileStream);
                }
            }

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

        public IEnumerable<ProductViewModel> GetProductsFromSearch(string searchString, int carId)
        {
            IEnumerable<ProductViewModel> products = null;

            var escapedSearchTokens = searchString.Split(new[] { ' ', ',', '.', ':', '=', ';' }, StringSplitOptions.RemoveEmptyEntries);

            Car car = null;
            CarBrand carBrand = null;

            if (carId != 0)
            {
                car = this.carsService.GetCar(carId);
                carBrand = this.carBrandsService.GetBrand(car.BrandId);

                products = this.productsRepository
               .AllAsNoTracking()
               .Where(p => p.CarId == carId)
               .ToArray()
               .Where(p => escapedSearchTokens.All(t => p.Name.ToLower().Contains(t.ToLower())))
               .Select(x => new ProductViewModel
               {
                   Id = x.Id,
                   Name = x.Name,
                   Description = x.Description,
                   CarId = car.Id,
                   CarBrand = carBrand.Brand,
                   CarModel = car.Model,
                   CarYear = car.Year,
                   Price = x.Price,
                   Discount = x.Discount,
                   PictureUrl = x.PictureUrl,
               })
               .ToArray();
            }
            else
            {
                products = this.productsRepository
               .All()
               .ToArray()
               .Where(p => escapedSearchTokens.All(t => p.Name.ToLower().Contains(t.ToLower())))
               .Select(x => new ProductViewModel
               {
                   Id = x.Id,
                   Name = x.Name,
                   Description = x.Description,
                   Price = x.Price,
                   CarId = x.CarId,
                   Discount = x.Discount,
                   PictureUrl = x.PictureUrl,
               })
               .ToArray();

                foreach (var product in products)
                {
                    var currentProduct = this.productsRepository.AllAsNoTracking().Where(x => x.Id == product.Id).FirstOrDefault();
                    car = this.carsService.GetCar(currentProduct.CarId);
                    carBrand = this.carBrandsService.GetBrand(car.BrandId);

                    product.CarId = car.Id;
                    product.CarBrand = carBrand.Brand;
                    product.CarModel = car.Model;
                    product.CarYear = car.Year;
                }
            }

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
                      Price = x.Price,
                      Description = x.Description,
                      Discount = x.Discount,
                      PictureUrl = x.PictureUrl,
                  })
               .FirstOrDefaultAsync();

            return product;
        }

        public async Task<int> EditAsync(ProductEditViewModel productEditModel)
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

            return product.CarId;
        }

        public async Task<int> DeleteByIdAsync(int id, string imagePath)
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

            var productPictures = await this.productPicturesRepository
                .AllAsNoTracking()
                .Where(s => s.ProductId == product.Id)
                .ToArrayAsync();

            foreach (var picture in productPictures)
            {
                var path = $"{imagePath}/pictures/{picture.Id}.{picture.Extension}";

                File.Delete(path);

                this.productPicturesRepository.HardDelete(picture);
                await this.productPicturesRepository.SaveChangesAsync();
            }

            return product.CarId;
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

        public async Task<ProductDetailsViewModel> GetDetailsViewModelByIdAsync(int id, string imagePath)
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

            var pictures = await this.productPicturesRepository
                .AllAsNoTracking()
                .Where(p => p.ProductId == product.Id)
                .ToArrayAsync();

            var productPicturesList = new List<ProductPicturesViewModel>();

            var basePathArr = imagePath.Split("/");
            var basePath = basePathArr[basePathArr.Length - 1];

            foreach (var picture in pictures)
            {
                var picutreName = picture.Id + "." + picture.Extension;
                var path = $"/{basePath}/pictures/{picutreName}";

                var pictureToAdd = new ProductPicturesViewModel
                {
                    Id = picture.Id,
                    Path = path,
                };

                productPicturesList.Add(pictureToAdd);
            }

            product.Pictures = productPicturesList;

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

        public async Task<IEnumerable<ProductViewModel>> ReviewLastAddedProductsAsync()
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
               CreateDate = x.CreatedOn.ToString(),
               PictureUrl = x.PictureUrl,
           })
           .OrderByDescending(p => p.CreateDate)
           .Take(3)
          .ToListAsync();

            return products;
        }

        public async Task<IEnumerable<ProductViewModel>> ReviewAllProductsAsync()
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
       })
      .ToListAsync();

            return products;
        }
    }
}
