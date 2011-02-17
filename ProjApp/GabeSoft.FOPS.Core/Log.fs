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

type LogImpl () = 
  interface Log with
    member x.Info message = Console.WriteLine(message)
    member x.Warn message = Console.WriteLine(message)
    member x.Fail message = Console.WriteLine(message)
    member x.InfoWriter = Console.Out
    member x.WarnWriter = Console.Out
    member x.FailWriter = Console.Out
