#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

#Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
#For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/core/aspnet:3.1.10-nanoserver-1809 AS base
WORKDIR /app
#EXPOSE 8080

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-nanoserver-1909 AS build
WORKDIR /src
COPY ["fmedge/fmedge.csproj", "fmedge/"]
RUN dotnet restore "fmedge/fmedge.csproj"
COPY . .
WORKDIR "/src/fmedge"
RUN dotnet build "fmedge.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "fmedge.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "fmedge.dll"]