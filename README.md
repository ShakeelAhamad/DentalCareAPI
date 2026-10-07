# DentalCare API

DentalCare API is an ASP.NET Core REST API for managing a dental clinic workflow, including authentication, admin operations, dentist schedules, patient records, and appointment booking.

## Base URLs

When running locally in development mode:

- HTTPS: https://localhost:7059
- HTTP: http://localhost:5103
- OpenAPI document: https://localhost:7059/openapi/v1.json

## Postman Collection

The Postman collection is available at [`PostmanCollection/DentalCareAPI.postman_collection.json`](./PostmanCollection/DentalCareAPI.postman_collection.json).

To use it:

1. Start the API using `dotnet run`.
2. In Postman, select **Import** and choose the collection JSON file linked above.
3. Set the collection or environment variable `baseURL` to `https://localhost:7059/api` (or `http://localhost:5103/api`).
4. Send the requests from the collection. For protected endpoints, log in first and provide a valid bearer access token.

## Project Modules

The project is organized into the following API modules:

| Module | Purpose | Access |
| --- | --- | --- |
| Auth | Login, registration, token refresh, and logout | Public / authenticated users |
| Admin | Manage dentists, services, patients, and appointments | Admin only |
| Dentist | Manage working hours, appointments, and treatment records | Dentist only |
| Patient | Create patient records and view patient appointments | Patient only |
| Appointment | Check availability, create, and cancel appointments | Authenticated users |
| Weather Forecast | Sample API endpoint for testing | Public |

## Step-by-Step Architecture Overview

### Step 1: Controllers
The `Controllers` folder exposes the HTTP API. Each controller handles incoming requests, calls the appropriate service, and returns structured responses.

- `AuthController` - handles registration, login, logout, token refresh, and revoke-token actions.
- `AdminController` - manages dentists, services, patient records, and appointment operations.
- `DentistController` - manages dentist schedules, working hours, appointment status, and treatment creation.
- `PatientController` - handles patient creation and patient-specific appointment queries.
- `AppointmentController` - checks availability, creates appointments, and cancels appointments.
- `WeatherForecastController` - sample template controller included for testing.

### Step 2: Services
The `Services` folder contains business logic and validation rules. Services coordinate repository access and enforce application flow.

- `IAuthService` and `AuthService` - registration, login, JWT generation, refresh token handling, and logout revoke logic.
- `IAdminService` and `AdminService` - admin operations for dentists, services, and clinic-wide appointment management.
- `IDentistService` and `DentistService` - dentist scheduling, treatment additions, and doctor-specific data access.
- `IAppointmentService` and `AppointmentService` - availability checks, appointment creation, cancellation, and status updates.
- `IPatientService` and `PatientService` - patient creation and patient query workflows.

### Step 3: Repositories
The `Repositories` folder isolates database interaction from business logic.

- `IRepository<T>` and `Repository<T>` provide the shared generic CRUD functionality for all entities.
- `IUserRepository` / `UserRepository` manage users, email checks, username checks, and role-aware queries.
- `IRoleRepository` / `RoleRepository` manage roles such as Admin, Dentist, and Patient.
- `IRefreshTokenRepository` / `RefreshTokenRepository` manage refresh token creation, cleanup, and invalidation.
- `IDentistRepository` / `DentistRepository` handle dentist-specific lookups.
- `IPatientRepository` / `PatientRepository` access patient data.
- `IServicesRepository` / `ServicesRepository` manage clinic service records.
- `ITreatmentRepository` / `TreatmentRepository` handle treatment queries.
- `IWorkHourRepository` / `WorkHoursRepository` manage dentist working hours.
- `IBlockedDateRepository` / `BlockedDateRepository` manage blocked dates.
- `IAppointmentRepository` / `AppointmentRepository` manage appointment query and update logic.

### Step 4: Data Layer
The `Data` folder includes the EF Core database context.

- `ApplicationDbContext` defines all DbSets for the system.
- It configures relations between users, roles, dentists, patients, appointments, services, treatments, working hours, and blocked dates.
- It applies indexes for better database performance and uniqueness.
- It seeds the default `Admin`, `Dentist`, and `Patient` roles, plus an initial admin account.

### Step 5: Models
The `Models` folder contains all database entities:

- `User` - login credentials, personal data, role, and status.
- `Role` - application roles.
- `RefreshToken` - token lifecycle for authentication.
- `Dentist` - dentist profile and linked user record.
- `Patient` - patient profile and linked user record.
- `Appointment` - scheduled clinic appointments.
- `Service` - available services/treatments in the clinic.
- `Treatment` - treatment details tied to an appointment.
- `WorkingHour` - a dentist's available schedule blocks.
- `BlockedDate` - dates the dentist is unavailable.

### Step 6: DTOs
The `DTOs` folder contains request and response objects used by the API layer.

Examples include:

- `LoginDto`, `LoginResponseDto`, `RefreshTokenRequestDto`, `RefreshTokenResponseDto`
- `AdminDto`, `AdminResponseDto`
- `DentistDto`, `DentistResponseDto`, `UpdateDentistDto`
- `PatientDto`, `PatientResponseDto`, `PatientListResponseDto`, `PatientDetailsResponseDto`
- `AppointmentDto`, `AppointmentResponseDto`, `AppointmentListDto`, `AppointmentViewDto`
- `AvailableSlotsRequestDto`, `AvailableSlotsResponseDto`, `AvailableSlotDto`
- `ServiceDto`, `ServiceResponseDto`
- `TreatmentDto`, `TreatmentResponseDto`
- `WorkingHourDto`, `WorkingHourResponseDto`
- `PaginatedResponseDto`

