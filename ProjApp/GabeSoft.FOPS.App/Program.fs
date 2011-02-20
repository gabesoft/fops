open System

open GabeSoft.FOPS.Core

[<EntryPoint>]
let main (args) = 
  let log = new LogImpl()
  let validatorLog = new LogImpl(ConsoleColor.DarkCyan)
  try
    let opts = new Options(args)
    let validator = new OptionsValidator(validatorLog)
    let server = new IOServer()
    let engine = new Engine(server, log)
    let jobs = validator.CreateJobs(opts)
    engine.Run(jobs)
  with e -> 
    (log :> Log).Fail (e.ToString())
  0
