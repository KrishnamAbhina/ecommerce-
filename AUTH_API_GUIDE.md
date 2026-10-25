# Authentication & Admin API Guide

## Authentication Endpoints

### 1. User Registration
**POST** `https://localhost:44394/api/Auth/register`

**Request Body:**
```json
{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "Password@123",
  "confirmPassword": "Password@123",
  "phoneNumber": "1234567890"
}
```

**Response:**
```json
{
  "message": "User registered successfully",
  "data": {
    "id": "user-id",
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token",
    "role": "user"
  }
}
```

---

### 2. User Login
**POST** `https://localhost:44394/api/Auth/login`

**Request Body:**
```json
{
  "email": "john@example.com",
  "password": "Password@123"
}
```

**Response:**
```json
{
  "message": "Login successful",
  "data": {
    "id": "user-id",
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token",
    "role": "user"
  }
}
```

---

### 3. Refresh Token
**POST** `https://localhost:44394/api/Auth/refresh-token`

**Request Body:**
```json
{
  "refreshToken": "your-refresh-token"
}
```

**Response:**
```json
{
  "message": "Token refreshed successfully",
  "data": {
    "id": "user-id",
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "accessToken": "new-jwt-token",
    "refreshToken": "new-refresh-token",
    "role": "user"
  }
}
```

---

### 4. Logout
**POST** `https://localhost:44394/api/Auth/logout?userId=user-id`

**Response:**
```json
{
  "message": "Logout successful"
}
```

---

### 5. Get User Profile
**GET** `https://localhost:44394/api/Auth/profile?userId=user-id`

**Response:**
```json
{
  "id": "user-id",
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "1234567890",
  "role": "user"
}
```

---

### 6. Update User Profile
**PUT** `https://localhost:44394/api/Auth/profile?userId=user-id`

**Request Body:**
```json
{
  "fullName": "John Updated",
  "phoneNumber": "9876543210"
}
```

**Response:**
```json
{
  "message": "Profile updated successfully"
}
```

---

### 7. Change Password
**PUT** `https://localhost:44394/api/Auth/change-password?userId=user-id`

**Request Body:**
```json
{
  "currentPassword": "Password@123",
  "newPassword": "NewPassword@456",
  "confirmPassword": "NewPassword@456"
}
```

**Response:**
```json
{
  "message": "Password changed successfully"
}
```

---

## Admin Endpoints

### 1. Get All Users
**GET** `https://localhost:44394/api/Admin/users`

**Response:**
```json
[
  {
    "id": "user-id",
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "role": "user",
    "createdAt": "2026-10-05T12:00:00Z",
    "isActive": true
  }
]
```

---

### 2. Get User by ID
**GET** `https://localhost:44394/api/Admin/users/{userId}`

**Response:**
```json
{
  "id": "user-id",
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "1234567890",
  "role": "user",
  "createdAt": "2026-10-05T12:00:00Z",
  "isActive": true
}
```

---

### 3. Create User (Admin)
**POST** `https://localhost:44394/api/Admin/users/create`

**Request Body:**
```json
{
  "fullName": "Jane Doe",
  "email": "jane@example.com",
  "password": "Password@123",
  "phoneNumber": "9876543210",
  "role": "admin"
}
```

**Response:**
```json
{
  "message": "User created successfully"
}
```

---

### 4. Update User Role
**PUT** `https://localhost:44394/api/Admin/users/{userId}/role`

**Request Body:**
```json
{
  "role": "admin"
}
```

**Roles:** `user`, `admin`, `moderator`

**Response:**
```json
{
  "message": "User role updated to admin successfully"
}
```

---

### 5. Deactivate User
**PUT** `https://localhost:44394/api/Admin/users/{userId}/deactivate`

**Response:**
```json
{
  "message": "User {userId} deactivated successfully"
}
```

---

### 6. Activate User
**PUT** `https://localhost:44394/api/Admin/users/{userId}/activate`

**Response:**
```json
{
  "message": "User {userId} activated successfully"
}
```

---

### 7. Delete User
**DELETE** `https://localhost:44394/api/Admin/users/{userId}`

**Response:**
```json
{
  "message": "User {userId} deleted successfully"
}
```

---

### 8. Dashboard Statistics
**GET** `https://localhost:44394/api/Admin/dashboard/stats`

**Response:**
```json
{
  "totalUsers": 50,
  "activeUsers": 45,
  "totalOrders": 200,
  "totalRevenue": 50000.00,
  "totalProducts": 100,
  "adminCount": 3,
  "generatedAt": "2026-10-05T12:00:00Z"
}
```

---

## User Flow Example

### 1. Register a new user
```bash
POST /api/Auth/register
```

### 2. Login with credentials
```bash
POST /api/Auth/login
```

### 3. Get user profile
```bash
GET /api/Auth/profile?userId={received-userId}
```

### 4. Create address
```bash
POST /api/Address?userId={userId}
```

### 5. Add items to cart
```bash
POST /api/Cart/add-item
```

### 6. Create order
```bash
POST /api/Orders/create-order
```

---

## Admin Flow Example

### 1. Login as admin
```bash
POST /api/Auth/login
```

### 2. View all users
```bash
GET /api/Admin/users
```

### 3. Create new admin user
```bash
POST /api/Admin/users/create
```

### 4. Update user role
```bash
PUT /api/Admin/users/{userId}/role
```

### 5. View dashboard stats
```bash
GET /api/Admin/dashboard/stats
```

---

## Test Users (for development)

After migration, these default users will be available:

| Email | Password | Role |
|-------|----------|------|
| admin@example.com | Admin@123 | admin |
| user@example.com | User@123 | user |

---

## Notes

- **Access Token**: Valid for 1 hour
- **Refresh Token**: Valid for 7 days
- **Password Requirements**: Minimum 6 characters
- **Role Restrictions**: Only admins can access `/api/Admin` endpoints
- **User Creation**: Admins can create users with specific roles
