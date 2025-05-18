# Микросервис управления промоакциями

## Возможности

- Получение активных промоакций для конкретного товара
- Получение всех активных промоакций
- Создание, обновление и удаление промоакций
- Назначение промоакций товарам

## API Endpoints

### REST API (HTTP)

Базовый URL: `http://localhost:8080/api/v1`

#### Промоакции

- `GET /promotions` - Получить все активные промоакции
- `POST /promotions` - Создать новую промоакцию
- `GET /promotions/{id}` - Получить промоакцию по ID
- `PUT /promotions/{id}` - Обновить промоакцию
- `DELETE /promotions/{id}` - Удалить промоакцию

#### Промоакции товаров

- `GET /product-promotions/{productID}` - Получить промоакцию для конкретного товара
- `POST /product-promotions` - Назначить промоакцию товару

### gRPC API

Сервис предоставляет следующие методы gRPC:

- `GetProductPromotion(GetProductPromotionRequest) returns (PromotionResponse)` - Получить промоакцию для товара
- `GetAllActivePromotions(GetAllActivePromotionsRequest) returns (PromotionsListResponse)` - Получить все активные промоакции

## Запуск сервиса

```bash
# Установка зависимостей
go mod tidy

# Запуск сервиса
go run main.go
```

Сервис запустится на:
- REST API: http://localhost:8080
- gRPC сервер: localhost:50051

## Примеры использования

### Создание промоакции (REST)

```bash
curl -X POST http://localhost:8080/api/v1/promotions \
  -H "Content-Type: application/json" \
  -d '{
    "name": "Летняя распродажа",
    "description": "Скидка 25% на летние товары",
    "discount_percentage": 25.0,
    "valid_until": "2023-12-31T23:59:59Z"
  }'
```

### Назначение промоакции товару (REST)

```bash
curl -X POST http://localhost:8080/api/v1/product-promotions \
  -H "Content-Type: application/json" \
  -d '{
    "product_id": "product123",
    "promotion_id": "promo1"
  }'
```

### Получение промоакции товара (REST)

```bash
curl http://localhost:8080/api/v1/product-promotions/product123
```

## Зависимости

- [go-chi/chi](https://github.com/go-chi/chi) - Для маршрутизации REST API
- gRPC - Для gRPC API 