namespace GabeSoft.FOPS.Core

open System
open System.Collections.Generic

type JobCreator(opts:Options, log:Log) =
  let statusItems = new Queue<string * string * (string -> unit)>()
  let status write title value = 
    statusItems.Enqueue(title, value, write)
  let printStatus () =
    let pad n (text:string) =
      let len = text.Length
      if len >= n then text else
      Seq.init (n - len) (fun _ -> " ")
      |> Seq.append [text]
      |> String.concat ""
    let items = statusItems.ToArray()
    let maxl = 
      items
      |> Seq.map (fun (t, _, _) -> t.Length)
      |> Seq.max
    items
    |> Seq.iter (fun (t, v, w) -> sprintf "- %s: %s" (pad maxl t) v |> w)

  let empty (s:string) = String.IsNullOrEmpty(s) 
  let action = status log.Info "ACTION"
  let info = status log.Info
  let fail = status log.Fail

  let complete parent path = 
      match Path.rooted path, empty parent with
      | true, _      -> path
      | false, true  -> Path.full path
      | false, false -> Path.combine parent path
  let completeDst = complete opts.BaseDst
  let completeSrc = complete opts.BaseSrc
    
  let makePathsAbsolute = function
  | Copy (s, d, o, e, c)  -> 
    Copy (completeSrc s, completeDst d, o, List.map completeSrc e, c)
  | Link (s, d, o, e, c)  -> 
    Link (completeSrc s, completeDst d, o, List.map completeSrc e, c)
  | Yank (s, c)           -> Yank (completeSrc s, c)

  let fromFile : unit -> Job =
    failwith "not implemented"

  let fromYank () =
    failwith "not implemented"

  let fromYankDir () =
    failwith "not implemented"

  let fromCopy () =
    failwith "not implemented"

  let fromCopyFile () =
    failwith "not implemented"                

  let fromCopyDir () =
    failwith "not implemented"

  let fromLink () = 
    failwith "not implemented"

  let fromLinkFile () =
    failwith "not implemented"

  let fromLinkDir () =
    failwith "not implemented"
        
//  let verifyFile () = 
//    let path = Path.full opts.File
//    let provider = new IOProviderImpl() :> IOProvider
//    action "run job(s) in file"
//    info "FILE" path
//    match provider.FileExists(path) with
//    | false ->  fail "ERROR" "file not found on disk"
//                false
//    | true  ->  match empty opts.JobId with
//                | true  -> info "RUN JOB" "all"
//                | false -> info "RUN JOB" opts.JobId
//                true
  
//  let verifyYank () =
//    match opts.Yank, opts.YankDir with
//    | true, _   ->  action "delete files"
//                    status "SOURCE" _src  // TODO: combine with base
//    | _, true   ->  action "delete directory (recursive)"
//                    status "SOURCE" _src
//    | _         ->  failwith "internal error"
//
//  let verify () =
//    let fileSelected = not (empty _file)
//    let cmds = [  fileSelected;
//                  _yank; _yankd; 
//                  _copy; _copyd; _copyf;
//                  _link; _linkd; _linkf ]

//    verifyFile ()
//    verifyYank ()

  let create () =
    let actions = [
      not (empty opts.File), fromFile
      opts.Copy, fromCopy
      opts.CopyDir, fromCopyDir
      opts.CopyFile, fromCopyFile
      opts.Link, fromLink
      opts.LinkDir, fromLinkDir
      opts.LinkFile, fromLinkFile ]
    let action = actions |> List.tryFind (fun (a, _) -> a)
    match action with
    | Some (a, f)   ->  f()
    | None          ->  fail "ERROR" "no action detected"
                        // TODO: write opts usage to info
                        Unchecked.defaultof<_>
                       
