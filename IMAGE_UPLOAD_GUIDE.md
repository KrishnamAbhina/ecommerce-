# Product Image Upload Guide

## Overview
Products now support image uploads. Images are validated, stored on the server, and the image URL is saved to the database.

## Features
✅ Upload images when creating products
✅ Upload/update images when editing products
✅ Automatic image validation (size, format)
✅ Unique filename generation
✅ Organized storage in `wwwroot/uploads/products/`

## Supported Image Formats
- `.jpg` / `.jpeg`
- `.png`
- `.gif`
- `.webp`

## Constraints
- **Maximum file size**: 5MB
- **Allowed formats**: JPG, PNG, GIF, WebP only
- **Storage location**: `wwwroot/uploads/products/`

## API Endpoints

### Create Product with Image
```
POST /api/product
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

Form Data:
- Name: string (required)
- Description: string (required)
- Price: decimal (required)
- CategoryId: int (required)
- Quantity: int (required)
- Image: IFormFile (optional)
```

**Response (Success):**
```json
{
  "id": 123,
  "message": "Product added successfully"
}
```

**Response (Image Error):**
```json
{
  "error": "We couldn't add the product",
  "details": "File size exceeds maximum allowed size of 5MB"
}
```

### Update Product with Image
```
PUT /api/product/id/{productId}
Authorization: Bearer {admin_token}
Content-Type: multipart/form-data

Form Data:
- Name: string (required)
- Description: string (required)
- Price: decimal (required)
- CategoryId: int (required)
- Quantity: int (required)
- Image: IFormFile (optional)
```

## Frontend Example (JavaScript/Fetch)

```javascript
const formData = new FormData();
formData.append('name', 'New Product');
formData.append('description', 'Product description');
formData.append('price', '99.99');
formData.append('categoryId', 1);
formData.append('quantity', 10);
formData.append('image', imageFileInput.files[0]); // File from input[type="file"]

const response = await fetch('https://localhost:44394/api/product', {
  method: 'POST',
  headers: {
    'Authorization': `Bearer ${token}`
  },
  body: formData
});

const result = await response.json();
console.log(result);
```

## Frontend Example (React)

```jsx
import { useState } from 'react';

export function AddProductForm() {
  const [image, setImage] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
    price: '',
    categoryId: '',
    quantity: ''
  });

  const handleSubmit = async (e) => {
    e.preventDefault();
    
    const data = new FormData();
    data.append('name', formData.name);
    data.append('description', formData.description);
    data.append('price', formData.price);
    data.append('categoryId', formData.categoryId);
    data.append('quantity', formData.quantity);
    
    if (image) {
      data.append('image', image);
    }

    try {
      const response = await fetch('https://localhost:44394/api/product', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${localStorage.getItem('token')}`
        },
        body: data
      });

      if (response.ok) {
        const result = await response.json();
        console.log('Product created:', result.id);
      } else {
        const error = await response.json();
        console.error('Error:', error.details);
      }
    } catch (err) {
      console.error('Request failed:', err);
    }
  };

  return (
    <form onSubmit={handleSubmit}>
      <input 
        type="text" 
        placeholder="Product Name"
        value={formData.name}
        onChange={(e) => setFormData({...formData, name: e.target.value})}
        required
      />
      
      <textarea 
        placeholder="Description"
        value={formData.description}
        onChange={(e) => setFormData({...formData, description: e.target.value})}
        required
      />
      
      <input 
        type="number" 
        step="0.01"
        placeholder="Price"
        value={formData.price}
        onChange={(e) => setFormData({...formData, price: e.target.value})}
        required
      />
      
      <input 
        type="number" 
        placeholder="Category ID"
        value={formData.categoryId}
        onChange={(e) => setFormData({...formData, categoryId: e.target.value})}
        required
      />
      
      <input 
        type="number" 
        placeholder="Quantity"
        value={formData.quantity}
        onChange={(e) => setFormData({...formData, quantity: e.target.value})}
        required
      />
      
      <input 
        type="file" 
        accept="image/*"
        onChange={(e) => setImage(e.target.files[0])}
      />
      
      <button type="submit">Add Product</button>
    </form>
  );
}
```

## File Storage

Images are stored in: `wwwroot/uploads/products/`

The database stores the relative URL path: `/uploads/products/{uuid}.{ext}`

Example:
```
Database: /uploads/products/123e4567-e89b-12d3-a456-426614174000.jpg
Disk: wwwroot/uploads/products/123e4567-e89b-12d3-a456-426614174000.jpg
Web URL: https://localhost:44394/uploads/products/123e4567-e89b-12d3-a456-426614174000.jpg
```

## Error Handling

### Invalid File Type
```json
{
  "error": "We couldn't add the product",
  "details": "File type '.pdf' is not allowed. Allowed types: .jpg, .jpeg, .png, .gif, .webp"
}
```

### File Too Large
```json
{
  "error": "We couldn't add the product",
  "details": "File size exceeds maximum allowed size of 5MB"
}
```

### Missing Required Field
```json
{
  "error": "Product name is required."
}
```

## Files Modified
- **DAO/DTO/ProductDTO.cs** - Added `IFormFile? Image` to CreateProductVM and UpdateProductVM
- **API/Controller/ProductController.cs** - Updated AddProduct and UpdateProduct endpoints for multipart/form-data
- **BLL/Services/ProductService.cs** - Added image upload handling
- **Utilities/FileUploadHelper.cs** - New utility class for file operations

## Security
✅ File type validation (whitelist only image formats)
✅ File size limit (5MB max)
✅ Unique filenames (prevents overwrites)
✅ Authorization required (admin role only)
✅ Organized directory structure
