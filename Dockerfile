# =========================
# Build stage
# =========================
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

WORKDIR /src

COPY . .

RUN dotnet restore

RUN dotnet publish AIStudyPlanner.csproj \
    -c Release \
    -o /app/publish \
    /p:UseAppHost=false


# =========================
# Runtime stage
# =========================
FROM mcr.microsoft.com/dotnet/aspnet:6.0

WORKDIR /app

# Install Python 3 and required system packages
RUN apt-get update \
    && apt-get install -y python3 python3-pip \
    && rm -rf /var/lib/apt/lists/*

# Copy ASP.NET application
COPY --from=build /app/publish .

# Copy Python dependencies
COPY requirements.txt /app/requirements.txt

# Install Python dependencies
RUN pip3 install --no-cache-dir -r /app/requirements.txt

# Render provides the PORT environment variable.
ENV ASPNETCORE_URLS=http://0.0.0.0:${PORT}

EXPOSE 10000

ENTRYPOINT ["dotnet", "AIStudyPlanner.dll"]