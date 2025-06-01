##################################################################
# Stage 0: Base image (để chạy production hoặc khi debug nhanh)  #
##################################################################
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app

# Đặt biến môi trường để Kestrel lắng nghe trên đúng cổng
#  - HTTP trên 8080
#  - HTTPS trên 8081
ENV ASPNETCORE_URLS="http://+:8080;https://+:8081"

# Expose cả 2 cổng để docker-compose có thể ánh xạ
EXPOSE 8080
EXPOSE 8081

##################################################################
# Stage 1: Build (restore + build)                              #
##################################################################
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# 1.1 Copy file csproj và restore
COPY ["Soccer.Font-end/Soccer.Font-end.csproj", "Soccer.Font-end/"]
RUN dotnet restore "Soccer.Font-end/Soccer.Font-end.csproj"

# 1.2 Copy toàn bộ mã nguồn, chuyển vào thư mục project
COPY . .
WORKDIR "/src/Soccer.Font-end"

# 1.3 Build project
RUN dotnet build "Soccer.Font-end.csproj" -c $BUILD_CONFIGURATION -o /app/build

##################################################################
# Stage 2: Publish (xuất ra /app/publish)                        #
##################################################################
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "Soccer.Font-end.csproj" \
    -c $BUILD_CONFIGURATION \
    -o /app/publish \
    /p:UseAppHost=false

##################################################################
# Stage 3: Runtime (chỉ copy từ thư mục publish vào)            #
##################################################################
FROM base AS final
WORKDIR /app

# Copy tất cả output đã publish vào container
COPY --from=publish /app/publish .

# Khởi chạy ứng dụng
ENTRYPOINT ["dotnet", "Soccer.Font-end.dll"]
