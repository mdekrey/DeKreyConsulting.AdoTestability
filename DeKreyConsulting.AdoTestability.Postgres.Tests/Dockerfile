FROM postgres:9.6

COPY *.sql /docker-entrypoint-initdb.d/

# Note: This is bad practice, but it's for a test database. Don't follow this practice for prod, etc.!
ENV POSTGRES_PASSWORD=testdb!1
