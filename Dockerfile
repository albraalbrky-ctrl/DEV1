# 1. مرحلة البناء والتجهيز
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /app

# نسخ ملف الـ Solution وملف المشروع من المجلد الفرعي بعمل Restore صحيح
COPY *.sln ./
COPY DEV1/*.csproj ./DEV1/
RUN dotnet restore

# نسخ باقي ملفات المشروع بالكامل وبناء النسخة النهائية
COPY . ./
RUN dotnet publish DEV1/DEV1.csproj -c Release -o out

# 2. مرحلة التشغيل الفعلي على سيرفر لينكس
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build-env /app/out .

# أمر تشغيل المشروع الفاخر
ENTRYPOINT ["dotnet", "DEV1.dll"]