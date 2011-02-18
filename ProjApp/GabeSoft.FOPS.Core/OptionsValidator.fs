namespace GabeSoft.FOPS.Core

open System
open System.Collections.Generic

/// Object used to validate a set of command line options
/// and use them to create a list of jobs.
type OptionsValidator(log:Log) =
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

  let mkJobs item = [new Job([item])]

  let usage (opts:Options) = 
    opts.WriteUsage()
    []
  
  let ofFile (opts:Options) = 
    let path = Path.full opts.File
    let provider = new IOProviderImpl() :> IOProvider
    action "run job(s) in file"
    info "FILE" path
    match provider.FileExists(path) with
    | false ->  fail "ERROR" "file not found on disk"
                []
    | true  ->  
        let runAll = empty opts.JobId
        let jobs = 
          JobsParser.parseFile(path)
          |> List.filter (fun j -> runAll || j.Id = opts.JobId)
          |> List.map (fun j -> match empty opts.BaseSrc with
                                | true  -> j
                                | false -> j.WithSrc opts.BaseSrc)
          |> List.map (fun j -> match empty opts.BaseDst with
                                | true  -> j
                                | false -> j.WithDst opts.BaseDst)
        info "RUN JOB(S)" (jobs 
                            |> List.map (fun j -> j.Id) 
                            |> String.concat ", ")
        if not (empty opts.BaseSrc) then info "BASE SOURCE" opts.BaseSrc
        if not (empty opts.BaseDst) then info "BASE DESTINATION" opts.BaseDst
        jobs
    
  let ofYank (opts:Options) = 
    action "delete all files matching PATTERN"
    info "PATTERN" opts.Src
    mkJobs (Item.yank PatternMode opts.Src)

  let ofYankDir (opts:Options) =
    action "delete directory at PATH"
    info "PATH" opts.Src
    mkJobs (Item.yank FolderMode opts.Src)

  let ofCopy (opts:Options) = 
    action "copy all files matching PATTERN to DESTINATION"
    info "PATTERN" opts.Src
    info "DESTINATION" opts.Dst
    info "OVERWRITE" (string opts.Force) 
    mkJobs (Item.copy PatternMode (opts.Src, opts.Dst, opts.Force, []))

  /// Creates a list of jobs that match the 
  /// specified options.
  member x.CreateJobs (opts:Options) = 
    let actions = [
      opts.Help, usage
      not (empty opts.File), ofFile
      opts.Copy, ofCopy
//      opts.CopyDir, fromCopyDir
//      opts.CopyFile, fromCopyFile
//      opts.Link, fromLink
//      opts.LinkDir, fromLinkDir
//      opts.LinkFile, fromLinkFile 
      opts.Yank, ofYank
      opts.YankDir, ofYankDir ]
    let action = actions |> List.tryFind (fun (a, _) -> a)
    let jobs = match action with
                | Some (a, f)   ->  f opts
                | None          ->  fail "ERROR" "no action detected"
                                    // TODO: write opts usage to info
                                    Unchecked.defaultof<_>
    printStatus()
    jobs

//type JobCreator(opts:Options, log:Log) =
//  let statusItems = new Queue<string * string * (string -> unit)>()
//  let status write title value = 
//    statusItems.Enqueue(title, value, write)
//  let printStatus () =
//    let pad n (text:string) =
//      let len = text.Length
//      if len >= n then text else
//      Seq.init (n - len) (fun _ -> " ")
//      |> Seq.append [text]
//      |> String.concat ""
//    let items = statusItems.ToArray()
//    let maxl = 
//      items
//      |> Seq.map (fun (t, _, _) -> t.Length)
//      |> Seq.max
//    items
//    |> Seq.iter (fun (t, v, w) -> sprintf "- %s: %s" (pad maxl t) v |> w)
//
//  let empty (s:string) = String.IsNullOrEmpty(s) 
//  let action = status log.Info "ACTION"
//  let info = status log.Info
//  let fail = status log.Fail
//
//  let complete parent path = 
//      match Path.rooted path, empty parent with
//      | true, _      -> path
//      | false, true  -> Path.full path
//      | false, false -> Path.combine parent path
//  let completeDst = complete opts.BaseDst
//  let completeSrc = complete opts.BaseSrc
//    
//  let makePathsAbsolute = function
//  | Copy (s, d, o, e, c)  -> 
//    Copy (completeSrc s, completeDst d, o, List.map completeSrc e, c)
//  | Link (s, d, o, e, c)  -> 
//    Link (completeSrc s, completeDst d, o, List.map completeSrc e, c)
//  | Yank (s, c)           -> Yank (completeSrc s, c)
//
//  let fromFile : unit -> Job =
//    failwith "not implemented"
//
//  let fromYank () =
//    failwith "not implemented"
//
//  let fromYankDir () =
//    failwith "not implemented"
//
//  let fromCopy () =
//    failwith "not implemented"
//
//  let fromCopyFile () =
//    failwith "not implemented"                
//
//  let fromCopyDir () =
//    failwith "not implemented"
//
//  let fromLink () = 
//    failwith "not implemented"
//
//  let fromLinkFile () =
//    failwith "not implemented"
//
//  let fromLinkDir () =
//    failwith "not implemented"
//        
////  let verifyFile () = 
////    let path = Path.full opts.File
////    let provider = new IOProviderImpl() :> IOProvider
////    action "run job(s) in file"
////    info "FILE" path
////    match provider.FileExists(path) with
////    | false ->  fail "ERROR" "file not found on disk"
////                false
////    | true  ->  match empty opts.JobId with
////                | true  -> info "RUN JOB" "all"
////                | false -> info "RUN JOB" opts.JobId
////                true
//  
////  let verifyYank () =
////    match opts.Yank, opts.YankDir with
////    | true, _   ->  action "delete files"
////                    status "SOURCE" _src  // TODO: combine with base
////    | _, true   ->  action "delete directory (recursive)"
////                    status "SOURCE" _src
////    | _         ->  failwith "internal error"
////
////  let verify () =
////    let fileSelected = not (empty _file)
////    let cmds = [  fileSelected;
////                  _yank; _yankd; 
////                  _copy; _copyd; _copyf;
////                  _link; _linkd; _linkf ]
//
////    verifyFile ()
////    verifyYank ()
//
//  let create () =
//    let actions = [
//      not (empty opts.File), fromFile
//      opts.Copy, fromCopy
//      opts.CopyDir, fromCopyDir
//      opts.CopyFile, fromCopyFile
//      opts.Link, fromLink
//      opts.LinkDir, fromLinkDir
//      opts.LinkFile, fromLinkFile ]
//    let action = actions |> List.tryFind (fun (a, _) -> a)
//    match action with
//    | Some (a, f)   ->  f()
//    | None          ->  fail "ERROR" "no action detected"
//                        // TODO: write opts usage to info
//                        Unchecked.defaultof<_>
                       
