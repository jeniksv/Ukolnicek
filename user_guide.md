# User guide

User guide is separated into two parts as usually more people will be interested in the client part.

---

## Client side

*This part includes both connection to the server and game controls.*

### Connection

User needs to specify Ip, port and name in a simple connection dialog.

### Controls

Controls are rather simple - user simply uses his mouse cursor according to the instructions on the top bar.

However, It is recommended to get familiar with game rules beforehand.

---

## Server side

*This part includes info about server configuration.*

## Setup server

Because server is potentially dangerous for hosting environment, it is recommended to run it from container.

Create a Dockerfile in the root directory of your C# app. This file defines how the Docker image will be built.

```dockerfile
# Use an appropriate base image, like .NET SDK image
FROM mcr.microsoft.com/dotnet/runtime:7.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the .csproj and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy the rest of the app and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build the runtime image
FROM mcr.microsoft.com/dotnet/runtime:7.0
WORKDIR /app
COPY --from=build-env /app/out .

# Start the app
ENTRYPOINT ["dotnet", "Server.dll"]
```

Navigate to the directory containing your Dockerfile and run the following command to build the Docker image:

```bash
docker build -t Ukolnicek-server-image .
```

Create a script (e.g., run-container.sh) to run the Docker container and save data locally.

```bash
#!/bin/bash

# Replace these paths with your actual paths
DATA_DIR="/path/to/local/data/directory"
CONTAINER_NAME="Ukolnicek-server-container"

docker run -d \
    --name "$CONTAINER_NAME" \
    -v "$DATA_DIR:/app/data" \
    Ukolnicek-server-image

```

Make the script executable:

```bash
chmod +x run-container.sh
```

Run the script to start the Docker container:

```bash
./run-container.sh
```

For convenience it will write you IP address to the console.
