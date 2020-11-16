using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using Syinpo.ApiDoc.Generation.Model;

namespace Syinpo.ApiDoc.Generation.swagger.Recursive
{
    public class RecursiveSourceCodeDescription
    {
        private readonly string[] DebugInfoAttributes =
        {
            "sourceFile",
            "sourceLink",
            "link",
        };

        public SourceCodeInfo Resolve(ref string lines)
        {
            if( string.IsNullOrEmpty(lines) )
                return null;

            HtmlDocument document = new HtmlDocument();
            document.LoadHtml(lines);

            List<SourceCodeInfo> sources = new List<SourceCodeInfo>();

            foreach( var node in document.DocumentNode.Descendants("sourcecode").ToList() )
            {
                SourceCodeInfo info = new SourceCodeInfo
                {
                    Type = node.InnerText
                };

                foreach( var remove in DebugInfoAttributes )
                {
                    foreach( var attr in node.ChildAttributes(remove) )
                    {
                        if( !string.IsNullOrEmpty(attr.Value) )
                            info.Link = attr.Value;
                    }
                }

                node.Remove();

                sources.Add(info);
            }

            foreach( var node in document.DocumentNode.Descendants("code").ToList() )
            {
                SourceCodeInfo info = new SourceCodeInfo
                {
                    Type = node.InnerText
                };

                foreach( var remove in DebugInfoAttributes )
                {
                    foreach( var attr in node.ChildAttributes(remove) )
                    {
                        if( !string.IsNullOrEmpty(attr.Value) )
                            info.Link = attr.Value;
                    }
                }

                node.Remove();

                sources.Add(info);
            }

            lines = document.DocumentNode.InnerHtml;

            if( sources.Any(s => !string.IsNullOrEmpty(s.Link)) )
                return sources.FirstOrDefault(s => !string.IsNullOrEmpty(s.Link));

            return sources.FirstOrDefault();
        }
    }
}
