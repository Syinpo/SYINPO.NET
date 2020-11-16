using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using AutoBogus;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Syinpo.ApiDoc.Generation.Compile;
using Syinpo.ApiDoc.Generation.Model;
using Syinpo.ApiDoc.Generation.swagger.Recursive;
using Syinpo.ApiDoc.Generation.T4;
using Syinpo.ApiDoc.Generation.Utitls;
using Syinpo.Core.Extensions;
using Syinpo.Core.IO;
using Syinpo.Core.Reflection;
using Syinpo.Unity.Bogus;
using Microsoft.AspNetCore.Hosting;
using Microsoft.CodeAnalysis;
using Microsoft.DocAsCode.Build.RestApi.Swagger;
using Microsoft.DocAsCode.DataContracts.RestApi;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using YamlDotNet.Serialization;
using BuildRestApiDocument = Syinpo.ApiDoc.Generation.swagger.BuildRestApiDocument;
using ReflectionUtils = Syinpo.ApiDoc.Generation.Utitls.ReflectionUtils;

namespace Syinpo.ApiDoc.Generation {
    class Program {
        private static List<ClassFileInfo> codes = new List<ClassFileInfo>();
        private static List<DocNode> docmap = new List<DocNode>();
        private static ITypeFinder find;

        static void Main( string[] args ) {
            // 获取代码文件路径
            var codeBuild = new CodeBuild();
            codes = codeBuild.Deep();
            codeBuild.Save( codes );

            // 生成Model
            if( true ) {
                var serviceProvider = new ServiceCollection();
                serviceProvider.AddSingleton( typeof( IAssemblyFinder ), typeof( AssemblyFinder ) );
                serviceProvider.AddSingleton( typeof( ITypeFinder ), typeof( TypeFinder ) );
                serviceProvider.AddSingleton( typeof( ITypeResolve ), typeof( TypeResolve ) );
                Syinpo.Core.IoC.Init( serviceProvider.BuildServiceProvider(), null );
                find = Syinpo.Core.IoC.Resolve<ITypeFinder>();

                new FakerDto().GenerateJsonFiles(
                    BuildWorkspaceHelper.GetRelativeWorkspacePath( @"Syinpo.SignalRDoc.Generation\swagger\Models" ) );
            }


            List<AllModel.DefinitionInfo> definitions = new List<AllModel.DefinitionInfo>();
            string swaggerFile = AppContext.BaseDirectory + @"\swagger\swagger.json";
            var swagger = SwaggerJsonParser.Parse( swaggerFile );
            if( swagger.Definitions != null ) {
                var a = (JObject)swagger.Definitions;
                ProcessDefinitions( a, definitions );
            }
            foreach( var definition in definitions ) {
                ProcessDefinitions( definition.Parameters, definitions );
            }


            BuildRestApiDocument doc = new BuildRestApiDocument();
            var model = doc.ProcessSwagger();
            foreach( var page in model.Children ) {
                DocNode rootNode = null;
                DocNode currentNode = null;

                {
                    string rootName = page.Tags[ 0 ];
                    rootNode = docmap.FirstOrDefault( f => f.Name == rootName );
                    if( rootNode == null ) {
                        var root = model.Tags.FirstOrDefault( f => f.Name == rootName );

                        string roottitle = Regex.Replace( root?.Description ?? rootName, "<[^>]*>", string.Empty )
                            .Replace( "\r", "" )
                            .Replace( "\n", "" );
                        rootNode = new DocNode {
                            Name = rootName,
                            Title = roottitle,
                            Level = 0,
                            Path = rootName
                        };
                        docmap.Add( rootNode );
                    }

                    string title = Regex.Replace( page.Summary ?? "", "<[^>]*>", string.Empty )
                        .Replace( "\r", "" )
                        .Replace( "\n", "" );
                    currentNode = new DocNode {
                        Level = 1,
                        Name = page.OperationId,
                        Title = string.IsNullOrEmpty( title ) ? page.OperationId : title,
                        Path = page.OperationId + ".md"
                    };
                    rootNode.Children.Add( currentNode );
                }


                AllModel all = new AllModel();
                all.Info = new AllModel.ApiActionInfo {
                    Name = StringUtils.RemoveHtmlTags( page.Summary ?? "" ),
                    Description = StringUtils.RemoveHtmlTags( page.Description ?? "" ),
                    OperationName = StringUtils.ProperCase( page.OperationName ),
                    OperationId = page.OperationId,
                    ResourcePath = page.Path ?? "",
                    Authentication = page.Metadata.ContainsKey( "security" ),
                };

                all.EnvironmentResources.Add( new AllModel.EnvironmentResource {
                    Environment = "Test",
                    LoacalUrl = "http://192.168.50.58:7000" + all.Info.ResourcePath,
                    Description = "测试"
                } );
                all.EnvironmentResources.Add( new AllModel.EnvironmentResource {
                    Environment = "Product",
                    LoacalUrl = "https://api-test.syinpo.com" + all.Info.ResourcePath,
                    Description = "生产"
                } );

                page.Parameters = page.Parameters ?? new List<RestApiParameterViewModel>();

                var requestBody = page.Parameters.Where( w => w.Metadata.Any( p => p.Key == "in" && (string)p.Value == "body" ) ).ToList();
                foreach( var par in requestBody ) {
                    if( par.Metadata.ContainsKey( "schema" ) ) {
                        Process( (JObject)par.Metadata[ "schema" ], all.RequestBodyParameters, definitions );

                        if( ( (JObject)par.Metadata[ "schema" ] )[ "x-internal-ref-name" ] != null ) {
                            var type = (string)( (JObject)par.Metadata[ "schema" ] )[ "x-internal-ref-name" ];
                            all.RequestBodyDefinition = definitions.FirstOrDefault( f => f.Name == type );
                        }
                    }
                    else {
                        all.RequestBodyParameters.Add( PreParameterInfo( par ) );
                    }

                    CurrentInDefinitions( all.RequestBodyParameters, all.Definitions );
                }



                var path = page.Parameters.Where( w => w.Metadata.Any( p => p.Key == "in" && (string)p.Value == "path" ) ).ToList();
                foreach( var par in path ) {
                    all.RequestPathParameters.Add( PreParameterInfo( par ) );
                }

                var queryString = page.Parameters.Where( w => w.Metadata.Any( p => p.Key == "in" && (string)p.Value == "query" ) ).ToList();
                foreach( var par in queryString ) {
                    if( par.Metadata.ContainsKey( "schema" ) ) {
                        Process( (JObject)par.Metadata[ "schema" ], all.RequestQueryParameters, definitions );
                    }
                    else {
                        all.RequestQueryParameters.Add( PreParameterInfo( par ) );
                    }

                    CurrentInDefinitions( all.RequestQueryParameters, all.Definitions );
                }



                foreach( var response in page.Responses ) {
                    var resp = new AllModel.ResponseInfo {
                        MediaType = "json/text",
                        StatusCode = response.HttpStatusCode,
                        Description = response.Description,
                    };


                    if( response.Metadata.ContainsKey( "schema" ) ) {
                        Process( (JObject)response.Metadata[ "schema" ], resp.Parameters, definitions );
                    }
                    else {
                        // all.RequestBodyParameters.Add(PreParameterInfo(response));
                    }
                    CurrentInDefinitions( resp.Parameters, all.Definitions );


                    all.Responses.Add( resp );
                }


                {
                    all.Example = new AllModel.ExampleInfo {
                        RequestBodyJson = all.RequestBodyDefinition?.SchemaJson ?? "",
                        ResponseJson = ""
                    };

                    var okResponse = all.Responses.FirstOrDefault( f => f.StatusCode == "0" );
                    if( okResponse != null ) {
                        JObject obj = new JObject();
                        obj.Add( "code", "0" );
                        obj.Add( "message", "ok" );


                        var data = okResponse.Parameters.FirstOrDefault( f => f.Name == "data" );
                        if( data != null ) {
                            if( data.IsDefinition ) {
                                var definition = data.Definition;
                                if( definition != null && definition.SourceCode != null && !string.IsNullOrEmpty( definition.SourceCode.Type ) ) {
                                    var ins = ReflectionUtils.CreateInstanceFromString( definition.SourceCode.Type );

                                    if( ins != null ) {
                                        //definition.SchemaJson = JsonHelper.ToJson(AutoFaker.Generate(ins.GetType()));
                                        obj.Add( "data", JToken.Parse( JsonHelper.ToJson( AutoFaker.Generate( ins.GetType() ) ) ) );
                                    }
                                    else {
                                        var text = GetSourCodeJson( definition.SourceCode.Type );
                                        if( !string.IsNullOrEmpty( text ) ) {
                                            obj.Add( "data", JToken.Parse( text ) );
                                        }
                                    }
                                }

                                else if( definition != null && definition.Name.Contains( "PageResult[" ) ) {
                                    var def = definition.Parameters.FirstOrDefault( f => f.IsDefinition );

                                    var result = GetSourCodeJson( def.Definition.SourceCode.Type );
                                    if( string.IsNullOrEmpty( result ) )
                                        result = def.Definition.SchemaJson;

                                    var jsonString = @"{
                                                            ""page"": 5,
                                                            ""pageSize"": 20,
                                                            ""totalCount"": 100,
                                                            ""totalPages"": 5,
                                                            ""hasPreviousPage"": true,
                                                            ""hasNextPage"": false,
                                                            ""results"": [ " + result + "]" +
                                                     "}";
                                    obj.Add( "data", JToken.Parse( jsonString ) );
                                }

                            }
                            else {
                                var type = StringToType( data.DataType );
                                if( type != null )
                                    obj.Add( "data", JToken.Parse( JsonHelper.ToJson( AutoFaker.Generate( type ) ) ) );
                            }

                        }

                        all.Example.ResponseJson = JsonHelper.ToJson( obj ) ?? "";
                    }
                }



                RestApiRtt apiRtt = new RestApiRtt( all );
                String pageContent = apiRtt.TransformText().Replace( "\t", "" );

                //string html = Markdown.Parse( pageContent );
                //var html2 = Markdig.Markdown.ToHtml( pageContent );

                var rootDir = Directory.GetCurrentDirectory() + "\\doc\\" + rootNode.Path;
                if( !Directory.Exists( rootDir ) ) {
                    Directory.CreateDirectory( rootDir );
                }

                var filePath = Path.Combine( rootDir, currentNode.Path );
                File.WriteAllText( filePath, pageContent, System.Text.Encoding.UTF8 );
            }


            {

                var roots = new JArray() as dynamic;
                {
                    dynamic item = new JObject();
                    item.name = "登录授权接入";

                    item.items = new JArray() as dynamic;
                    {
                        dynamic song = new JObject();
                        song.name = "烽客账号体系简介";
                        song.href = "IdentityServer/index.md";
                        item.items.Add( song );
                    }
                    {
                        dynamic song = new JObject();
                        song.name = "申请授权令牌AccessToken";
                        song.href = "IdentityServer/CreateToken.md";
                        item.items.Add( song );
                    }
                    {
                        dynamic song = new JObject();
                        song.name = "刷新授权令牌";
                        song.href = "IdentityServer/RefeshToken.md";
                        item.items.Add( song );
                    }
                    {
                        dynamic song = new JObject();
                        song.name = "使用OAuth2.0调用API";
                        song.href = "IdentityServer/InvokApi.md";
                        item.items.Add( song );
                    }

                    roots.Add( item );
                }

                dynamic common = new JObject();
                {
                    common.name = "公共";
                    common.items = new JArray() as dynamic;
                }

                dynamic admin = new JObject();
                {
                    admin.name = "后台";
                    admin.items = new JArray() as dynamic;
                }

                dynamic console = new JObject();
                {
                    console.name = "中台";
                    console.items = new JArray() as dynamic;
                }

                dynamic device = new JObject();
                {
                    device.name = "设备";
                    device.items = new JArray() as dynamic;
                }

                dynamic customer = new JObject();
                {
                    customer.name = "客服";
                    customer.items = new JArray() as dynamic;
                }

                dynamic monitor = new JObject();
                {
                    monitor.name = "监控";
                    monitor.items = new JArray() as dynamic;
                }

                foreach( var docroot in docmap ) {
                    if( docroot.Children.Count > 0 ) {
                        dynamic item = new JObject();
                        item.name = docroot.Title;

                        item.items = new JArray() as dynamic;
                        foreach( var child in docroot.Children ) {
                            dynamic song = new JObject();
                            song.name = child.Title;
                            song.href = docroot.Path + "/" + child.Path;
                            item.items.Add( song );
                        }

                        switch( docroot.Name ) {
                            case "Common":
                            case "ShortUrl":
                                common.items.Add( item );
                                break;
                            case "DeviceManagement":
                            case "QuickReplyManagement":
                            case "CustomerServiceManagement":
                            case "UpdateManagement":
                            case "DeviceBatchManagement":
                            case "UploadRetryManagement":
                            case "GlobalManagement":
                                device.items.Add( item );
                                break;
                            case "CustomerWkf":
                            case "QuickReplyWkf":
                            case "WeChatWkf":
                                customer.items.Add( item );
                                break;
                            case "MonitorMetric":
                            case "MonitorLog":
                                monitor.items.Add( item );
                                break;
                            case "AutoReplyConsole":
                            case "CallLogConsole":
                            case "CatalogConsole":
                            case "CustomerConsole":
                            case "DeptConsole":
                            case "DeviceConsole":
                            case "DeviceContactConsole":
                            case "DeviceGroupConsole":
                            case "DeviceSettingConsole":
                            case "DeviceSmsConsole":
                            case "HuomaConsole":
                            case "JieLongConsole":
                            case "MaterialConsole":
                            case "MenuConsole":
                            case "MessageConsole":
                            case "PartnerQuickReplyConsole":
                            case "PartnerSysTagConsole":
                            case "PermissionConsole":
                            case "PostConsole":
                            case "QunAnnouncementConsole":
                            case "QunAutoReplyConsole":
                            case "QunBehaviorRuleConsole":
                            case "QunGreetingsConsole":
                            case "QunGroupConsole":
                            case "QunMemberDistinctConsole":
                            case "QunSettingConsole":
                            case "QunWhiteBlackListConsole":
                            case "SendFirendCircleConsole":
                            case "SensitiveRecordConsole":
                            case "SensitiveWordConsole":
                            case "StatisticConsole":
                            case "SystemLogConsole":
                            case "SystemSettingsConsole":
                            case "TemplateConsole":
                            case "TrackingStatusConsole":
                            case "TimeMessageConsole":
                            case "UserConsole":
                            case "UserRoleConsole":
                            case "WeiXinConsole":
                            case "WeiXinFinancialConsole":
                            case "License":
                            case "PartnerWallet":
                            case "PartnerRenew":
                            case "SmsTemplate":
                                console.items.Add( item );
                                break;
                            case "Token":
                                break;
                            default:
                                admin.items.Add( item );
                                break;
                        }

                    }
                }

                roots.Add( common );
                roots.Add( admin );
                roots.Add( console );
                roots.Add( monitor );

                {
                    var files = new List<string>
                    {
                        BuildWorkspaceHelper.GetRelativeWorkspacePath(@"Syinpo.SignalRDoc.Generation\bin\Debug\netcoreapp3.1\doc\WeiXinMessage.yml"),
                        BuildWorkspaceHelper.GetRelativeWorkspacePath(@"Syinpo.SignalRDoc.Generation\bin\Debug\netcoreapp3.1\doc\Enums.yml")
                    };

                    foreach( var file in files ) {
                        var yamlText = File.ReadAllText( file );

                        var deserializer = new Deserializer();
                        var yamlObject = deserializer.Deserialize( new StringReader( yamlText ) );

                        var yamlSerializer = new SerializerBuilder()
                            .JsonCompatible()
                            .Build();

                        var jsonObject = JArray.Parse( yamlSerializer.Serialize( yamlObject ) ).FirstOrDefault();
                        if( jsonObject != null )
                            device.items.Add( jsonObject );
                    }
                }
                roots.Add( device );

                {
                    var files = new List<string>
                    {
                        BuildWorkspaceHelper.GetRelativeWorkspacePath(@"Syinpo.SignalRDoc.Generation\bin\Debug\netcoreapp3.1\doc\WeiXinMessage.yml"),
                        BuildWorkspaceHelper.GetRelativeWorkspacePath(@"Syinpo.SignalRDoc.Generation\bin\Debug\netcoreapp3.1\doc\Enums.yml")
                    };

                    foreach( var file in files ) {
                        var yamlText = File.ReadAllText( file );

                        var deserializer = new Deserializer();
                        var yamlObject = deserializer.Deserialize( new StringReader( yamlText ) );

                        var yamlSerializer = new SerializerBuilder()
                            .JsonCompatible()
                            .Build();

                        var jsonObject = JArray.Parse( yamlSerializer.Serialize( yamlObject ) ).FirstOrDefault();
                        if( jsonObject != null )
                            customer.items.Add( jsonObject );
                    }
                }
                roots.Add( customer );

                {
                    var files = new List<string>
                    {
                        BuildWorkspaceHelper.GetRelativeWorkspacePath( @"Syinpo.SignalRDoc.Generation\bin\Debug\netcoreapp3.1\doc\GroupControl.yml"),
                    };

                    foreach( var file in files ) {
                        var yamlText = File.ReadAllText( file );

                        var deserializer = new Deserializer();
                        var yamlObject = deserializer.Deserialize( new StringReader( yamlText ) );

                        var yamlSerializer = new SerializerBuilder()
                            .JsonCompatible()
                            .Build();

                        var jsonObject = JArray.Parse( yamlSerializer.Serialize( yamlObject ) ).FirstOrDefault();
                        if( jsonObject != null )
                            roots.Add( jsonObject );
                    }
                }

                {
                    var files = new List<string>
                    {
                        BuildWorkspaceHelper.GetRelativeWorkspacePath( @"Syinpo.SignalRDoc.Generation\bin\Debug\netcoreapp3.1\doc\SensitiveControl.yml"),
                    };

                    foreach( var file in files ) {
                        var yamlText = File.ReadAllText( file );

                        var deserializer = new Deserializer();
                        var yamlObject = deserializer.Deserialize( new StringReader( yamlText ) );

                        var yamlSerializer = new SerializerBuilder()
                            .JsonCompatible()
                            .Build();

                        var jsonObject = JArray.Parse( yamlSerializer.Serialize( yamlObject ) ).FirstOrDefault();
                        if( jsonObject != null )
                            roots.Add( jsonObject );
                    }
                }

                {
                    var files = new List<string>
                    {
                        BuildWorkspaceHelper.GetRelativeWorkspacePath(@"Syinpo.SignalRDoc.Generation\bin\Debug\netcoreapp3.1\doc\toc.yml"),
                    };

                    foreach( var file in files ) {
                        var yamlText = File.ReadAllText( file );

                        var deserializer = new Deserializer();
                        var yamlObject = deserializer.Deserialize( new StringReader( yamlText ) );

                        var yamlSerializer = new SerializerBuilder()
                            .JsonCompatible()
                            .Build();

                        var jsonObject = JArray.Parse( yamlSerializer.Serialize( yamlObject ) ).FirstOrDefault();
                        if( jsonObject != null )
                            roots.Add( jsonObject );
                    }
                }

                var json = JsonHelper.ToJson( roots );

                var expConverter = new ExpandoObjectConverter();
                // deserializedObject = JsonConvert.DeserializeObject<object[]>(json);
                dynamic deserializedObject = deserializedObject = JsonConvert.DeserializeObject<List<ExpandoObject>>( json, expConverter );

                var serializer = new YamlDotNet.Serialization.Serializer();
                string yaml = serializer.Serialize( deserializedObject );

                var filePath = Directory.GetCurrentDirectory() + "\\doc\\toc.yml";
                File.WriteAllText( filePath, yaml, System.Text.Encoding.UTF8 );
            }
        }


