using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Buildalyzer;
using Buildalyzer.Workspaces;
using Syinpo.ApiDoc.Generation.Model;
using Syinpo.ApiDoc.Generation.Utitls;
using Syinpo.Core.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Classification;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;

namespace Syinpo.ApiDoc.Generation.Compile {
    /// <summary>
    /// https://github.com/dotnet/roslyn-sdk/blob/master/samples/CSharp/ConsoleClassifier/Program.cs
    /// </summary>
    public class CodeBuild {
        public List<ClassFileInfo> Deep() {
            List<ClassFileInfo> result = new List<ClassFileInfo>();

            var solutionFilePath = BuildWorkspaceHelper.GetRelativeWorkspacePath( "Syinpo.Model\\Syinpo.Model.csproj");

            {
                //using( var work = MSBuildWorkspace.Create() ) {
                //    var project = work.OpenProjectAsync( solutionFilePath ).Result;
                //    var documents = project.Documents.Where( w => w.Name.EndsWith( ".cs" ) ).ToList();
                //}
            }

            AnalyzerManager manager = new AnalyzerManager();
            var analyzer = manager.GetProject( solutionFilePath );
            AdhocWorkspace workspace = analyzer.GetWorkspace();
            var project = workspace.CurrentSolution.Projects.First();
            var documents = project.Documents.Where( w => w.Name.EndsWith( ".cs" ) ).ToList();

            foreach( var document in documents ) {
                string fileName = document.Name;
                string filePath = document.FilePath;

                var classes = document.GetSyntaxRootAsync().Result.DescendantNodes().OfType<ClassDeclarationSyntax>();
                var classes2 = document.GetSyntaxRootAsync().Result.DescendantNodes().ToList().OfType<EnumDeclarationSyntax>();

                if( classes.Any() ) {
                    foreach( var cl in classes ) {
                        NamespaceDeclarationSyntax namespaceDeclarationSyntax = null;
                        if( !SyntaxNodeHelper.TryGetParentSyntax( cl, out namespaceDeclarationSyntax ) ) {
                            continue;
                        }

                        var namespaceName = namespaceDeclarationSyntax.Name.ToString();
                        var fullClassName = namespaceName + "." + cl.Identifier.ToString();

                        var keys = document.Folders.ToList();
                        keys.Add( fileName );
                        result.Add( new ClassFileInfo {
                            FileName = fileName,
                            FilePath = filePath,
                            ClassName = cl.Identifier.ToString(),
                            FullClassName = fullClassName,
                            Key = string.Join( @"/", keys.ToArray() )
                        } );
                    }
                }

                if( classes2.Any() ) {
                    foreach( var cl in classes2 ) {
                        NamespaceDeclarationSyntax namespaceDeclarationSyntax = null;
                        if( !SyntaxNodeHelper.TryGetParentSyntax( cl, out namespaceDeclarationSyntax ) ) {
                            continue;
                        }

                        var namespaceName = namespaceDeclarationSyntax.Name.ToString();
                        var fullClassName = namespaceName + "." + cl.Identifier.ToString();

                        var keys = document.Folders.ToList();
                        keys.Add( fileName );
                        result.Add( new ClassFileInfo {
                            FileName = fileName,
                            FilePath = filePath,
                            ClassName = cl.Identifier.ToString(),
                            FullClassName = fullClassName,
                            Key = string.Join( @"/", keys.ToArray() )
                        } );
                    }
                }

                #region Old
                if( false ) {

                    SourceText text = document.GetTextAsync().Result;
                    var span = TextSpan.FromBounds( 0, text.Length );
                    IEnumerable<ClassifiedSpan> classifiedSpans = null;
                    try {
                        classifiedSpans = Classifier.GetClassifiedSpansAsync( document, span ).Result;

                        IEnumerable<Range> ranges = classifiedSpans.Select( classifiedSpan => new Range( classifiedSpan, text.GetSubText( classifiedSpan.TextSpan ).ToString() ) );

                        // var classes = ranges.Where(w => w.ClassificationType == "class name").ToList();
                    }
                    catch( Exception ex ) {
                        throw new Exception( "Exception during Classification of document: " + document.FilePath );
                    }
                }
                #endregion
            }


            return result;
        }

        public void Save( List<ClassFileInfo> data ) {
            File.WriteAllText( BuildWorkspaceHelper.GetRelativeWorkspacePath( @"Syinpo.SignalRDoc.Generation\swagger\classes.json"), JsonHelper.ToJson( data ) );

        }
    }

    static class SyntaxNodeHelper {
        public static bool TryGetParentSyntax<T>( SyntaxNode syntaxNode, out T result )
            where T : SyntaxNode {
            // set defaults
            result = null;

            if( syntaxNode == null ) {
                return false;
            }

            try {
                syntaxNode = syntaxNode.Parent;

                if( syntaxNode == null ) {
                    return false;
                }

                if( syntaxNode.GetType() == typeof( T ) ) {
                    result = syntaxNode as T;
                    return true;
                }

                return TryGetParentSyntax<T>( syntaxNode, out result );
            }
            catch {
                return false;
            }
        }
    }
}
