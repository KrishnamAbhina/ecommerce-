# Multi-Tenant Architecture Update Guide

## Overview
All endpoints now require `userId` in the route. This is a complete multi-tenant system where each user has their own data.

## ✅ Completed Updates

### DTOs Updated
- ✅ **CategoryDTO.cs** - Added `UserId` to CreateCategoryVM, CategoryVM, CategoryWithProductCountVM, CategoryWithProductsVM
- ✅ **ProductDTO.cs** - Added `UserId` to CreateProductVM, ProductVM
- ✅ **InventoryDTO.cs** - Added `UserId` to CreateInventoryVM, InventoryVM, InventorySummaryVM
- ✅ **CartDTO.cs** - Already updated with `UserId`
- ✅ **OrderDTO.cs** - Already has `UserId`

### Controllers Updated
- ✅ **CategoryController** - All endpoints now require `userId` in route

## 🔄 Controllers Still Need Updates

### 1. **ProductController**
**Pattern for all endpoints:**
```csharp
[HttpGet("{userId}")]                           // Get all products for user
[HttpGet("{userId}/id/{id}")]                   // Get product by ID
[HttpGet("{userId}/active/list")]               // Get active products
[HttpPost("{userId}")]                          // Create product
[HttpPut("{userId}/id/{id}")]                   // Update product
[HttpDelete("{userId}/id/{id}")]                // Delete product
[HttpPatch("{userId}/id/{id}/deactivate")]      // Deactivate product
[HttpPatch("{userId}/id/{id}/activate")]        // Activate product
[HttpGet("{userId}/statistics")]                // Get product statistics
```

### 2. **InventoryController**
**Pattern for all endpoints:**
```csharp
[HttpGet("{userId}")]                           // Get all inventory
[HttpGet("{userId}/id/{id}")]                   // Get inventory by ID
[HttpPost("{userId}")]                          // Create inventory
[HttpPut("{userId}/id/{id}")]                   // Update inventory
[HttpDelete("{userId}/id/{id}")]                // Delete inventory
[HttpGet("{userId}/product/{productId}")]       // Get inventory for product
[HttpGet("{userId}/summary")]                   // Get inventory summary
```

### 3. **AddressController**
**Pattern for all endpoints:**
```csharp
[HttpGet("{userId}")]                           // Get all addresses
[HttpGet("{userId}/id/{id}")]                   // Get address by ID
[HttpPost("{userId}")]                          // Create address
[HttpPut("{userId}/id/{id}")]                   // Update address
[HttpDelete("{userId}/id/{id}")]                // Delete address
[HttpPatch("{userId}/id/{id}/set-default")]    // Set default address
```

### 4. **OrdersController**
**Already partially updated, but needs:**
```csharp
[HttpPost("{userId}/create-order")]             // Create order
[HttpGet("{userId}")]                           // Get all user orders
[HttpGet("{userId}/id/{id}")]                   // Get order by ID
[HttpGet("{userId}/status/{status}")]           // Get orders by status
[HttpPut("{userId}/id/{id}/status")]            // Update order status
[HttpGet("{userId}/summary")]                   // Get order summary
```

### 5. **CartController**
**Already updated** - All endpoints use `userId`

## Service Layer Updates Needed

All service methods need to accept `userId` parameter and pass it to DAOs.

## DAO Layer Updates Needed

All DAO methods need to filter by `userId`:
```sql
WHERE UserId = @userId
```

## API Endpoint Pattern

All endpoints follow this pattern:
```
METHOD /api/Resource/{userId}[/sub-resource]
```

Examples:
```
POST   /api/Category/user-001
GET    /api/Category/user-001
GET    /api/Category/user-001/id/1
PUT    /api/Category/user-001/id/1
DELETE /api/Category/user-001/id/1
```

## Key Points

1. **Every route must include `userId`** - No exceptions
2. **Validate userId** - Return 400 BadRequest if missing
3. **Filter by userId at DAO level** - Prevent cross-user data access
4. **DTOs must include userId** - In responses
5. **All service methods** - Must accept and pass userId to DAOs

## Status Summary

- ✅ CategoryDTO: Updated
- ✅ ProductDTO: Updated
- ✅ InventoryDTO: Updated  
- ✅ CartDTO: Already had userId
- ✅ OrderDTO: Already had userId
- ✅ CategoryController: Updated
- 🔄 ProductController: Needs update
- 🔄 InventoryController: Needs update
- 🔄 AddressController: Needs update
- 🔄 OrdersController: Needs update
- 🔄 All Services: Need userId parameter
- 🔄 All DAOs: Need userId filtering

