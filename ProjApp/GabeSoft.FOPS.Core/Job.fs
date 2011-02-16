namespace GabeSoft.FOPS.Core

open System

type CopyType = 
   /// Copy a single file.
   | FileMode 
   /// Copy all contents of a folder recursively.
   | FolderMode 
   /// Copy a list of files that match a wildcard pattern.
   | PatternMode

/// File operations job item.
type Item = 
   /// Copy (from, to, force, excludes, type)
   | Copy of string * string * bool * string list * CopyType
   /// Hard link (from, to, force, excludes, type)
   | Link of string * string * bool * string list * CopyType
   /// Delete (from)
   | Yank of string
   /// Constructs a copy item.
   static member copy c (f, t, o, e) = Copy (f, t, o, e, c)
   /// Constructs a link item.
   static member link c (f, t, o, e) = Link (f, t, o, e, c)
   /// Constructs a delete item.
   static member yank f = Yank f

/// File operations job.
type Job (items:Item list, id) =
   member x.Id with get() = id
   member x.Items with get() = items
