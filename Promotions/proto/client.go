package proto

import (
	"context"
	"errors"
)

type promotionsClient struct{}

func NewPromotionsClient() PromotionsClient {
	return &promotionsClient{}
}

func (c *promotionsClient) GetProductPromotion(ctx context.Context, req *GetProductPromotionRequest) (*PromotionResponse, error) {
	return nil, errors.New("unimplemented client method")
}

func (c *promotionsClient) GetAllActivePromotions(ctx context.Context, req *GetAllActivePromotionsRequest) (*PromotionsListResponse, error) {
	return nil, errors.New("unimplemented client method")
}
