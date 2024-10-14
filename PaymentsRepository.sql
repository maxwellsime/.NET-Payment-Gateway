CREATE TABLE PaymentsRepository (
    ID uuid PRIMARY KEY,
    Authorized boolean NOT NULL,
    Amount integer NOT NULL,
    CardNumberLastFour integer(4) NOT NULL,
    Currency char(3) NOT NULL,
    ExpirationDate date NOT NULL,
)