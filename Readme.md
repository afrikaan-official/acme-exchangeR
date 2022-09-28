## AcmeExchangeR

AcmeExchangeR is a simple web api for exchange rates

## Project
### Tech stack
- Api is written with `dotnet core 6`
- My database choice is `postgresql` (In order to make it much funnier i'm using `postgres` as document db for one table `ExchangeRates`)

### FolderStructure
```
.
├── AcmeExcangeR
│   ├── src/    
│   │   ├── API/
│   │   │   ├── AcmeExcangeR.API/
│   │   │   │   ├── appsettings.json
│   │   │   │   ├── Controllers/
│   │   │   │   ├── Middlewares/
│   │   │   │   ├── Validators/
│   │   │   │   ├── ...
│   │   │   ├── AcmeExcangeR.Bus/
│   │   │   │   ├── Services/
│   │   │   ├── AcmeExcangeR.Data/
│   │   │   │   ├── Entities/
│   │   │   │   ├── ExchangeRateDbContext.cs
│   │   ├── Utils/
│   │   │   ├── AcmeExcangeR.Utils/
│   │   │   │   ├── FastForexClient/
│   │   │   │   ├── Models/
│   │   ├── Misc/
│   │   │   ├── Scripts/
│   │   │   │   ├── tables.sql
│   │   │   │   ├── Dockerfile
│   │   Readme.md
│   └── docker-compose.yaml
```

## How to run this project
There is a docker-compose file in the root of the project. You can simply run `docker-compose up -d`
it will create a `postgres` and `api`. `postgres` will run at port `5432` and `api` will run at port `5000`.
After `docker-compose` finishes you can go `http://localhost:5000/swagger/index.html` to interact with api. Or you can use `postman`/`curl`


## Assumptions
- Exchange rate resource is `fastforex.io`
- There is a background service called `RateFetcherBackgroundService` inside `api` project. It will fetch rates in every `5` seconds. (That is configurable through `appsetting.json`) So i assume system is always works with fresh data. 
- There is a config section with key `Exchanges` in `appsettings.json` which is the exchanges that system will fetch and restore. You can add or remove exchanges by editing that file.
- I assume that all clients should send `X-Client-Id` as identifier so any request without that header will get `BadRequest`.