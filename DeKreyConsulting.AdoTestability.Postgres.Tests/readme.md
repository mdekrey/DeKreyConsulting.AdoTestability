Launch the demo postgres database via docker:

    docker build --tag adotestability-psql .
    docker run -ti --rm -p65432:5432 adotestability-psql

Or, use docker-compose:

    docker-compose up --build
