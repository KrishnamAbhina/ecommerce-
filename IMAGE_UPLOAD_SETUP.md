# Image Upload Setup & Configuration

## Step 1: Ensure wwwroot Directory Exists

The uploads directory will be created automatically by the `FileUploadHelper` if it doesn't exist.

**Manual creation (optional):**
```bash
mkdir wwwroot
mkdir wwwroot/uploads
mkdir wwwroot/uploads/products
```

## Step 2: Configure Program.cs for Static Files

Add static file serving in `Program.cs`:

```csharp
// Add after app.UseAuthentication()
app.UseStaticFiles();
```

**Example in context:**
```csharp
var app = builder.Build();

app.UseCors("ReactPolicy");
app.MapSwagger();
app.MapSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseStaticFiles();  // ← Add this line

app.MapControllers();
```

## Step 3: Verify File Structure

After implementation, your project should have:

```
Ecommerce2/
├── API/
│   └── Controller/
│       └── ProductController.cs ✓ (Updated)
├── BLL/
│   └── Services/
│       └── ProductService.cs ✓ (Updated)
├── DAO/
│   └── DTO/
│       └── ProductDTO.cs ✓ (Updated)
├── Utilities/
│   └── FileUploadHelper.cs ✓ (NEW - Handle uploads)
├── wwwroot/
│   ├── uploads/
│   │   └── products/ ← Images stored here
│   └── ...existing static files
├── Program.cs ✓ (Add UseStaticFiles())
└── ...
```

## Step 4: File Permissions

Ensure the application has write permissions to `wwwroot/uploads/products/`:

### On Windows:
```bash
# IIS or app pool user needs Modify permission
# Usually automatic with application directory
```

### On Linux:
```bash
chmod 755 wwwroot/uploads/products
```

## Step 5: Test the Implementation

### Test 1: Create Product with Image
```bash
# Using curl
curl -X POST "https://localhost:44394/api/product" \
  -H "Authorization: Bearer YOUR_ADMIN_TOKEN" \
  -F "name=Test Product" \
  -F "description=Test" \
  -F "price=99.99" \
  -F "categoryId=1" \
  -F "quantity=10" \
  -F "image=@test-image.jpg"
```

### Test 2: Verify Image is Accessible
```
Open in browser:
https://localhost:44394/uploads/products/{filename}.jpg
```

## Configuration Options

### Change Maximum File Size
Edit `FileUploadHelper.cs` line 13:
```csharp
private const long MaxFileSize = 5 * 1024 * 1024; // Change 5 to your desired size in MB
```

### Change Upload Directory
Edit `FileUploadHelper.cs` line 14:
```csharp
private const string UploadDirectory = "wwwroot/uploads/products"; // Change path as needed
```

### Add More Image Formats
Edit `FileUploadHelper.cs` line 11:
```csharp
private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".bmp" };
```

## Common Issues & Solutions

### Issue: 400 Bad Request when uploading image
**Cause:** Content-Type not set to `multipart/form-data`
**Solution:** Ensure API client sets `Content-Type: multipart/form-data`

### Issue: 401 Unauthorized
**Cause:** Missing or invalid authorization token
**Solution:** Include valid admin JWT token in Authorization header

### Issue: Image saved but returns 404 when accessed
**Cause:** `UseStaticFiles()` not added to Program.cs
**Solution:** Add `app.UseStaticFiles();` before `app.MapControllers();`

### Issue: "File size exceeds maximum allowed size"
**Cause:** File is larger than 5MB limit
**Solution:** Either increase `MaxFileSize` constant or upload smaller image

### Issue: "File type '.pdf' is not allowed"
**Cause:** Only images are allowed, not PDFs
**Solution:** Upload image format (.jpg, .png, .gif, .webp)

### Issue: Permission denied error on file save
**Cause:** Application doesn't have write permission to wwwroot directory
**Solution:** 
- On Windows: Set folder permissions for app pool user
- On Linux: Run `chmod 755 wwwroot/uploads/products`

## Deployment Considerations

### For Production:
1. **Persistent Storage**: If deploying to cloud (Azure, AWS, etc.), consider:
   - Azure Blob Storage instead of local filesystem
   - AWS S3 instead of local filesystem
   - Modify `FileUploadHelper.cs` to use cloud storage

2. **CDN Integration**: Serve images through CDN:
   - CloudFlare
   - AWS CloudFront
   - Azure CDN

3. **Image Optimization**:
   - Add image compression before saving
   - Generate thumbnails for faster loading
   - Consider WebP optimization

### Example: Modify for Azure Blob Storage
```csharp
// Instead of saving to disk:
var blobClient = new BlobClient(new Uri(blobUri), new DefaultAzureCredential());
await blobClient.UploadAsync(stream, overwrite: true);
return blobUri.ToString();
```

## API Documentation Update

Update your Swagger documentation:

```csharp
[HttpPost]
[Authorize(Roles = "admin")]
[Consumes("multipart/form-data")]
[ProducesResponseType(StatusCodes.Status201Created)]
[ProducesResponseType(StatusCodes.Status400BadRequest)]
[ProducesResponseType(StatusCodes.Status401Unauthorized)]
[ProducesResponseType(StatusCodes.Status403Forbidden)]
public async Task<IActionResult> AddProduct([FromForm] CreateProductVM model)
{
    // Implementation
}
```

## Client-Side Setup (React)

Install axios for easier multipart requests:
```bash
npm install axios
```

Example hook:
```javascript
import { useState } from 'react';
import axios from 'axios';

export function useProductUpload() {
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);

  const uploadProduct = async (productData, imageFile, token) => {
    setLoading(true);
    setError(null);

    try {
      const formData = new FormData();
      Object.entries(productData).forEach(([key, value]) => {
        formData.append(key, value);
      });
      
      if (imageFile) {
        formData.append('image', imageFile);
      }

      const response = await axios.post(
        'https://localhost:44394/api/product',
        formData,
        {
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'multipart/form-data'
          }
        }
      );

      return response.data;
    } catch (err) {
      setError(err.response?.data?.details || 'Upload failed');
      throw err;
    } finally {
      setLoading(false);
    }
  };

  return { uploadProduct, loading, error };
}
```

## Summary Checklist

- ✅ Add `[FromForm]` and `[Consumes("multipart/form-data")]` to controller endpoints
- ✅ Add `IFormFile? Image` to DTOs
- ✅ Create `FileUploadHelper.cs` utility class
- ✅ Update ProductService to call FileUploadHelper
- ✅ Add `app.UseStaticFiles()` to Program.cs
- ✅ Create `wwwroot/uploads/products/` directory (auto-created by helper)
- ✅ Test image upload functionality
- ✅ Verify images are accessible via HTTP
- ✅ Document API changes in Swagger
