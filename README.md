
# Caching Proxy Server CLI

A simple CLI-based caching proxy server written in C#. It forwards HTTP requests to an origin server, caches the responses for repeated requests, and provides functionality to clear the cache. Sample solution for the [Caching Proxy](https://roadmap.sh/projects/caching-server) challenge from [roadmap.sh](https://roadmap.sh/).

## Features

- **Forward Requests**: Forwards incoming HTTP requests to a specified origin server.
- **Caching Mechanism**: Caches the responses for repeated requests, improving performance.
- **Cache Control**: Includes headers (`X-Cache: HIT` or `X-Cache: MISS`) to indicate whether a response was served from the cache or the origin server.
- **Clear Cache**: Provides a command to clear the cache manually via the CLI.

## Requirements

- [.NET SDK](https://dotnet.microsoft.com/download) installed.

## Getting Started

1. **Clone the repository**:
   ```bash
   git clone https://github.com/your-username/caching-proxy-server.git
   cd caching-proxy-server
   ```

2. **Build the Project**:
   ```bash
   dotnet build
   ```

3. **Run the Proxy Server**:
   ```bash
   dotnet run --port <port-number> --origin <origin-url>
   ```
   Example:
   ```bash
   dotnet run --port 3000 --origin http://dummyjson.com
   ```
   This will start the proxy server on port `3000` and forward all requests to `http://dummyjson.com`.

## Usage

### Forward Requests

When the proxy server is running, it will forward any request made to `http://localhost:<port>` to the specified `origin` URL. 

For example, if started with:
```bash
dotnet run --port 3000 --origin http://dummyjson.com
```
A request to:
```
http://localhost:3000/products
```
will be forwarded to:
```
http://dummyjson.com/products
```

### Caching Behavior

- **X-Cache: HIT**: Indicates the response was served from the cache.
- **X-Cache: MISS**: Indicates the response was retrieved from the origin server and then cached.

### Clear the Cache

To clear the cache, run:
```bash
dotnet run --clear-cache
```

## Example Scenarios

1. **Start the Proxy**: Forward requests to an origin server.
   ```bash
   dotnet run --port 3000 --origin http://example.com
   ```
   Requests to `http://localhost:3000/path` will be forwarded to `http://example.com/path`.

2. **Clear Cache**: Reset the cache storage.
   ```bash
   dotnet run --clear-cache
   ```

## How It Works

1. The proxy server uses `HttpListener` to handle incoming HTTP requests on the specified port.
2. It checks the cache for existing responses based on the request URL.
3. If a cached response exists, it returns it immediately (`X-Cache: HIT`).
4. If no cached response exists, it forwards the request to the origin server, caches the response, and then returns it (`X-Cache: MISS`).
5. The cache can be cleared by using the `--clear-cache` option.

## Support My Work
If you enjoy my work or want to support what I do, feel free to [Buy Me a Coffee](https://buymeacoffee.com/marvynharry)!

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.
