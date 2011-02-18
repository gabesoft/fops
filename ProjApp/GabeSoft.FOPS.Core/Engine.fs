namespace GabeSoft.FOPS.Core

open System

type OperationException (message:string, ?innerException:Exception) =
   inherit Exception (
      message, 
      match innerException with | Some ex -> ex | None -> null)

/// Operations engine.
type Engine(server: IOServer, ?log:Log) =
  let info, warn, fail =   
    let logger = match log with 
                  | Some l -> l 
                  | None -> new LogImpl () :> Log
    logger.Info, logger.Warn, logger.Fail
  let copy src dst = server.Provider.Copy (src, dst)
  let link src dst = server.Provider.Link (src, dst)
  let yank src = server.Provider.DeleteFile src
  let yankd src = server.Provider.DeleteFolder (src, true)

  let cinfo src dst = sprintf "copy: %s -> %s (DONE)" src dst |> info
  let cwarn src dst reason = sprintf "copy: %s -> %s (%s)" src dst reason |> warn
  let linfo src dst = sprintf "link: %s -> %s (DONE)" src dst |> info
  let lwarn src dst reason = sprintf "link: %s -> %s (%s)" src dst reason |> warn
  let yinfo src = sprintf "delete: %s (DONE)" src |> info
  let ywarn src reason = sprintf "delete: %s (%s)" src reason |> warn
  let ydinfo src = sprintf "delete-dir: %s (DONE)" src |> info
  let ydwarn src reason = sprintf "delete-dir: %s (%s)" src reason |> warn

  let yankPattern src = 
    let spec = {
      Pattern = Wildcard.toRegex src
      Exclude = []
      Recursive = Wildcard.isRecursive src }
    let node = 
      src 
      |> Wildcard.root 
      |> server.Node
      |> Filter.apply spec
    node.AllFiles |> Seq.iter (fun f -> 
                                  yank f.Path
                                  yinfo f.Path)

  let yankFolder src =
    yankd src
    match server.Provider.FolderExists src with
    | true  -> ydwarn src "DONE: some files could not be deleted"
    | false -> ydinfo src

  let copyFile (copy, info, warn) (src, dst, force) =
    let exists = server.Provider.FileExists
    let mkdir = server.Provider.CreateFolder
    match exists dst, force with
    | true, false   ->  warn src dst "SKIPPED: file already exists"
    | e, _          ->  dst |> Path.directory |> mkdir
                        copy src dst
                        match e with
                        | false  -> info src dst
                        | true   -> warn src dst "DONE: replaced"
  
  let rec copyDeep (copy, info, warn) (fdst, force) (node:IONode) =
    let src = node.Path
    let dst = fdst src
    match node.Type with
    | FileNode      -> copyFile (copy, info, warn) (src, dst, force)
    | FolderNode    -> 
      node.Files 
      |> Seq.append node.Folders
      |> Seq.iter (copyDeep (copy, info, warn) (fdst, force))
    | _             -> fail "unknown node type"

  let copyFolder (copy, info, warn) (src, dst, force, excludes) =
    let excludes = excludes |> List.map Wildcard.toRegex    
    let spec = {
      Pattern = (Wildcard.matchAll src)
      Exclude = (Wildcard.matchAll dst) :: excludes
      Recursive = true }
    let node = server.Node src |> Filter.apply spec
    let fdst path = Path.combine dst (Path.part src path)
    copyDeep (copy, info, warn) (fdst, force) node

  let copyPattern (copy, info, warn) (src, dst, force, excludes) =
    let excludes = excludes |> List.map Wildcard.toRegex
    let spec = {
      Pattern = (Wildcard.toRegex src)
      Exclude = (Wildcard.matchAll dst) :: excludes
      Recursive = Wildcard.isRecursive src }
    let node = src |> Wildcard.root |> server.Node
    let fdst path = Path.combine dst (Path.file path)
    node.AllFiles |> Seq.iter (copyDeep (copy, info, warn) (fdst, force))

  let runItem = function
  | Copy (f, t, o, e, c)  -> 
    match c with
    | FileMode    -> copyFile (copy, cinfo, cwarn) (f, t, o)
    | FolderMode  -> copyFolder (copy, cinfo, cwarn) (f, t, o, e)
    | PatternMode -> copyPattern (copy, cinfo, cwarn) (f, t, o, e)
  | Link (f, t, o, e, c)  ->
    match c with
    | FileMode    -> copyFile (link, linfo, lwarn) (f, t, o)
    | FolderMode  -> copyFolder (link, linfo, lwarn) (f, t, o, e)
    | PatternMode -> copyPattern (link, linfo, lwarn) (f, t, o, e)
  | Yank (s, t)   ->  
    match t with
    | FileMode    -> fail "invalid mode for delete"
    | FolderMode  -> yankFolder s
    | PatternMode -> yankPattern s

  let runJob (job:Job) = Seq.iter runItem job.Items
  
  member x.Run job = runJob job  
  member x.Run jobs = Seq.iter runJob jobs
