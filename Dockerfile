# Build runtime image
FROM postgres
WORKDIR /app

# Copy test data

RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        ca-certificates \
        \
# .NET Core dependencies
        libc6 \
        libgcc1 \
        libgssapi-krb5-2 \
        libicu57 \
        liblttng-ust0 \
        libssl1.0.2 \
        libstdc++6 \
        zlib1g \
        curl \
    && rm -rf /var/lib/apt/lists/*

# Configure web servers to bind to port 80 when present
ENV ASPNETCORE_URLS=http://+:80 \
    # Enable detection of running in a container
    DOTNET_RUNNING_IN_CONTAINER=true

# Install ASP.NET Core
ENV ASPNETCORE_VERSION 2.2.0

RUN curl -SL --output aspnetcore.tar.gz https://dotnetcli.blob.core.windows.net/dotnet/aspnetcore/Runtime/$ASPNETCORE_VERSION/aspnetcore-runtime-$ASPNETCORE_VERSION-linux-x64.tar.gz \
    && aspnetcore_sha512='26b3a52eb0b55eedaf731af1c1553653c73ed8e7c385119a421e33c8fca9691bae378904ee8f6fc13e1c621c9d64303ea5337750bb34e34d6ad0de788319f9bc' \
    && echo "$aspnetcore_sha512  aspnetcore.tar.gz" | sha512sum -c - \
    && mkdir -p /usr/share/dotnet \
    && tar -zxf aspnetcore.tar.gz -C /usr/share/dotnet \
    && rm aspnetcore.tar.gz \
    && ln -s /usr/share/dotnet/dotnet /usr/bin/dotnet

COPY /AccountsApi/out .
COPY /tmp/ ../tmp/

COPY start-container.sh /usr/lib/postgresql/$PG_MAJOR/bin/start-container.sh

COPY db_init.sql /docker-entrypoint-initdb.d/

ENTRYPOINT ["start-container.sh"]

