[<RequireQualifiedAccess>]
module Dnn.Fable
open System.Web.Http.Controllers

type WithThothJsonNetFormatterAttribute () =
    inherit System.Attribute()
    interface IControllerConfiguration with
       member __.Initialize ( (controllerSettings:HttpControllerSettings) , _ ) =
            controllerSettings.Formatters.Clear()
            controllerSettings.Formatters.Add <| Thoth.Json.Net.Formatter()

[<WithThothJsonNetFormatter>]
type ApiController  ()  =
    inherit DotNetNuke.Web.Api.DnnApiController ()