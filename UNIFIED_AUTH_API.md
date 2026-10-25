# Unified Authentication & User Management API

## Overview
Single Auth controller handles:
- User registration (role: "user")
- Admin registration (role: "admin")
- Common login for both user and admin
- User/Admin management (all stored in single UserModel table)
- Profile management
- Token management

---

## API Endpoints

### 1. Register User
```
POST /api/Auth/register-user
{
  "fullName": "John Doe",
  "email": "john@example.com",
  "password": "Password@123",
  "confirmPassword": "Password@123",
  "phoneNumber": "1234567890"
}

Response: 200 OK
{
  "message": "User registered successfully",
  "data": {
    "id": "user-uuid",
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

### 2. Register Admin
```
POST /api/Auth/register-admin
{
  "fullName": "Admin User",
  "email": "admin@example.com",
  "password": "Admin@123",
  "confirmPassword": "Admin@123",
  "phoneNumber": "9999999999"
}

Response: 200 OK
{
  "message": "Admin registered successfully",
  "data": {
    "id": "admin-uuid",
    "fullName": "Admin User",
    "email": "admin@example.com",
    "phoneNumber": "9999999999",
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token",
    "role": "admin"
  }
}
```

---

### 3. Login (Common for Both User & Admin)
```
POST /api/Auth/login
{
  "email": "john@example.com",
  "password": "Password@123"
}

Response: 200 OK
{
  "message": "Login successful",
  "data": {
    "id": "user-uuid",
    "fullName": "John Doe",
    "email": "john@example.com",
    "accessToken": "jwt-token",
    "refreshToken": "refresh-token",
    "role": "user"
  }
}
```

---

### 4. Get All Users & Admins
```
GET /api/Auth/all

Response: 200 OK
[
  {
    "id": "user1-uuid",
    "fullName": "John Doe",
    "email": "john@example.com",
    "phoneNumber": "1234567890",
    "role": "user",
    "createdAt": "2026-10-05T10:00:00Z",
    "isActive": true
  },
  {
    "id": "admin1-uuid",
    "fullName": "Admin User",
    "email": "admin@example.com",
    "phoneNumber": "9999999999",
    "role": "admin",
    "createdAt": "2026-10-05T08:00:00Z",
    "isActive": true
  }
]
```

---

### 5. Get User/Admin by ID
```
GET /api/Auth/{userId}

Response: 200 OK
{
  "id": "user-uuid",
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "1234567890",
  "role": "user",
  "createdAt": "2026-10-05T10:00:00Z",
  "isActive": true
}
```

---

### 6. Delete User or Admin Account
```
DELETE /api/Auth/{userId}

Response: 200 OK
{
  "message": "User user-uuid deleted successfully"
}
```

---

### 7. Update User/Admin Role
```
PUT /api/Auth/{userId}/role
{
  "role": "admin"
}

Valid roles: "user", "admin", "moderator"

Response: 200 OK
{
  "message": "Role updated to admin successfully"
}
```

---

### 8. Deactivate User/Admin
```
PUT /api/Auth/{userId}/deactivate

Response: 200 OK
{
  "message": "User user-uuid deactivated successfully"
}
```

---

### 9. Activate User/Admin
```
PUT /api/Auth/{userId}/activate

Response: 200 OK
{
  "message": "User user-uuid activated successfully"
}
```

---

### 10. Get Profile
```
GET /api/Auth/profile?userId={userId}

Response: 200 OK
{
  "id": "user-uuid",
  "fullName": "John Doe",
  "email": "john@example.com",
  "phoneNumber": "1234567890",
  "role": "user"
}
```

---

### 11. Update Profile
```
PUT /api/Auth/profile?userId={userId}
{
  "fullName": "John Updated",
  "phoneNumber": "9876543210"
}

Response: 200 OK
{
  "message": "Profile updated successfully"
}
```

---

### 12. Change Password
```
PUT /api/Auth/change-password?userId={userId}
{
  "currentPassword": "Password@123",
  "newPassword": "NewPassword@456",
  "confirmPassword": "NewPassword@456"
}

