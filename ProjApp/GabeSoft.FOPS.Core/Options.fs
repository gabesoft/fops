namespace GabeSoft.FOPS.Core

open System
open System.Collections.Generic
open Microsoft.FSharp.Text
open GabeSoft.FOPS.Cmd

/// Object used to hold all options specified on the command line.
type Options (args, ?log:Log, ?app) =
  let _app = match app with Some x -> x | None -> "fops"
  let _log = match log with Some x -> x | None -> new LogImpl() :> Log
  
  let mutable _help = false
  let mutable _file = String.Empty
  let mutable _yank = false
  let mutable _yankd = false
  let mutable _copy = false
  let mutable _copyf = false
  let mutable _copyd = false
  let mutable _link = false
  let mutable _linkf = false
  let mutable _linkd = false
  let mutable _movef = false
  let mutable _moved = false
  let mutable _force = false
  let mutable _verbose = false
  let mutable _src = String.Empty
  let mutable _dst = String.Empty
  let mutable _baseSrc = String.Empty
  let mutable _baseDst = String.Empty
  let mutable _jobId = String.Empty
  
  let set = new OptionSet()
  let add arg (action:string -> unit) desc = 
    set.Add(arg, desc, new Action<_>(action)) |> ignore
  let add2 arg (action:string -> string -> unit) desc =
    set.Add(arg, desc, new OptionAction<_, _>(action)) |> ignore

  do 
    add   "?|help"        (fun v -> _help <- true)
          "Show usage." 
    add   "f|file="       (fun v -> _file <- v)
          "Path to the file containing the jobs to run." 
    add   "d|delete="     (fun v -> _yank <- true; _src <- v)
          @"Delete all files that match a wildcard
          pattern (including read-only!)." 
    add   "D|deleted="    (fun v -> _yankd <- true; _src <- v)
          "Delete an entire directory recursively!"
    add2  "c|copy={}"     (fun src dst -> _copy <- true; _src <- src; _dst <- dst)
          "Copy files according to a wildcard pattern." 
    add2  "l|link={}"     (fun src dst -> _link <- true; _src <- src; _dst <- dst)
          "Link files according to a wildcard pattern." 
    add2  "copyf={}"      (fun src dst ->  _copyf <- true; _src <- src; _dst <- dst)
          "Copy a single file."
    add2  "linkf={}"      (fun src dst -> _linkf <- true; _src <- src; _dst <- dst)
          "Link a single file."
    add2  "m|movef={}"    (fun src dst -> _movef <- true; _src <- src; _dst <- dst)
          "Rename a file."
    add2  "C|copyd={}"    (fun src dst ->  _copyd <- true; _src <- src; _dst <- dst)
          "Copy a directory recursively."
    add2  "L|linkd={}"    (fun src dst -> _linkd <- true; _src <- src; _dst <- dst)
          "Link a directory recursively."
    add2  "M|moved={}"    (fun src dst -> _moved <- true; _src <- src; _dst <- dst)
          "Rename or move a directory."
    add   "F|force"       (fun v -> _force <- true)
          "Overwrite any existing files at destination"
    add   "v|verbose"     (fun v -> _verbose <- true)
          "Displays detailed information"
    add   "b|basesrc="    (fun v -> _baseSrc <- v)
          "Base source directory path."
    add   "B|basedst="    (fun v -> _baseDst <- v)
          "Base destination directory path."
    add   "j|jobid="      (fun v -> _jobId <- v)
          "The id of a job to run. Omit to run all jobs."

    set.Parse(args) |> ignore

  let writeln text = _log.Info text
  let cmd text = sprintf "  %s %s" _app text |> writeln
  let writeOpts () = set.WriteOptionDescriptions(_log.InfoWriter)
        
  member x.Help with get() = _help
  member x.File with get() = _file
  member x.Yank with get() = _yank
  member x.YankDir with get() = _yankd
  member x.Copy with get() = _copy
  member x.CopyFile with get() = _copyf
  member x.CopyDir with get() = _copyd
  member x.Link with get() = _link
  member x.LinkFile with get() = _linkf
  member x.LinkDir with get() = _linkd
  member x.MoveFile with get() = _movef
  member x.MoveDir with get() = _moved
  member x.Force with get() = _force
  member x.Verbose with get() = _verbose
  member x.Src with get() = _src
  member x.Dst with get() = _dst
  member x.BaseSrc with get() = _baseSrc
  member x.BaseDst with get() = _baseDst
  member x.JobId with get() = _jobId
  member x.WriteUsage () = 
    cmd "--file=<path> [-basesrc=<path>] [-basedst=<path>] [-jobid=<id>]"
    cmd "--copy       <src_pattern>  <dst_path> [options]"
    cmd "--link       <src_pattern>  <dst_path> [options]"
    cmd "--delete     <src_pattern>"
    cmd "--copyf      <src_path>     <dst_path> [options]"
    cmd "--linkf      <src_path>     <dst_path> [options]"
    cmd "--movef      <src_path>     <dst_path> [options]"
    cmd "--copyd      <src_path>     <dst_path> [options]"
    cmd "--linkd      <src_path>     <dst_path> [options]"
    cmd "--moved      <src_path>     <dst_path> [options]"
    cmd "--deleted    <src_path>"
    writeln String.Empty
    writeOpts ()
    writeln  ""
    writeln  "NOTES"
    writeln  "- options:"
    writeln  "  -F, --force"
    writeln  "  -v, --verbose"
    writeln  "- copydir and movedir work as follows:"
    writeln  "  if the destination path is an existing directory"
    writeln  "  the source directory gets copied/moved inside the"
    writeln  "  destination directory, otherwise the source directory"
    writeln  "  gets copied/moved as the destination directory"
    writeln  "- path: "
    writeln  "  a filesystem path which can be absolute"
    writeln  "  or relative to the basesrc or basedst"
    writeln  "- pattern: "
    writeln  "  a wildcard pattern which can be absolute"
    writeln  "  or relative to the basesrc."
    writeln  "- the following wildcard characters are supported"
    writeln  "   * : matches zero or more characters"
    writeln  "   ? : matches exactly one character"
    writeln @"- directory match works as follows"
    writeln @"   /*/  : matches any directory any level deep"
    writeln @"   /?*/ : matches any directory exactly one level deep"
    writeln @"- pattern examples:"
    writeln @"  C:\*       (matches all files in the root directory)"
    writeln @"  C:\*\*     (matches all files in the root directory "
    writeln @"                and all its sub directories any level deep)"
    writeln @"  C:\a\*     (matches all files in 'a' directory)"
    writeln @"  C:\a\b*\*  (matches all files in all sub directories of"
    writeln @"                'a' that start with letter 'b')"
    writeln @"  C:\?*\f.txt"
    writeln @"  C:\a\*\b\*.txt"
    writeln @"  C:\a\?*\b\c*.pd?"
    writeln @"  C:\a\*\b\?*\c\f?*.txt"
    