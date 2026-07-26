# API Gateway + Services

Microservice orchestration pattern with an API gateway routing to backend services.

## Stack
- **Gateway**: NGINX / Kong API Gateway
- **Services**: .NET 10 Product Service, .NET 10 User Service
- **Database**: PostgreSQL (per-service)
- **Message Queue**: RabbitMQ for async communication
- **Infrastructure**: Docker, Kubernetes
