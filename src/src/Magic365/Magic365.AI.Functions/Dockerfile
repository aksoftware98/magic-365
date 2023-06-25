FROM mcr.microsoft.com/azure-functions/dotnet:4 AS base
WORKDIR /home/site/wwwroot
EXPOSE 80

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY . "output/"

RUN dotnet restore "output/Magic365.AI.Functions.csproj"
COPY . . 
WORKDIR "/src/"
RUN dotnet build "Magic365.AI.Functions.csproj" -c Release -o /app/build

FROM build AS publish 
RUN dotnet publish "Magic365.AI.Functions.csproj" -c Release -o /app/publish /p:UseAppHost=false 

FROM base AS final
WORKDIR /home/site/wwwroot
COPY --from=publish /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
	AzureFunctionsJobHost__Logging__Console__IsEnabled=true
