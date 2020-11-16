using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using Microsoft.Extensions.Logging;

namespace Syinpo.Core.Monitor.PackModule {
    public class MonitorZipFileHelper {
        public static void CreateFromDirectory( string sourceDirectoryName, string destinationArchiveFileName, bool includeBaseDirectory ) {
            // Rely on Path.GetFullPath for validation of sourceDirectoryName and destinationArchive

            // Checking of compressionLevel is passed down to DeflateStream and the IDeflater implementation
            // as it is a pluggable component that completely encapsulates the meaning of compressionLevel.

            sourceDirectoryName = Path.GetFullPath( sourceDirectoryName );
            destinationArchiveFileName = Path.GetFullPath( destinationArchiveFileName );

            var root = new FileInfo( destinationArchiveFileName ).DirectoryName;
            if( !Directory.Exists( root ) ) {
                Directory.CreateDirectory( root );
            }

            //add files and directories
            DirectoryInfo di = new DirectoryInfo( sourceDirectoryName );
            if (di.GetFiles().Length == 0)
                return;

            using( ZipArchive archive = Open( destinationArchiveFileName, ZipArchiveMode.Create ) ) {
                bool directoryIsEmpty = true;

                string basePath = di.FullName;

                if( includeBaseDirectory && di.Parent != null )
                    basePath = di.Parent.FullName;

                // Windows' MaxPath (260) is used as an arbitrary default capacity, as it is likely
                // to be greater than the length of typical entry names from the file system, even
                // on non-Windows platforms. The capacity will be increased, if needed.
                const int DefaultCapacity = 260;
                char[] entryNameBuffer = ArrayPool<char>.Shared.Rent( DefaultCapacity );

                try {
                    foreach( FileSystemInfo file in di.EnumerateFileSystemInfos( "*.log", SearchOption.TopDirectoryOnly ) ) {
                        directoryIsEmpty = false;

                        int entryNameLength = file.FullName.Length - basePath.Length;

                        // Create entry for file:
                        string entryName = ZipFileUtils.EntryFromPath( file.FullName, basePath.Length, entryNameLength,
                            ref entryNameBuffer );
                        ZipFileExtensions.CreateEntryFromFile( archive, file.FullName, entryName );

                        // 创建后删除
                        file.Delete();

                    } // foreach

                    // If no entries create an empty root directory entry:
                    if( includeBaseDirectory && directoryIsEmpty ) {
                        archive.CreateEntry( ZipFileUtils.EntryFromPath( di.Name, 0, di.Name.Length, ref entryNameBuffer,
                            appendPathSeparator: true ) );
                    }
                }
                catch( Exception ex ) {
                    IoC.Resolve<ILogger<MonitorZipFileHelper>>().LogError( ex, "MonitorZipFileHelper CreateFromDirectory error." );
                }
                finally {
                    ArrayPool<char>.Shared.Return( entryNameBuffer );
                }

            }
        }

        public static void UnFromDirectory( string sourceDirectoryName, string destinationArchiveFileName ) {
            sourceDirectoryName = Path.GetFullPath( sourceDirectoryName );
            destinationArchiveFileName = Path.GetFullPath( destinationArchiveFileName );

            var root = destinationArchiveFileName;
            if( !Directory.Exists( root ) ) {
                Directory.CreateDirectory( root );
            }

            try {
                DirectoryInfo di = new DirectoryInfo( sourceDirectoryName );
                if( di.GetFiles( "*.zip" ).Length == 0 )
                    return;

                foreach( FileSystemInfo file in di.EnumerateFileSystemInfos( "*.zip", SearchOption.AllDirectories ) ) {
                    System.IO.Compression.ZipFile.ExtractToDirectory( file.FullName, destinationArchiveFileName );

                    // 创建后删除
                    file.Delete();
                }

            }
            catch( Exception ex ) {
                IoC.Resolve<ILogger<MonitorZipFileHelper>>().LogError( ex, "MonitorZipFileHelper UnFromDirectory error." );
            }
        }

