#!/bin/bash

retries=100
while ((retries > 0)); do
    /opt/mssql-tools/bin/sqlcmd -S localhost \
        -U sa -P $SA_PASSWORD \
        -d master \
        -q "SELECT 1" \
        > /dev/null \
    && break

    sleep 1
    ((retries --))
done
if ((retries == 0 )); then
    exit 1
fi
