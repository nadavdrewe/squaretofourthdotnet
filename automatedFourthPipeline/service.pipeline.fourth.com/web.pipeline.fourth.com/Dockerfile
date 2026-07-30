#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["web.pipeline.fourth.com/web.pipeline.fourth.com.csproj", "web.pipeline.fourth.com/"]
COPY ["com.fourth.pipeline.credentials/com.fourth.pipeline.credentials.csproj", "com.fourth.pipeline.credentials/"]
COPY ["data.pipeline.fourth.com/data.pipeline.fourth.com.csproj", "data.pipeline.fourth.com/"]
COPY ["domain.pipeline.fourth.com/domain.pipeline.fourth.com.csproj", "domain.pipeline.fourth.com/"]
COPY ["com.fourth.pipeline.pos/com.fourth.pipeline.pos.csproj", "com.fourth.pipeline.pos/"]
COPY ["shared.pipeline.fouth.com/shared.pipeline.fouth.com.csproj", "shared.pipeline.fouth.com/"]
RUN dotnet restore "web.pipeline.fourth.com/web.pipeline.fourth.com.csproj"
COPY . .
WORKDIR "/src/web.pipeline.fourth.com"
RUN dotnet build "web.pipeline.fourth.com.csproj" -c Release --no-restore -o /app/build

FROM build AS publish
RUN dotnet publish "web.pipeline.fourth.com.csproj" -c Release --no-restore -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "web.pipeline.fourth.com.dll"]
