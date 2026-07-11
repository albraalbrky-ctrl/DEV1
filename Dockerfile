# 1. مرحلة البناء باستخدام .NET 10 SDK
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build-env
WORKDIR /app

# نسخ ملف المشروع وعمل restore للمكتبات
COPY DEV1.csproj ./
RUN dotnet restore

# نسخ باقي ملفات المتجر وعمل البناء النهائي للإنتاج
COPY . ./
RUN dotnet publish DEV1.csproj -c Release -o out

# 2. مرحلة التشغيل الفعلي باستخدام بيئة .NET 10 Runtime
FROM mcr.microsoft.com/dotnet/aspnet:10.0
WORKDIR /app
COPY --from=build-env /app/out .

# أمر تشغيل متجر Loka Store
ENTRYPOINT ["dotnet", "DEV1.dll"]