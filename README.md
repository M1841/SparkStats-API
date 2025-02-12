# SparkStats API

REST API server behind a music statistics platform.

## Prerequisites

- [Spotify developer account](https://developer.spotify.com/)
- a SparkStats Web instance - get one from [GitHub](https://github.com/m1841/sparkstats-web) or [GitLab](https://gitlab.com/mihai_muresan/sparkstats-web)

And either one of:

- [.NET 8](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) or newer
- [Docker](https://docs.docker.com/engine/install)

## Configuration

Create an app in your Spotify developer dashboard

Add `http://localhost:8080/auth/callback` to redirect URIs

Create a `.env` file with the client ID and secret and other necessary data

```bash
SPOTIFY_CLIENT_ID=your-client-id
SPOTIFY_CLIENT_SECRET=your-client-secret
PORT=8080
FRONTEND_URL=http://localhost:4200
BACKEND_URL=http://localhost:8080
```

## Running the program

- as a regular .NET app (must have .NET 8 installed)

```bash
./start
```

- same as above but with hot-reloading enabled

```bash
./start -w # or ./start --watch
```

- as a Docker container (must have Docker installed)

```bash
./start -d # or ./start --docker
```