        public static void Process_Tree( JObject target, List<AllModel.ParameterInfo> parameters ) {
            //if( par.Metadata.ContainsKey("schema") )
            //{
            //    Process((JObject)par.Metadata[ "schema" ], all.RequestBodyParameters);
            //}

            if( target == null )
                return;

            var type = target[ "type" ].Value<string>();
            JObject properties = null;

            if( IsEnumerableType( type ) ) {
                if( target.Properties().Any( a => a.Name == "properties" ) ) {
                    properties = (JObject)( target[ "properties" ] );
                }
                // 字典
                else if( target.Properties().Any( a => a.Name == "additionalProperties" ) ) {
                    if( IsEnumerableType( target[ "additionalProperties" ][ "type" ].Value<string>() ) ) {
                        properties = (JObject)( target[ "additionalProperties" ][ "properties" ] );
                    }
                }
            }
            else if( type == "array" ) {
                if( IsEnumerableType( target[ "type" ][ "items" ][ "type" ].Value<string>() ) ) {
                    properties = (JObject)( target[ "type" ][ "items" ][ "properties" ] );
                }
            }


            if( properties == null ) {
                return;
            }

            foreach( var property in properties ) {
                var p = PreParameterInfo( property );

                JObject array = null;
                var pType = property.Value.Value<string>( "type" );
                if( IsEnumerableType( pType ) ) {
                    if( property.Value[ "properties" ] != null ) {
                        array = (JObject)property.Value[ "properties" ];
                    }
                    // 字典
                    else if( property.Value[ "additionalProperties" ] != null ) {
                        if( IsEnumerableType( property.Value[ "additionalProperties" ][ "type" ].Value<string>() ) ) {
                            array = (JObject)( property.Value[ "additionalProperties" ][ "properties" ] );
                        }
                    }
                }
                else if( pType == "array" ) {
                    if( IsEnumerableType( property.Value[ "items" ][ "type" ].Value<string>() ) ) {
                        array = (JObject)( property.Value[ "items" ][ "properties" ] );
                    }
                }

                //if( IsEnumerableType(property.Value.Value<string>("type")) )
                //{
                //    var array = (JObject)property.Value[ "properties" ];
                if( array != null ) {
                    foreach( var item in array ) {
                        var p2 = PreParameterInfo( item );

                        if( IsEnumerableType( item.Value.Value<string>( "type" ) ) ) {
                            /* Process_Tree((JObject)item.Value, p2.Parameters); */
                        }

                        /*  p.Parameters.Add(p2); */
                    }
                }
                //}

                parameters.Add( p );
            }
        }