        public static ZipArchive Open( string archiveFileName, ZipArchiveMode mode ) {
            // Relies on FileStream's ctor for checking of archiveFileName

            FileMode fileMode;
            FileAccess access;
            FileShare fileShare;

            switch( mode ) {
                case ZipArchiveMode.Read:
                    fileMode = FileMode.Open;
                    access = FileAccess.Read;
                    fileShare = FileShare.Read;
                    break;

                case ZipArchiveMode.Create:
                    fileMode = FileMode.CreateNew;
                    access = FileAccess.Write;
                    fileShare = FileShare.None;
                    break;

                case ZipArchiveMode.Update:
                    fileMode = FileMode.OpenOrCreate;
                    access = FileAccess.ReadWrite;
                    fileShare = FileShare.None;
                    break;

                default:
                    throw new ArgumentOutOfRangeException( nameof( mode ) );
            }

            // Suppress CA2000: fs gets passed to the new ZipArchive, which stores it internally.
            // The stream will then be owned by the archive and be disposed when the archive is disposed.
            // If the ctor completes without throwing, we know fs has been successfully stores in the archive;
            // If the ctor throws, we need to close it here.

            FileStream fs = new FileStream( archiveFileName, fileMode, access, fileShare, bufferSize: 0x1000, useAsync: false );

            try {
                return new ZipArchive( fs, mode, leaveOpen: false );
            }
            catch {
                fs.Dispose();
                throw;
            }
        }
    }

    internal static partial class ZipFileUtils {
        // Per the .ZIP File Format Specification 4.4.17.1 all slashes should be forward slashes
        private const char PathSeparatorChar = '/';
        private const string PathSeparatorString = "/";

        public static string EntryFromPath( string entry, int offset, int length, ref char[] buffer, bool appendPathSeparator = false ) {
            Debug.Assert( length <= entry.Length - offset );
            Debug.Assert( buffer != null );

            // Remove any leading slashes from the entry name:
            while( length > 0 ) {
                if( entry[ offset ] != Path.DirectorySeparatorChar &&
                    entry[ offset ] != Path.AltDirectorySeparatorChar )
                    break;

                offset++;
                length--;
            }

            if( length == 0 )
                return appendPathSeparator ? PathSeparatorString : string.Empty;

            int resultLength = appendPathSeparator ? length + 1 : length;
            EnsureCapacity( ref buffer, resultLength );
            entry.CopyTo( offset, buffer, 0, length );

            // '/' is a more broadly recognized directory separator on all platforms (eg: mac, linux)
            // We don't use Path.DirectorySeparatorChar or AltDirectorySeparatorChar because this is
            // explicitly trying to standardize to '/'
            for( int i = 0; i < length; i++ ) {
                char ch = buffer[ i ];
                if( ch == Path.DirectorySeparatorChar || ch == Path.AltDirectorySeparatorChar )
                    buffer[ i ] = PathSeparatorChar;
            }

            if( appendPathSeparator )
                buffer[ length ] = PathSeparatorChar;

            return new string( buffer, 0, resultLength );
        }

        public static void EnsureCapacity( ref char[] buffer, int min ) {
            Debug.Assert( buffer != null );
            Debug.Assert( min > 0 );

            if( buffer.Length < min ) {
                int newCapacity = buffer.Length * 2;
                if( newCapacity < min )
                    newCapacity = min;

                char[] oldBuffer = buffer;
                buffer = ArrayPool<char>.Shared.Rent( newCapacity );
                ArrayPool<char>.Shared.Return( oldBuffer );
            }
        }

        public static bool IsDirEmpty( DirectoryInfo possiblyEmptyDir ) {
            using( IEnumerator<string> enumerator = Directory.EnumerateFileSystemEntries( possiblyEmptyDir.FullName ).GetEnumerator() )
                return !enumerator.MoveNext();
        }
    }
}