Response: 200 OK
{
  "message": "Password changed successfully"
}
```

---

### 13. Refresh Token
```
POST /api/Auth/refresh-token
{
  "refreshToken": "your-refresh-token"
}

Response: 200 OK
{
  "message": "Token refreshed successfully",
  "data": {
    "id": "user-uuid",
    "fullName": "John Doe",
    "email": "john@example.com",
    "accessToken": "new-jwt-token",
    "refreshToken": "new-refresh-token",
    "role": "user"
  }
}
```

---

### 14. Logout
```
POST /api/Auth/logout?userId={userId}

Response: 200 OK
{
  "message": "Logout successful"
}
```

---

### 15. Dashboard Statistics
```
GET /api/Auth/stats/dashboard

Response: 200 OK
{
  "totalUsers": 50,
  "totalAdmins": 3,
  "totalRegistrations": 53,
  "activeAccounts": 45,
  "totalOrders": 200,
  "totalRevenue": 50000.00,
  "totalProducts": 100,
  "generatedAt": "2026-10-05T12:00:00Z"
}
```

---

## Complete User/Admin Flow

### User Journey
1. **Register** → POST `/api/Auth/register-user`
2. **Login** → POST `/api/Auth/login`
3. **Get Profile** → GET `/api/Auth/profile?userId={id}`
4. **Update Profile** → PUT `/api/Auth/profile?userId={id}`
5. **Change Password** → PUT `/api/Auth/change-password?userId={id}`
6. **Logout** → POST `/api/Auth/logout?userId={id}`
7. **Delete Account** → DELETE `/api/Auth/{userId}`

### Admin Journey
1. **Register Admin** → POST `/api/Auth/register-admin`
2. **Login** → POST `/api/Auth/login`
3. **Get All Users/Admins** → GET `/api/Auth/all`
4. **Get Specific User** → GET `/api/Auth/{userId}`
5. **Change User Role** → PUT `/api/Auth/{userId}/role`
6. **Deactivate User** → PUT `/api/Auth/{userId}/deactivate`
7. **Activate User** → PUT `/api/Auth/{userId}/activate`
8. **Delete User** → DELETE `/api/Auth/{userId}`
9. **View Dashboard** → GET `/api/Auth/stats/dashboard`
10. **Logout** → POST `/api/Auth/logout?userId={id}`

---

## Database Storage
All users and admins are stored in the **Users** table with these fields:

| Field | Type | Description |
|-------|------|-------------|
| Id | string (GUID) | Unique identifier |
| FullName | string | User/Admin full name |
| Email | string | Email address |
| PhoneNumber | string | Contact number |
| PasswordHash | string | Hashed password (SHA256) |
| **Role** | string | `"user"` / `"admin"` / `"moderator"` |
| **IsActive** | bool | Account active status |
| CreatedAt | DateTime | Registration date |
| UpdatedAt | DateTime | Last update date |

---

## Authentication

### Access Token
- Valid for: **1 hour**
- Type: **JWT**
- Used in: All authenticated requests

### Refresh Token
- Valid for: **7 days**
- Revoked on: **logout**
- Use to get new access token when it expires

### Token Claims
```json
{
  "userId": "user-uuid",
  "email": "user@example.com",
  "role": "user"
}
```

---

## Error Responses

### Invalid Credentials
```json
{
  "error": "Invalid email or password."
}
```

### User Not Found
```json
{
  "error": "User with ID {userId} not found."
}
```

### Invalid Password
```json
{
  "error": "Current password is incorrect."
}
```

### Duplicate Email
```json
{
  "error": "Email already registered."
}
```

### Server Error
```json
{
  "error": "Error during login",
  "details": "Exception message"
}
```

---

## HTTP Status Codes

- **200 OK** - Success
- **400 Bad Request** - Invalid input
- **404 Not Found** - Resource not found
- **500 Internal Server Error** - Server error

---

## Summary

✅ **Single Auth Controller** - Handles all authentication and user management
✅ **Common Login** - Works for both users and admins
✅ **Single Database** - All stored in UserModel table
✅ **Role-Based** - Distinguish user/admin by role field
✅ **Unified Endpoints** - No separate admin controller needed
✅ **Complete Management** - Create, read, update, delete users/admins
