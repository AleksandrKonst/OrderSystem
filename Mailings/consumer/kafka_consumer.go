package consumer

import (
	"context"
	"encoding/json"
	"fmt"
	"log"
	"strings"
	"time"

	"github.com/confluentinc/confluent-kafka-go/v2/kafka"
)

type KafkaConsumer struct {
	consumer    *kafka.Consumer
	topics      []string
	stopChannel chan struct{}
}

func NewKafkaConsumer(bootstrapServers string, groupID string, topics []string) (*KafkaConsumer, error) {
	consumer, err := kafka.NewConsumer(&kafka.ConfigMap{
		"bootstrap.servers":       bootstrapServers,
		"group.id":                groupID,
		"auto.offset.reset":       "earliest",
		"enable.auto.commit":      true,
		"auto.commit.interval.ms": 5000,
	})

	if err != nil {
		return nil, err
	}

	log.Printf("Создан Kafka consumer, подключение к: %s, группа: %s", bootstrapServers, groupID)
	return &KafkaConsumer{
		consumer:    consumer,
		topics:      topics,
		stopChannel: make(chan struct{}),
	}, nil
}

func (c *KafkaConsumer) Start(ctx context.Context) error {
	err := c.consumer.SubscribeTopics(c.topics, nil)
	if err != nil {
		return fmt.Errorf("ошибка подписки на топики %v: %w", c.topics, err)
	}

	log.Printf("Подписка на топики: %s", strings.Join(c.topics, ", "))

	go func() {
		for {
			select {
			case <-ctx.Done():
				log.Println("Получен сигнал остановки")
				c.Stop()
				return
			case <-c.stopChannel:
				log.Println("Получен сигнал остановки")
				return
			default:
				msg, err := c.consumer.ReadMessage(100 * time.Millisecond)
				if err != nil {
					if err.(kafka.Error).Code() != kafka.ErrTimedOut {
						log.Printf("Ошибка чтения сообщения: %v", err)
					}
					continue
				}

				c.handleMessage(msg)
			}
		}
	}()

	return nil
}

func (c *KafkaConsumer) Stop() {
	close(c.stopChannel)
	if err := c.consumer.Close(); err != nil {
		log.Printf("Ошибка закрытия Kafka consumer: %v", err)
	}
}

func (c *KafkaConsumer) handleMessage(msg *kafka.Message) {
	topic := *msg.TopicPartition.Topic
	key := string(msg.Key)
	value := string(msg.Value)

	fmt.Printf("\n\n==== Новое сообщение ====\n")
	fmt.Printf("Топик: %s\n", topic)
	fmt.Printf("Ключ: %s\n", key)
	fmt.Printf("Значение: %s\n", value)

	eventType := getEventTypeFromKey(key)
	fmt.Printf("Тип события: %s\n\n", eventType)

	switch eventType {
	case "OrderCreatedEvent":
		handleOrderCreated(value)
	case "OrderStatusChangedEvent":
		handleOrderStatusChanged(value)
	case "OrderCancelledEvent":
		handleOrderCancelled(value)
	case "OrderDiscountAppliedEvent":
		handleOrderDiscountApplied(value)
	case "OrderDiscountRemovedEvent":
		handleOrderDiscountRemoved(value)
	default:
		fmt.Printf("Неизвестный тип события: %s\n", eventType)
	}
}

func getEventTypeFromKey(key string) string {
	parts := strings.Split(key, "-")
	if len(parts) > 0 {
		return parts[0]
	}
	return "Unknown"
}

func handleOrderCreated(value string) {
	fmt.Println("✉️ ОТПРАВКА УВЕДОМЛЕНИЯ: Новый заказ создан")
	fmt.Println("📧 Тема: Спасибо за ваш заказ!")
	fmt.Println("📝 Текст: Ваш заказ успешно создан и скоро будет обработан.")
	fmt.Println("🔄 Статус: Создан")
}

