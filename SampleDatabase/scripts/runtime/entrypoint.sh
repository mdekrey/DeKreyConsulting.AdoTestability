#!/bin/bash

export ACCEPT_EULA=$(cat /run/secrets/sql-eula.txt)

/opt/mssql/bin/sqlservr &

./wait-for-sql.sh

/opt/mssql-tools/bin/sqlcmd -S localhost \
    -U sa -P $SA_PASSWORD \
    -Q "RESTORE DATABASE [adotestability] FROM DISK = N'/src/adotestability.bak' WITH  FILE = 1, MOVE 'master' TO '/var/opt/mssql/data/adotestability.mdf', MOVE 'mastlog' TO '/var/opt/mssql/data/adotestability_msdblog.ldf', NOUNLOAD,  REPLACE,  STATS = 5"

tail -f /dev/null
