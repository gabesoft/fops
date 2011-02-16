namespace GabeSoft.FOPS.Core

open System

type Log =
  abstract member Info : string -> unit
  abstract member Warn : string -> unit
  abstract member Fail : string -> unit

type LogImpl () = 
  interface Log with
    member x.Info message = Console.WriteLine(message)
    member x.Warn message = Console.WriteLine(message)
    member x.Fail message = Console.WriteLine(message)
