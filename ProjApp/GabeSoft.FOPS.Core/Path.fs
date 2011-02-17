namespace GabeSoft.FOPS.Core

open System
open System.Text
open SofGem.DSBK.IO

/// Filesystem path operations.
module Path =    
  let private clean (path:string) = path.Trim([|'/'; '\\'|])  

  /// Provides a platform-specific character used to separate directory levels
  /// in a path string that reflects a hierarchical file system organization.
  let separator = PathW.DirectorySeparatorChar
  /// Returns the directory information for the specified path string.
  let directory path = PathW.GetDirectoryName(path)
  /// Returns the file name and extension of the specified path string.
  let file path = PathW.GetFileName(path)
  /// Returns the extension of the specified path string.
  let extension path = PathW.GetExtension(path)
  /// Gets a value indicating whether the specified path string contains absolute
  /// or relative path information.
  let rooted path = PathW.IsPathRooted(path)
  /// Returns the absolute path for the specified path string.
  let full path = PathW.GetFullPath(path)
  /// Gets the root directory information of the specified path.
  let root path = PathW.GetPathRoot(path)
  /// Gets the full path of the current working directory.
  let cwd = Environment.CurrentDirectory

  /// Combines two path strings.
  let combine path1 path2 = PathW.Combine(clean path1, clean path2)
  
  /// <summary>
  /// Gets the part of a path that remains
  /// after removing the specified parent directory.
  /// The paths are expected to have the same root.
  /// </summary>
  /// <param name="parent">The parent directory.</param>
  /// <param name="path">The path from which to extract the child part.</param>
  let part (parent:string) (path:string) = 
    path.Replace(parent, String.Empty) |> clean

  /// Normalizes all path separators.
  let normalize (input:string) = 
    (new StringBuilder(input))
        .Replace('/', separator)
        .Replace('\\', separator)
        .ToString()

  /// Trims any start and end path separators.
  let trim (input:string) = input.Trim([|'/';'\\'|])
