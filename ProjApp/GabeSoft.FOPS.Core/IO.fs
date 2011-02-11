namespace GabeSoft.FOPS.Core

open System
open SofGem.DSBK.Core
open SofGem.DSBK.IO
open SofGem.DSBK.Domain

type IONodeType = File | Folder | Unknown
type IONode = 
    {   Path: string
        Type: IONodeType
        Files: seq<IONode>
        Folders: seq<IONode> }
    static member Make path nodeType = { 
        Path = path 
        Type = nodeType 
        Files = Seq.empty<_>
        Folders = Seq.empty<_>
    }
    static member File path = IONode.Make path IONodeType.File
    static member Folder path = IONode.Make path IONodeType.Folder
    member x.AllFiles = seq {
        yield! x.Files
        yield! x.Folders |> Seq.map (fun f -> f.AllFiles) |> Seq.concat
    }
    member x.AllFolders = seq {
        yield! x.Folders
        yield! x.Folders |> Seq.map (fun f -> f.AllFolders) |> Seq.concat
    }
        
type IOProviderImpl () = 
    interface IOProvider with
        member x.FileExists path = FileW.Exists(path)
        member x.FolderExists path = DirectoryW.Exists(path)
        member x.GetFiles path = DirectoryW.GetFiles(path)
        member x.GetFolders path = DirectoryW.GetDirectories(path)
        member x.DeleteFile path = FileW.Delete(path)
        member x.DeleteFolder path deep = DirectoryW.Delete(path, deep)
        member x.CreateFolder path = DirectoryW.CreateDirectory(path) |> ignore
        member x.Link source destination = FileCopier.CreateHardLink(source, destination)
        member x.Copy source destination = FileCopier.Copy(source, destination)
            
type IOServer(?ioProvider) = 
    let provider = match ioProvider with
                    | Some p    -> p
                    | None      -> new IOProviderImpl() :> IOProvider

    let files path =
        seq {
            for file in provider.GetFiles path do
                yield IONode.File file
        }

    let rec folders path =
        seq {
            for folder in provider.GetFolders path do
                let node = IONode.Folder folder
                yield { 
                node with 
                    Files = files folder
                    Folders = folders folder }
    } 

    let node path = 
        if provider.FileExists path then 
            IONode.File path
        else if provider.FolderExists path then 
            { IONode.Folder path with
                Files = files path
                Folders = folders path }
        else
            IONode.Make path IONodeType.Unknown

    member x.Node = node
    member x.Files = files
    member x.Folders = folders
    member x.Provider = provider
