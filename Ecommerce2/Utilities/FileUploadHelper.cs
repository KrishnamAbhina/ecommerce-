using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace ecommerce_application.Utilities
{
    public static class FileUploadHelper
    {
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        private const long MaxFileSize = 5 * 1024 * 1024; // 5MB
        private const string UploadDirectory = "wwwroot/uploads/products";

        /// <summary>
        /// Upload a product image and return the file path
        /// </summary>
        public static async Task<string?> UploadProductImageAsync(IFormFile? image)
        {
            if (image == null || image.Length == 0)
            {
                return null;
            }

            // Validate file size
            if (image.Length > MaxFileSize)
            {
                throw new InvalidOperationException($"File size exceeds maximum allowed size of {MaxFileSize / (1024 * 1024)}MB");
            }

            // Validate file extension
            var extension = Path.GetExtension(image.FileName).ToLower();
            if (!Array.Exists(AllowedExtensions, ext => ext == extension))
            {
                throw new InvalidOperationException($"File type '{extension}' is not allowed. Allowed types: {string.Join(", ", AllowedExtensions)}");
            }

            try
            {
                // Create directory if it doesn't exist
                if (!Directory.Exists(UploadDirectory))
                {
                    Directory.CreateDirectory(UploadDirectory);
                }

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(UploadDirectory, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // Return relative path for storage in database
                return $"/uploads/products/{fileName}";
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error uploading file: {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Delete a product image file
        /// </summary>
        public static void DeleteProductImage(string? imagePath)
        {
            if (string.IsNullOrEmpty(imagePath))
            {
                return;
            }

            try
            {
                // imagePath is already in format: /uploads/products/{filename}
                var filePath = Path.Combine("wwwroot", imagePath.TrimStart('/'));

                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
            }
            catch (Exception ex)
            {
                // Log error but don't throw - file deletion failure shouldn't break the operation
                Console.WriteLine($"Error deleting image file: {ex.Message}");
            }
        }
    }
}
