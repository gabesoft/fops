namespace GabeSoft.FOPS.Core

open System
open Microsoft.FSharp.Text
open GabeSoft.FOPS.Cmd

type Options (args) =
   let mutable _help = false
   let mutable _file = String.Empty
   let mutable _from = String.Empty
   let mutable _to = String.Empty
   let mutable _jobId = String.Empty
   let mutable _basePath = String.Empty
   let mutable _overwrite = false
   let mutable _hlinks = false
   let set = new OptionSet()

   do 
      set.Add("?|help", "Show usage.", fun v -> _help <- true) |> ignore
    
      set.Add( "x|file=", 
               "Path to the file specifying the copy jobs.", 
               fun v -> _file <- v) |> ignore
      set.Add( "j|jobId=", 
               "The id of a job to run. Omit to run all jobs.",
               fun v -> _jobId <- v) |> ignore
      
      set.Add("f|from=", "Source path.", fun v -> _from <- v) |> ignore
      set.Add("t|to=", "Destination path.", fun v -> _to <- v) |> ignore
      set.Add("o|overwrite", "Overwrite read only files.", fun v -> _overwrite <- true) |> ignore
      set.Add("h|hlinks", "Create hard links instead of copy.", fun v -> _hlinks <- true) |> ignore
    
      set.Add("b|basePath=", "Base directory path.", fun v -> _basePath <- v) |> ignore

      set.Parse(args) |> ignore

   let invalid () =
      let fileSpecified = not (String.IsNullOrEmpty(_file))
      let fromSpecified = not (String.IsNullOrEmpty(_from))
      let toSpecified = not (String.IsNullOrEmpty(_to))
      let toOrFromSpecified = fromSpecified || toSpecified
      let toAndFromSpecified = fromSpecified && toSpecified

      (fileSpecified && toOrFromSpecified) ||
      (toOrFromSpecified && not toAndFromSpecified)

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
      Console.WriteLine("fcopy (jobs <jobId>)|(from to <overwrite> <hlinks>) [basePath]")
      set.WriteOptionDescriptions(Console.Out)
      Console.WriteLine("Note: Either specify a job file and optionally a job id")
      Console.WriteLine("      or specify a source and a destination path.")
      Console.WriteLine("      The source path may contain wildcards.")
      Console.WriteLine("      If a job file is not specified then both a source and")
      Console.WriteLine("      a destination path are required.")
      Console.WriteLine("      If a base path is specified it will be applied")
      Console.WriteLine("      to all relative paths.")