        public static void Process( JObject target, List<AllModel.ParameterInfo> parameters, List<AllModel.DefinitionInfo> find ) {
            if( target == null )
                return;

            JObject properties = (JObject)( target[ "properties" ] );

            if( properties == null ) {
                return;
            }

            AllModel.DefinitionInfo d = null;
            if( target[ "x-internal-ref-name" ] != null ) {
                var type = (string)( target[ "x-internal-ref-name" ] );
                d = find.FirstOrDefault( f => f.Name == type );
            }


            foreach( var property in properties ) {
                var p = PreParameterInfo( property );


                if( d != null ) {
                    var cp = d.Parameters.FirstOrDefault( f => f.Name == p.Name );
                    if( cp != null ) {
                        var typeString = cp.DataType;
                        if( !string.IsNullOrEmpty( typeString ) ) {
                            p.DataType = typeString;
                        }
                    }
                }


                if( IsDefinitionType( property.Value ) ) {
                    p.IsDefinition = true;
                    var dname = DefinitionName( property.Value );
                    if( !p.DataType.EndsWith( ")" ) ) {
                        p.DataType += $"({dname})";
                    }
                    p.Ref = dname;

                    var definition = find.Find( f => f.Name == dname );
                    if( definition != null )
                        p.Definition = definition;

                }

                parameters.Add( p );
            }
        }


