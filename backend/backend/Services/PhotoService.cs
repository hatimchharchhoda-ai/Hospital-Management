using backend.Exceptions;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;

namespace backend.Services
{
    public interface IPhotoService
    {
        Task<(string Url, string PublicId)> UploadPrescriptionImageAsync(IFormFile file);
    }

    public class PhotoService : IPhotoService
    {
        private readonly Cloudinary _cloudinary;

        public PhotoService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<(string Url, string PublicId)> UploadPrescriptionImageAsync(IFormFile file)
        {
            // 1️⃣ Null / empty check
            if (file == null || file.Length == 0)
                throw new AppException("No image file was uploaded");

            // 2️⃣ File size validation (5 MB)
            const long maxFileSize = 4 * 1024 * 1024; // 5 MB
            if (file.Length > maxFileSize)
                throw new AppException("Image size must be less than 4 MB");

            // 3️⃣ File extension validation
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();

            if (!allowedExtensions.Contains(fileExtension))
                throw new AppException("Only JPG, JPEG, and PNG image formats are allowed");

            // 4️⃣ MIME type validation (extra safety)
            var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedMimeTypes.Contains(file.ContentType))
                throw new AppException("Invalid image content type");

            // 5️⃣ Upload to Cloudinary
            using var stream = file.OpenReadStream();

            var uploadParams = new ImageUploadParams
            {
                File = new FileDescription(file.FileName, stream),
                Folder = "prescriptions",
                Transformation = new Transformation()
                    .Quality("auto")
                    .FetchFormat("auto")
            };

            var result = await _cloudinary.UploadAsync(uploadParams);

            if (result.Error != null)
                throw new AppException(result.Error.Message);

            return (result.SecureUrl.ToString(), result.PublicId);
        }
    }
}
