# مرحلة البناء
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# نسخ ملف المشروع وعمل ريستور
COPY DEV1.csproj ./
RUN dotnet restore

# نسخ باقي الملفات وعمل البناء النهائي
COPY . ./
RUN dotnet publish DEV1.csproj -c Release -o out

# مرحلة التشغيل
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

ENTRYPOINT ["dotnet", "DEV1.dll"]