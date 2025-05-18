package repository

import (
	"context"
	"promotions/internal/models"
)

type PromotionRepository interface {
	GetByProductID(ctx context.Context, productID string) (*models.Promotion, error)

	GetAllActive(ctx context.Context) ([]*models.Promotion, error)

	Create(ctx context.Context, promotion *models.Promotion) error

	Update(ctx context.Context, promotion *models.Promotion) error

	Delete(ctx context.Context, id string) error
}
