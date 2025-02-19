# Use the official .NET SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the project files and restore dependencies
COPY ./*.csproj ./
RUN dotnet restore

# Copy the rest of the application files and build
COPY . ./
RUN dotnet publish -c Release -o out

# Use the official ASP.NET Core runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy the built files from the previous stage
COPY --from=build /app/out ./

# Set the entry point for the container
CMD ["dotnet", "SocialMediaAPI23Okt.dll"]
