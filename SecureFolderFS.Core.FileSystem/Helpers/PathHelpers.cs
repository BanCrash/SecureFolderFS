﻿using System;
using System.IO;
using System.Runtime.CompilerServices;

namespace SecureFolderFS.Core.FileSystem.Helpers
{
    public static class PathHelpers
    {
        public static string? GetDirectoryIdPathOfParent(string path, string rootPath)
        {
            var parentPath = Path.GetDirectoryName(path);
            if (parentPath is null)
                return null;

            // Parent path is the same as rootPath where the directory ID should be empty
            if (parentPath.Equals(rootPath))
                return string.Empty;

            return Path.Combine(parentPath, Constants.DIRECTORY_ID_FILENAME);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCoreFile(ReadOnlySpan<char> fileName)
        {
            return fileName.Contains(Constants.DIRECTORY_ID_FILENAME, StringComparison.Ordinal);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EnsureTrailingPathSeparator(string path)
        {
            return path[^1] == Path.DirectorySeparatorChar ? path : path + Path.DirectorySeparatorChar;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EnsureNoTrailingPathSeparator(string path)
        {
            return path[^1] == Path.DirectorySeparatorChar ? path.Substring(0, path.Length - 1) : path;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string EnsureNoLeadingPathSeparator(string path)
        {
            return path.StartsWith(Path.DirectorySeparatorChar) ? path.Substring(1) : path;
        }

        public static string PathFromVaultRoot(string fileName, string vaultRootPath)
        {
            if (fileName.Length == 0 || (fileName.Length == 1 && fileName[0] == Path.DirectorySeparatorChar))
                return EnsureTrailingPathSeparator(vaultRootPath);

            return Path.Combine(vaultRootPath, EnsureNoLeadingPathSeparator(fileName));
        }
    }
}