These DTOs standardize API communication and keep entity models separate from client contracts.

### Step 7: Constants
The `Constants` folder stores shared fixed values.

- `RoleConstants` defines:
  - `Admin`
  - `Dentist`
  - `Patient`

These values are used in authorization and role checks across the project.

### Step 8: Extensions
The `Extensions` folder contains utility methods that help reduce repetitive logic.

- `ClaimsPrincipalExtensions` adds `GetUserId()` for reading the authenticated user ID from JWT claims.

### Step 9: Startup and Dependency Injection
`Program.cs` configures the application startup and is responsible for:

- registering controllers and OpenAPI setup
- configuring EF Core and SQL Server
- registering repositories and services
- configuring JWT authentication
- adding authorization policies like `AdminOnly`, `DentistOnly`, `PatientOnly`, and `AuthenticatedUser`
- enabling the pipeline for authentication and authorization

## Authentication

This API uses JWT-based authentication.

After a successful login, include the token in the request header:

```http
Authorization: Bearer <your_token>
```

Role-based policies are used for authorization:

- AdminOnly
- DentistOnly
- PatientOnly
- AuthenticatedUser

## Auth Module APIs

Base path: `/api/auth`

| Method | URL | Description |
| --- | --- | --- |
| POST | `/api/auth/create` | Create an admin account |
| POST | `/api/auth/login` | Log in and receive JWT token |
| POST | `/api/auth/logout` | Log out and revoke refresh token |
| POST | `/api/auth/refresh-token` | Refresh access token using refresh token |
| POST | `/api/auth/revoke-token` | Revoke a refresh token |

## Admin Module APIs

Base path: `/api/admin`

| Method | URL | Description |
| --- | --- | --- |
| POST | `/api/admin/dentists/create` | Create a dentist |
| PUT | `/api/admin/dentists/update/{userId}` | Update dentist information |
| GET | `/api/admin/dentists` | Get all dentists |
| DELETE | `/api/admin/dentists/delete/{userId}` | Delete a dentist |
| POST | `/api/admin/services/create` | Create a dental service |
| PUT | `/api/admin/services/update/{Id}` | Update a service |
| DELETE | `/api/admin/services/delete/{Id}` | Delete a service |
| GET | `/api/admin/services` | Get all services |
| GET | `/api/admin/appointments` | Get all appointments |
| GET | `/api/admin/appointments/{appointmentId}` | Get appointment by id |
| DELETE | `/api/admin/appointments/delete/{Id}` | Delete an appointment |
| PUT | `/api/admin/appointments/update-status/{appointmentId}` | Update appointment status |
| GET | `/api/admin/patients` | Get all patients |
| GET | `/api/admin/patients/{patientId}` | Get patient by id |

## Dentist Module APIs

Base path: `/api/dentist`

| Method | URL | Description |
| --- | --- | --- |
| POST | `/api/dentist/schedule` | Create dentist working hours/schedule |
| GET | `/api/dentist` | Get all working hours for the logged-in dentist |
| DELETE | `/api/dentist/delete/{Id}` | Delete a working hour entry |
| GET | `/api/dentist/appointments` | Get appointments for the dentist |
| GET | `/api/dentist/appointments/{appointmentId}` | Get appointment details by id |
| PUT | `/api/dentist/appointments/{appointmentId}/status` | Update appointment status |
| POST | `/api/dentist/appointments/{appointmentId}/create` | Add treatment to an appointment |
| GET | `/api/dentist/treatments` | Get all treatments |
| GET | `/api/dentist/treatments/{treatmentId}` | Get treatment by id |

## Patient Module APIs

Base path: `/api/patient`

| Method | URL | Description |
| --- | --- | --- |
| POST | `/api/patient/create` | Create a patient record |
| GET | `/api/patient/appointments` | Get patient appointments |
| GET | `/api/patient/appointments/{appointmentId}` | Get patient appointment by id |

## Appointment Module APIs

Base path: `/api/appointment`

| Method | URL | Description |
| --- | --- | --- |
| POST | `/api/appointment/available-slots` | Get available appointment slots |
| PUT | `/api/appointment/{appointmentId}/cancel` | Cancel an appointment |
| POST | `/api/appointment/create` | Create a new appointment |

## Weather Forecast API

Base path: `/WeatherForecast`

| Method | URL | Description |
| --- | --- | --- |
| GET | `/WeatherForecast` | Returns sample weather forecast data |

## Example Request

### Login

```http
POST https://localhost:7059/api/auth/login
Content-Type: application/json

{
  "email": "admin@example.com",
  "password": "YourPassword123"
}
```

### Get all dentists

```http
GET https://localhost:7059/api/admin/dentists
Authorization: Bearer <your_token>
```

## Run the API

From the project folder, run:

```bash
dotnet restore
dotnet run
```

The app will start using the development URLs configured in `Properties/launchSettings.json`.

## Notes

- OpenAPI generation is enabled in development mode (`MapOpenApi()`).
- The project uses SQL Server via the connection string in `appsettings.json`.
- Refresh tokens are stored and managed as part of the authentication workflow.
