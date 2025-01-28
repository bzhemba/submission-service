FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER $APP_UID
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ./*.props ./
COPY ["src/SubmissionService/SubmissionService.csproj", "src/SubmissionService/"]
COPY ["src/Application/SubmissionService.Application/SubmissionService.Application.csproj", "src/Application/SubmissionService.Application/"]
COPY ["src/Application/SubmissionService.Application.Abstractions/SubmissionService.Application.Abstractions.csproj", "src/Application/SubmissionService.Application.Abstractions/"]
COPY ["src/Application/SubmissionService.Application.Contracts/SubmissionService.Application.Contracts.csproj", "src/Application/SubmissionService.Application.Contracts/"]
COPY ["src/Application/SubmissionService.Application.Models/SubmissionService.Application.Models.csproj", "src/Application/SubmissionService.Application.Models/"]
COPY ["src/Infrastructure/SubmissionService.Infrastructure.Persistence/SubmissionService.Infrastructure.Persistence.csproj", "src/Infrastructure/SubmissionService.Infrastructure.Persistence/"]
COPY ["src/Presentation/SubmissionService.Presentation.Grpc/SubmissionService.Presentation.Grpc.csproj", "src/Presentation/SubmissionService.Presentation.Grpc/"]
COPY ["src/Presentation/SubmissionService.Presentation.Kafka/SubmissionService.Presentation.Kafka.csproj", "src/Presentation/SubmissionService.Presentation.Kafka/"]
RUN dotnet restore "src/SubmissionService/SubmissionService.csproj"
COPY . .
WORKDIR "/src/src/SubmissionService"
RUN dotnet build "SubmissionService.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "SubmissionService.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "SubmissionService.dll"]
