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
    using Auto3D.Web.ViewModels.Inquiries;
    using Grpc.Core;
    using Microsoft.EntityFrameworkCore;

    public class InquiriesService : IInquiriesService
    {
        private readonly string[] allowedExtensions = new[] { "jpg", "png", "jpeg" };
        private readonly IDeletableEntityRepository<Inquiry> inquiriesRepository;
        private readonly IDeletableEntityRepository<InquiryPicture> inquiryPicturesRepository;

        public InquiriesService(
            IDeletableEntityRepository<Inquiry> inquiriesRespository,
            IDeletableEntityRepository<InquiryPicture> inquiryPicturesRepository)
        {
            this.inquiriesRepository = inquiriesRespository;
            this.inquiryPicturesRepository = inquiryPicturesRepository;
        }

        public async Task<Inquiry> CreateAsync(InquiryCreateInputModel input, string imagePath)
        {
            var inquiry = new Inquiry
            {
                RecipientName = input.RecipientName,
                RecipientEmail = input.RecipientEmail,
                RecipientPhoneNumber = input.RecipientPhoneNumber,
                Subject = input.Subject,
                Message = input.Message,
            };

            if (input.Pictures.Count > 0)
            {
                Directory.CreateDirectory($"{imagePath}/pictures/");
                foreach (var picture in input.Pictures)
                {
                    var extension = Path.GetExtension(picture.FileName).TrimStart('.');

                    if (!this.allowedExtensions.Any(x => extension.ToLower().EndsWith(x)))
                    {
                        throw new ArgumentException(ServicesDataConstants.InvalidImageExtension);
                    }

                    var inquiryPicture = new InquiryPicture
                    {
                        Extension = extension,
                        InquiryId = inquiry.Id,
                    };

                    inquiry.Pictures.Add(inquiryPicture);

                    var physicalPath = $"{imagePath}/pictures/{inquiryPicture.Id}.{extension}";

                    inquiryPicture.RemoteImageUrl = physicalPath;

                    using Stream fileStream = new FileStream(physicalPath, FileMode.Create);
                    await picture.CopyToAsync(fileStream);
                }
            }

            await this.inquiriesRepository.AddAsync(inquiry);
            await this.inquiriesRepository.SaveChangesAsync();

            return inquiry;
        }

        public async Task DeleteByIdAsync(int id, string imagePath)
        {
            var inquiry = await this.inquiriesRepository
                  .AllAsNoTracking()
                  .FirstOrDefaultAsync(x => x.Id == id);

            if (inquiry == null)
            {
                throw new NullReferenceException(string.Format(ServicesDataConstants.NullReferenceInquiryId, id));
            }

            this.inquiriesRepository.Delete(inquiry);
            await this.inquiriesRepository.SaveChangesAsync();

            var inquiryPictures = await this.inquiryPicturesRepository
                .AllAsNoTracking()
                .Where(s => s.InquiryId == inquiry.Id)
                .ToArrayAsync();

            foreach (var picture in inquiryPictures)
            {
                var path = $"{imagePath}/pictures/{picture.Id}.{picture.Extension}";

                File.Delete(path);

                this.inquiryPicturesRepository.HardDelete(picture);
                await this.inquiryPicturesRepository.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<InquiriesAllViewModel>> GetAllInquiriesAsync()
        {
            var inquiries = await this.inquiriesRepository
                .AllAsNoTracking()
                .OrderByDescending(i => i.CreatedOn)
                .Select(i => new InquiriesAllViewModel
                {
                    Id = i.Id,
                    RecipientName = i.RecipientName,
                    Subject = i.Subject,
                    CreateDate = i.CreatedOn.ToString("dd MMM yy HH:mm:ss"),
                })
                .OrderBy(i => i.RecipientName)
                .ToListAsync();

            return inquiries;
        }

        public async Task<InquiryDeleteViewModel> GetDeleteViewModelByIdAsync(int id)
        {
            var inquiry = await this.inquiriesRepository
                .AllAsNoTracking()
                .Where(i => i.Id == id)
                .Select(i => new InquiryDeleteViewModel
                {
                    Id = id,
                    RecipientName = i.RecipientName,
                    Subject = i.Subject,
                    Message = i.Message,
                })
                .FirstOrDefaultAsync();

            return inquiry;
        }

        public async Task<InquiryDetailsViewModel> GetDetailsViewModelByIdAsync(int id, string imagePath)
        {
            var inquiry = await this.inquiriesRepository
                 .AllAsNoTracking()
                 .Where(i => i.Id == id)
                 .Select(i => new InquiryDetailsViewModel
                 {
                     Id = i.Id,
                     RecipientName = i.RecipientName,
                     RecipientEmail = i.RecipientEmail,
                     RecipientPhoneNumber = i.RecipientPhoneNumber,
                     Subject = i.Subject,
                     Message = i.Message,
                 })
                 .FirstOrDefaultAsync();

            var pictures = await this.inquiryPicturesRepository
                .AllAsNoTracking()
                .Where(p => p.InquiryId == inquiry.Id)
                .ToArrayAsync();

            var inquiryPicturesList = new List<InquiryPicturesViewModel>();

            var basePathArr = imagePath.Split("/");
            var basePath = basePathArr[basePathArr.Length - 1];

            foreach (var picture in pictures)
            {
                var picutreName = picture.Id + "." + picture.Extension;
                var path = $"/{basePath}/pictures/{picutreName}";

                var pictureToAdd = new InquiryPicturesViewModel
                {
                    Id = picture.Id,
                    Path = path,
                };

                inquiryPicturesList.Add(pictureToAdd);
            }

            inquiry.Pictures = inquiryPicturesList;

            return inquiry;
        }
    }
}
