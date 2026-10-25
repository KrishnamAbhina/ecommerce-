# Image Upload Implementation Summary

## What Was Added

### 1. **Data Transfer Objects (DTOs)**
- **DAO/DTO/ProductDTO.cs**
  - Added `IFormFile? Image` to `CreateProductVM`
  - Added `IFormFile? Image` to `UpdateProductVM`

### 2. **File Upload Utility**
- **Utilities/FileUploadHelper.cs** (NEW)
  - `UploadProductImageAsync()` - Handles file upload, validation, and storage
  - `DeleteProductImage()` - Cleans up old images
  - Validates: file type (JPG, PNG, GIF, WebP), file size (max 5MB)
  - Generates unique filenames using GUID
  - Saves to: `wwwroot/uploads/products/`

### 3. **API Controller Updates**
- **API/Controller/ProductController.cs**
  - Updated `AddProduct()` endpoint
    - Changed from `[FromBody]` to `[FromForm]`
    - Added `[Consumes("multipart/form-data")]`
  - Updated `UpdateProduct()` endpoint
    - Changed from `[FromBody]` to `[FromForm]`
    - Added `[Consumes("multipart/form-data")]`

### 4. **Business Logic Layer Updates**
- **BLL/Services/ProductService.cs**
  - Added import: `using ecommerce_application.Utilities;`
  - Updated `AddProductAsync(CreateProductVM model)` - Processes image upload
  - Updated `AddProductAsync(string userId, CreateProductVM model)` - Processes image upload
  - Updated `UpdateProductAsync(int id, UpdateProductVM model)` - Processes image upload
  - Updated `UpdateProductAsync(string userId, int id, UpdateProductVM model)` - Processes image upload

## How It Works

### Create Product with Image:
```
1. User sends POST /api/product with multipart/form-data
2. Controller receives form data with optional Image file
3. ProductService calls FileUploadHelper.UploadProductImageAsync()
4. Helper validates file (type, size)
5. Helper generates unique filename with GUID
6. Helper saves file to wwwroot/uploads/products/
7. Helper returns relative URL path
8. ProductService sets model.ImageUrl to the path
9. ProductDAO saves product with ImageUrl to database
```

### Update Product with Image:
```
Same as create, but:
1. ProductService processes image upload (if provided)
2. ProductDAO updates existing product
3. Old image URL is preserved if no new image provided
```

## API Usage

### Create Product with Image
```bash
curl -X POST "https://localhost:44394/api/product" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -F "name=iPhone 15" \
  -F "description=Latest Apple phone" \
  -F "price=999.99" \
  -F "categoryId=1" \
  -F "quantity=50" \
  -F "image=@path/to/image.jpg"
```

### Update Product with Image
```bash
curl -X PUT "https://localhost:44394/api/product/id/1" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -F "name=iPhone 15 Pro" \
  -F "description=Updated description" \
  -F "price=1099.99" \
  -F "categoryId=1" \
  -F "quantity=50" \
  -F "image=@path/to/new_image.jpg"
```

## Validation

### File Type Whitelist:
- ✅ `.jpg`, `.jpeg`
- ✅ `.png`
- ✅ `.gif`
- ✅ `.webp`

### File Size Limit:
- ✅ Maximum: 5MB

### Error Responses:
```json
{
  "error": "We couldn't add the product",
  "details": "File type '.pdf' is not allowed. Allowed types: .jpg, .jpeg, .png, .gif, .webp"
}
```

## Storage Structure

```
Project Root
└── wwwroot/
    └── uploads/
        └── products/
            ├── 123e4567-e89b-12d3-a456-426614174000.jpg
            ├── 234e5678-f9ac-23e4-b567-526725285111.png
            └── 345f6789-0abd-34f5-c678-637836396222.gif
```

## Database Schema
The existing `Products` table already has `ImageUrl` column:
```csharp
public string? ImageUrl { get; set; }
```

Example value in database:
```
/uploads/products/123e4567-e89b-12d3-a456-426614174000.jpg
```

## Requirements
- ✅ Admin role required (authorization)
- ✅ Valid JWT token in Authorization header
- ✅ Content-Type must be `multipart/form-data`
- ✅ Image file is optional (can create/update products without image)

## Testing

### Manual Test in Postman:
1. Set request type to: **POST**
2. URL: `https://localhost:44394/api/product`
3. Authorization: Set Bearer token (admin)
4. Body: Select **form-data**
5. Add fields:
   - `name` (Text) = "Test Product"
   - `description` (Text) = "Test Description"
   - `price` (Text) = "99.99"
   - `categoryId` (Text) = "1"
   - `quantity` (Text) = "10"
   - `image` (File) = Select image file
6. Click Send

Response:
```json
{
  "id": 123,
  "message": "Product added successfully"
}
```

## Notes
- Images are stored with unique GUID-based filenames to prevent conflicts
- Old images are not automatically deleted on update (consider cleanup policy)
- Image URLs are relative paths for flexibility (can change domain/CDN)
- Images are accessible via: `https://your-domain/uploads/products/{filename}`
