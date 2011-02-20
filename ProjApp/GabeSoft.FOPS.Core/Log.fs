namespace GabeSoft.FOPS.Core

open System
open System.IO

type Log =
  abstract member Info : string -> unit
  abstract member Warn : string -> unit
  abstract member Fail : string -> unit
  abstract member InfoWriter : TextWriter
  abstract member WarnWriter : TextWriter
  abstract member FailWriter : TextWriter

type LogImpl (?color) = 
  let _color = match color with | Some c -> c | None -> Console.ForegroundColor
  let changeColor color = 
    let orig = Console.ForegroundColor
    Console.ForegroundColor <- color
    { new IDisposable with
        member x.Dispose() = Console.ForegroundColor <- orig }
  let writeln color = 
    fun message -> 
      use c = changeColor color
      Console.WriteLine (message:string)   

  interface Log with
    member x.Info message = writeln _color message
    member x.Warn message = writeln ConsoleColor.Yellow message
    member x.Fail message = writeln ConsoleColor.Red message
    member x.InfoWriter = Console.Out
    member x.WarnWriter = Console.Out
    member x.FailWriter = Console.Out
