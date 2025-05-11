package service

import (
	"context"
	"github.com/promotions/internal/models"
	"github.com/promotions/internal/repository"
	"time"
)

// PromotionService handles business logic for promotions
type PromotionService struct {
	repo repository.PromotionRepository
}

// NewPromotionService creates a new promotion service
func NewPromotionService(repo repository.PromotionRepository) *PromotionService {
	return &PromotionService{
		repo: repo,
	}
}

// GetPromotionByProductID retrieves a promotion for a specific product
func (s *PromotionService) GetPromotionByProductID(ctx context.Context, productID string) (*models.Promotion, error) {
	return s.repo.GetByProductID(ctx, productID)
}

// GetAllActivePromotions retrieves all currently active promotions
func (s *PromotionService) GetAllActivePromotions(ctx context.Context) ([]*models.Promotion, error) {
	return s.repo.GetAllActive(ctx)
}

// CreatePromotion adds a new promotion
func (s *PromotionService) CreatePromotion(ctx context.Context, promotion *models.Promotion) error {
	return s.repo.Create(ctx, promotion)
}

// UpdatePromotion modifies an existing promotion
func (s *PromotionService) UpdatePromotion(ctx context.Context, promotion *models.Promotion) error {
	return s.repo.Update(ctx, promotion)
}

// DeletePromotion removes a promotion
func (s *PromotionService) DeletePromotion(ctx context.Context, id string) error {
	return s.repo.Delete(ctx, id)
}

// AssignPromotionToProduct assigns a promotion to a product
func (s *PromotionService) AssignPromotionToProduct(ctx context.Context, productID, promotionID string) error {
	if repo, ok := s.repo.(*repository.MemoryRepository); ok {
		return repo.AssignPromotionToProduct(ctx, productID, promotionID)
	}
	return nil
}

// TimeToString converts time.Time to string in RFC3339 format
func TimeToString(t time.Time) string {
	return t.Format(time.RFC3339)
}

// StringToTime converts string in RFC3339 format to time.Time
func StringToTime(s string) (time.Time, error) {
	return time.Parse(time.RFC3339, s)
}