        public static void ProcessDefinitions( JObject target, List<AllModel.DefinitionInfo> all ) {
            if( target == null )
                return;

            JObject properties = target;
            foreach( var property in properties ) {
                var p = PreDefinitionInfo( property );

                var array = (JObject)property.Value[ "properties" ];
                if( array != null ) {
                    var type = find.Find( f => f.Name == property.Key ).FirstOrDefault();
                    var pros = new List<PropertyInfo>();
                    if( type != null ) {
                        pros = type
                        .GetProperties( BindingFlags.Instance | BindingFlags.Public )
                        .Where( x =>
                        {
                            if( x.CanWrite && x.GetSetMethod( false ) != null )
                                return x.GetSetMethod().GetParameters().Length == 1;
                            return false;
                        } )
                        .ToList();
                    }

                    foreach( var item in array ) {
                        var p2 = PreParameterInfo( item );

                        var cp = pros.FirstOrDefault( f => f.Name.ToLowerInvariant() == p2.Name.ToLowerInvariant() );
                        if( cp != null ) {
                            var typeString = TypeToString( cp.PropertyType );
                            if( !string.IsNullOrEmpty( typeString ) ) {
                                p2.DataType = typeString;
                            }
                        }

                        if( IsDefinitionType( item.Value ) ) {
                            p2.IsDefinition = true;
                            var dname = DefinitionName( item.Value );
                            p2.DataType += $"({dname})";
                            p2.Ref = dname;
                        }

                        p.Parameters.Add( p2 );
                    }
                }
                else if( property.Value[ "enum" ] != null ) {
                    var enm = find.Find( f => f.Name.Contains( property.Key ) ).FirstOrDefault();
                    if( enm != null ) {
                        var enumValues = Enum.GetValues( enm ).Cast<Enum>().ToList();
                        foreach( var @enum in enumValues ) {
                            var par = new AllModel.ParameterInfo {
                                Name = @enum.ToString(),
                                Description = @enum.GetDisplayName(),
                                DataType = Convert.ToInt32( @enum ).ToString() /*+ "(enum)"*/,
                                DefaultValue = Convert.ToInt32( @enum ).ToString(),
                                Required = false
                            };

                            p.Parameters.Add( par );
                        }
                    }
                }

                all.Add( p );
            }
        }

