namespace GabeSoft.FOPS.Core

type IOProvider =   
  /// <summary>
  /// Determines whether the specified file exists.
  /// </summary>
  /// <parameter name="path">
  /// The file to check.
  /// </parameter>
  /// <returns>
  /// true if the caller has the required permissions and path contains the name
  /// of an existing file; otherwise, false. This method also returns false if
  /// path is null or a zero-length string. If the caller does not have sufficient
  /// permissions to read the specified file, no exception is thrown and the method
  /// returns false regardless of the existence of path.
  /// </returns>
  abstract member FileExists : string -> bool

  /// <summary>
  /// Determines whether the given path refers to an existing directory on disk.
  /// </summary>
  /// <parameter name="path">
  /// The path to test.
  /// </parameter>
  /// <returns>
  /// true if path refers to an existing directory; otherwise, false.
  /// </returns>
  abstract member FolderExists : string -> bool

  /// <summary>
  /// Returns the names of files in the specified directory.
  /// </summary>
  /// <parameter name="path">
  /// The directory from which to retrieve the files.
  /// </parameter>
  /// <returns>
  /// A String array of file names in the specified directory. File names include
  /// the full path.
  /// </returns>
  /// <exception cref="System.ArgumentNullException">
  /// path is null.
  /// </exception>
  /// <exception cref="System.UnauthorizedAccessException">
  /// The caller does not have the required permission.
  /// </exception>
  /// <exception cref="System.IO.IOException">
  /// path is a file name.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  /// path is a zero-length string, contains only white space, or contains one
  /// or more invalid characters as defined by SofGem.DSBK.IO.PathW.InvalidPathChars.
  /// </exception>
  /// <exception cref="System.IO.DirectoryNotFoundException">
  /// The specified path is invalid (for example, it is on an unmapped drive).
  /// </exception>
  abstract member GetFiles : string -> string []

  /// <summary>
  /// Gets the names of subdirectories in the specified directory.
  /// </summary>
  /// <parameter name="path">
  /// The path for which an array of subdirectory names is returned.
  /// </parameter>
  /// <returns>
  /// An array of type String containing the names of subdirectories in path.
  /// </returns>
  /// <exception cref="System.ArgumentNullException">
  /// path is null.
  /// </exception>
  /// <exception cref="System.UnauthorizedAccessException">
  /// The caller does not have the required permission.
  /// </exception>
  /// <exception cref="System.IO.IOException">
  /// path is a file name.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  /// path is a zero-length string, contains only white space, or contains one
  /// or more invalid characters as defined by SofGem.DSBK.IO.PathW.InvalidPathChars.
  /// </exception>
  /// <exception cref="System.IO.DirectoryNotFoundException">
  /// The specified path is invalid (for example, it is on an unmapped drive).
  /// </exception>
  abstract member GetFolders : string -> string []

  /// <summary>
  /// Deletes the specified file (even if the file is read-only).
  /// </summary>
  /// <parameter name="path">
  /// The name of the file to be deleted.
  /// </parameter>
  /// <exception cref="System.IO.IOException">
  /// The specified file is in use.
  /// </exception>
  /// <exception cref="System.ArgumentNullException">
  /// path is null.
  /// </exception>
  /// <exception cref="System.IO.DirectoryNotFoundException">
  /// The specified path is invalid, (for example, it is on an unmapped drive).
  /// </exception>
  /// <exception cref="System.NotSupportedException">
  /// path is in an invalid format.
  /// </exception>
  /// <exception cref="System.UnauthorizedAccessException">
  /// The caller does not have the required permission.-or- path is a directory.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  /// path is a zero-length string, contains only white space, or contains one
  /// or more invalid characters as defined by SofGem.DSBK.IO.PathW.InvalidPathChars.
  /// </exception>
  abstract member DeleteFile : string -> unit

  /// <summary>
  /// Deletes the specified directory and, if indicated, any subdirectories in
  /// the directory.
  /// </summary>
  /// <parameter name="path">The name of the directory to remove.</parameter>
  /// <parameter name="deep">True to remove directories, subdirectories, and files in path; otherwise, false.</parameter>
  /// <exception cref="System.IO.IOException">
  /// A file with the same name and location specifiedby path exists.-or-The directory
  /// specified by path is read-only, or recursive is false and path is not an
  /// empty directory.-or-The directory is the application's current working directory.
  /// </exception>
  /// <exception cref="System.ArgumentNullException">
  /// path is null.
  /// </exception>
  /// <exception cref="System.UnauthorizedAccessException">
  /// The caller does not have the required permission.
  /// </exception>
  /// <exception cref="System.ArgumentException">
  /// path is a zero-length string, contains only white space, or contains one
  /// or more invalid characters as defined by SofGem.DSBK.IO.PathW.InvalidPathChars.
  /// </exception>
  /// <exception cref="System.IO.DirectoryNotFoundException">
  /// The specified path is invalid (for example, it is on an unmapped drive).
  /// </exception>   
  abstract member DeleteFolder : string * bool -> unit

  /// <summary>
  /// Creates all directories and subdirectories as specified by path.
  /// </summary>
  /// <parameter name="path">
  /// The directory path to create.
  /// </parameter>
  /// <exception cref="System.ArgumentNullException">
  /// path is null.
  /// </exception>
  /// <exception cref="System.UnauthorizedAccessException">
  /// The caller does not have the required permission.
  /// </exception>
  /// <exception cref="System.IO.IOException">
  /// The directory specified by path is read-only or is not empty.-or-A file with
  /// the same name and location specified by path exists.
  /// </exception>
  /// <exception cref="System.NotSupportedException">
  /// An attempt was made to create a directory with only the colon character (:).
  /// </exception>
  /// <exception cref="System.ArgumentException">
  /// path is a zero-length string, contains only white space, or contains one
  /// or more invalid characters as defined by SofGem.DSBK.IO.PathW.InvalidPathChars.
  /// </exception>
  /// <exception cref="System.IO.DirectoryNotFoundException">
  /// The specified path is invalid (for example, it is on an unmapped drive).
  /// </exception>
  abstract member CreateFolder : string -> unit

  /// <summary>
  /// Creates a hard link for the specified source file to the specified destination.
  /// </summary>
  /// <param name="source">The source file path.</param>
  /// <param name="destination">
  /// The destination file path. This path must be on the same volume as the source path.
  /// </param>
  /// <exception cref="System.IO.IOException">
  /// The source and destination are on different volumes or the hard link creation failed.
  /// </exception>
  /// <exception cref="ArgumentNullException">The source or destination is null or empty.</exception>
  abstract member Link : string * string -> unit

  /// <summary>
  /// Copies an existing file to a new file. If the destination file exists it will be overwritten.
  /// </summary>
  /// <param name="source">The existing file path.</param>
  /// <param name="destination">The destination file path. If this is an existing file it will be overwritten. This cannot be a directory path.</param>
  abstract member Copy : string * string -> unit
