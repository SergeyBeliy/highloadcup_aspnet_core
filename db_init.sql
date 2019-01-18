--
-- PostgreSQL database dump
--

CREATE USER accountsdb_user WITH
  LOGIN
  SUPERUSER
  INHERIT
  CREATEDB
  CREATEROLE
  NOREPLICATION;

-- Dumped from database version 11.1
-- Dumped by pg_dump version 11.1

-- Started on 2019-01-15 22:02:34

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_with_oids = false;

--
-- TOC entry 196 (class 1259 OID 16386)
-- Name: __EFMigrationsHistory; Type: TABLE; Schema: public; Owner: accountsdb_user
--

CREATE TABLE public."__EFMigrationsHistory" (
    "MigrationId" character varying(150) NOT NULL,
    "ProductVersion" character varying(32) NOT NULL
);


ALTER TABLE public."__EFMigrationsHistory" OWNER TO accountsdb_user;

--
-- TOC entry 198 (class 1259 OID 16393)
-- Name: accounts; Type: TABLE; Schema: public; Owner: accountsdb_user
--

CREATE TABLE public.accounts (
    id bigint NOT NULL,
    sname character varying(50),
    fname character varying(50),
    country character varying(50),
    city character varying(50),
    phone character varying(16),
    email character varying(100),
    sex integer NOT NULL,
    birth timestamp without time zone NOT NULL,
    joined timestamp without time zone NOT NULL,
    status character varying(10),
    interests text[],
    like_ids bigint[],
    like_tss bigint[],
    premium_start bigint,
    premium_finish bigint
);


ALTER TABLE public.accounts OWNER TO accountsdb_user;

--
-- TOC entry 197 (class 1259 OID 16391)
-- Name: accounts_id_seq; Type: SEQUENCE; Schema: public; Owner: accountsdb_user
--

CREATE SEQUENCE public.accounts_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.accounts_id_seq OWNER TO accountsdb_user;

--
-- TOC entry 2175 (class 0 OID 0)
-- Dependencies: 197
-- Name: accounts_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: accountsdb_user
--

ALTER SEQUENCE public.accounts_id_seq OWNED BY public.accounts.id;


--
-- TOC entry 2044 (class 2604 OID 16396)
-- Name: accounts id; Type: DEFAULT; Schema: public; Owner: accountsdb_user
--

ALTER TABLE ONLY public.accounts ALTER COLUMN id SET DEFAULT nextval('public.accounts_id_seq'::regclass);


--
-- TOC entry 2046 (class 2606 OID 16390)
-- Name: __EFMigrationsHistory PK___EFMigrationsHistory; Type: CONSTRAINT; Schema: public; Owner: accountsdb_user
--

ALTER TABLE ONLY public."__EFMigrationsHistory"
    ADD CONSTRAINT "PK___EFMigrationsHistory" PRIMARY KEY ("MigrationId");


--
-- TOC entry 2048 (class 2606 OID 16401)
-- Name: accounts PK_accounts; Type: CONSTRAINT; Schema: public; Owner: accountsdb_user
--

ALTER TABLE ONLY public.accounts
    ADD CONSTRAINT "PK_accounts" PRIMARY KEY (id);


-- Completed on 2019-01-15 22:02:34

--
-- PostgreSQL database dump complete
--

