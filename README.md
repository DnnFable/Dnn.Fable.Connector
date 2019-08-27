# DNN.Fable.Connector

## Introduction

[DNN](https://dnndocs.com) as Web Application Framework offers multiple ways of writing extensions. Modules can be written as WebForms, MVC or simple [SPA modules](https://dnndocs.com/content/guides/tutorials/modules/about-modules/spa-module-development/index.html#spa-module-development).

SPA modules are typically build with HTML 5, CSS and JS-Frameworks like React, Angular or Vue.js. These frameworks talk to Web API based services for querying data or executing scripts.

Functional Programming is growing popularity and tools like [Elm lang](https://elm-lang.org) are also available in DNN. In fact, it is possible to write both client side code and the services in F#. Combining the excellent F# to Javascript compiler [Fable](https://fable.io), the powerfull [Elmish](https://elmish.github.io/elmish/) Model-View-Update implementation and the Web API written in F# directly provides a modern type safe webstack. 

And all plays well with DNN, and developers can benefit from the proven security and extensibility.

`DNN.Fable.Fetch` and `DNN.Fable.ApiController` simplify the communication between Web Api and the SPA module.

## DNN.Fable.ApiController

Usually DNN Web Services inherit from `DotNetNuke.Web.Api.DnnApiController`, which is again based on Web Api. It is a perfect match, it is only that it uses `Newtonsoft.Json` for encoding and decoding data for requests and responses.

This is fine for C# structs and classes, but just ugly and error prone for F# data structures like discriminated unions or option types.

The good news is that it is possible to share code between service and client.

_shared.fs:_
```fsharp
module SharedData

type User =
    | Unauthenticated
    | LoggedInAs of
        {| DisplayName: string
           UserId: int |}
```

The web service in F# is quite familiar beside from the syntax, the only difference is that it inherits now from [DNN.Fable.ApiController](https://www.nuget.org/packages/DNN.Fable.ApiController/)

```fsharp
namespace MyCompany.MyModule

open SharedData
open System.Web.Http
open System.Net
open System.Net.Http
open DotNetNuke.Web.Api
open DotNetNuke.Security

type RouteMapper ()=
    interface IServiceRouteMapper with
      member __.RegisterRoutes (rtm:IMapRoute) = 
        let namespaces = [|"MyCompany.MyModule"|]
        let moduleApiName ="mycomp/mymod" 
        rtm.MapHttpRoute  (moduleApiName, "default", "{controller}/{action}", namespaces) |> ignore                                

type MyServiceController() = 
    inherit DNN.Fable.ApiController() // instead of DnnApiController

    [<HttpGet>]
    [<ValidateAntiForgeryToken>] 
    [<DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.View)>] 
    member __.CurrentUser () =
        let result = 
            if __.User.Identity.IsAuthenticated 
                then LoggedInAs 
                      {| DisplayName =__.UserInfo.DisplayName; 
                         UserId =__.UserInfo.UserID |}
                else Unauthenticated

        __.Request.CreateResponse (HttpStatusCode.OK, result)
```


## DNN.Fable.Fetch

DNN provides a `ServicesFramework` as `JQuery` extension which is using AJAX calls to talk to the service. A more modern approach would be based on fetch.
However, each call needs the correct ModuleHeaders with `moduleId`, `tabId`, `antiForgeryToken` and maybe also requestCredentials.

The moduleId needs to be passed as Token within the SPA template:  

__index.html_:
```html
[AntiForgeryToken: {}]
<div id="myModule-container" data-moduleId ="[Module:ModuleId]"/>
[JavaScript:{ path: "~/desktopModules/mycomp/mymod/bundle.js", provider:"DnnFormBottomProvider"}]
```
Then this script talks to the service above and queries the current user.

_module.fs_:
```fsharp
namespace HelloAgain

open SharedData
open Browser

let moduleName = "mycomp/mymod" // same as in RouterMapper
let url = "myservice/currentuser" // MyServiceController.CurrentUser()

let container = document.getElementById "myModule-container"
let moduleId = container.dataset.["moduleId"]

let text =
    promise { return! DNN.Fable.Fetch.get (moduleId, moduleName, url) }

    |> function
       | Unauthenticated -> "Please login"
       | LoggedInAs p    -> sprintf "Hello %s" p.DisplayName

container.innerText <- text  
```

`DNN.Fable.Fetch` offers methods to handle GET, POST, PUT, DELETE, PATCH requests.

### Credits
DNN.Fable is build on top of [Thoth.Json.Net](https://github.com/thoth-org/Thoth.Json.Net) and [Thoth.Fetch](https://github.com/thoth-org/Thoth.Fetch)