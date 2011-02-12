module CopyEngineTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core
open GabeSoft.FOPS.Test

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
  |> setup <@fun x -> x.DeleteFile@> (fun src -> ())
  |> setup<@fun x -> x.Copy@>(fun (src, dst) -> ())

let create_job item = new Job([item])
let create_server () = new IOServer(mock_provider())  

[<Scenario>]
let ``Yank should delete the correct file`` () = 
  let src = @"C:\a\b\c\f.doc"
  let job = src |> Item.yank |> create_job
  
  Given (create_server (), job)
  |> When running_job
  |> Called <@fun x -> x.DeleteFile @>(src)
  |> Verify

[<Scenario>]
let ``Copy file should use correct paths`` () =
  let src = @"C:\a\b\f1.txt"
  let dst = @"C:\e\f\g\f2.doc"
  let job = new Job([Item.copy File (src, dst, true, [])])

  Given (create_server(), job)
  |> When running_job
  |> Called <@fun x -> x.Copy@> (src, dst)
  |> Verify

[<Scenario>]
let ``Copy dir should create directory structure`` () =
  let src = @"C:\a\b"
  let dst = @"C:\e\f"
  let srcPath path = Path.combine src path
  let dstPath path = Path.combine dst path
  let job = new Job ([Item.copy Folder (src, dst, true, [])])
  let files = [ @"f1.txt"
                @"c\f2.txt"
                @"d\f2.txt"
                @"c\d\f3.txt" ] 
  // TODO: combine provider with mock
  let provider = new TestIOProvider(files |> List.map srcPath)
  
  Given (new IOServer(provider), job)
  |> When running_job
  |> Called <@fun x -> x.Copy@>(srcPath files.[0], dstPath files.[0])
  |> Called <@fun x -> x.Copy@>(srcPath files.[1], dstPath files.[1])
  |> Verify