# Payment Gateway

## Routes

- **/api/v1/Payment/ProcessPayment**

Given a PaymentRequestModel, this route will process a payment to a bank and attempt to save the payment into the database. The payment will be saved regardless of whether the bank successfully processed the request to enable auditing. The paymentId assigned to the payment once saved in the database will be returned to allow retrieval.


- **/api/v1/Reporting/GetPayment/{Id}**

Given a paymentId, this route will return the payment found matching the given ID.

## Running Instructions

Please ensure you have powershell and the .NET CLI installed. This application is available for testing locally using powershell and swagger. 
  1. Run the LocalTestingRunner.ps1 script using powershell 
  2. Navigate towards https://localhost:5001/index.html in your browser of choice and use swagger to interact with the API 

The fake bank api has been programmed to respond with certain responses to enable testing. The behaviours are configured by switching the amount property in the payment request.
Behaviours:
- amount == 100, Bank responds with 200 status code
- amount == 1000, Bank responds with 400 status code with content.
- Any other amount will result in a 400 status code without content being returned

## Assumptions Made

- **Interacting with Bank**

I made the assumption that interacting with the 3rd party bank would be a quick interaction therefore not needing real time updates on the status of the payment. I also made the assumption on the model returned from the bank API and used previous experience interacting with restful APIs to add detailed message and status in the model.

- **Model Validation**

Other than requiring all the properties to be populated I didn't put too much model validation on the API. I feel without a domain expert any assumptions made on the incoming data would be premature. In reality card numbers will have to fall between a certain length and the same can be said with the other properties. If more time was allocated I would go back and research & implement greater model validation, as this would simplify the business logic due to not having to assume the model could be malformed.

- **API interaction**

I opted to expose the payment gateway using a restful API. I thought this way was more lightweight than say providing a merchant / api consumer with a library as all it requires them to do is submit payloads to our endpoints over HTTP. 

- **Saving data**

When a payment is not successfully processed to the 3rd party bank the application will save the request with the failed response and other corresponding details in the database anyway. This feature could be potentially expanded upon to stop duplicate payments and offer a greater audit history of a payment attempt through correlating the separate payments, however a unifying ID would be needed to do or a composite key to isolate the payment request attempts.

- **Data storage**

I opted to store the data in a structured format and not use a noSQL database. I didn’t think the payment data was going to be unknown enough that it wouldn’t conform to a predetermined shema.

- **Code Structure**

If I were to put this code into production I would separate out the fake bank into its own repo and deploy it separately. The code is grouped together to make reviewing of the application easier. I also found that there seems to be two separate domains emerging from this service. Whilst probably too early to segregate them into their own solutions. My gut feeling was to have a reporting micro service that could handle auditing and retrieval of the payment requests and a payment processing micro service that would handle processing payments and integrating with 3rd parties. Having them bundled together seems like a violation of the single responsibility principle, and I'm unsure how it would scale.

## Limitations of Code

- **Identity**

I attempted to set up auth for the application using an identity server. This work was attempted on the AddIdentityServer branch and can be viewed on the repo. While I did manage to get the routes locked down with separate scopes, the inclusion of having authorisation broke my swagger code and my integration tests. On the branch there is a Runner folder containing a console app in the root folder to showcase the requesting of a bearer token from the server and using that token to authenticate with the paymentgateway API. This was my first time setting up Identity server and unfortunately time constraints meant I had to leave this feature unfinished. With the exclusion of this feature the app remains unsecure. Without this feature it is currently possible to query the get route with random IDs and get payment information you shouldn’t have access to. I was going to implement further filtering on the repository, using merchant ID as well as payment ID, but without the inclusion of identity it felt like a hollow measure.

- **Code architecture**

It was my plan, if enough time was remaining to refactor this to use the CQRS pattern. Around halfway through writing my integration I felt the need for separating out the command and query models and feel it would've made my code cleaner. I haven't had production experience of CQRS so decided against implementing it, in favour of a more typical N tier (with a somewhat anemic domain model) application.



