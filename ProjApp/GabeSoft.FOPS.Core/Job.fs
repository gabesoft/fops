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
type Job (items:Item list, ?id, ?basePath) =
   let get arg def =
      match arg with 
      | Some v -> v
      | None   -> def
   let _id = get id (Guid.NewGuid().ToString())
   let _basePath = get basePath String.Empty

   let complete path = 
      match Path.rooted path, String.IsNullOrWhiteSpace(_basePath) with
      | true, _      -> path
      | false, true  -> Path.full path
      | false, false -> Path.combine _basePath path
   let makePathsAbsolute = function
   | Copy (f, t, o, e, c)  -> 
      Copy (complete f, complete t, o, List.map complete e, c)
   | Link (f, t, o, e, c)  -> 
      Link (complete f, complete t, o, List.map complete e, c)
   | Yank (f)              -> Yank (complete f)
   let _items = 
      items
      |> Seq.map makePathsAbsolute
      |> Seq.toList

   member x.Id with get() = _id
   member x.BasePath with get() = _basePath
   member x.Items with get() = _items
