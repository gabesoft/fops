namespace GabeSoft.FOPS.Core

open System

type CopyType = File | Folder | Pattern

// TODO: yank is not a copy item
//       job is not a copy job
//       Job, Item

/// File operations job item.
type CopyItem = 
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
      CopyItem.map (fun f _ _ _ -> f) (fun f _ _ _ -> f) id
   /// Gets the to path of the given item.
   static member toPath = 
      CopyItem.map (fun _ t _ _ -> t) (fun _ t _ _ -> t) (fun _ -> String.Empty)
   /// Gets the overwrite flag of the given item.
   static member overwrite =
      CopyItem.map (fun _ _ o _ -> o) (fun _ _ o _ -> o) (fun _ -> false)
   static member isCopy =
      CopyItem.map (fun _ _ _ _ -> true) (fun _ _ _ _ -> false) (fun _ -> false)
   static member isLink =
      CopyItem.map (fun _ _ _ _ -> false) (fun _ _ _ _ -> true) (fun _ -> false)
   static member isYank =
      CopyItem.map (fun _ _ _ _ -> false) (fun _ _ _ _ -> false) (fun _ -> true)

/// File operations job.
type CopyJob (items:CopyItem list, ?id, ?basePath) =
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
