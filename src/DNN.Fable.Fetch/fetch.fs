module Dnn.Fable

open Fable.Core
open Fetch
open Thoth.Json

[<RequireQualifiedAccess>]
module ServicesFramework =
    type IServicesFramework =
        abstract getServiceRoot: string -> string
        abstract setModuleHeaders: obj -> unit
        abstract getTabId: unit -> int option
        abstract getModuleId: unit -> int option
        abstract getAntiForgeryValue: unit -> string option

    [<Emit("window['$'].ServicesFramework($0)")>]
    let init moduleid: IServicesFramework = jsNative

    let moduleHeaders (sf: IServicesFramework) =
        [ Custom("ModuleId", sf.getModuleId())
          Custom("TabId", sf.getTabId())
          Custom("RequestVerificationToken", sf.getAntiForgeryValue()) ]

    let setup' isGet moduleId moduleName url props =
        let sf = init moduleId
        let serviceroot = sf.getServiceRoot (moduleName)
        let props = [ Credentials RequestCredentials.Sameorigin ] @ defaultArg props []
        {| Url = serviceroot + url
           Props = props
           Headers = moduleHeaders sf |}

    let setup = setup' false

type Fetch =

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
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member fetchAs<'Response> (moduleId: string, moduleName: string, url: string,
                                      ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                      [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        let sf = ServicesFramework.setup' true moduleId moduleName url properties
        Thoth.Fetch.Fetch.fetchAs
            (sf.Url, properties = sf.Props, headers = sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver)

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
    ///   * `extra` - parameter of type `ExtraCoders option` - Options passed to Thoth.Json to extends the known coders
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<Result<'Response,string>>`
    ///
    /// **Exceptions**
    ///
    static member tryFetchAs<'Response> (moduleId: string, moduleName: string, url: string,
                                         ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                         [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        let sf = ServicesFramework.setup' true moduleId moduleName url properties
        Thoth.Fetch.Fetch.tryFetchAs
            (sf.Url, properties = sf.Props, headers = sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver)

    /// Alias to `Fetch.fetchAs`
    static member get<'Response> (moduleId: string, moduleName: string, url: string, ?properties: RequestProperties list,
                                  ?isCamelCase: bool, ?extra: ExtraCoders,
                                  [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.fetchAs<'Response>
            (moduleId, moduleName, url, ?properties = properties, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver)

    /// Alias to `Fetch.tryFetchAs`
    static member tryGet<'Response> (moduleId: string, moduleName: string, url: string,
                                     ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                     [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Response>
            (moduleId, moduleName, url, ?properties = properties, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver)

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
    static member post<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                          ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                          [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                          [<Inject>] ?dataResolver: ITypeResolver<'Data>) =

        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.post<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)

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
    static member tryPost<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                             ?properties: RequestProperties list, ?isCamelCase: bool,
                                             ?extra: ExtraCoders, [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                             [<Inject>] ?dataResolver: ITypeResolver<'Data>) =
        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.tryPost<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)

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
    static member put<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                         ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                         [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                         [<Inject>] ?dataResolver: ITypeResolver<'Data>) =

        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.put<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)

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
    static member tryPut<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                            ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                            [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                            [<Inject>] ?dataResolver: ITypeResolver<'Data>) =
        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.tryPut<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)

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
    static member patch<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                           ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                           [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                           [<Inject>] ?dataResolver: ITypeResolver<'Data>) =

        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.patch<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)

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
    static member tryPatch<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                              ?properties: RequestProperties list, ?isCamelCase: bool,
                                              ?extra: ExtraCoders,
                                              [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                              [<Inject>] ?dataResolver: ITypeResolver<'Data>) =
        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.tryPatch<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)

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
    static member delete<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                            ?properties: RequestProperties list, ?isCamelCase: bool, ?extra: ExtraCoders,
                                            [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                            [<Inject>] ?dataResolver: ITypeResolver<'Data>) =

        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.delete<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)

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
    static member tryDelete<'Data, 'Response> (moduleId: string, moduleName: string, url: string, data: 'Data,
                                               ?properties: RequestProperties list, ?isCamelCase: bool,
                                               ?extra: ExtraCoders,
                                               [<Inject>] ?responseResolver: ITypeResolver<'Response>,
                                               [<Inject>] ?dataResolver: ITypeResolver<'Data>) =
        let sf = ServicesFramework.setup moduleId moduleName url properties
        Thoth.Fetch.Fetch.tryDelete<'Data, 'Response>
            (sf.Url, data, sf.Props, sf.Headers, ?isCamelCase = isCamelCase, ?extra = extra,
             ?responseResolver = responseResolver, ?dataResolver = dataResolver)