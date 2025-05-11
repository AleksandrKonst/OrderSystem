package proto

import (
	"context"
	"errors"
)

type PromotionsClient interface {
	GetProductPromotion(ctx context.Context, in *GetProductPromotionRequest) (*PromotionResponse, error)
	GetAllActivePromotions(ctx context.Context, in *GetAllActivePromotionsRequest) (*PromotionsListResponse, error)
}

type PromotionsServer interface {
	GetProductPromotion(context.Context, *GetProductPromotionRequest) (*PromotionResponse, error)
	GetAllActivePromotions(context.Context, *GetAllActivePromotionsRequest) (*PromotionsListResponse, error)
}

type UnimplementedPromotionsServer struct{}

var _ PromotionsServer = (*UnimplementedPromotionsServer)(nil)

func (s *UnimplementedPromotionsServer) GetProductPromotion(ctx context.Context, req *GetProductPromotionRequest) (*PromotionResponse, error) {
	return nil, errors.New("method GetProductPromotion not implemented")
}

func (s *UnimplementedPromotionsServer) GetAllActivePromotions(ctx context.Context, req *GetAllActivePromotionsRequest) (*PromotionsListResponse, error) {
	return nil, errors.New("method GetAllActivePromotions not implemented")
}

func RegisterPromotionsServer(s interface{}, srv PromotionsServer) {
	// In a real implementation, this would register the server with gRPC
}
