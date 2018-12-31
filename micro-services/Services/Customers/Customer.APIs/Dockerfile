FROM microsoft/dotnet:2.1-aspnetcore-runtime AS base
WORKDIR /app
EXPOSE 80

FROM microsoft/dotnet:2.1-sdk AS build
WORKDIR /src
COPY Services/Customers/Customer.APIs/Customer.APIs.csproj Services/Customers/Customer.APIs/
COPY Common/Alten.Automotive.Common.Helpers/Common.Helpers.csproj Common/Alten.Automotive.Common.Helpers/
COPY Services/Customers/Customers.Domain/Customers.Domain.csproj Services/Customers/Customers.Domain/
COPY Common/Alten.Automotive.Common.Messaging/Common.Messaging.csproj Common/Alten.Automotive.Common.Messaging/
COPY Services/Customers/Customers.Infrastructure/Customers.Infrastructure.csproj Services/Customers/Customers.Infrastructure/
RUN dotnet restore Services/Customers/Customer.APIs/Customer.APIs.csproj
COPY . .
WORKDIR /src/Services/Customers/Customer.APIs
RUN dotnet build Customer.APIs.csproj -c Release -o /app

FROM build AS publish
RUN dotnet publish Customer.APIs.csproj -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "Customer.APIs.dll"]
