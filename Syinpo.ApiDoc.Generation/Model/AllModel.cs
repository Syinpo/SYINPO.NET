using System.Collections.Generic;

namespace Syinpo.ApiDoc.Generation.Model
{
    public class AllModel
    {
        public AllModel()
        {
            Info = new ApiActionInfo();
            EnvironmentResources = new List<EnvironmentResource>();
            RequestPathParameters = new List<ParameterInfo>();
            RequestBodyParameters = new List<ParameterInfo>();
            RequestQueryParameters = new List<ParameterInfo>();
            Responses = new List<ResponseInfo>();
            Definitions = new List<DefinitionInfo>();
            Example = new ExampleInfo();
        }

        public ApiActionInfo Info { get; set; }

        public List<EnvironmentResource> EnvironmentResources { get; set; }

        public List<ParameterInfo> RequestPathParameters { get; set; }

        public List<ParameterInfo> RequestBodyParameters { get; set; }
        public DefinitionInfo RequestBodyDefinition { get; set; }

        public List<ParameterInfo> RequestQueryParameters { get; set; }

        public List<ResponseInfo> Responses { get; set; }

        public List<DefinitionInfo> Definitions { get; set; }

        public ExampleInfo Example { get; set; }


        public class ApiActionInfo
        {
            public string Name { get; set; }

            public string Description { get; set; }

            public string OperationId { get; set; }

            public string OperationName {
                get; set;
            }

            public string ResourcePath { get; set; }

            public bool Authentication { get; set; }
        }


        public class EnvironmentResource
        {
            public string Environment { get; set; }

            public string LoacalUrl { get; set; }

            public string Description { get; set; }
        }


        public class ParameterInfo
        {
            public ParameterInfo()
            {
                //Parameters = new List<ParameterInfo>();
            }

            public string Name { get; set; }

            public string DataType { get; set; }

            public string Description { get; set; }

            public string DefaultValue { get; set; }

            public bool Required { get; set; }

            public bool IsDefinition { get; set; }

            public string Ref { get; set; }

            public DefinitionInfo Definition { get; set; }

            //public List<ParameterInfo> Parameters { get; set; }
        }


        public class DefinitionInfo
        {
            public DefinitionInfo()
            {
                Parameters = new List<ParameterInfo>();
                SourceCode = new SourceCodeInfo();
            }

            public string Name { get; set; }

            public string Description { get; set; }

            public string DataType { get; set; }

            public string Ref { get; set; }

            public List<ParameterInfo> Parameters { get; set; }

            public SourceCodeInfo SourceCode { get; set; }

            public string SchemaJson { get; set; }
        }


        public class ResponseInfo
        {
            public ResponseInfo()
            {
                Parameters = new List<ParameterInfo>();
            }

            public string MediaType { get; set; }

            public string StatusCode { get; set; }

            public string Description { get; set; }

            public List<ParameterInfo> Parameters { get; set; }
        }

        public class ExampleInfo
        {
            public string RequestBodyJson { get; set; }

            public string ResponseJson { get; set; }
        }

    }
}
