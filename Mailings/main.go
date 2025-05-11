package main

import (
	"context"
	"log"
	"os"
	"os/signal"
	"syscall"

	"github.com/joho/godotenv"
	"mailings/consumer"
)

func main() {
	err := godotenv.Load("config.env")
	if err != nil {
		log.Printf("Предупреждение: не удалось загрузить файл .env: %v", err)
	}

	bootstrapServers := getEnv("KAFKA_BOOTSTRAP_SERVERS", "localhost:9092")
	ordersTopic := getEnv("KAFKA_ORDERS_TOPIC", "orders")
	ordersDiscountTopic := getEnv("KAFKA_ORDERS_DISCOUNT_TOPIC", "orders-discounts")
	groupID := getEnv("KAFKA_GROUP_ID", "mailings-service")

	topics := []string{ordersTopic, ordersDiscountTopic}

	ctx, cancel := context.WithCancel(context.Background())
	defer cancel()

	kafkaConsumer, err := consumer.NewKafkaConsumer(bootstrapServers, groupID, topics)
	if err != nil {
		log.Fatalf("Ошибка создания Kafka consumer: %v", err)
	}

	err = kafkaConsumer.Start(ctx)
	if err != nil {
		log.Fatalf("Ошибка запуска Kafka consumer: %v", err)
	}

	log.Println("Сервис рассылок запущен!")
	log.Printf("Прослушивание топиков: %v", topics)

	sigChan := make(chan os.Signal, 1)
	signal.Notify(sigChan, syscall.SIGINT, syscall.SIGTERM)
	<-sigChan

	log.Println("Получен сигнал завершения, закрываем сервис...")
	cancel()
}

func getEnv(key, defaultValue string) string {
	if value, exists := os.LookupEnv(key); exists {
		return value
	}
	return defaultValue
}
