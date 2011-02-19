module OptionsValidatorTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core
open GabeSoft.FOPS.Test

let opts args = new Options(args)
let item (jobs: Job list) = 
  jobs
  |> List.head 
  |> (fun j -> j.Items)
  |> List.head

let srcArg src = sprintf "--src=%s" src
let dstArg dst = sprintf "--dst=%s" dst

let validating opts =
  printMethod "\n"
  let log = new LogImpl()
  let validator = new OptionsValidator(log)
  validator.CreateJobs opts

let expected_jobs expected (jobs:Job list) =
  let actual = jobs |> List.map (fun j -> j.Id) |> List.sort
  printMethod actual  
  actual = expected

let expected_base_paths src dst (jobs:Job list) =
  let job = List.head jobs
  let srcOk (path:string) = path.StartsWith(src)
  let dstOk (path:string) = path.StartsWith(dst)
  let actual = 
    job.Items 
    |> List.map (function
      | Copy (s, d, _, _, _)  -> s, d
      | Link (s, d, _, _, _)  -> s, d
      | Yank (s, _)           -> s, dst)
    |> List.map (fun (s, d) -> srcOk s && dstOk dst)
  printMethod actual
  actual |> List.forall id

let check_yank src mode = function
  | Yank (s, c)   -> s = src && c = mode 
  | _             -> false

let check_copy src dst force mode = function
  | Copy (s, d, f, _, c)  -> s = src && d = dst && f = force && c = mode
  | _                     -> false

let check_link src dst force mode = function
  | Link (s, d, f, _, c)  -> s = src && d = dst && f = force && c = mode
  | _                     -> false

let check_funs = Map.ofList ["copy", check_copy; "link", check_link]

let expected_type check jobs = 
  let item = item jobs
  printMethod item
  check item

[<Scenario>]
let ``File - creates all jobs`` () =
  Given (opts ["-f=Files/jobs2.xml"])
  |> When validating
  |> It should have (expected_jobs ["j1"; "j2"; "j3"])
  |> Verify

[<Scenario>]
let ``File - filters on job id`` () =
  Given (opts ["-f=Files/jobs2.xml"; "-j=j2"])
  |> When validating
  |> It should have (expected_jobs ["j2"])
  |> Verify

[<Scenario>]  
let ``File - overrides base-src and base-dst`` () =
  Given (opts ["-f=Files/jobs5.xml"; @"-b=C:\Temp1"; @"-B=C:\Temp2"])
  |> When validating
  |> It should have (expected_base_paths @"C:\Temp1" @"C:\Temp2")
  |> Verify

[<Scenario>]
let ``Delete - has expected type and source`` () =
  let src = @"C:\a\b\*\c.txt"
  Given (opts ["-d"; srcArg src])
  |> When validating
  |> It should have (expected_type (check_yank src PatternMode))
  |> Verify

[<Scenario>]
let ``Delete dir - has expected type and source`` () =
  let src = @"C:\a\b"
  Given (opts ["-D"; srcArg src])
  |> When validating
  |> It should have (expected_type (check_yank src FolderMode))
  |> Verify

[<ScenarioTemplate("copy")>]
[<ScenarioTemplate("link")>]
let ``Copy - has expected type and paths`` (fkey) =
  let src = @"C:\a\?*\b\f?.p*"
  let dst = @"F:\Temp"
  let fn = check_funs.[fkey]
  Given (opts ["-c"; srcArg src; dstArg dst; "-F"])
  |> When validating
  |> It should have (expected_type (fn src dst true PatternMode))
  |> Verify

[<ScenarioTemplate("copy")>]
[<ScenarioTemplate("link")>]
let  ``Copy file - has expected type and paths`` (fkey) =
  let src = @"C:\a\b\f1.doc"
  let dst = @"C:\a\c\f2.txt"
  let fn = check_funs.[fkey]
  Given (opts ["--copyfile"; srcArg src; dstArg dst])
  |> When validating
  |> It should have (expected_type (fn src dst false FileMode))
  |> Verify

[<ScenarioTemplate("copy")>]
[<ScenarioTemplate("link")>]
let ``Copy dir - has expected type and paths`` (fkey) =
  let src = @"C:\a\b\"
  let dst = @"C:\a\c\"
  let fn = check_funs.[fkey]
  Given (opts ["-C"; srcArg src; dstArg dst])
  |> When validating
  |> It should have (expected_type (fn src dst false FolderMode))
  |> Verify
  
//[<Scenario>]
//let ``Move file - has expected job items`` () =
//  let src = @"C:\a\b\f1.doc"
//  let dst = @"C:\a\c\f2.txt"
//  Given (opts ["-m", srcArg; dstArg; "-F"]) 
//  |> When validating
//  |> It should have 
//  |> Verify  