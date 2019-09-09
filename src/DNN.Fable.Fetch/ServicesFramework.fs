namespace Dnn.Fable

open Fable.Core
open Fable.Core.JsInterop
open Fetch

[<RequireQualifiedAccess>]
module ServicesFramework =
    type IServicesFramework = 
        abstract getServiceRoot: string -> string 
        abstract setModuleHeaders: obj -> unit
        abstract getTabId : unit -> int option
        abstract getModuleId : unit -> int option 
        abstract getAntiForgeryValue : unit -> string option

    [<Emit("window['$'].ServicesFramework($0)")>]
    let init moduleid :IServicesFramework  = jsNative 

    let moduleHeaders (sf:IServicesFramework) = [    
        Custom ("ModuleId", sf.getModuleId())
        Custom ("TabId", sf.getTabId())
        Custom ("RequestVerificationToken", sf.getAntiForgeryValue())]

    let setup moduleId moduleName url =
        let sf = init moduleId    
        let serviceroot = sf.getServiceRoot(moduleName)
        {| Url = serviceroot + url ; Headers = moduleHeaders sf  |}
