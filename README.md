#  FakeStore Gateway API

API Gateway desarrollada con .NET 10 que consume y extiende la funcionalidad de [FakeStore API](https://fakestoreapi.com/), implementando patrones de diseño profesionales, Clean Architecture y buenas prácticas de desarrollo.

##  Arquitectura

El proyecto sigue **Clean Architecture** con separación clara de responsabilidades:

```
FakeStore.Gateway/
│
├── 📂 Api/                          # Capa de Presentación
│   ├── Controllers/                 # Endpoints REST
│   ├── Extensions/                  # Configuración de DI
│   ├── Program.cs                   # Pipeline HTTP
│   └── Dockerfile                   # Contenedor Docker
│
├── 📂 Application/                  # Capa de Lógica de Negocio
│   ├── Commons/
│   │   ├── Bases/                  # BaseResponse, BaseResponseList
│   │   ├── Enums/                  # ErrorCode
│   │   └── Helpers/                # BaseService
│   ├── DTOs/                       # Data Transfer Objects
│   ├── Interfaces/                 # Contratos de servicios
│   ├── Mappers/                    # Transformaciones
│   ├── Services/                   # Lógica CRUD y Caché
│   ├── Validators/                 # FluentValidation
│   └── Extensions/                 # DI de Application
│
├── 📂 Infrastructure/               # Capa de Infraestructura
│   ├── Clients/                    # Cliente HTTP externo
│   ├── Configuration/              # Settings tipadas
│   └── Extensions/                 # DI + Polly
│
└── 📂 Domain/                       # Capa de Dominio
    └── Entities/                   # Entidades de negocio
```

##  Tecnologías

- .NET 10.0
- ASP.NET Core Web API
- FluentValidation 11.3.1
- Polly (via Microsoft.Extensions.Http.Polly 10.0.1)
- Swashbuckle.AspNetCore 10.0.1
- Docker
- C# 14.0

##  Instalación

### Opción 1: Ejecución Local

```bash
# Clonar el repositorio
git clone https://github.com/AlexGP7199/FakeStoreAPI_Gateway-Sigma.git
cd FakeStoreAPI_Gateway-Sigma

# Restaurar dependencias
dotnet restore

# Ejecutar la aplicación
dotnet run --project FakeStore.Gateway.Api

# Abrir Swagger en el navegador
# https://localhost:7038/swagger
```

### Opción 2: Docker

```bash
# Build de la imagen
docker build -t fakestore-gateway -f FakeStore.Gateway.Api/Dockerfile .

# Ejecutar contenedor
docker run -d -p 8080:8080 --name fakestore-api fakestore-gateway

# Acceder a la API
# http://localhost:8080/swagger
```

### Opción 3: Docker Hub

```bash
# Pull de la imagen desde Docker Hub
docker pull moonxz/fakestore-gateway-api:v1

# Ejecutar contenedor
docker run -d -p 8080:8080 --name fakestore-api moonxz/fakestore-gateway-api:v1
```

##  Configuración

Edita `appsettings.json` para personalizar:

```json
{
  "FakeStoreApi": {
    "BaseUrl": "https://fakestoreapi.com/",
    "TimeoutSeconds": 30,
    "RetryCount": 3,
    "CircuitBreakerThreshold": 5,
    "CircuitBreakerDurationSeconds": 30
  },
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:3000",
      "http://localhost:4200",
      "http://localhost:5173"
    ]
  }
}
```

### Parámetros de Configuración

| Parámetro | Descripción | Valor por Defecto |
|-----------|-------------|-------------------|
| `BaseUrl` | URL de FakeStore API | `https://fakestoreapi.com/` |
| `TimeoutSeconds` | Timeout de peticiones HTTP | `30` |
| `RetryCount` | Número de reintentos | `3` |
| `CircuitBreakerThreshold` | Fallos antes de abrir circuito | `5` |
| `CircuitBreakerDurationSeconds` | Duración del circuito abierto | `30` |
| `AllowedOrigins` | Orígenes permitidos para CORS | Array de URLs |

##  Endpoints

### Products

| Método | Endpoint | Descripción | Respuesta |
|--------|----------|-------------|-----------|
| `GET` | `/api/products` | Obtener todos los productos | `200 OK` |
| `GET` | `/api/products/{id}` | Obtener producto por ID | `200 OK`, `404 Not Found` |
| `POST` | `/api/products` | Crear nuevo producto | `201 Created`, `400 Bad Request` |
| `PUT` | `/api/products/{id}` | Actualizar producto | `200 OK`, `400 Bad Request`, `404 Not Found` |
| `DELETE` | `/api/products/{id}` | Eliminar producto | `200 OK`, `404 Not Found` |

### Ejemplo de Request (POST)

```json
POST /api/products
Content-Type: application/json

{
  "title": "Producto Demo",
  "price": 99.99,
  "description": "Descripción del producto",
  "category": "electronics",
  "image": "https://example.com/image.jpg"
}
```

### Ejemplo de Response

```json
{
  "isSuccess": true,
  "data": {
    "id": 21,
    "title": "Producto Demo",
    "price": 99.99,
    "description": "Descripción del producto",
    "category": "electronics",
    "image": "https://example.com/image.jpg"
  },
  "message": "Producto creado exitosamente",
  "errores": null,
  "errorCode": 0
}
```

### Códigos de Error

| Código | Descripción | HTTP Status |
|--------|-------------|-------------|
| `0` | Sin error | `200 OK` |
| `ValidationError` | Error de validación | `400 Bad Request` |
| `NotFound` | Recurso no encontrado | `404 Not Found` |
| `Conflict` | Conflicto de datos | `409 Conflict` |
| `InternalServerError` | Error interno | `500 Internal Server Error` |
| `ServiceUnavailable` | Servicio no disponible | `503 Service Unavailable` |
| `GatewayTimeout` | Timeout de gateway | `504 Gateway Timeout` |

##  Patrones Implementados

### Repository Pattern
```csharp
public interface IFakeStoreApiClient
{
    Task<IEnumerable<Products>> GetProductsAsync();
    Task<Products?> GetProductByIdAsync(int id);
    // ...
}
```
**Ventaja:** Abstracción del acceso a datos externos.

### Service Layer Pattern
```csharp
public class ProductsService : BaseService, IProductsService
{
    // Lógica de negocio centralizada
}
```
**Ventaja:** Controladores delgados, lógica reutilizable.

### DTO Pattern
```csharp
CreateProductDto, UpdateProductDto, ProductDto
```
**Ventaja:** Control sobre contratos de API, prevención de over-posting.

### Validator Pattern
```csharp
public class CreateProductDtoValidator : AbstractValidator<CreateProductDto>
{
    // Reglas de validación declarativas
}
```
**Ventaja:** Validaciones expresivas y testables.

### Base Service Pattern
```csharp
public abstract class BaseService
{
    protected async Task<T> ExecuteAsync<T>(T response, Func<Task> action)
    {
        // Manejo centralizado de excepciones
    }
}
```
**Ventaja:** DRY (Don't Repeat Yourself), manejo consistente de errores.

### Options Pattern
```csharp
services.Configure<FakeStoreApiSettings>(configuration.GetSection("FakeStoreApi"));
```
**Ventaja:** Configuración tipada y externa.

##  Resiliencia

### Retry Policy
- **Reintentos:** 3 intentos
- **Estrategia:** Backoff exponencial (2^n segundos)
- **Cobertura:** Errores HTTP transitorios (500, 502, 503, 504)

```csharp
// Intento 1: espera 2 segundos
// Intento 2: espera 4 segundos
// Intento 3: espera 8 segundos
```

### Circuit Breaker
- **Umbral:** 5 fallos consecutivos
- **Duración:** 30 segundos abierto
- **Protección:** Evita colapsar la API externa

```
[Cerrado] → 5 fallos → [Abierto (30s)] → [Semi-Abierto] → [Cerrado]
```

##  Docker

### Multi-Stage Build

El Dockerfile está optimizado con múltiples etapas:

1. **Base** - Runtime .NET 10 (aspnet)
2. **Build** - SDK .NET 10 + compilación
3. **Publish** - Publicación optimizada
4. **Final** - Solo runtime + binarios (~220MB)

### Ventajas
- Imagen final ligera (solo runtime, sin SDK)
- Caché de capas optimizado
- Seguridad (usuario no-root)
- Portabilidad total

### Docker Compose (ejemplo)

```yaml
version: '3.8'

services:
  fakestore-gateway:
    image: moonxz/fakestore-gateway-api:v1
    container_name: fakestore-backend
    ports:
      - "8080:8080"
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    restart: unless-stopped
```

##  Estructura del Proyecto

### API Layer (Presentación)
- **Controllers:** ProductsController con endpoints REST
- **Extensions:** ServiceCollectionExtensions (orquestador de DI)
- **Program.cs:** Pipeline HTTP, CORS, Swagger, Middleware

### Application Layer (Lógica de Negocio)
- **Services:** 
  - ProductsService (CRUD con lógica de negocio)
  - ProductCacheService (3 diccionarios concurrentes)
  - BaseService (manejo centralizado de excepciones)
- **DTOs:** CreateProductDto, UpdateProductDto, ProductDto
- **Validators:** FluentValidation rules para cada DTO
- **Mappers:** ProductMapper para transformaciones entidad-DTO
- **Commons:** 
  - BaseResponse<T>, BaseResponseList<T>
  - ErrorCode enum
  - BaseService helper

### Infrastructure Layer (Acceso a Datos)
- **Clients:** FakeStoreApiClient con HttpClient tipado
- **Configuration:** FakeStoreApiSettings (Options pattern)
- **Extensions:** InfrastructureServiceExtensions (DI + Polly policies)

### Domain Layer (Entidades)
- **Entities:** Products (entidad pura de negocio sin dependencias)

##  Licencia

Este proyecto es de código abierto para fines educativos.

##  Autor

**Alex GP**  
GitHub: [@AlexGP7199](https://github.com/AlexGP7199)  
Proyecto: [FakeStore Gateway API](https://github.com/AlexGP7199/FakeStoreAPI_Gateway-Sigma)

---

⭐ **Si te gusta este proyecto, dale una estrella en GitHub!**

 **Documentación completa:** [Swagger UI](https://localhost:7038/swagger) (cuando ejecutes localmente)
