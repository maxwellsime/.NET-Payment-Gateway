# Mock Payment-Gateway

REST API mimicking a payment gateway service using .NET. External bank is mocked using [Mountebank]("http://www.mbtest.org/docs/gettingStarted"). Testing is done with XUnit and Moq.

## Using
Write `dotnet run` inside the terminal. Use opened Swagger webpage to use api endpoints.

### Make Payment
These are the only two valid requests the mocked bank can interact with.

| Card number      | Expiry date | Currency | Amount | CVV | Authorized  | Authorization code                   |
|------------------|-------------|----------|--------|-----|-------------|--------------------------------------|
| 2222405343248877 | 04/2025     | GBP      | 100    | 123 | true        | 0bb07405-6d44-4b50-a14f-7ae0beff13ad |
| 2222405343248112 | 01/2026     | USD      | 60000  | 456 | false       | < empty >                            |


## Running tests
Write `dotnet test` inside the terminal.

## TODO
- [ ] Modularize testing
- [ ] Add database