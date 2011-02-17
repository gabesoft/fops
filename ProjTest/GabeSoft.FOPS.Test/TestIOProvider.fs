namespace GabeSoft.FOPS.Test

open System
open System.Collections.Generic
open GabeSoft.FOPS.Core

type TestIOProvider (paths:string list) =
  let folderDict = new Dictionary<string, string>()
  let yankedDict = new Dictionary<string, string>()
  let addFolder path = folderDict.Add(path, String.Empty)
  let addDeleted path = yankedDict.Add(path, String.Empty)
  let clean (path: string) = 
    let root = Path.root path
    if root = path then path else path.TrimEnd('\\')
  let equal a b = String.Equals(a, b, StringComparison.OrdinalIgnoreCase)
  let dirName path = Path.directory path
  let rec dirAll path = seq {
    let d = dirName path
    if d <> null then 
        yield d
        yield! dirAll d }
  let files folder =
    paths
    |> List.filter (fun p -> equal folder (dirName p))
    |> Seq.distinct
    |> Seq.toArray
  let folders folder =
    paths
    |> List.map (fun p -> dirAll p)
    |> Seq.concat
    |> Seq.filter (fun p -> equal folder (dirName p))
    |> Seq.distinct
    |> Seq.toArray
  let hasFile file =
    List.exists (fun p -> equal p file) paths
  let hasFolder folder =
    dirName folder
    |> folders 
    |> Seq.exists (fun p -> equal folder p) 

  member x.CreatedFolders 
    with get () = seq { for k in folderDict.Keys -> k } |> Set.ofSeq
  member x.DeletedFiles
    with get () = seq { for k in yankedDict.Keys -> k } |> Set.ofSeq

  interface IOProvider with
    member x.FileExists path = path |> clean |> hasFile
    member x.FolderExists path = path |> clean |> hasFolder 
    member x.GetFiles path = path |> clean |> files 
    member x.GetFolders path = path |> clean |> folders 
    member x.DeleteFile path = addDeleted path
    member x.DeleteFolder (path, deep) = addDeleted path
    member x.CreateFolder path = addFolder path
    member x.Link (source, destination) = failwith "not implemented"
    member x.Copy (source, destination) = failwith "not implemented"
