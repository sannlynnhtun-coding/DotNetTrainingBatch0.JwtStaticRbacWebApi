# JWT Static RBAC Web API

## Project Overview
This project demonstrates a **JWT (JSON Web Token)** authentication implementation in a .NET 8 Web API with **Static Role-Based Access Control (RBAC)**. In this model, roles and permissions are directly associated with the user entity and embedded into the JWT token during the authentication process.

## Step-by-Step Flow

### 1. Authentication (Login)
- The client sends a POST request to `api/auth/login` with `Username` and `Password`.
- The `AuthService` validates the credentials against the `TblUsers` table in the database.
- Upon successful validation, the server retrieves the user's **Role** (e.g., "Admin", "Staff") and **Permissions** (e.g., "Product.View", "Product.Create") which are stored as static fields or simple lists in the user record.

### 2. Token Generation
- The server generates a JWT containing:
  - `sub`: User ID
  - `unique_name`: Username
  - `role`: User's Role(s)
  - `permission`: Custom claims for specific access rights.
- The token is signed using a secret key and returned to the client.

### 3. Authorized Requests
- The client includes the JWT in the `Authorization` header as a `Bearer` token for subsequent API calls.
- The `JwtBearer` middleware validates the token's signature, issuer, and expiration.

### 4. Authorization Check
- Controllers or Actions are decorated with `[Authorize(Roles = "Admin")]` or specific policies.
- The system checks the claims within the decrypted JWT to grant or deny access.

## Key Components
- **AuthService**: Handles login logic and JWT creation.
- **AppDbContext**: Manages the database connection and user entities.
- **Program.cs**: Configures JWT Authentication and Authorization policies.
