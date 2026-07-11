# 1. مرحلة البناء والتجهيز
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# نسخ كل شيء في المجلد الرئيسي أولاً لضمان عدم ضياع أي ملف
COPY . ./

# عمل Restore للمكتبات بناءً على الملفات المنسوخة
RUN dotnet restore

# بناء النسخة النهائية للإنتاج
RUN dotnet publish -c Release -o out

# 2. مرحلة التشغيل الفعلي على السيرفر
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# أمر تشغيل المشروع الفاخر
ENTRYPOINT ["dotnet", "DEV1.dll"]