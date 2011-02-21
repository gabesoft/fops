namespace GabeSoft.FOPS.Core

open System
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
  let add arg (action:'a -> unit) desc = 
    set.Add(arg, desc, new Action<_>(action)) |> ignore

  do 
    add "?|help"        (fun (v:string) -> _help <- true)
        "Show usage." 
    add "f|file="       (fun v -> _file <- v)
        "Path to the file containing the jobs to run." 
    add "d|delete"      (fun (v:string) -> _yank <- true)
        @"Delete all files that match a wildcard
          pattern (including read-only!)." 
    add "D|deletedir"   (fun (v:string) -> _yankd <- true)
        "Delete an entire directory recursively!"
    add "c|copy"        (fun (v:string) -> _copy <- true)
        "Copy files according to a wildcard pattern." 
    add "l|link"        (fun (v:string) -> _link <- true)
        "Link files according to a wildcard pattern." 
    add "copyfile"      (fun (v:string) -> _copyf <- true)
        "Copy a single file."
    add "linkfile"      (fun (v:string) -> _linkf <- true)
        "Link a single file."
    add "m|movefile"    (fun (v:string) -> _movef <- true)
        "Rename a file."
    add "C|copydir"     (fun (v:string) -> _copyd <- true)
        "Copy a directory recursively."
    add "L|linkdir"     (fun (v:string) -> _linkd <- true)
        "Link a directory recursively."
    add "M|movedir"     (fun (v:string) -> _moved <- true)
        "Rename or move a directory."
    add "F|force"       (fun (v:string) -> _force <- true)
        "Overwrite any existing files at destination"
    add "v|verbose"     (fun (v:string) -> _verbose <- true)
        "Displays detailed information"
    add "p|src="        (fun v -> _src <- v)
        "Source path (filesystem path or wildcard pattern)."
    add "P|dst="        (fun v -> _dst <- v)
        "Destination path (filesystem path)."
    add "b|basesrc="    (fun v -> _baseSrc <- v)
        "Base source directory path."
    add "B|basedst="    (fun v -> _baseDst <- v)
        "Base destination directory path."
    add "j|jobid="      (fun v -> _jobId <- v)
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
    cmd "--delete      --src=<pattern>"
    cmd "--deletedir   --src=<path>"
    cmd "--copy        --src=<pattern>  --dst=<path> [options]"
    cmd "--copyfile    --src=<path>     --dst=<path> [options]"
    cmd "--copydir     --src=<path>     --dst=<path> [options]"
    cmd "--link        --src=<pattern>  --dst=<path> [options]"
    cmd "--linkfile    --src=<path>     --dst=<path> [options]"
    cmd "--linkdir     --src=<path>     --dst=<path> [options]"
    writeln String.Empty
    writeOpts ()
    writeln ""
    writeln "NOTES"
    writeln "- options:"
    writeln "  -F, --force"
    writeln "  -v, --verbose"
    writeln "- copydir and movedir work as follows:"
    writeln "  if the destination path is an existing directory"
    writeln "  the source directory gets copied/moved inside the"
    writeln "  destination directory, otherwise the source directory"
    writeln "  gets copied/moved as the destination directory"
    writeln "- path: "
    writeln "  a filesystem path which can be absolute"
    writeln "  or relative to the basesrc or basedst"
    writeln "- pattern: "
    writeln "  a wildcard pattern which can be absolute"
    writeln "  or relative to the basesrc."
    writeln "  The following wildcard characters are supported"
    writeln "   * : matches zero or more characters"
    writeln "   ? : matches exactly one character"
    writeln @"  Directory match works as follows"
    writeln @"   /*/  : matches any directory any level deep"
    writeln @"   /?*/ : matches any directory exactly one level deep"
    writeln @"  Pattern examples:"
    writeln @"  - C:\*       (matches all files in the root directory)"
    writeln @"  - C:\*\*     (matches all files in the root directory "
    writeln @"                and all its sub directories any level deep)"
    writeln @"  - C:\a\*     (matches all files in 'a' directory)"
    writeln @"  - C:\a\b*\*  (matches all files in all sub directories of"
    writeln @"                'a' that start with letter 'b')"
    writeln @"  - C:\?*\f.txt"
    writeln @"  - C:\a\*\b\*.txt"
    writeln @"  - C:\a\?*\b\c*.pd?"
    writeln @"  - C:\a\*\b\?*\c\f?*.txt"
    