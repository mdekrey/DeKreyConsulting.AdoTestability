FROM microsoft/mssql-server-linux:rc2

WORKDIR /src

ARG source
COPY scripts/shared/*.sh ./
COPY scripts/runtime/*.sh ./
COPY obj/Docker/publish/master.bak ./

EXPOSE 1433

ENV SA_PASSWORD=weakPASSw0rd

# Needs environment variables set up;
# https://docs.microsoft.com/en-us/sql/linux/quickstart-install-connect-docker
CMD /bin/bash -c "./entrypoint.sh"