        public static void ProcessDefinitions( List<AllModel.ParameterInfo> target, List<AllModel.DefinitionInfo> find ) {
            if( target == null || !target.Any() )
                return;

            var properties = target;
            foreach( var property in properties ) {
                if( property.IsDefinition && property.Definition == null ) {
                    var definition = find.Find( f => f.Name == property.Ref );
                    if( definition != null ) {
                        property.Definition = definition;

                        var array = property.Definition.Parameters;
                        if( array != null && array.Any() ) {
                            ProcessDefinitions( array, find );
                        }
                    }
                }
            }
        }

        public static void CurrentInDefinitions( List<AllModel.ParameterInfo> target, List<AllModel.DefinitionInfo> all ) {
            if( target == null || !target.Any() )
                return;

            var properties = target;
            foreach( var property in properties ) {
                if( property.IsDefinition && property.Definition != null ) {
                    if( !all.Any( p => p.Name == property.Definition.Name ) ) {
                        all.Add( property.Definition );

                        // }

                        var array = property.Definition.Parameters;
                        if( array != null && array.Any() ) {
                            CurrentInDefinitions( array, all );
                        }
                    }
                }
            }
        }



        public static bool IsEnumerableType( string type ) {
            return type == "object";
        }

        public static bool IsDefinitionType( JToken token ) {
            var flag1 = false;
            flag1 = token[ "x-internal-ref-name" ] != null;
            if( !flag1 ) {
                flag1 = token[ "additionalProperties" ] != null && token[ "additionalProperties" ][ "x-internal-ref-name" ] != null;
            }

            if( !flag1 ) {
                flag1 = token[ "items" ] != null && token[ "items" ][ "x-internal-ref-name" ] != null;
            }

            return flag1;
        }

