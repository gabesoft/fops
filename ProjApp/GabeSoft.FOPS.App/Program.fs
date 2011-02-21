open System

open GabeSoft.FOPS.Core

[<EntryPoint>]
let main (args) = 
  try
    let opts = new Options(args)
    let log = match opts.Verbose with
              | true  -> new LogImpl() :> Log
              | false -> new ErrorLog(new LogImpl()) :> Log
    let validatorLog = new LogImpl(ConsoleColor.DarkCyan)
    let validator = new OptionsValidator(validatorLog)
    let server = new IOServer()
    let engine = new Engine(server, log)
    let jobs = validator.CreateJobs(opts)
    engine.Run(jobs)
  with e -> 
    let log = new LogImpl() :> Log
    log.Fail (e.ToString())
  0
