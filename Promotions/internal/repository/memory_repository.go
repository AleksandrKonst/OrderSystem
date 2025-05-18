package repository

import (
	"context"
	"errors"
	"sync"
	"time"

	"promotions/internal/models"
)

var (
	ErrPromotionNotFound = errors.New("promotion not found")
)

type MemoryRepository struct {
	promotions         map[string]*models.Promotion
	productToPromotion map[string]string
	mu                 sync.RWMutex
}

func NewMemoryRepository() *MemoryRepository {
	repo := &MemoryRepository{
		promotions:         make(map[string]*models.Promotion),
		productToPromotion: make(map[string]string),
	}

	samplePromotions := []*models.Promotion{
		{
			ID:                 "promo1",
			Name:               "Spring Sale",
			Description:        "20% off all spring items",
			DiscountPercentage: 20.0,
			ValidUntil:         time.Now().AddDate(0, 1, 0),
		},
		{
			ID:                 "promo2",
			Name:               "New Customer Discount",
			Description:        "15% off for new customers",
			DiscountPercentage: 15.0,
			ValidUntil:         time.Now().AddDate(0, 3, 0),
		},
	}

	for _, p := range samplePromotions {
		repo.promotions[p.ID] = p
	}

	repo.productToPromotion["product1"] = "promo1"
	repo.productToPromotion["product2"] = "promo2"

	return repo
}

func (r *MemoryRepository) GetByProductID(ctx context.Context, productID string) (*models.Promotion, error) {
	r.mu.RLock()
	defer r.mu.RUnlock()

	promoID, exists := r.productToPromotion[productID]
	if !exists {
		return nil, ErrPromotionNotFound
	}

	promo, exists := r.promotions[promoID]
	if !exists {
		return nil, ErrPromotionNotFound
	}

	if !promo.IsActive() {
		return nil, ErrPromotionNotFound
	}

	return promo, nil
}

func (r *MemoryRepository) GetAllActive(ctx context.Context) ([]*models.Promotion, error) {
	r.mu.RLock()
	defer r.mu.RUnlock()

	var activePromos []*models.Promotion

	for _, promo := range r.promotions {
		if promo.IsActive() {
			activePromos = append(activePromos, promo)
		}
	}

	return activePromos, nil
}

func (r *MemoryRepository) Create(ctx context.Context, promotion *models.Promotion) error {
	r.mu.Lock()
	defer r.mu.Unlock()

	if _, exists := r.promotions[promotion.ID]; exists {
		return errors.New("promotion with this ID already exists")
	}

	r.promotions[promotion.ID] = promotion
	return nil
}

func (r *MemoryRepository) Update(ctx context.Context, promotion *models.Promotion) error {
	r.mu.Lock()
	defer r.mu.Unlock()

	if _, exists := r.promotions[promotion.ID]; !exists {
		return ErrPromotionNotFound
	}

	r.promotions[promotion.ID] = promotion
	return nil
}

func (r *MemoryRepository) Delete(ctx context.Context, id string) error {
	r.mu.Lock()
	defer r.mu.Unlock()

	if _, exists := r.promotions[id]; !exists {
		return ErrPromotionNotFound
	}

	delete(r.promotions, id)

	for prodID, promoID := range r.productToPromotion {
		if promoID == id {
			delete(r.productToPromotion, prodID)
		}
	}

	return nil
}

func (r *MemoryRepository) AssignPromotionToProduct(ctx context.Context, productID, promotionID string) error {
	r.mu.Lock()
	defer r.mu.Unlock()

	if _, exists := r.promotions[promotionID]; !exists {
		return ErrPromotionNotFound
	}

	r.productToPromotion[productID] = promotionID
	return nil
}