        public static string DefinitionName( JToken token ) {
            string result = string.Empty;

            if( token[ "x-internal-ref-name" ] != null ) {
                result = token[ "x-internal-ref-name" ].Value<string>();
            }
            if( string.IsNullOrEmpty( result ) ) {
                if( token[ "additionalProperties" ] != null )
                    result = token[ "additionalProperties" ][ "x-internal-ref-name" ].Value<string>();
            }

            if( string.IsNullOrEmpty( result ) ) {
                if( token[ "items" ] != null )
                    result = token[ "items" ][ "x-internal-ref-name" ].Value<string>();
            }

            return result;
        }

        public static AllModel.ParameterInfo PreParameterInfo( KeyValuePair<string, JToken> pro ) {
            return new AllModel.ParameterInfo {
                Name = pro.Key,
                Description = ( (string)pro.Value[ "description" ] ) ?? string.Empty,
                DataType = ( (string)pro.Value[ "type" ] ) ?? string.Empty,
                DefaultValue = ( (string)pro.Value[ "default" ] ) ?? string.Empty,
                Required = pro.Value.Contains( "required" ) ? (bool)pro.Value[ "required" ] : false,
                IsDefinition = false,
            };
        }

        public static AllModel.ParameterInfo PreParameterInfo( RestApiParameterViewModel par ) {
            return new AllModel.ParameterInfo {
                Name = par.Name,
                Description = par.Description,
                DataType = par.Metadata.ContainsKey( "type" ) ? par.Metadata[ "type" ].ToString() : string.Empty,
                DefaultValue = par.Metadata.ContainsKey( "default" ) ? par.Metadata[ "default" ].ToString() : string.Empty,
                Required = par.Metadata.ContainsKey( "required" ) ? (bool)par.Metadata[ "required" ] : false,
                IsDefinition = false,
            };
        }

