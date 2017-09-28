dotnet pack .\DeKreyConsulting.AdoTestability\                   --configuration Release --output artifacts 
dotnet pack .\DeKreyConsulting.AdoTestability.Testing.Moq\       --configuration Release --output artifacts
dotnet pack .\DeKreyConsulting.AdoTestability.Testing.SqlServer\ --configuration Release --output artifacts
dotnet pack .\DeKreyConsulting.AdoTestability.Testing.Sqlite\    --configuration Release --output artifacts
dotnet pack .\DeKreyConsulting.AdoTestability.Testing.Postgres\  --configuration Release --output artifacts
dotnet pack .\DeKreyConsulting.AdoTestability.Testing.Stubs\     --configuration Release --output artifacts
