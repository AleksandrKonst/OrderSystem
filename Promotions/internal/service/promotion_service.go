package service

import (
	"context"
	"promotions/internal/models"
	"promotions/internal/repository"
	"time"
)

type PromotionService struct {
	repo repository.PromotionRepository
}

func NewPromotionService(repo repository.PromotionRepository) *PromotionService {
	return &PromotionService{
		repo: repo,
	}
}

func (s *PromotionService) GetPromotionByProductID(ctx context.Context, productID string) (*models.Promotion, error) {
	return s.repo.GetByProductID(ctx, productID)
}

func (s *PromotionService) GetAllActivePromotions(ctx context.Context) ([]*models.Promotion, error) {
	return s.repo.GetAllActive(ctx)
}

func (s *PromotionService) CreatePromotion(ctx context.Context, promotion *models.Promotion) error {
	return s.repo.Create(ctx, promotion)
}

func (s *PromotionService) UpdatePromotion(ctx context.Context, promotion *models.Promotion) error {
	return s.repo.Update(ctx, promotion)
}

func (s *PromotionService) DeletePromotion(ctx context.Context, id string) error {
	return s.repo.Delete(ctx, id)
}

func (s *PromotionService) AssignPromotionToProduct(ctx context.Context, productID, promotionID string) error {
	if repo, ok := s.repo.(*repository.MemoryRepository); ok {
		return repo.AssignPromotionToProduct(ctx, productID, promotionID)
	}
	return nil
}

func TimeToString(t time.Time) string {
	return t.Format(time.RFC3339)
}

func StringToTime(s string) (time.Time, error) {
	return time.Parse(time.RFC3339, s)
}
