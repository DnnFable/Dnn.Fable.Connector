module DNN.Fable
open System.Web.Http.Controllers
open System.Net.Http.Formatting
open Newtonsoft.Json
open Thoth.Json.Net.Converters

type WithThothJsonNetConverterAttribute() =
    inherit System.Attribute()
    interface IControllerConfiguration with
        member __.Initialize((controllerSettings : HttpControllerSettings), _) =
            let converter = CacheConverter(converters)
            let thothFormatter =
                JsonMediaTypeFormatter
                    (SerializerSettings = JsonSerializerSettings(Converters = [| converter |]))
            controllerSettings.Formatters.Clear()
            controllerSettings.Formatters.Add thothFormatter

[<WithThothJsonNetConverter>]
type ApiController  ()  =
    inherit DotNetNuke.Web.Api.DnnApiController ()