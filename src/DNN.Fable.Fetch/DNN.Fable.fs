module DNN.Fable

open Fable.Core
open Fetch
open DNN.Thoth.Json
open Fable.Core.JsInterop

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

type RequestProperties = Fetch.Types.RequestProperties
type Fetch =

    /// **Description**
    ///
    /// Retrieves data from the specified resource by applying the provided `decoder`.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `decoder` - parameter of type `Decoder<'Response>` - Decoder applied to the server response
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member fetchAs<'Response>
                                    (url : string,
                                     decoder : Decode.Decoder<'Response>,
                                     ?properties : RequestProperties list) =
        promise {
            let properties = defaultArg properties []
            let! response = fetch url properties
            let! body = response.text()
            let result = Decode.fromString decoder  body
                         |> function
                            | Ok result -> result
                            | Error msg -> failwith msg
            return result 
        }
         
    /// **Description**
    ///
    /// Retrieves data from the specified resource.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member fetchAs<'Response>(url : string,
                                     ?properties : RequestProperties list,
                                     ?isCamelCase : bool,
                                     [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        let decoder = Decode.Auto.generateDecoder<'Response'>(?isCamelCase=isCamelCase,?resolver= responseResolver)
        Fetch.fetchAs(url, decoder, ?properties = properties)


    static member fetchAs<'Data, 'Response>(url : string,
                                            data : 'Data,
                                            httpMethod : HttpMethod,
                                            ?properties : RequestProperties list,
                                            ?headers: HttpRequestHeaders list,
                                            ?isCamelCase : bool,
                                            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =

        let responseDecoder = Decode.Auto.generateDecoder<'Response>(?isCamelCase = isCamelCase, ?resolver = responseResolver)

        let body:BodyInit = !^ (Encode.Auto.toString (4, data))
            
        let properties =
            [ RequestProperties.Method httpMethod
              requestHeaders ( (ContentType "application/json") :: defaultArg headers [] ) 
              RequestProperties.Body body ]
            @ defaultArg properties []

        Fetch.fetchAs(url, responseDecoder, properties = properties)


    /// **Description**
    ///
    /// Retrieves data from the specified resource by applying the provided `decoder`.
    ///
    /// If the decoder succeed, we return `Ok 'Response`.
    ///
    /// If the decoder failed, we return `Error "explanation..."`
    ///
    /// **Parameters**
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `decoder` - parameter of type `Decoder<'Response>` - Decoder applied to the server response
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///
    /// **Output Type**
    ///   * `JS.Promise<Result<'Response,string>>`
    ///
    /// **Exceptions**
    ///
    static member tryFetchAs<'Response>( url : string,
                                         decoder : Decode.Decoder<'Response>,
                                         ?properties : RequestProperties list) =
        promise {
            let properties = defaultArg properties []
            let! response = fetch url properties
            let! body = response.text()
            return Decode.fromString decoder  body
        }

    /// **Description**
    ///
    /// Retrieves data from the specified resource.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member tryFetchAs<'Response>(url : string,
                                        ?properties : RequestProperties list,
                                        ?isCamelCase : bool,
                                        [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        let decoder = Decode.Auto.generateDecoder<'Response'>(?isCamelCase=isCamelCase, ?resolver= responseResolver)
        Fetch.tryFetchAs(url, decoder, ?properties = properties)

    static member tryFetchAs<'Data, 'Response>(url : string,
                                               data : 'Data,
                                               httpMethod : HttpMethod,
                                               ?properties : RequestProperties list,
                                               ?headers: HttpRequestHeaders list,
                                               ?isCamelCase : bool,
                                               [<Inject>] ?responseResolver: ITypeResolver<'Response>) =

        let responseDecoder = Decode.Auto.generateDecoder<'Response>(?isCamelCase = isCamelCase, ?resolver = responseResolver)

        let body:BodyInit = !^ (Encode.Auto.toString (4, data))

        let properties =
            [ RequestProperties.Method httpMethod
              requestHeaders ( (ContentType "application/json") :: defaultArg headers [] ) 
              RequestProperties.Body body ]
            @ defaultArg properties []

        Fetch.tryFetchAs(url, responseDecoder, properties = properties)

    /// **Description**
    ///
    /// Retrieves data from the specified resource.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member fetchAs<'Response>
           (moduleId : string, 
            moduleName : string,  
            url : string,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        let sf = ServicesFramework.setup moduleId moduleName url 
        let props = [ requestHeaders sf.Headers ; Credentials RequestCredentials.Sameorigin]  @ defaultArg properties []
        Fetch.fetchAs<'Response> (sf.Url, props , ?isCamelCase = isCamelCase, ?responseResolver = responseResolver )


    static member fetchAs<'Data, 'Response>
          ( moduleId : string, 
            moduleName : string,
            url: string,
            data : 'Data,
            httpMethod : HttpMethod,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =  
        let sf = ServicesFramework.setup moduleId moduleName url
        let props = (Credentials RequestCredentials.Sameorigin) :: defaultArg properties []
        let headers = sf.Headers 
        Fetch.fetchAs<'Data,'Response> (sf.Url, data, httpMethod, ?properties = Some props, ?headers = Some headers, ?isCamelCase= isCamelCase, ?responseResolver= responseResolver) 
    
    static member tryFetchAs<'Data, 'Response>
          ( moduleId : string, 
            moduleName : string,
            url: string,
            data : 'Data,
            httpMethod : HttpMethod,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =  
        let sf = ServicesFramework.setup moduleId moduleName url
        let props = (Credentials RequestCredentials.Sameorigin) :: defaultArg properties []
        let headers = sf.Headers 
        Fetch.tryFetchAs<'Data,'Response> (sf.Url, data, httpMethod, ?properties = Some props, ?headers = Some headers, ?isCamelCase= isCamelCase, ?responseResolver= responseResolver) 
    
    /// **Description**
    ///
    /// Retrieves data from the specified resource.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// If the decoder succeed, we return `Ok 'Response`.
    ///
    /// If the decoder failed, we return `Error "explanation..."`
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<Result<'Response,string>>`
    ///
    /// **Exceptions**
    ///
    static member tryFetchAs<'Response>
           (moduleId : string, 
            moduleName : string,  
            url : string,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        let sf = ServicesFramework.setup moduleId moduleName url 
        let props = [ requestHeaders sf.Headers ; Credentials RequestCredentials.Sameorigin]  @ defaultArg properties []
        Fetch.tryFetchAs<'Response> (sf.Url, props, ?isCamelCase = isCamelCase, ?responseResolver= responseResolver)

    /// Alias to `Fetch.fetchAs`
    static member get<'Response>
           (moduleId : string, 
            moduleName : string,  
            url : string,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.fetchAs<'Response>(moduleId, moduleName,url, ?properties = properties, ?isCamelCase = isCamelCase, ?responseResolver = responseResolver)

    /// Alias to `Fetch.tryFetchAs`
    static member tryGet<'Response>
              (moduleId : string, 
               moduleName : string, 
               url : string,
               ?properties : RequestProperties list,
               ?isCamelCase : bool,
               [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Response>(moduleId, moduleName, url, ?properties = properties, ?isCamelCase = isCamelCase, ?responseResolver = responseResolver)
    
    /// **Description**
    ///
    /// Send a **POST** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module   
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member post<'Data, 'Response> 
           (moduleId : string, 
            moduleName : string, 
            url : string,
            data : 'Data,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.POST, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)

    /// **Description**
    ///
    /// Send a **POST** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// If the decoder succeed, we return `Ok 'Response`.
    ///
    /// If the decoder failed, we return `Error "explanation..."`
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module  
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<Result<'Response,string>>`
    ///
    /// **Exceptions**
    ///
    static member tryPost<'Data, 'Response>(moduleId : string,
                                            moduleName : string, 
                                            url : string,
                                            data : 'Data,
                                            ?properties : RequestProperties list,
                                            ?isCamelCase : bool,
                                            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.POST, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)

    /// **Description**
    ///
    /// Send a **PUT** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module   
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member put<'Data, 'Response> 
           (moduleId : string, 
            moduleName : string, 
            url : string,
            data : 'Data,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =

        Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.PUT, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)

    /// **Description**
    ///
    /// Send a **PUT** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// If the decoder succeed, we return `Ok 'Response`.
    ///
    /// If the decoder failed, we return `Error "explanation..."`
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module  
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<Result<'Response,string>>`
    ///
    /// **Exceptions**
    ///
    static member tryPut<'Data, 'Response>(moduleId : string,
                                           moduleName : string, 
                                           url : string,
                                           data : 'Data,
                                           ?properties : RequestProperties list,
                                           ?isCamelCase : bool,
                                           [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.PUT, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)                                  
    
    /// **Description**
    ///
    /// Send a **PATCH** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module   
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member patch<'Data, 'Response> 
           (moduleId : string, 
            moduleName : string, 
            url : string,
            data : 'Data,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =

        Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.PATCH, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)

    /// **Description**
    ///
    /// Send a **PATCH** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// If the decoder succeed, we return `Ok 'Response`.
    ///
    /// If the decoder failed, we return `Error "explanation..."`
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module  
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<Result<'Response,string>>`
    ///
    /// **Exceptions**
    ///
    static member tryPatch<'Data, 'Response>(moduleId : string,
                                             moduleName : string, 
                                             url : string,
                                             data : 'Data,
                                             ?properties : RequestProperties list,
                                             ?isCamelCase : bool,
                                             [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.PATCH, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)   

    /// **Description**
    ///
    /// Send a **DELETE** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// An exception will be thrown if the decoder failed.
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module   
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member delete<'Data, 'Response> 
           (moduleId : string, 
            moduleName : string, 
            url : string,
            data : 'Data,
            ?properties : RequestProperties list,
            ?isCamelCase : bool,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.DELETE, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)

    /// **Description**
    ///
    /// Send a **DELETE** request to the specified resource and apply the provided `decoder` to the response.
    ///
    /// This method set the `ContentType` header to `"application/json"`.
    ///
    /// An encoder will be generated or retrieved from the cache for the `'Data` type.
    ///
    /// A decoder will be generated or retrieved from the cache for the `'Response` type.
    ///
    /// If the decoder succeed, we return `Ok 'Response`.
    ///
    /// If the decoder failed, we return `Error "explanation..."`
    ///
    /// **Parameters**
    ///   * `moduleId` - parameter of type `string` - DNN ModuleId ("int")
    ///   * `moduleName` - parameter of type `string` - Name of DNN Module  
    ///   * `url` - parameter of type `string` - URL to request
    ///   * `data` - parameter of type `'Data` - Data sent via the body, it will be converted to JSON before
    ///   * `properties` - parameter of type `RequestProperties list option` - Parameters passed to fetch
    ///   * `isCamelCase` - parameter of type `bool option` - Options passed to Thoth.Json to control JSON keys representation
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///   * `dataResolver` - parameter of type `ITypeResolver<'Data> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<Result<'Response,string>>`
    ///
    /// **Exceptions**
    ///
    static member tryDelete<'Data, 'Response>(moduleId : string,
                                              moduleName : string, 
                                              url : string,
                                              data : 'Data,
                                              ?properties : RequestProperties list,
                                              ?isCamelCase : bool,
                                              [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  data, HttpMethod.DELETE, ?properties = properties, ?isCamelCase= isCamelCase, ?responseResolver=responseResolver)                            