namespace GabeSoft.FOPS.Core

open System
open Microsoft.FSharp.Text
open GabeSoft.FOPS.Cmd

type Options (args, ?app) =
  let _app = match app with Some x -> x | None -> "fops"
  let mutable _help = false
  let mutable _file = String.Empty
  let mutable _yank = false
  let mutable _copy = false
  let mutable _copyf = false
  let mutable _copyd = false
  let mutable _link = false
  let mutable _link = false
  let mutable _linkf = false
  let mutable _linkd = false
  let mutable _force = false
  let mutable _src = String.Empty
  let mutable _dst = String.Empty
  let mutable _baseSrc = String.Empty
  let mutable _baseDst = String.Empty
  let mutable _jobId = String.Empty

  let mutable _from = String.Empty
  let mutable _to = String.Empty
  let mutable _basePath = String.Empty
  let mutable _overwrite = false
  let mutable _hlinks = false
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
    add "c|copy"        (fun v -> _copy <- v)
        "Copy files according to a wildcard pattern." 
    add "l|link"        (fun v -> _link <- v)
        "Link files according to a wildcard pattern." 
    add "cf|copyfile"   (fun v -> _copyf <- v)
        "Copy a single file."
    add "lf|linkfile"   (fun v -> _linkf <- v)
        "Link a single file."
    add "cd|copydir"    (fun v -> _copyd <- v)
        "Copy a directory recursively."
    add "ld|linkdir"    (fun v -> _linkd <- v)
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


//    set.Add("?|help", "Show usage.", fun v -> _help <- true) |> ignore
//    
//    set.Add( "x|file=", 
//              "Path to the file specifying the copy jobs.", 
//              fun v -> _file <- v) |> ignore
//    set.Add( "j|jobId=", 
//              "The id of a job to run. Omit to run all jobs.",
//              fun v -> _jobId <- v) |> ignore
//      
//    set.Add("f|from=", "Source path.", fun v -> _from <- v) |> ignore
//    set.Add("t|to=", "Destination path.", fun v -> _to <- v) |> ignore
//    set.Add("o|overwrite", "Overwrite read only files.", fun v -> _overwrite <- true) |> ignore
//    set.Add("h|hlinks", "Create hard links instead of copy.", fun v -> _hlinks <- true) |> ignore
//    
//    set.Add("b|basePath=", "Base directory path.", fun v -> _basePath <- v) |> ignore

    set.Parse(args) |> ignore

  let invalid () =
    let fileSpecified = not (String.IsNullOrEmpty(_file))
    let fromSpecified = not (String.IsNullOrEmpty(_from))
    let toSpecified = not (String.IsNullOrEmpty(_to))
    let toOrFromSpecified = fromSpecified || toSpecified
    let toAndFromSpecified = fromSpecified && toSpecified

    (fileSpecified && toOrFromSpecified) ||
    (toOrFromSpecified && not toAndFromSpecified)

  let writeln text = Console.WriteLine(text:string)
  let cmd text = sprintf "  %s %s" _app text |> writeln
  let writeOpts () = set.WriteOptionDescriptions(Console.Out)

  member x.Help with get () = _help
  member x.File with get () = _file
  member x.From with get () = _from
  member x.To with get () = _to
  member x.JobId with get () = _jobId
  member x.BasePath with get () = _basePath
  member x.Overwrite with get () = _overwrite
  member x.Hlinks with get () = _hlinks
  member x.Valid with get () = not (invalid())
  member x.WriteUsage () = 
    cmd "-file=<path> [-basesrc=<path>] [-basedst=<path>] [-jobid=<id>]"
    cmd "-delete      -src=<pattern>"
    cmd "-copy        -src=<pattern>  -dst=<path> [-f]"
    cmd "-copyfile    -src=<path>     -dst=<path> [-f]"
    cmd "-copydir     -src=<path>     -dst=<path> [-f]"
    cmd "-link        -src=<pattern>  -dst=<path> [-f]"
    cmd "-linkfile    -src=<path>     -dst=<path> [-f]"
    cmd "-linkdir     -src=<path>     -dst=<path> [-f]"
    writeln String.Empty
    writeOpts ()

