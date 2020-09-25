# Payment Gateway API - 1.0

## Request a payment using card details
#### Endpoint
> **[POST]** http://domain.com/api/v1/payments

#### Header parameters
| Header |Value| Description|
|--|--|--|
| Authorization |  secret_key| The secret_key of the merchant
| Content-Type|  application/json| Content Type of the payload must be json


#### Body parameters
| FieldName |Type| Description| Required
|--|--|--|--|
| merchant_id |  guid| The Unique Id of the merchant| Yes
| card_Info|  object| Details of the card to be used for payment| Yes
| card_Info.id|  guid| The Unique Id of the previously used/saved card | If using a saved card
| card_Info.number|  string| The 16 digit card number| For new card
| card_Info.expiry_Month|  string| The 2 digit expiry month of the card| For new card
| card_Info.expiry_Year|  string| The 4 digit expiry year of the card| For new card
| card_Info.cvv|  string| The card verification value/code. Three digits, except for Amex (four digits)| Yes
| card_Info.card_Holder_Name|  string| Full name of the card owner| Optional
| currency|  string| The 3-digit Currency Code| Yes
| amount|  double| The payment amount (must be greater than 0)| Yes
| payment_Description|  string| The payment description that appears in account statement| Yes
**Request Example with for new card :**

    {
	  "merchant_id": "ecc52805-7f1b-429f-be2a-e6651600751d",
	  "card_Info": {
	    "number": "1111222233334444",
	    "expiry_Month": "12",
	    "expiry_Year": "2020",
	    "cvv": "435",
	    "card_Holder_Name": "Test Card"
	  },
	  "currency": "EUR",
	  "amount": 45,
	  "payment_Description": "Test payment"
	}
   
**Request Example with for saved card :**

    {
	  "merchant_id": "ecc52805-7f1b-429f-be2a-e6651600751d",
	  "card_Info": {
	    "id":"b8161c7d-fcd2-4f6f-a61e-75a95fcda77c",
		"cvv": "435"
	  },
	  "currency": "EUR",
	  "amount": 45,
	  "payment_Description": "Test payment"
	}


### Response
| FieldName |Type| Description
|--|--|--|
| transaction_Reference_Id|  guid| The Unique Id of the transaction (internal to payment gateway) 
| acquirer_Reference_Id|  guid| The Unique Id of the payment (received from acquirer) and can be used to fetch a previous payment. Only provided if payment request went to the acquirer for processing.
| currency|  string| The 3-digit Currency Code
| amount|  double| The payment amount 
| card_Info|  object| Details of the card used for the payment
| card_Info.id|  guid| The Unique Id saved card which can be used in future requests
| card_Info.number|  string| 16-digit masked card number with last 4 digits visible
| card_Info.expiry_Month|  string| The 2 digit expiry month of the card
| card_Info.expiry_Year|  string| The 4 digit expiry year of the card
| status|  integer| The status code of the transaction
| processed_On|  datetime| The timestamp when the transaction was processed by the acquirer
| errors|  string[]| Array/List of error messages in case there were any errors




#### Response field "status"  description
| Value| Description|
|--|--|
| 1000|  Approved|
| 2000|  Insufficient funds/credit limit|
| 3000|  Invalid Data|
| 4000|  Unauthorized|
| 9000|  Unknown Error/Exception in Gateway|


**Response Example for successful payment:**

    {
	  "transaction_Reference_Id": "f357c68f-ddf4-4ddf-ae4b-764c845944c7",
	  "acquirer_Reference_Id": "a384f094-90ee-4ea6-8696-edb24702d32b",
	  "currency": "EUR",
	  "amount": 50,
	  "card_Info": {
	    "id": "b8161c7d-fcd2-4f6f-a61e-75a95fcda77c",
	    "number": "XXXXXXXXXXXX4444",
	    "expiry_Month": "12",
	    "expiry_Year": "2020"
	  },
	  "status": 1000,
	  "processed_On": "2020-09-25T06:26:54.961443Z",
	  "errors": null
	}

**Response Example for failed payment:**

    {
	  "transaction_Reference_Id": "727afa69-0e2d-4a2e-af25-059f58478ebc",
	  "acquirer_Reference_Id": "00000000-0000-0000-0000-000000000000",
	  "currency": "EUR",
	  "amount": 45,
	  "card_Info": null,
	  "status": 3000,
	  "processed_On": "0001-01-01T00:00:00",
	  "errors": [
	    "The provided card is expired"
	  ]
	}

## Fetch a previous payment using payment identifier
This endpoint can only be used if the transaction was valid enough to be sent to the acquirer and the acquirer_Reference_Id was provided in the response to the payment request

#### Endpoint
> **[GET]** http://domain.com/api/v1/payments/{acquirer_reference_id}

#### Header parameters
| Header |Value| Description|
|--|--|--|
| Authorization |  secret_key| The secret_key of the merchant
| Content-Type|  application/json| Content Type of the payload must be json

#### Path parameters
| Parameter|Type| Description| Required
|--|--|--|--|
| acquirer_reference_id|  guid| The Unique Id of the payment returned in the response to the payment request| Yes

### Response

> Same as the response of the payment request





