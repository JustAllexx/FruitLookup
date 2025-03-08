# Building FruityLookup
## Windows
### Visual Studio
FruityLookup comes with a .sln file which can be opened in Visual Studio, this should download all the necessary packages. Set either CLI or GUI as startup project and publish
### DotNet
In the base folder execute\
`dotnet publish`\
You will find the Command Line interface in `\FruityLookup.CLI\bin\Release\net9.0\publish\`\
To compile the MAUI GUI you might need to access the FruityLookup.GUI and dotnet publish in this folder
## Linux
The MAUI GUI cannot be compiled for Linux, only the CLI.\
Open FruityLookup.CLI as the base folder and execute\
`dotnet publish -c Release -r linux-x64`\
You should find the compiled executable in `bin\Release\net9.0\linux-x64\publish\`