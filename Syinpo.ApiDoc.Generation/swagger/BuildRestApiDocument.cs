using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using Microsoft.DocAsCode.Build.Engine;
using Microsoft.DocAsCode.Build.RestApi;
using Microsoft.DocAsCode.Common;
using Microsoft.DocAsCode.DataContracts.RestApi;
using Microsoft.DocAsCode.Plugins;

namespace Syinpo.ApiDoc.Generation.swagger
{
    public class BuildRestApiDocument: BuildBase
    {
        private string _outputFolder;
        private string _inputFolder;
        private string _templateFolder;
        private FileCollection _defaultFiles;
        private ApplyTemplateSettings _applyTemplateSettings;

        private const string RawModelFileExtension = ".raw.json";
        private const string SwaggerDirectory = "swagger";

        public BuildRestApiDocument()
        {
            _outputFolder = GetRandomFolder();
            _inputFolder = GetRandomFolder();
            _templateFolder = GetRandomFolder();
            _defaultFiles = new FileCollection(Directory.GetCurrentDirectory());
            _defaultFiles.Add(DocumentType.Article, new[] { "swagger/swagger.json" } );
            _applyTemplateSettings = new ApplyTemplateSettings(_inputFolder, _outputFolder)
            {
                RawModelExportSettings = { Export = true }
            };
        }

        public RestApiRootItemViewModel ProcessSwagger()
        {
            FileCollection files = new FileCollection(_defaultFiles);
            BuildDocument(files);

            var outputRawModelPath = GetRawModelFilePath("swagger.json");

            var model = JsonUtility.Deserialize<RestApiRootItemViewModel>(outputRawModelPath);

            return model;
        }

        private void BuildDocument(FileCollection files)
        {
            var parameters = new DocumentBuildParameters
            {
                Files = files,
                OutputBaseDir = _outputFolder,
                ApplyTemplateSettings = _applyTemplateSettings,
                Metadata = new Dictionary<string, object>
                {
                    [ "meta" ] = "Hello world!",
                }.ToImmutableDictionary()
            };

            using( var builder = new DocumentBuilder(LoadAssemblies(), ImmutableArray<string>.Empty, null) )
            {
                builder.Build(parameters);
            }
        }

        private static IEnumerable<System.Reflection.Assembly> LoadAssemblies()
        {
            yield return typeof(RestApiDocumentProcessor).Assembly;
        }

        private string GetRawModelFilePath(string fileName)
        {
            return Path.Combine(_outputFolder, SwaggerDirectory, Path.ChangeExtension(fileName, RawModelFileExtension));
        }
    }
}
