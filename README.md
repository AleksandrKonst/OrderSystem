# Сервис рассылок (Mailings)

Микросервис для обработки событий заказов и отправки уведомлений.

## Обзор

Сервис подписывается на события заказов в Kafka и имитирует отправку уведомлений для следующих событий:
- Создание заказа
- Изменение статуса заказа
- Отмена заказа
- Применение скидки к заказу
- Удаление скидки из заказа

## Настройка

Настройки загружаются из файла `config.env` и переменных окружения:

```
KAFKA_BOOTSTRAP_SERVERS=localhost:9092  # Адрес Kafka
KAFKA_ORDERS_TOPIC=orders               # Топик для событий заказов
KAFKA_ORDERS_DISCOUNT_TOPIC=orders-discounts # Топик для событий скидок
KAFKA_GROUP_ID=mailings-service         # ID группы потребителей
```

## Запуск

### Локально

```bash
# Установка зависимостей
go mod download

# Запуск сервиса
go run main.go
```

### В Docker

```bash
# Сборка образа
docker build -t mailings-service .

# Запуск контейнера
docker run -it --net=host mailings-service
```

### В Docker Compose

```bash
# Запуск вместе с Kafka
docker-compose up -d
```

## События

Сервис обрабатывает следующие типы событий:

1. **OrderCreatedEvent** - Создание нового заказа
2. **OrderStatusChangedEvent** - Изменение статуса заказа
3. **OrderCancelledEvent** - Отмена заказа
4. **OrderDiscountAppliedEvent** - Применение скидки к заказу
5. **OrderDiscountRemovedEvent** - Удаление скидки из заказа 

# Order System с Docker Compose

Этот проект представляет собой микросервисную систему обработки заказов, состоящую из трех основных сервисов:

1. **ProcessingOrders** - основной сервис обработки заказов (C# / .NET)
2. **Mailings** - сервис для отправки уведомлений (Golang)
3. **Promotions** - сервис управления акциями и скидками (Golang)

## Архитектура системы

Система использует следующие технологии и компоненты:

- **Apache Kafka** для обмена сообщениями между сервисами
- **PostgreSQL** для хранения данных в сервисах Mailings и Promotions
- **PgAdmin** для управления PostgreSQL

## Предварительные требования

- [Docker](https://www.docker.com/get-started)
- [Docker Compose](https://docs.docker.com/compose/install/)

## Запуск системы

### 1. Клонирование репозитория

```bash
git clone <repository-url>
cd order-system
```

### 2. Запуск с помощью Docker Compose

Запустите все сервисы с помощью Docker Compose:

```bash
docker compose up -d
```

Для просмотра логов всех сервисов:

```bash
docker compose logs -f
```

Для просмотра логов конкретного сервиса:

```bash
docker compose logs -f <service-name>
```

Например:
```bash
docker compose logs -f processing-orders-api
```

### 3. Остановка системы

Для остановки всех сервисов:

```bash
docker compose down
```

Для остановки и удаления всех данных (включая тома):

```bash
docker compose down -v
```

## Доступ к сервисам

После запуска, сервисы будут доступны по следующим адресам:

- **ProcessingOrders API**: http://localhost:5000
- **Mailings Service**: http://localhost:8081
- **Promotions API**: http://localhost:8080
- **PgAdmin**: http://localhost:5050
  - Логин: admin@ordersystem.com
  - Пароль: admin
- **Kafka**: localhost:9092
- **PostgreSQL**: localhost:5432
  - Логин: postgres
  - Пароль: postgres

## Структура проекта

- `ProcessingOrders/` - сервис обработки заказов (C# / .NET)
- `Mailings/` - сервис отправки уведомлений (Golang)
- `Promotions/` - сервис управления акциями и скидками (Golang)
- `docker-compose.yml` - конфигурация Docker Compose для запуска всей системы

## Особенности Docker-конфигурации

При настройке Docker-инфраструктуры были решены следующие проблемы:

1. **Go-зависимости в сервисе Mailings** - использована техника `go mod edit -replace` для правильного управления зависимостями.
2. **Версия Go для сервиса Promotions** - обновлена до Go 1.23 для поддержки современных зависимостей.
3. **Версия .NET для сервиса ProcessingOrders** - обновлена до .NET 9.0, так как проекты нацелены на эту версию фреймворка.
4. **Конфигурации Kafka** - настроены для корректной работы всех сервисов в единой сети.
5. **Переменные окружения** - адаптированы для разных форматов конфигурации в разных сервисах.
6. **GLIBC для mailings-service** - обновлен базовый образ с debian:bullseye-slim на debian:bookworm-slim для поддержки более новых версий GLIBC.
7. **Отключен HTTPS** - для простоты настройки используется только HTTP.

## Разработка

Для разработки каждого сервиса, обратитесь к README.md в соответствующей директории. 