        public static AllModel.DefinitionInfo PreDefinitionInfo( KeyValuePair<string, JToken> pro ) {
            var definition = new AllModel.DefinitionInfo {
                Name = pro.Key,
                Description = ( (string)pro.Value[ "description" ] ) ?? string.Empty,
                DataType = ( (string)pro.Value[ "type" ] ) ?? string.Empty,
                Ref = "#/definitions/" + pro.Key
            };

            string newDescription = definition.Description;
            var sourceCodeInfo = new RecursiveSourceCodeDescription().Resolve( ref newDescription );
            definition.Description = newDescription;
            if( sourceCodeInfo == null ) {
                var source = codes.FirstOrDefault( f => f.ClassName == pro.Key );
                if( source != null ) {
                    sourceCodeInfo = new SourceCodeInfo {
                        Link = @"../../src/Syinpo.Model/" + source.Key,
                        Type = source.FullClassName,
                        ViewSource = "http://192.168.50.58:9090/#Syinpo.Model/" + source.Key,
                    };
                }
                else {
                    sourceCodeInfo = new SourceCodeInfo { };
                }
            }

            definition.SourceCode = sourceCodeInfo;
            if( definition.SourceCode != null && !string.IsNullOrEmpty( definition.SourceCode.Type ) ) {

                var ins = ReflectionUtils.CreateInstanceFromString( definition.SourceCode.Type );

                if( ins != null ) {
                    definition.SchemaJson = JsonHelper.ToJson( AutoFaker.Generate( ins.GetType() ) );
                }
                else {
                    var text = GetSourCodeJson( definition.SourceCode.Type );
                    if( !string.IsNullOrEmpty( text ) ) {
                        definition.SchemaJson = text;
                    }
                }
            }

            if( string.IsNullOrWhiteSpace( definition.SchemaJson ) )
                definition.SchemaJson = string.Empty;

            return definition;
        }

