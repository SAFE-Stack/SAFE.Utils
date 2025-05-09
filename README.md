# SAFE.Utils

SAFE.Utils provides essential utilities for building [SAFE](https://safe-stack.github.io/) applications. It's distributed as two NuGet packages:

- SAFE.Client.Utils - Utilities for client-side code 
 [![NuGet](https://img.shields.io/nuget/v/SAFE.Client.Utils.svg)](https://www.nuget.org/packages/SAFE.Client.Utils/)

- SAFE.Server.Utils - Utilities for server-side code [![NuGet](https://img.shields.io/nuget/v/SAFE.Server.Utils.svg)](https://www.nuget.org/packages/SAFE.Server.Utils/)

While designed to work together in SAFE applications, each package can be used independently in F# applications.

## Features
The included functionalities are:

### SAFE.Client.Utils
* Types to represent remote data on the front end, and Elmish messages deal with remote requests
* Functions that help you get started with Fable.Remoting

### SAFE.Server.Utils
* Functions that help you get started with Fable.Remoting

## Installation

```fsharp
// In your client project
dotnet add package SAFE.Client.Utils

// In your server project
dotnet add package SAFE.Server.Utils
```

If you have any questions, please open an issue on the [GitHub repository](https://github.com/SAFE-Stack/SAFE.Utils)