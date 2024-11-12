# Mock Payment-Gateway

REST API mimicking a payment gateway service using .NET, Swagger, Docker, and MongoDB. External bank is mocked using [Mountebank]("http://www.mbtest.org/docs/gettingStarted"). Testing is done with XUnit and Moq.
Originally this project was a takehome test that I enjoyed so decided to fully develop the implementation as a .NET testbed.

## Using
To run locally, write `docker-compose up` inside the terminal. Open the swagger api webpage (https://localhost:8080/swagger/index.html) in a browser of your choosing to access the endpoints.

## Endpoints
### GET /api/payments/{id}
Gets a payment that is stored in the database by the payment ID, parsed as a url query. Note: Payment IDs are returned upon creation.

200 Response:
```
{
  "_id": "67329a4ee21b53b57fb10b37",
  "status": 1,
  "cardNumberLastFour": "8112",
  "expiryMonth": 1,
  "expiryYear": 1,
  "currency": "USD",
  "amount": 60000
}
```
404 Response:
```
"No payment with id: 67329a4ee21b53b52 found."
```

### GET /api/payments/multiple/{cardNumberLastFour}
Gets a history of payments stored in the database all linked to the same last four digits of a card number. Note: In a real-world example this would be insufficient, and some form of user_id would be useful.

200 Response:
```
[
  {
    "_id": "6722a56d2c0acc6c267ba603",
    "status": 1,
    "cardNumberLastFour": "8112",
    "expiryMonth": 1,
    "expiryYear": 1,
    "currency": "USD",
    "amount": 60000
  },
  {
    "_id": "6722a5ab2c0acc6c267ba604",
    "status": 1,
    "cardNumberLastFour": "8112",
    "expiryMonth": 1,
    "expiryYear": 1,
    "currency": "USD",
    "amount": 60000
  }
]
```

404 Response:
```
"No payments found for card ending in 2221."
```

### POST /api/payments/create-payment/
Creates a payment, saving it to the Mongo DB and hitting the mock bank server.

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
200 Response:
```
{
  "_id": "67329a2fe21b53b57fb10b36",
  "status": 0,
  "cardNumberLastFour": "8877",
  "expiryMonth": 4,
  "expiryYear": 1,
  "currency": "GBP",
  "amount": 100
}
```
400 Response:
```
{
  "type": "https://tools.ietf.org/html/rfc9110#section-15.5.1",
  "title": "One or more validation errors occurred.",
  "status": 400,
  "errors": {
    "Currency": [
      "Currency only accepts 3 character long strings."
    ],
    "CardNumber": [
      "CardNumber only accepts integers between 14 and 19."
    ],
    "ExpiryYear": [
      "ExpiryYear needs to be beyond the current year."
    ],
    "ExpiryMonth": [
      "ExpiryMonth only accepts integers between 1 and 12."
    ]
  },
  "traceId": "00-2c6964409a696a00dc6a685131e22384-4b4c97f9252e2ad8-00"
}
```

The only other valid request the mocked bank intakes:
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

## Running tests
Write `dotnet test` inside the terminal. 
If you don't have some form of docker running the PaymentRepository tests will fail to create testcontainers.

## TODO
- [x] Modularize testing
- [x] Add MongoDB database
    - [x] Unique models for handling repository service
- [x] Improved error handling
- [x] Enable full dockerisation, allowing for the app to be run entirely through `docker-compose`
    - [x] Create functional Dockerfile
    - [x] Enable MongoDB container