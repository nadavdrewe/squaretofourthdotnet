FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src
COPY ["squareservice.pipeline.fourth.com/squareservice.pipeline.fourth.com.csproj", "squareservice.pipeline.fourth.com/"]
COPY ["data.pipeline.fourth.com/data.pipeline.fourth.com.csproj", "data.pipeline.fourth.com/"]
COPY ["domain.pipeline.fourth.com/domain.pipeline.fourth.com.csproj", "domain.pipeline.fourth.com/"]
COPY ["square.pipeline.fourth.com/square.pipeline.fourth.com.csproj", "square.pipeline.fourth.com/"]
COPY ["shared.pipeline.fouth.com/shared.pipeline.fouth.com.csproj", "shared.pipeline.fouth.com/"]
RUN dotnet restore "squareservice.pipeline.fourth.com/squareservice.pipeline.fourth.com.csproj"
COPY . .
WORKDIR "/src/squareservice.pipeline.fourth.com"
RUN dotnet publish "squareservice.pipeline.fourth.com.csproj" -c Release --no-restore -o /app/publish

FROM mcr.microsoft.com/dotnet/runtime:10.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "squareservice.pipeline.fourth.com.dll"]
