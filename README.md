
# Receipts API

## Overview

The Receipts API is a simple receipt processing service built with .NET 9. It allows users to submit receipts and retrieve points awarded for each receipt. The API also includes OpenAPI documentation for easy integration and testing.

## Features

- **Submit Receipts**: Submit a receipt for processing and receive a unique receipt ID.
- **Retrieve Points**: Retrieve the points awarded for a specific receipt using the receipt ID.
- **OpenAPI Documentation**: Access detailed API documentation in development mode.

## Getting Started

### Prerequisites

- .NET 9 SDK

OR
- Docker

### Running the Application (Docker)
1.  Clone the repository and navigate to the project directory.
```
git clone https://github.com/schimmelc/receipts-api-csharp.git
cd receipts-api-csharp
```
2.  Build the Docker image and run the container.
```
docker build -t receipts-api
docker run -p 8080:8080 --name receipts-api-container receipts-api

# To run and specify the dotnet environment
docker run -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development --name receipts-api-container receipts-api
```

### API Endpoints

#### Submit a Receipt

- **Endpoint**: `POST /receipts/process`
- **Description**: Submits a receipt for processing.

#### Get Points For Receipt

- **Endpoint**: `GET /receipts/{id}/points`
- **Description**: Retrieves the points awarded for a receipt.

The API includes OpenAPI documentation. In development mode, you can access it at `http://localhost:8080/scalar/v1`.

## Technologies Used

- .NET 9
- Entity Framework Core (In-Memory Database)
- OpenTelemetry
- Scalar.AspNetCore
