#!/bin/bash

export ACCEPT_EULA=$(cat /run/secrets/sql-eula.txt)

/opt/mssql/bin/sqlservr &

./wait-for-sql.sh

/opt/mssql-tools/bin/sqlcmd -S localhost \
    -U sa -P $SA_PASSWORD \
    -Q "RESTORE DATABASE [adotestability] FROM DISK = N'/src/master.bak' WITH  FILE = 1,  NOUNLOAD,  REPLACE,  STATS = 5"

tail -f /dev/null
