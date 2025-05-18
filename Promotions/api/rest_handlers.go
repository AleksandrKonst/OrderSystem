package api

import (
	"encoding/json"
	"github.com/go-chi/chi/v5"
	"github.com/go-chi/chi/v5/middleware"
	"net/http"
	"promotions/internal/models"
	"promotions/internal/repository"
	"promotions/internal/service"
	"time"
)

type PromotionRequest struct {
	Name               string  `json:"name"`
	Description        string  `json:"description"`
	DiscountPercentage float64 `json:"discount_percentage"`
	ValidUntil         string  `json:"valid_until"` // RFC3339 format
}

type ProductPromotionRequest struct {
	ProductID   string `json:"product_id"`
	PromotionID string `json:"promotion_id"`
}

func NewRestServer(promotionService *service.PromotionService) http.Handler {
	r := chi.NewRouter()

	r.Use(middleware.Logger)
	r.Use(middleware.Recoverer)
	r.Use(middleware.RequestID)
	r.Use(middleware.RealIP)

	// Routes
	r.Route("/api/v1", func(r chi.Router) {
		// Promotions
		r.Route("/promotions", func(r chi.Router) {
			r.Get("/", getAllPromotions(promotionService))
			r.Post("/", createPromotion(promotionService))
			r.Route("/{id}", func(r chi.Router) {
				r.Get("/", getPromotionByID(promotionService))
				r.Put("/", updatePromotion(promotionService))
				r.Delete("/", deletePromotion(promotionService))
			})
		})

		// Product promotions
		r.Route("/product-promotions", func(r chi.Router) {
			r.Get("/{productID}", getPromotionByProductID(promotionService))
			r.Post("/", assignPromotionToProduct(promotionService))
		})
	})

	return r
}

func getAllPromotions(svc *service.PromotionService) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()

		promotions, err := svc.GetAllActivePromotions(ctx)
		if err != nil {
			http.Error(w, "Failed to get promotions", http.StatusInternalServerError)
			return
		}

		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(promotions)
	}
}

func getPromotionByID(svc *service.PromotionService) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		http.Error(w, "Not implemented", http.StatusNotImplemented)
	}
}

func getPromotionByProductID(svc *service.PromotionService) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()
		productID := chi.URLParam(r, "productID")

		promotion, err := svc.GetPromotionByProductID(ctx, productID)
		if err != nil {
			if err == repository.ErrPromotionNotFound {
				http.Error(w, "No promotion found for this product", http.StatusNotFound)
				return
			}
			http.Error(w, "Failed to get promotion", http.StatusInternalServerError)
			return
		}

		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(promotion)
	}
}

func createPromotion(svc *service.PromotionService) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()

		var req PromotionRequest
		if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
			http.Error(w, "Invalid request body", http.StatusBadRequest)
			return
		}

		validUntil, err := service.StringToTime(req.ValidUntil)
		if err != nil {
			http.Error(w, "Invalid date format for valid_until", http.StatusBadRequest)
			return
		}

		id := "promo-" + time.Now().Format("20060102150405")

		promotion := &models.Promotion{
			ID:                 id,
			Name:               req.Name,
			Description:        req.Description,
			DiscountPercentage: req.DiscountPercentage,
			ValidUntil:         validUntil,
		}

		if err := svc.CreatePromotion(ctx, promotion); err != nil {
			http.Error(w, "Failed to create promotion", http.StatusInternalServerError)
			return
		}

		w.Header().Set("Content-Type", "application/json")
		w.WriteHeader(http.StatusCreated)
		json.NewEncoder(w).Encode(promotion)
	}
}

func updatePromotion(svc *service.PromotionService) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()
		id := chi.URLParam(r, "id")

		var req PromotionRequest
		if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
			http.Error(w, "Invalid request body", http.StatusBadRequest)
			return
		}

		validUntil, err := service.StringToTime(req.ValidUntil)
		if err != nil {
			http.Error(w, "Invalid date format for valid_until", http.StatusBadRequest)
			return
		}

		promotion := &models.Promotion{
			ID:                 id,
			Name:               req.Name,
			Description:        req.Description,
			DiscountPercentage: req.DiscountPercentage,
			ValidUntil:         validUntil,
		}

		if err := svc.UpdatePromotion(ctx, promotion); err != nil {
			if err == repository.ErrPromotionNotFound {
				http.Error(w, "Promotion not found", http.StatusNotFound)
				return
			}
			http.Error(w, "Failed to update promotion", http.StatusInternalServerError)
			return
		}

		w.Header().Set("Content-Type", "application/json")
		json.NewEncoder(w).Encode(promotion)
	}
}

func deletePromotion(svc *service.PromotionService) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()
		id := chi.URLParam(r, "id")

		if err := svc.DeletePromotion(ctx, id); err != nil {
			if err == repository.ErrPromotionNotFound {
				http.Error(w, "Promotion not found", http.StatusNotFound)
				return
			}
			http.Error(w, "Failed to delete promotion", http.StatusInternalServerError)
			return
		}

		w.WriteHeader(http.StatusNoContent)
	}
}

func assignPromotionToProduct(svc *service.PromotionService) http.HandlerFunc {
	return func(w http.ResponseWriter, r *http.Request) {
		ctx := r.Context()

		var req ProductPromotionRequest
		if err := json.NewDecoder(r.Body).Decode(&req); err != nil {
			http.Error(w, "Invalid request body", http.StatusBadRequest)
			return
		}

		if err := svc.AssignPromotionToProduct(ctx, req.ProductID, req.PromotionID); err != nil {
			if err == repository.ErrPromotionNotFound {
				http.Error(w, "Promotion not found", http.StatusNotFound)
				return
			}
			http.Error(w, "Failed to assign promotion to product", http.StatusInternalServerError)
			return
		}

		w.WriteHeader(http.StatusNoContent)
	}
}
