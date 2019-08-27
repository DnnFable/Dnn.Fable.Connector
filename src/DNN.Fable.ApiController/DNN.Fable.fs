namespace DNN.Fable
open System.Web.Http.Controllers
open System.Net.Http.Formatting
open Newtonsoft.Json
open Thoth.Json.Net

type WithThothJsonNetConverterAttribute (?isCamelCase : bool, ?extra : ExtraCoders) =
    inherit System.Attribute()
 
    interface IControllerConfiguration with
       member __.Initialize ( (controllerSettings:HttpControllerSettings) , _ ) =
            let converter = Thoth.Json.Net.Converters.Converter(?isCamelCase = isCamelCase, ?extra = extra)
            let thothFormatter =
                JsonMediaTypeFormatter
                    (SerializerSettings = JsonSerializerSettings(Converters = [| converter |]))
            controllerSettings.Formatters.Clear()
            controllerSettings.Formatters.Add thothFormatter
    new() = WithThothJsonNetConverterAttribute(true, Map.empty)

[<RequireQualifiedAccess>]
[<WithThothJsonNetConverter>]
type ApiController  ()  =
    inherit DotNetNuke.Web.Api.DnnApiController ()