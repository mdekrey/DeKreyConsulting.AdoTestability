Create the `sql-eula.txt` with a `Y` as its sole contents if you agree to
[Microsoft's SQL Server Docker](https://hub.docker.com/r/microsoft/mssql-server-linux/) EULA.

Launch the demo sql server database via docker (powershell script):

    docker build --tag adotestability-sql:build -f Dockerfile-ci .
    docker run --rm -v "${pwd}:/src" adotestability-sql:build
    docker build --tag adotestability-sql .
    docker run -ti --rm -p11433:1433 -v "${pwd}:/run/secrets" adotestability-sql

Or, use docker-compose:

    docker-compose -f docker-compose.ci.yml up --build
	docker-compose up --build