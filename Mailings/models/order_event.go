package models

import (
	"time"
)

type OrderEvent struct {
	EventId    string    `json:"eventId"`
	OccurredOn time.Time `json:"occurredOn"`
	OrderId    OrderId   `json:"orderId"`
}

type OrderId struct {
	Value int64 `json:"value"`
}

type CustomerId struct {
	Value int64 `json:"value"`
}

type Money struct {
	Amount   float64 `json:"amount"`
	Currency string  `json:"currency"`
}

type OrderCreatedEvent struct {
	OrderEvent
	CustomerId  CustomerId `json:"customerId"`
	TotalAmount Money      `json:"totalAmount"`
}

type OrderItemAddedEvent struct {
	OrderEvent
	Item OrderItem `json:"item"`
}

type OrderItemRemovedEvent struct {
	OrderEvent
	ItemId OrderItemId `json:"itemId"`
}

type OrderItemId struct {
	Value int64 `json:"value"`
}

type OrderItem struct {
	Id          OrderItemId `json:"id"`
	OrderId     OrderId     `json:"orderId"`
	ProductId   string      `json:"productId"`
	ProductName string      `json:"productName"`
	Quantity    int         `json:"quantity"`
	Price       Money       `json:"price"`
}

type OrderStatusChangedEvent struct {
	OrderEvent
	PreviousStatus string `json:"previousStatus"`
	NewStatus      string `json:"newStatus"`
}

type OrderCancelledEvent struct {
	OrderEvent
	PreviousStatus string `json:"previousStatus"`
	Reason         string `json:"reason"`
}

type OrderDiscountAppliedEvent struct {
	OrderEvent
	DiscountId         string  `json:"discountId"`
	DiscountName       string  `json:"discountName"`
	DiscountPercentage float64 `json:"discountPercentage"`
	OriginalAmount     Money   `json:"originalAmount"`
	DiscountedAmount   Money   `json:"discountedAmount"`
}

type OrderDiscountRemovedEvent struct {
	OrderEvent
	DiscountId       string `json:"discountId"`
	OriginalAmount   Money  `json:"originalAmount"`
	DiscountedAmount Money  `json:"discountedAmount"`
}
