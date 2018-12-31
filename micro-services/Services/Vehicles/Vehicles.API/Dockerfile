FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Services/Vehicles/Vehicles.API/Vehicles.API.csproj Services/Vehicles/Vehicles.API/
COPY Common/Alten.Automotive.Common.Helpers/Common.Helpers.csproj Common/Alten.Automotive.Common.Helpers/
COPY Services/Vehicles/Vehicles.Domain/Vehicles.Domain.csproj Services/Vehicles/Vehicles.Domain/
COPY Common/Alten.Automotive.Common.Messaging/Common.Messaging.csproj Common/Alten.Automotive.Common.Messaging/
COPY Services/Vehicles/Vehicles.Infrastructure/Vehicles.Infrastructure.csproj Services/Vehicles/Vehicles.Infrastructure/
COPY Services/Customers/Customers.Domain/Customers.Domain.csproj Services/Customers/Customers.Domain/
COPY Services/Customers/Customers.Infrastructure/Customers.Infrastructure.csproj Services/Customers/Customers.Infrastructure/
RUN dotnet restore Services/Vehicles/Vehicles.API/Vehicles.API.csproj
COPY . .
WORKDIR /src/Services/Vehicles/Vehicles.API
RUN dotnet build Vehicles.API.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Vehicles.API.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Vehicles.API.dll"]
