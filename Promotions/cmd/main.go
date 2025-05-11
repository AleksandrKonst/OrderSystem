package main

import (
	"context"
	"log"
	"net"
	"net/http"
	"os"
	"os/signal"
	"syscall"
	"time"

	"github.com/promotions/api"
	"github.com/promotions/internal/repository"
	"github.com/promotions/internal/service"
	pb "github.com/promotions/proto"
	"google.golang.org/grpc"
)

const (
	grpcPort = ":50051"
	httpPort = ":8080"
)

func main() {
	repo := repository.NewMemoryRepository()
	promotionService := service.NewPromotionService(repo)

	restHandler := api.NewRestServer(promotionService)
	httpServer := &http.Server{
		Addr:    httpPort,
		Handler: restHandler,
	}

	grpcServer := grpc.NewServer()
	pb.RegisterPromotionsServer(grpcServer, service.NewGRPCServer(promotionService))

	go func() {
		lis, err := net.Listen("tcp", grpcPort)
		if err != nil {
			log.Fatalf("Failed to listen on port %s: %v", grpcPort, err)
		}

		log.Printf("Starting gRPC server on port %s", grpcPort)
		if err := grpcServer.Serve(lis); err != nil {
			log.Fatalf("Failed to serve gRPC server: %v", err)
		}
	}()

	go func() {
		log.Printf("Starting HTTP server on port %s", httpPort)
		if err := httpServer.ListenAndServe(); err != nil && err != http.ErrServerClosed {
			log.Fatalf("Failed to serve HTTP server: %v", err)
		}
	}()

	log.Println("Promotions microservice started successfully")

	quit := make(chan os.Signal, 1)
	signal.Notify(quit, syscall.SIGINT, syscall.SIGTERM)
	<-quit

	log.Println("Shutting down servers...")

	ctx, cancel := context.WithTimeout(context.Background(), 5*time.Second)
	defer cancel()

	if err := httpServer.Shutdown(ctx); err != nil {
		log.Fatalf("HTTP server forced to shutdown: %v", err)
	}

	grpcServer.GracefulStop()

	log.Println("Servers exited properly")
}
