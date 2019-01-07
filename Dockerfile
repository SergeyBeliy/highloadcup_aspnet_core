FROM microsoft/dotnet:2.2-sdk AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY /AccountsApi/*.csproj ./
RUN dotnet restore


# Copy everything else and build
COPY ./AccountsApi/ ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY --from=build-env /app/out .

# Copy test data
COPY tmp/ /tmp/
RUN ls -la /tmp/*

RUN apt-get update \
    && mkdir -p /usr/share/man/man1 \
    && mkdir -p /usr/share/man/man7 \
    && apt-get install -y --no-install-recommends postgresql \
    && rm -rf /var/lib/apt/lists/* \
    && apt-get clean 

USER postgres

RUN  /etc/init.d/postgresql start &&\
    psql --command "CREATE USER accountsdb_user WITH LOGIN NOSUPERUSER INHERIT CREATEDB NOCREATEROLE REPLICATION PASSWORD 'Tester01';" \ 
    &&\
    psql --command "CREATE SEQUENCE public.accounts_id_seq; ALTER SEQUENCE public.accounts_id_seq OWNER TO accountsdb_user;" \
    &&\
    psql --command "CREATE SEQUENCE public.like_like_id_seq;ALTER SEQUENCE public.like_like_id_seq OWNER TO accountsdb_user;" \
    &&\
    psql --command "CREATE SEQUENCE public.premium_id_seq;ALTER SEQUENCE public.premium_id_seq OWNER TO accountsdb_user;" \
    && \
    psql --command "CREATE TABLE public.accounts \
    ( \
        id bigint NOT NULL DEFAULT nextval('accounts_id_seq'::regclass), \
        sname character varying(50) COLLATE pg_catalog.""default"",\
        fname character varying(50) COLLATE pg_catalog.""default"", \
        country character varying(50) COLLATE pg_catalog.""default"", \
        city character varying(50) COLLATE pg_catalog.""default"", \
        phone character varying(16) COLLATE pg_catalog.""default"", \
        email character varying(100) COLLATE pg_catalog.""default"", \
        sex integer NOT NULL, \
        birth bigint NOT NULL, \
        joined bigint NOT NULL, \
        status character varying(10) COLLATE pg_catalog.""default"", \
        interests text[] COLLATE pg_catalog.""default"", \
        CONSTRAINT ""PK_accounts"" PRIMARY KEY (id) \
    ) \
    WITH ( \
        OIDS = FALSE \
    ) \
    TABLESPACE pg_default; \
    ALTER TABLE public.accounts \
        OWNER to accountsdb_user;  \
    CREATE TABLE public.""like"" \
    ( \
        like_id bigint NOT NULL DEFAULT nextval('like_like_id_seq'::regclass), \
        account_id bigint NOT NULL, \
        id bigint NOT NULL, \
        ts bigint NOT NULL, \
        CONSTRAINT ""PK_like"" PRIMARY KEY (like_id), \
        CONSTRAINT ""FK_like_accounts_account_id"" FOREIGN KEY (account_id) \
            REFERENCES public.accounts (id) MATCH SIMPLE \
            ON UPDATE NO ACTION \
            ON DELETE CASCADE \
    ) \
    WITH ( \
        OIDS = FALSE \
    ) \
    TABLESPACE pg_default; \
    ALTER TABLE public.""like"" \
        OWNER to accountsdb_user; \
    CREATE INDEX ""IX_like_account_id"" \
        ON public.""like"" USING btree \
        (account_id) \
        TABLESPACE pg_default;\ 
        CREATE TABLE public.premium \
        ( \
            id bigint NOT NULL DEFAULT nextval('premium_id_seq'::regclass), \
            account_id bigint NOT NULL, \
            start bigint NOT NULL, \
            finish bigint NOT NULL, \
            CONSTRAINT ""PK_premium"" PRIMARY KEY (id), \
            CONSTRAINT ""FK_premium_accounts_account_id"" FOREIGN KEY (account_id) \
                REFERENCES public.accounts (id) MATCH SIMPLE \
                ON UPDATE NO ACTION \
                ON DELETE CASCADE \
        ) \
        WITH ( \
            OIDS = FALSE \
        ) \
        TABLESPACE pg_default; \
        ALTER TABLE public.premium \
            OWNER to accountsdb_user; \
        CREATE UNIQUE INDEX ""IX_premium_account_id"" \
            ON public.premium USING btree \
            (account_id) \
            TABLESPACE pg_default;" 

USER root


ENV PATH $PATH:/usr/lib/postgresql/9.6/bin
ENV PGDATA /var/lib/postgresql/data

CMD ["../etc/init.d/postgresql start"]
ENTRYPOINT ["dotnet", "AccountsApi.dll"]
