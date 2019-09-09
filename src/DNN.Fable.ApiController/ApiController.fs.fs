[<RequireQualifiedAccess>]
module Dnn.Fable

open Fable.WebApi

[<FableWebApi>]
type ApiController  ()  =
    inherit DotNetNuke.Web.Api.DnnApiController ()