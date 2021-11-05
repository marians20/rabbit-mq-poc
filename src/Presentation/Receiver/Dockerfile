#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:5.0 AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build
WORKDIR /src
COPY ["src/Presentation/Receiver/Receiver.csproj", "src/Presentation/Receiver/"]
COPY ["src/Infrastructure/RabbitMqAdapter/RabbitMqAdapter.csproj", "src/Infrastructure/RabbitMqAdapter/"]
COPY ["src/Domain/Events/Events.csproj", "src/Domain/Events/"]
RUN dotnet restore "src/Presentation/Receiver/Receiver.csproj"
COPY . .
WORKDIR "/src/src/Presentation/Receiver"
RUN dotnet build "Receiver.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Receiver.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Receiver.dll"]