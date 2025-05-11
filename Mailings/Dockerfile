FROM golang:1.21 as builder

WORKDIR /app

COPY . .

RUN go mod edit -replace=github.com/joho/godotenv=github.com/joho/godotenv@v1.5.1
RUN go mod edit -replace=github.com/confluentinc/confluent-kafka-go/v2=github.com/confluentinc/confluent-kafka-go/v2@v2.3.0
RUN go get -u ./...
RUN go mod tidy

RUN CGO_ENABLED=1 GOOS=linux go build -o mailings-service .

FROM debian:bookworm-slim

WORKDIR /app

RUN apt-get update && apt-get install -y --no-install-recommends \
    ca-certificates \
    librdkafka-dev \
    && rm -rf /var/lib/apt/lists/*

COPY --from=builder /app/mailings-service .
COPY config.env .

CMD ["./mailings-service"] 