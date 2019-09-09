namespace Dnn.Fable

open Fable.Core
open Fetch

type Fetch =
    inherit Fable.WebApi.Fetch
     
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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
    ///
    /// **Output Type**
    ///   * `JS.Promise<'Response>`
    ///
    /// **Exceptions**
    ///   * `System.Exception` - Contains information explaining why the decoder failed
    ///
    static member fetchAs<'Data,'Response>
           (moduleId : string, 
            moduleName : string,  
            url : string,
            ?httpMethod: HttpMethod,
            ?data: 'Data,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        let sf = ServicesFramework.setup moduleId moduleName url 
        let props = [Credentials RequestCredentials.Sameorigin]
        Fetch.fetchAs<'Data,'Response> (sf.Url, ?properties = Some props ,?httpMethod = httpMethod, ?data = data, ?headers = Some sf.Headers, ?responseResolver = responseResolver )

    static member tryFetchAs<'Data,'Response>
          ( moduleId : string, 
            moduleName : string,
            url: string,
            ?httpMethod: HttpMethod,
            ?data: 'Data,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =  
        let sf = ServicesFramework.setup moduleId moduleName url
        let props = [Credentials RequestCredentials.Sameorigin]
        Fetch.tryFetchAs<'Data,'Response> (sf.Url, ?properties = Some props ,?httpMethod = httpMethod, ?data = data, ?headers = Some sf.Headers, ?responseResolver = responseResolver )
        
    static member get<'Response>
           (moduleId : string, 
            moduleName : string,  
            url : string,
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.fetchAs<_,'Response>(moduleId, moduleName, url, ?responseResolver = responseResolver)

    static member tryGet<'Response>
              (moduleId : string, 
               moduleName : string, 
               url : string,
               [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<_,'Response>(moduleId, moduleName, url, ?responseResolver = responseResolver)
        
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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
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
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.POST, ?responseResolver=responseResolver)

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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
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
                                            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.POST, ?responseResolver=responseResolver)
        
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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
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
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =

        Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.PUT, ?responseResolver=responseResolver)
    
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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
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
                                           [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.PUT, ?responseResolver=responseResolver)
    
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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
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
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =

        Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.PATCH, ?responseResolver=responseResolver)
 
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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
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
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.PATCH, ?responseResolver=responseResolver)
 
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
            [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
       Fetch.fetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.DELETE, ?responseResolver=responseResolver)
 
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
    ///   * `responseResolver` - parameter of type `ITypeResolver<'Response> option` - Used by Fable to provide generic type info
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
                                              [<Inject>] ?responseResolver: ITypeResolver<'Response>) =
        Fetch.tryFetchAs<'Data, 'Response> (moduleId, moduleName, url,  ?data= Some data, ?httpMethod = Some HttpMethod.DELETE, ?responseResolver=responseResolver)
     