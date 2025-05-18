# Сервис рассылок (Mailings)

Микросервис для обработки событий заказов и отправки уведомлений.

## Обзор

Сервис подписывается на события заказов в Kafka и имитирует отправку уведомлений для следующих событий:
- Создание заказа
- Изменение статуса заказа
- Отмена заказа
- Применение скидки к заказу
- Удаление скидки из заказа

## Запуск

### В Docker Compose

```bash
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