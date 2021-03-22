# Payment Gateway

## Routes

- **/api/v1/Payment/ProcessPayment**

Given a PaymentRequestModel, this route will process a payment to a bank and attempt to save the payment in to the database. The payment will be saved regardless of whether the bank sucessfully processed the request to enable auditing. The paymentId assigned to the payment once saved in the database will be returned to allow retrieval.



- **/api/v1/Reporting/GetPayment/{Id}**

Given a paymentId, this route will return the payment found matching the given ID.



## Running Instructions
This application is available for testing locally using visual studio and swagger. Open the .sln file in visual studio (visual studio 2019 tested) and navigate to the top and hit the start button. To the left of the start button it should display Multiple Startup Projects. If it doesn't, righy click the .sln from within visual studio. Select properties and then configure the projects: PaymentGateway.FakeBankApi and PaymentGateway.Gateway to have start in the action drop down. Hit apply and ok.

Once the start button has been clicked two windows should launch. One should be running swagger which will allow you to interact with the two API routes.

The fake bank api has been programmed to response with certain responses to enable testing. The behaviours are configured by switching the amount property in the payment request.
Behaviours:
amount 100 > Bank responds with 200 status code
amount 1000 > Bank responds with 400 status code with content.
Any other amount will result in a 400 status code without content being returned


## Assumptions Made

- **Interacting with Bank**

I made the assumption that interacting with the 3rd party bank would be a quick interaction therefore not needing real time updates on the status of the payment. I also made the assumption on the model returned from the bank API and used previous experience interacting with restful APIs to add detailed message and status in the model.

- **Model Validation**

Other than requiring all the properties to be populated I didn't put too much model validation on the API. I feel without a domain expert any assumptions made on the incoming data would be premature. In reality card numbers will have to fall between a certain length and the same can be said with the other properties. If more time was allocated I would go back and research & implement greater model validation, as this would simply the business logic due to not having to assume the model could be malformed.

- **API interaction**

I opted to expose the payment gateway using a restful API. I thought this way was more light weight than say providing a merchant / api consumer with a library as all it requires them to do is submit payloads to our endpoints over HTTP. 

- **Saving data**

When a payment is not sucessfully processed to the 3rd party bank the application will save the request with the failed response and other coresponding details in the database anyway. This feature could be potentially expanded upon to stop duplicate payments and offer a greater audit history of a payment attempt through correlating the separate payments, however a unifying ID would be needed to do or a composite key to isolate the payment request attempts.

- **Data storage**

I opted to store the data in a structured format and not use a noSQL db. After researching I had some concerns about my choice due to a noSQL DB's horizontal scaling capabilities that work well with distrubuted systems (perhaps an unneeded concern for my locally deployed payment gateway using a stubbed 3rd party bank), however after reading stack overflow operate using mostly SQL dbs and more research into the difficulties associated with noSQL dbs complicating application code I felt fine sticking with a SQL DB (even if it is only in memory :D)

- **Code Structure**

If I were to put this code into production I would separate out the fake bank into its own repo and deploy it separately. The code is grouped together to make reviewing of the application easier. I also found that there seems to be two separate domains emerging from this service. Whilst probably too early to segregate them into their own solutions. My gut feeling was to have a reporting micro service that could handle auditing and retrieval of the payment requests and a payment processing micro service that would handle processing payments and integrating with 3rd parties. Having them bundled together to me seems like a violation of single responsbility pattern, and I'm unsure how it would scale.

## Limitations of Code

- **Identity**

I attempted to setup auth for the application using identity server. This work was attempted on the AddIdentityServer branch and can be viewed on the repo. While I did manage to get the routes locked down with separate scopes the inclusion of having authorisation broke my swagger code and my integration tests. On the branch there is a Runner folder in the route to showcase the requesting of a bearer token from the server and using that token to authenticate with the paymentgateway API to showcase it working. Within the runner folder is just a simple console app. Unfortunately time constraints led me to having to leave this feature unfinished. With the exlusion of this feature the app remains unlocked down, which since it contains financial information, isn't ideal since just randomly querying the API with a payment ID can get you payment information from different merchants. I was going to implement further filtering on the repository, using merchant ID as well as payment ID, but without the inclusion of identity it felt like a hollow measure.

- **Code architechture**

It was my plan, if enough time was remaining to refactor this to use the CQRS pattern. Around halfway through writing my integration I felt the need for seprating out the command and query models and feel it would've made my code cleaner. I havent had production experience of CQRS so decided against implementing it, in favour of a more typical N tier (with a somewhat anemic domain model) application.


