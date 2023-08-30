#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src
COPY ["src/AspnetIdentity/AspnetIdentity.csproj", "src/AspnetIdentity/"]
COPY ["src/AspnetIdentity.Application/AspnetIdentity.Application.csproj", "src/AspnetIdentity.Application/"]
COPY ["src/AspnetIdentity.Infra/AspnetIdentity.Infra.csproj", "src/AspnetIdentity.Infra/"]
RUN dotnet restore "src/AspnetIdentity/AspnetIdentity.csproj"
COPY . .
WORKDIR "/src/src/AspnetIdentity"
RUN dotnet build "AspnetIdentity.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "AspnetIdentity.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "AspnetIdentity.dll"]