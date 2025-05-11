package models

import (
	"time"
)

type Promotion struct {
	ID                 string    `json:"id"`
	Name               string    `json:"name"`
	Description        string    `json:"description"`
	DiscountPercentage float64   `json:"discount_percentage"`
	ValidUntil         time.Time `json:"valid_until"`
}

func (p *Promotion) IsActive() bool {
	return p.ValidUntil.After(time.Now())
}
