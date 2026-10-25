# Image Upload - Quick Reference

## What Changed?

| Component | Change |
|-----------|--------|
| **CreateProductVM** | Added `IFormFile? Image` |
| **UpdateProductVM** | Added `IFormFile? Image` |
| **POST /api/product** | Changed to `multipart/form-data` |
| **PUT /api/product/id/{id}** | Changed to `multipart/form-data` |
| **New Utility** | `FileUploadHelper.cs` - Handles uploads |

## Minimum Frontend Example

```javascript
// Get form data
const formData = new FormData();
formData.append('name', 'Product Name');
formData.append('description', 'Product Description');
formData.append('price', 99.99);
formData.append('categoryId', 1);
formData.append('quantity', 10);
formData.append('image', imageFile); // From file input

// Send request
const response = await fetch('https://localhost:44394/api/product', {
  method: 'POST',
  headers: { 'Authorization': 'Bearer ' + token },
  body: formData
});

const result = await response.json();
console.log('Product ID:', result.id);
```

## Allowed Image Types
- `.jpg` / `.jpeg`
- `.png`
- `.gif`
- `.webp`

**Max Size:** 5MB

## Storage
- **Path:** `wwwroot/uploads/products/`
- **URL:** `https://localhost:44394/uploads/products/{uuid}.{ext}`
- **DB:** `/uploads/products/{uuid}.{ext}`

## Postman Test
1. POST `https://localhost:44394/api/product`
2. Auth: Bearer Token (admin)
3. Body: form-data
4. Fields:
   - name (Text)
   - description (Text)
   - price (Text)
   - categoryId (Text)
   - quantity (Text)
   - image (File) ← Select image

## Response Examples

### Success
```json
{
  "id": 123,
  "message": "Product added successfully"
}
```

### Error - Invalid File Type
```json
{
  "error": "We couldn't add the product",
  "details": "File type '.pdf' is not allowed. Allowed types: .jpg, .jpeg, .png, .gif, .webp"
}
```

### Error - File Too Large
```json
{
  "error": "We couldn't add the product",
  "details": "File size exceeds maximum allowed size of 5MB"
}
```

## Code Locations

| Need | Location |
|------|----------|
| Upload logic | `Utilities/FileUploadHelper.cs` |
| API endpoints | `API/Controller/ProductController.cs` |
| DTOs | `DAO/DTO/ProductDTO.cs` |
| Service logic | `BLL/Services/ProductService.cs` |

## Things to Remember

✅ Image is **optional** (can create product without image)
✅ **Admin role required** for upload
✅ Images stored with **GUID names** (secure, unique)
✅ Images **publicly accessible** via HTTP
✅ **Relative URLs** in database (flexible, no hardcoded domain)
✅ **Auto-validation**: Type & size checked automatically

## One-Line Setup

Add `app.UseStaticFiles();` to Program.cs before `app.MapControllers();`

## For React Component

```jsx
<input 
  type="file" 
  accept="image/*"
  onChange={(e) => setImage(e.target.files[0])}
/>
```

Then append to FormData:
```jsx
formData.append('image', image);
```

## TypeScript Interface

```typescript
interface CreateProductVM {
  name: string;
  description: string;
  price: number;
  categoryId: number;
  quantity: number;
  imageUrl?: string;
  image?: File;
}
```

## Common Issues & Fixes

| Issue | Fix |
|-------|-----|
| 400 Bad Request | Check `Content-Type: multipart/form-data` |
| 401 Unauthorized | Add valid admin JWT token |
| File not found on disk | Check `wwwroot/uploads/products/` exists |
| Image returns 404 | Add `app.UseStaticFiles();` to Program.cs |
| File size error | Upload file < 5MB |
| Invalid file type error | Use .jpg, .png, .gif, or .webp only |

## Documentation Files

- **`IMAGE_UPLOAD_GUIDE.md`** - Complete API reference
- **`IMAGE_UPLOAD_IMPLEMENTATION.md`** - Technical deep dive
- **`IMAGE_UPLOAD_SETUP.md`** - Setup, config, deployment
- **`PRODUCT_IMAGE_UPLOAD_SUMMARY.md`** - Full implementation summary
- **`IMAGE_UPLOAD_QUICK_REFERENCE.md`** - This file

---

**Status:** ✅ Implementation Complete - Ready to Use
