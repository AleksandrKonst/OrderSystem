package service

import (
	"context"
	"encoding/json"
	"errors"
	"log"
	"time"

	"promotions/internal/repository"
	pb "promotions/proto/gen"
)

type GRPCServer struct {
	pb.UnimplementedPromotionsServer
	service *PromotionService
}

func NewGRPCServer(service *PromotionService) *GRPCServer {
	return &GRPCServer{
		service: service,
	}
}

func (s *GRPCServer) GetProductPromotion(ctx context.Context, req *pb.GetProductPromotionRequest) (*pb.PromotionResponse, error) {
	startTime := time.Now()
	log.Printf("[gRPC] Получен запрос GetProductPromotion: productId=%s", req.ProductId)

	promotion, err := s.service.GetPromotionByProductID(ctx, req.ProductId)
	if err != nil {
		if errors.Is(err, repository.ErrPromotionNotFound) {
			log.Printf("[gRPC] Скидка для продукта %s не найдена", req.ProductId)
			return &pb.PromotionResponse{
				Promotion: nil,
			}, nil
		}
		log.Printf("[gRPC] Ошибка при получении скидки для продукта %s: %v", req.ProductId, err)
		return nil, err
	}

	response := &pb.PromotionResponse{
		Promotion: &pb.Promotion{
			Id:                 promotion.ID,
			Name:               promotion.Name,
			Description:        promotion.Description,
			DiscountPercentage: promotion.DiscountPercentage,
			ValidUntil:         TimeToString(promotion.ValidUntil),
		},
	}

	promotionJson, _ := json.Marshal(response.Promotion)
	log.Printf("[gRPC] Отправлен ответ GetProductPromotion: продукт=%s, скидка=%s, время выполнения=%v",
		req.ProductId, string(promotionJson), time.Since(startTime))

	return response, nil
}

func (s *GRPCServer) GetAllActivePromotions(ctx context.Context, req *pb.GetAllActivePromotionsRequest) (*pb.PromotionsListResponse, error) {
	startTime := time.Now()
	log.Printf("[gRPC] Получен запрос GetAllActivePromotions")

	promotions, err := s.service.GetAllActivePromotions(ctx)
	if err != nil {
		log.Printf("[gRPC] Ошибка при получении всех активных скидок: %v", err)
		return nil, err
	}

	response := &pb.PromotionsListResponse{
		Promotions: make([]*pb.Promotion, 0, len(promotions)),
	}

	for _, p := range promotions {
		response.Promotions = append(response.Promotions, &pb.Promotion{
			Id:                 p.ID,
			Name:               p.Name,
			Description:        p.Description,
			DiscountPercentage: p.DiscountPercentage,
			ValidUntil:         TimeToString(p.ValidUntil),
		})
	}

	log.Printf("[gRPC] Отправлен ответ GetAllActivePromotions: найдено %d активных скидок, время выполнения=%v",
		len(response.Promotions), time.Since(startTime))

	return response, nil
}
