#!/bin/bash

/opt/mssql/bin/sqlservr &

./wait-for-sql.sh

for file in /working/*.sql
do
    /opt/mssql-tools/bin/sqlcmd -S localhost \
        -U sa -P $SA_PASSWORD \
        -i "$file"
done

/opt/mssql-tools/bin/sqlcmd -S localhost \
    -U sa -P $SA_PASSWORD \
    -Q "BACKUP DATABASE [master] TO DISK = N'/out/obj/Docker/publish/master.bak'"
