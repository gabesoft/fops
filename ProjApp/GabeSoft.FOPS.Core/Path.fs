namespace GabeSoft.FOPS.Core

open SofGem.DSBK.IO

/// Filesystem path operations.
module Path =    
   /// Combines two path strings.
   let combine path1 path2 = PathW.Combine(path1, path2)
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
