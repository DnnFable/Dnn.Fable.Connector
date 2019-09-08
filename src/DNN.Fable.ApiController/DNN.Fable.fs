module DNN.Fable

open Fable.WebApi

[<FableWebApi>]
[<RequireQualifiedAccess>]
type ApiController  ()  =
    inherit DotNetNuke.Web.Api.DnnApiController ()