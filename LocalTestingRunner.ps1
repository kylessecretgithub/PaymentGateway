invoke-expression 'cmd /c start powershell -Command {  dotnet run -p .\src\PaymentGateway.Gateway\PaymentGateway.Gateway.csproj }'
invoke-expression 'cmd /c start powershell -Command {  dotnet run -p .\src\PaymentGateway.FakeBankApi\PaymentGateway.FakeBankApi.csproj }'