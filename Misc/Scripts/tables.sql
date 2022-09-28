CREATE TABLE "ExchangeRates"(
    "Id" serial primary key,
    "Payload" jsonb
);

CREATE TABLE "TradeHistories"(
    "Id" serial primary key,
    "From" varchar(3) not null,
    "To" varchar(3) not null,
    "Amount" decimal,
    "ClientId" varchar(50),
    "CreatedDate" timestamp not null
);

create table "ClientLimits"
(
    "Id" serial primary key ,
    "ClientId" varchar(50),
    "LastTradeDate" timestamp,
    "Count" int
);