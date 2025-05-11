FROM golang:1.23-alpine AS build

WORKDIR /app

# Copy the source code
COPY . .

# Build the application (не обновляем зависимости)
RUN CGO_ENABLED=0 GOOS=linux go build -o /promotions-service ./cmd/main.go

# Use a small alpine image for the final stage
FROM alpine:latest

# Add ca certificates and set timezone
RUN apk --no-cache add ca-certificates tzdata
RUN mkdir /app

WORKDIR /app

# Copy the binary from the build stage
COPY --from=build /promotions-service .

# Expose the port the service runs on
EXPOSE 8080

# Run the binary
CMD ["/app/promotions-service"] 