CREATE TABLE payments_repository (
    id guid PRIMARY KEY,
    authorized boolean NOT NULL,
    amount integer NOT NULL,
    card_number_last_four integer(4) NOT NULL,
    currency char(3) NOT NULL,
    expiration_date date NOT NULL,
)