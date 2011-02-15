namespace GabeSoft.FOPS.Core

open System
open Microsoft.FSharp.Text
open GabeSoft.FOPS.Cmd

type Options (args, ?app) =
  let _app = match app with Some x -> x | None -> "fops"
  
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
  let mutable _force = false
  let mutable _src = String.Empty
  let mutable _dst = String.Empty
  let mutable _baseSrc = String.Empty
  let mutable _baseDst = String.Empty
  let mutable _jobId = String.Empty
  
  let set = new OptionSet()
  let add arg (action:'a -> unit) desc = 
    set.Add(arg, desc, new Action<_>(action)) |> ignore

  do 
    add "?|help"        (fun v -> _help <- true)
        "Show usage." 
    add "f|file="       (fun v -> _file <- v)
        "Path to the file containing the jobs to run." 
    add "d|delete"      (fun v -> _yank <- v)
        "Delete files according to a wildcard pattern." 
    add "deletedir"  (fun v -> _yankd <- v)
        "Delete an entire directory recursively!"
    add "c|copy"        (fun v -> _copy <- v)
        "Copy files according to a wildcard pattern." 
    add "l|link"        (fun v -> _link <- v)
        "Link files according to a wildcard pattern." 
    add "copyfile"   (fun v -> _copyf <- v)
        "Copy a single file."
    add "linkfile"   (fun v -> _linkf <- v)
        "Link a single file."
    add "p|copydir"    (fun v -> _copyd <- v)
        "Copy a directory recursively."
    add "n|linkdir"    (fun v -> _linkd <- v)
        "Link a directory recursively."
    add "o|force"       (fun v -> _force <- v)
        "Overwrite any existing files at destination"
    add "src="          (fun v -> _src <- v)
        "Source path (filesystem path or wildcard pattern)."
    add "dst="          (fun v -> _dst <- v)
        "Destination path (filesystem path or wildcard pattern)."
    add "basesrc="      (fun v -> _baseSrc <- v)
        "Base source directory path."
    add "basedst="      (fun v -> _baseDst <- v)
        "Base destination directory path."
    add "j|jobid="      (fun v -> _jobId <- v)
        "The id of a job to run. Omit to run all jobs."

    set.Parse(args) |> ignore

  let verify () =
    let provider = new IOProviderImpl() :> IOProvider
    let fileSelected = 
      String.IsNullOrEmpty(_file) |> not && 
      provider.FileExists(_file) 
    if fileSelected then false else
    let cmds = [  _yank; _yankd; 
                  _copy; _copyd; _copyf;
                  _link; _linkd; _linkf ]
    failwith "not implemented"
        

  let writeln text = Console.WriteLine(text:string)
  let cmd text = sprintf "  %s %s" _app text |> writeln
  let writeOpts () = set.WriteOptionDescriptions(Console.Out)

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
  member x.Force with get() = _force
  member x.Src with get() = _src
  member x.Dst with get() = _dst
  member x.BaseSrc with get() = _baseSrc
  member x.BaseDst with get() = _baseDst
  member x.JobId with get() = _jobId
  /// Verify selected option and print status.
  /// Returns false if no valid option could be determined.
  member x.Verify = verify 
  member x.WriteUsage () = 
    cmd "--file=<path> [-basesrc=<path>] [-basedst=<path>] [-jobid=<id>]"
    cmd "--delete      -src=<pattern>"
    cmd "--deletedir   -src=<path>"
    cmd "--copy        -src=<pattern>  -dst=<path> [-f]"
    cmd "--copyfile    -src=<path>     -dst=<path> [-f]"
    cmd "--copydir     -src=<path>     -dst=<path> [-f]"
    cmd "--link        -src=<pattern>  -dst=<path> [-f]"
    cmd "--linkfile    -src=<path>     -dst=<path> [-f]"
    cmd "--linkdir     -src=<path>     -dst=<path> [-f]"
    writeln String.Empty
    writeOpts ()

