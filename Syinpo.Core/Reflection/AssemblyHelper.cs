using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Syinpo.Core.Reflection
{
    internal static class AssemblyHelper
    {
        public static List<Assembly> GetAllAssembliesInFolder(string folderPath, SearchOption searchOption)
        {
            var assemblyFiles = Directory
                .EnumerateFiles(folderPath, "*.*", searchOption)
                .Where(s => s.EndsWith(".dll") || s.EndsWith(".exe"));

            return assemblyFiles.Select(
                s =>
                {
                    try
                    {
                        if (s.EndsWith("Syinpo.Admin.Api.exe"))
                            return null;

                        return Assembly.LoadFile(s);
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
            ).Where( w => w != null ).ToList();
        }
    }
}
