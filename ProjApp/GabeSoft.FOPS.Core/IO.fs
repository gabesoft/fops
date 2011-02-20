namespace GabeSoft.FOPS.Core

open System
open SofGem.DSBK.Core
open SofGem.DSBK.IO
open SofGem.DSBK.Domain

type IONodeType = FileNode | DirectoryNode | UnknownNode
type IONode = 
    {   Path: string
        Type: IONodeType
        Files: seq<IONode>
        Directories: seq<IONode> }
    static member Make path nodeType = { 
        Path = path 
        Type = nodeType 
        Files = Seq.empty<_>
        Directories = Seq.empty<_>
    }
    static member File path = IONode.Make path IONodeType.FileNode
    static member Directory path = IONode.Make path IONodeType.DirectoryNode
    member x.AllFiles = seq {
        yield! x.Files
        yield! x.Directories |> Seq.map (fun f -> f.AllFiles) |> Seq.concat
    }
    member x.AllDirectories = seq {
        yield! x.Directories
        yield! x.Directories |> Seq.map (fun f -> f.AllDirectories) |> Seq.concat
    }
        
type IOProviderImpl () = 
    interface IOProvider with
        member x.FileExists path = FileW.Exists(path)
        member x.DirectoryExists path = DirectoryW.Exists(path)
        member x.GetFiles path = DirectoryW.GetFiles(path)
        member x.GetDirectories path = DirectoryW.GetDirectories(path)
        member x.DeleteFile path = DeleteHelper.DeleteFile(path)
        member x.DeleteDirectory (path, deep) = 
          match deep with
          | true -> DeleteHelper.DeleteDirectoryRecursive(path, true)
          | false -> DeleteHelper.DeleteDirectory(path, false)
        member x.CreateDirectory path = 
          DirectoryW.CreateDirectory(path) |> ignore
        member x.Link (source, destination) = 
          FileCopier.CreateHardLink(source, destination)
        member x.Copy (source, destination) = 
          FileCopier.Copy(source, destination)
            
type IOServer(?ioProvider) = 
    let provider = match ioProvider with
                    | Some p    -> p
                    | None      -> new IOProviderImpl() :> IOProvider

    let files path =
        seq {
            for file in provider.GetFiles path do
                yield IONode.File file
        }

    let rec dirs path =
        seq {
            for dir in provider.GetDirectories path do
                let node = IONode.Directory dir
                yield { 
                node with 
                    Files = files dir
                    Directories = dirs dir }
    } 

    let node path = 
        if provider.FileExists path then 
            IONode.File path
        else if provider.DirectoryExists path then 
            { IONode.Directory path with
                Files = files path
                Directories = dirs path }
        else
            IONode.Make path IONodeType.UnknownNode

    member x.Node = node
    member x.Files = files
    member x.Directories = dirs
    member x.Provider = provider
