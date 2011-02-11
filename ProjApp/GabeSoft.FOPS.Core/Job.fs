namespace GabeSoft.FOPS.Core

open System

type CopyType = File | Folder | Pattern

/// File operations job item.
type Item = 
   /// Copy (from, to, overwrite, excludes)
   | Copy of string * string * bool * string list
   /// Link (from, to, overwrite, excludes)
   | Link of string * string * bool * string list
   /// Yank (from)
   | Yank of string
   static member map fcopy flink fyank = function
      | Copy (f, t, o, e)  -> fcopy f t o e
      | Link (f, t, o, e)  -> flink f t o e
      | Yank (f)           -> fyank f
   /// Gets the from path of the given item.
   static member fromPath = 
      Item.map (fun f _ _ _ -> f) (fun f _ _ _ -> f) id
   /// Gets the to path of the given item.
   static member toPath = 
      Item.map (fun _ t _ _ -> t) (fun _ t _ _ -> t) (fun _ -> String.Empty)
   /// Gets the overwrite flag of the given item.
   static member overwrite =
      Item.map (fun _ _ o _ -> o) (fun _ _ o _ -> o) (fun _ -> false)
   static member isCopy =
      Item.map (fun _ _ _ _ -> true) (fun _ _ _ _ -> false) (fun _ -> false)
   static member isLink =
      Item.map (fun _ _ _ _ -> false) (fun _ _ _ _ -> true) (fun _ -> false)
   static member isYank =
      Item.map (fun _ _ _ _ -> false) (fun _ _ _ _ -> false) (fun _ -> true)

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
   | Copy (f, t, o, e)  -> Copy (complete f, complete t, o, List.map complete e)
   | Link (f, t, o, e)  -> Link (complete f, complete t, o, List.map complete e)
   | Yank (f)           -> Yank (complete f)
   let _items = 
      items
      |> Seq.map makePathsAbsolute
      |> Seq.toList

   member x.Id with get() = _id
   member x.BasePath with get() = _basePath
   member x.ItemsRelative with get() = items
   member x.ItemsAbsolute with get() = _items