func handleOrderStatusChanged(value string) {
	var event struct {
		NewStatus string `json:"newStatus"`
	}

	if err := json.Unmarshal([]byte(value), &event); err != nil {
		fmt.Printf("Ошибка разбора JSON: %v\n", err)
		return
	}

	fmt.Println("✉️ ОТПРАВКА УВЕДОМЛЕНИЯ: Статус заказа изменен")
	fmt.Println("📧 Тема: Обновление статуса вашего заказа")
	fmt.Printf("📝 Текст: Статус вашего заказа изменен на: %s\n", event.NewStatus)
	fmt.Printf("🔄 Новый статус: %s\n", event.NewStatus)
}

func handleOrderCancelled(value string) {
	var event struct {
		Reason string `json:"reason"`
	}

	if err := json.Unmarshal([]byte(value), &event); err != nil {
		fmt.Printf("Ошибка разбора JSON: %v\n", err)
		return
	}

	fmt.Println("✉️ ОТПРАВКА УВЕДОМЛЕНИЯ: Заказ отменен")
	fmt.Println("📧 Тема: Ваш заказ был отменен")
	fmt.Printf("📝 Текст: К сожалению, ваш заказ был отменен. Причина: %s\n", event.Reason)
	fmt.Println("❌ Статус: Отменен")
}

func handleOrderDiscountApplied(value string) {
	var event struct {
		DiscountName       string  `json:"discountName"`
		DiscountPercentage float64 `json:"discountPercentage"`
		OriginalAmount     struct {
			Amount   float64 `json:"amount"`
			Currency string  `json:"currency"`
		} `json:"originalAmount"`
		DiscountedAmount struct {
			Amount   float64 `json:"amount"`
			Currency string  `json:"currency"`
		} `json:"discountedAmount"`
	}

	if err := json.Unmarshal([]byte(value), &event); err != nil {
		fmt.Printf("Ошибка разбора JSON: %v\n", err)
		return
	}

	fmt.Println("✉️ ОТПРАВКА УВЕДОМЛЕНИЯ: Применена скидка к заказу")
	fmt.Println("📧 Тема: К вашему заказу применена скидка!")
	fmt.Printf("📝 Текст: К вашему заказу применена скидка '%s' (%.2f%%).\n",
		event.DiscountName, event.DiscountPercentage)
	fmt.Printf("💰 Исходная сумма: %.2f %s\n",
		event.OriginalAmount.Amount, event.OriginalAmount.Currency)
	fmt.Printf("💸 Сумма со скидкой: %.2f %s\n",
		event.DiscountedAmount.Amount, event.DiscountedAmount.Currency)
	fmt.Printf("🔥 Вы сэкономили: %.2f %s\n",
		event.OriginalAmount.Amount-event.DiscountedAmount.Amount,
		event.OriginalAmount.Currency)
}

func handleOrderDiscountRemoved(value string) {
	var event struct {
		DiscountId     string `json:"discountId"`
		OriginalAmount struct {
			Amount   float64 `json:"amount"`
			Currency string  `json:"currency"`
		} `json:"originalAmount"`
		DiscountedAmount struct {
			Amount   float64 `json:"amount"`
			Currency string  `json:"currency"`
		} `json:"discountedAmount"`
	}

	if err := json.Unmarshal([]byte(value), &event); err != nil {
		fmt.Printf("Ошибка разбора JSON: %v\n", err)
		return
	}

	fmt.Println("✉️ ОТПРАВКА УВЕДОМЛЕНИЯ: Скидка удалена из заказа")
	fmt.Println("📧 Тема: Изменение в вашем заказе")
	fmt.Println("📝 Текст: Скидка была удалена из вашего заказа.")
	fmt.Printf("💰 Новая сумма заказа: %.2f %s\n",
		event.OriginalAmount.Amount, event.OriginalAmount.Currency)
}
