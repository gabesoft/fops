namespace GabeSoft.FOPS.Core

module CopyEngine =

  open System

  let copyFile (server: IOServer) (src, dst, overwrite) =
    match server.Provider.FileExists dst, overwrite with
    | true, false -> ()
    | _           -> server.Provider.Copy (src, dst)

  let rec copyRec (server: IOServer) overwrite fdst (node: IONode) =
    let src = node.Path
    let dst = fdst src
    match node.Type with
    | FileNode      ->  let parent = Path.directory src
                        server.Provider.CreateFolder parent
                        copyFile server (src, dst, overwrite)
    | FolderNode    ->  node.Files |> Seq.iter (copyRec server overwrite fdst)
                        node.Folders |> Seq.iter (copyRec server overwrite fdst)
    | UnknownNode   ->  failwith "unknown node type"  // TODO: raise CopyException                

  let copyFolder (server: IOServer) (src, dst, overwrite, excludes) =
    let spec = { 
      Pattern = Wildcard.matchAll src
      Exclude = (Wildcard.matchAll dst) :: excludes
      Recursive = true }
    let node = server.Node src |> Filter.apply spec
    let fdst (path:string) = 
      path
        .Replace(src, String.Empty)
        .TrimStart([|'/'; '\\'|])   // put this logic in Path.combine if it makes sense
        |> Path.combine dst
    copyRec server overwrite fdst node

  let private copy (server: IOServer) (f, t, o, e) =
    // TODO: dest should not be added to excludes if
    //       we're copying a single file                  !
    //       or if the wildcard pattern is not recursive  ?
    let dest = Wildcard.matchAll t
    let spec = {
        Pattern = Wildcard.toRegex f
        Exclude = dest :: (e |> List.map Wildcard.toRegex)
        Recursive = Wildcard.isRecursive f }
    let node = f |> Wildcard.root |> server.Node |> Filter.apply spec
    node.AllFiles 
    |> Seq.iter (fun f -> 
          let dest = Path.combine t (Path.file f.Path)
          server.Provider.Copy (f.Path, dest))

  let private delete (server: IOServer) path = 
    server.Provider.DeleteFile path

  /// TODO: check file existence if necessary
  let private runItem (server: IOServer) = function
  | Copy (f, t, o, e, c)  -> match c with 
                              | File    -> copyFile server (f, t, o)
                              | Folder  -> copyFolder server (f, t, o, e)
                              | _       -> failwith "not implemented"
  | Link (f, t, o, e, c)  -> printfn "link from %s to %s" f t
  | Yank (f)              -> delete server f

  let private runJob (server: IOServer) (job: Job) =
    job.Items |> Seq.iter (runItem server)
         
  let run server jobs = 
    jobs |> Seq.iter (runJob server)
      

