# Promotions Microservice

A microservice for managing product promotions with both REST API and gRPC interfaces.

## Features

- Get active promotions for a specific product
- Get all active promotions
- Create, update, and delete promotions
- Assign promotions to products

## API Endpoints

### REST API (HTTP)

Base URL: `http://localhost:8080/api/v1`

#### Promotions

- `GET /promotions` - Get all active promotions
- `POST /promotions` - Create a new promotion
- `GET /promotions/{id}` - Get a specific promotion by ID
- `PUT /promotions/{id}` - Update a promotion
- `DELETE /promotions/{id}` - Delete a promotion

#### Product Promotions

- `GET /product-promotions/{productID}` - Get promotion for a specific product
- `POST /product-promotions` - Assign a promotion to a product

### gRPC API

The service exposes the following gRPC methods:

- `GetProductPromotion(GetProductPromotionRequest) returns (PromotionResponse)`
- `GetAllActivePromotions(GetAllActivePromotionsRequest) returns (PromotionsListResponse)`

## Running the Service

```bash
# Install dependencies
go mod download

# Run the service
go run cmd/main.go
```

The service will start:
- REST API at http://localhost:8080
- gRPC server at localhost:50051

## Example Usage

### Create a Promotion (REST)

```bash
curl -X POST http://localhost:8080/api/v1/promotions \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Summer Sale",
    "description": "25% off on summer items",
    "discount_percentage": 25.0,
    "valid_until": "2023-12-31T23:59:59Z"
  }'
```

### Assign a Promotion to a Product (REST)

```bash
curl -X POST http://localhost:8080/api/v1/product-promotions \
  -H "Content-Type: application/json" \
  -d '{
    "product_id": "product123",
    "promotion_id": "promo1"
  }'
```

### Get a Product's Promotion (REST)

```bash
curl http://localhost:8080/api/v1/product-promotions/product123
```

## Dependencies

- [go-chi/chi](https://github.com/go-chi/chi) - For REST API routing
- [gRPC](https://grpc.io/) - For gRPC API 