        public static string GetSourCodeJson( string type ) {
            var path = BuildWorkspaceHelper.GetRelativeWorkspacePath( @"Syinpo.SignalRDoc.Generation\swagger\Models\" + type +
                       ".json" );
            if( File.Exists( path ) )
                return File.ReadAllText( path );

            return string.Empty;
        }


        public static Type StringToType( string stringValue ) {
            Type value = null;
            if( stringValue != null )
                stringValue = stringValue.ToLowerInvariant();

            if( stringValue == null )
                value = null;
            else if( stringValue == "Boolean".ToLowerInvariant() )
                value = typeof( bool );
            else if( stringValue == "Char".ToLowerInvariant() )
                value = typeof( Char );
            else if( stringValue == "SByte".ToLowerInvariant() )
                value = typeof( SByte );
            else if( stringValue == "Byte".ToLowerInvariant() )
                value = typeof( Byte );
            else if( stringValue == "Short".ToLowerInvariant() )
                value = typeof( Int16 );
            else if( stringValue == "UShort".ToLowerInvariant() )
                value = typeof( UInt16 );
            else if( stringValue == "Integer".ToLowerInvariant() )
                value = typeof( int );
            else if( stringValue == "UInteger".ToLowerInvariant() )
                value = typeof( UInt32 );
            else if( stringValue == "Long".ToLowerInvariant() )
                value = typeof( long );
            else if( stringValue == "ULong".ToLowerInvariant() )
                value = typeof( UInt64 );
            else if( stringValue == "Date".ToLowerInvariant() )
                value = typeof( DateTime );
            else if( stringValue == "Decimal".ToLowerInvariant() )
                value = typeof( decimal );
            else if( stringValue == "Single".ToLowerInvariant() )
                value = typeof( Single );
            else if( stringValue == "Double".ToLowerInvariant() )
                value = typeof( double );
            else if( stringValue == "String".ToLowerInvariant() )
                value = typeof( string );
            else
                value = typeof( object );

            return value;
        }

        public static string TypeToString( Type stringValue ) {
            string value = null;
            if( stringValue == null )
                value = null;
            else if( stringValue == typeof( Boolean ) || stringValue == typeof( bool ) )
                value = "bool";
            else if( stringValue == typeof( Boolean? ) || stringValue == typeof( bool? ) )
                value = "bool?(可空)";
            else if( stringValue == typeof( Char ) )
                value = "char";
            else if( stringValue == typeof( SByte ) )
                value = "sbyte";
            else if( stringValue == typeof( Byte ) )
                value = "byte";
            else if( stringValue == typeof( Int16 ) )
                value = "int16";
            else if( stringValue == typeof( Int16? ) )
                value = "int16?(可空)";
            else if( stringValue == typeof( int ) )
                value = "int";
            else if( stringValue == typeof( int? ) )
                value = "int?(可空)";
            else if( stringValue == typeof( long ) )
                value = "long";
            else if( stringValue == typeof( long? ) )
                value = "long?(可空)";
            else if( stringValue == typeof( DateTime ) )
                value = "datetime";
            else if( stringValue == typeof( DateTime? ) )
                value = "datetime?(可空)";
            else if( stringValue == typeof( Decimal ) )
                value = "decimal";
            else if( stringValue == typeof( Decimal? ) )
                value = "decimal?(可空)";
            else if( stringValue == typeof( Single ) )
                value = "single";
            else if( stringValue == typeof( Double ) )
                value = "double";
            else if( stringValue == typeof( Double? ) )
                value = "double?(可空)";
            else if( stringValue == typeof( String ) )
                value = "string";
            else
                value = "object";

            return value;
        }
    }
}
