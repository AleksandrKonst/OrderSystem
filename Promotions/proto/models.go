package proto

import (
	"time"
)

type Promotion struct {
	Id                 string  `json:"id"`
	Name               string  `json:"name"`
	Description        string  `json:"description"`
	DiscountPercentage float64 `json:"discount_percentage"`
	ValidUntil         string  `json:"valid_until"`
}

type GetProductPromotionRequest struct {
	ProductId string `json:"product_id"`
}

type GetAllActivePromotionsRequest struct {
	// Empty request
}

type PromotionResponse struct {
	Promotion *Promotion `json:"promotion"`
}

type PromotionsListResponse struct {
	Promotions []*Promotion `json:"promotions"`
}

// TimeToString converts time.Time to string
func TimeToString(t time.Time) string {
	return t.Format(time.RFC3339)
}

// StringToTime converts string to time.Time
func StringToTime(s string) (time.Time, error) {
	return time.Parse(time.RFC3339, s)
}
