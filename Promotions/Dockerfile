FROM golang:latest AS build

WORKDIR /app

COPY . .

RUN CGO_ENABLED=0 GOOS=linux go build -o /promotions-service ./main.go

FROM alpine:latest

RUN apk --no-cache add ca-certificates tzdata
RUN mkdir /app

WORKDIR /app

COPY --from=build /promotions-service .

EXPOSE 8080

CMD ["/app/promotions-service"] 