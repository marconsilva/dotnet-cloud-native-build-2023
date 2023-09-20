FROM mcr.microsoft.com/dotnet/aspnet:8.0.0-rc.1-alpine AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0.100-rc.1-alpine3.18 AS build

WORKDIR /src

RUN /apk add \
	# Add NativeAOT pre-reqs
	clang build-base zlib-dev \
	# workaround so GRPC protoc works - https://github.com/grpc/grpc/blob/master/src/csharp/BUILD-INTEGRATION.md#using-grpctools-with-unsupported-architectures
	grpc-plugins

ENV PROTOBUF_PROTOC=/usr/bin/protoc
ENV GRPC_PROTOC_PLUGIN=/usr/bin/grpc_csharp_plugin

COPY ["NuGet.config", "."]
COPY ["ContosoOnline/OrderProcessor/OrderProcessor.csproj", "OrderProcessor/"]
COPY ["ContosoOnline/Diagnostics/Diagnostics.csproj", "Diagnostics/"]
RUN dotnet restore "OrderProcessor/OrderProcessor.csproj"
COPY ./ContosoOnline .
WORKDIR "/src/OrderProcessor"
RUN dotnet publish "OrderProcessor.csproj" -c Release -o /app/publish

# remove the symbols so they aren't part of the published app
RUN rm -rf /app/publish/*.dbg

FROM alpine AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["./OrderProcessor"]
