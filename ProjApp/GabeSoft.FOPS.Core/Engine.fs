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
  let yankd src = server.Provider.DeleteDirectory (src, true)

  let cinfo src dst = sprintf "copy: %s -> %s (DONE)" src dst |> info
  let cwarn src dst reason = sprintf "copy: %s -> %s (%s)" src dst reason |> warn
  let linfo src dst = sprintf "link: %s -> %s (DONE)" src dst |> info
  let lwarn src dst reason = sprintf "link: %s -> %s (%s)" src dst reason |> warn
  let yinfo src = sprintf "delete: %s (DONE)" src |> info
  let ywarn src reason = sprintf "delete: %s (%s)" src reason |> warn
  let ydinfo src = sprintf "delete-dir: %s (DONE)" src |> info
  let ydwarn src reason = sprintf "delete-dir: %s (%s)" src reason |> warn

  let getNode path spec = path |> server.Node |> Filter.apply spec

  let yankPattern src = 
    let spec = {
      Pattern = Wildcard.toRegex src
      Exclude = []
      Recursive = Wildcard.isRecursive src }
    let node = getNode (Wildcard.root src) spec
    node.AllFiles |> Seq.iter (fun f -> 
                                  yank f.Path
                                  yinfo f.Path)

  let yankDir src =
    let exists = server.Provider.DirectoryExists
    match exists src with
    | false   -> ydwarn src "SKIPPED: folder does not exist"
    | true    ->
        yankd src
        match exists src with
        | true  -> ydwarn src "DONE: some files could not be deleted"
        | false -> ydinfo src

  let copyFile (copy, info, warn) (src, dst, force) =
    let exists = server.Provider.FileExists
    let mkdir = server.Provider.CreateDirectory
    match exists src with
    | false -> warn src dst "SKIPPED: source file does not exist"
    | true  ->
        match exists dst, force with
        | true, false   ->  warn src dst "SKIPPED: destination file already exists"
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
    | DirectoryNode    -> 
      node.Files 
      |> Seq.append node.Directories
      |> Seq.iter (copyDeep (copy, info, warn) (fdst, force))
    | _             -> fail "unknown node type"

  let copyDir (copy, info, warn) (src, dst, force, excludes) =
    let excludes = excludes |> List.map Wildcard.toRegex    
    let dstExists = server.Provider.DirectoryExists dst
    let spec = {
      Pattern = (Wildcard.matchAll src)
      Exclude = (Wildcard.matchAll dst) :: excludes
      Recursive = true }
    let node = getNode src spec
    let fdst = 
      let dst = match dstExists with 
                | true -> Path.combine dst (Path.file src) 
                | false -> dst
      fun path -> Path.combine dst (Path.part src path)
    copyDeep (copy, info, warn) (fdst, force) node

  let copyPattern (copy, info, warn) (src, dst, force, excludes) =
    let excludes = excludes |> List.map Wildcard.toRegex
    let spec = {
      Pattern = (Wildcard.toRegex src)
      Exclude = (Wildcard.matchAll dst) :: excludes
      Recursive = Wildcard.isRecursive src }
    let node = getNode (Wildcard.root src) spec
    let fdst path = Path.combine dst (Path.file path)
    node.AllFiles |> Seq.iter (copyDeep (copy, info, warn) (fdst, force))

  let runItem = function
  | Copy (f, t, o, e, c)  -> 
    match c with
    | FileMode    -> copyFile (copy, cinfo, cwarn) (f, t, o)
    | DirectoryMode  -> copyDir (copy, cinfo, cwarn) (f, t, o, e)
    | PatternMode -> copyPattern (copy, cinfo, cwarn) (f, t, o, e)
  | Link (f, t, o, e, c)  ->
    match c with
    | FileMode    -> copyFile (link, linfo, lwarn) (f, t, o)
    | DirectoryMode  -> copyDir (link, linfo, lwarn) (f, t, o, e)
    | PatternMode -> copyPattern (link, linfo, lwarn) (f, t, o, e)
  | Yank (s, t)   ->  
    match t with
    | FileMode    -> fail "invalid mode for delete"
    | DirectoryMode  -> yankDir s
    | PatternMode -> yankPattern s

  let runJob (job:Job) = Seq.iter runItem job.Items
  
  member x.Run job = runJob job  
  member x.Run jobs = Seq.iter runJob jobs
