# مرحلة البناء والتجهيز
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# نسخ ملفات المشروع وعمل Restore للمكتبات
COPY *.csproj ./
RUN dotnet restore

# نسخ باقي الأكواد وبناء النسخة النهائية للإنتاج
COPY . ./
RUN dotnet publish -c Release -o out

# مرحلة التشغيل الفعلي على السيرفر
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# أمر تشغيل المشروع (تأكدي من تغيير DEV1.dll إذا كان اسم مشروعك مختلفاً)
ENTRYPOINT ["dotnet", "DEV1.dll"]