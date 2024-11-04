# Mock Payment-Gateway

REST API mimicking a payment gateway service using .NET, Swagger, Docker, and MongoDB. External bank is mocked using [Mountebank]("http://www.mbtest.org/docs/gettingStarted"). Testing is done with XUnit and Moq.
Originally this project was a takehome test that I enjoyed so decided to fully develop the implementation as a .NET testbed.

## Using
To run locally, write `docker-compose up` inside the terminal. Open the swagger api webpage (https://localhost:8080/swagger/index.html) in a browser of your choosing to access the endpoints.

### Get Payment

### Make Payment
The mocked bank only has two valid requests.
Request:
```
{
  "cardNumber": "2222405343248877",
  "expiryMonth": 4,
  "expiryYear": 2025,
  "currency": "GBP",
  "amount": 100,
  "cvv": 123
}
```
Response:

Request:

```
{
  "cardNumber": "2222405343248112",
  "expiryMonth": 1,
  "expiryYear": 2026,
  "currency": "USD",
  "amount": 60000,
  "cvv": 456
}
```
Response:



## Running tests
Write `dotnet test` inside the terminal.

## TODO
- [x] Modularize testing
- [x] Add MongoDB database
    - [x] Unique models for handling repository service
- [x] Improved error handling
- [ ] Implement better logger and logging
- [ ] Enable full dockerisation, allowing for the app to be run entirely through `docker-compose`
    - [x] Create functional Dockerfile
    - [ ] Enable configuration through docker-compose
    - [ ] Enable MongoDB container