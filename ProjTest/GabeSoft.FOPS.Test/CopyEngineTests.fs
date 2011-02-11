module CopyEngineTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core

// TODO: 
//  - test copy-file
//  - test copy-dir
//  - test copy
//  - test copy-file
//  - test copy-dir
//  - test copy
//  - test yank

let running_job (server: IOServer, job: Job) =
  printMethod String.Empty
  CopyEngine.run server [job]
  server.Provider

let mock_provider () = 
  mock<IOProvider> "provider"
  |> setup<@fun x -> x.FileExists@> (fun _ -> true)
  |> setup<@fun x -> x.FolderExists@> (fun _ -> true)                    

let create_job item = new Job([item])
let server provider = 
  new IOServer(provider)  

[<Scenario>]
let ``Yank should delete the correct file`` () = 
  let src = @"C:\a\b\c\f.doc"
  let job = src |> Item.yank |> create_job
  let provider = 
    mock_provider() 
    |> register <@fun x -> x.DeleteFile@> (fun src -> ())
  
  Given (server provider, job)
  |> When running_job
  |> Called <@fun x -> x.DeleteFile @>(src)
  |> Verify

[<Scenario>]
let ``Copy file should use correct paths`` () =
  let src = @"C:\a\b\f1.txt"
  let dst = @"C:\e\f\g\f2.doc"
  let item = Item.copy File (src, dst, false, [])
  let job = new Job([item])
  let provider = 
    mock_provider () 
    |> setup<@fun x -> x.Copy@>(fun (src, dst) -> ())

  Given (server provider, job)
  |> When running_job
  |> Called <@fun x -> x.Copy@> (src, dst)
  |> Verify

