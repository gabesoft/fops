module JobsParserTests

open System
open System.IO
open System.Diagnostics

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core

let paths = 
   [ "Files\jobs1.xml"; "Files\jobs2.xml"; "Files\jobs3.xml"; "Files\jobs4.xml" ] 
   |> Seq.map (fun p -> Path.GetFullPath(p))

let file2 = paths |> Seq.skip 1 |> Seq.head
let file3 = paths |> Seq.skip 2 |> Seq.head
let file4 = paths |> Seq.skip 3 |> Seq.head

let checking_existence paths =
   printMethod ""
   paths |> Seq.map (fun p -> File.Exists(p))

let all_found seq =
   printMethod (Seq.toList seq)
   Seq.forall (fun p -> p) seq

let parsing file =
   printMethod file
   JobsParser.parseFile file

let expected_ids (l: Job list) =
   let ids = l |> List.map (fun j -> j.Id)
   
   printMethod ids
   ids = ["j1"; "j2"; "j3" ]

let expected_item_count (l: Job list) =
   let counts = l |> List.map (fun j -> j.ItemsAbsolute.Length)

   printMethod counts
   counts = [3; 2; 2]

let expected_item_count_by_type (l: Job list) =
   let isCopy = function Copy _ -> true | _ -> false
   let isLink = function Link _ -> true | _ -> false
   let isYank = function Yank _ -> true | _ -> false
   let intOfBool = function | true -> 1 | false -> 0
   let countsByType (job: Job) =
      job.ItemsAbsolute
      |> List.map (fun e -> 
            e |> isCopy |> intOfBool, 
            e |> isLink |> intOfBool, 
            e |> isYank |> intOfBool)
      |> List.fold (fun (cacc,lacc,yacc) (c,l,y) -> cacc+c, lacc+l, yacc+y) (0,0,0)
   let counts = l |> List.map countsByType

   printMethod counts
   counts = [0, 3, 0; 1, 0, 1; 2, 0, 0]

let expected_exclude_items (l: Job list) =
   let len = function 
   | Copy (_, _, _, e, _)  -> e.Length
   | Link (_, _, _, e, _)  -> e.Length
   | _                     -> 0
   let excludeCounts (job: Job) =
      job.ItemsAbsolute
      |> List.map len
      |> List.sum
   let actual = l |> List.map excludeCounts
   let expected = [1; 2; 5]

   printMethod (expected, actual)
   expected = actual

let get_job_with_id id (jobs: Job list) =
   jobs |> List.find (fun j -> j.Id = id)

let expected_item_types jobs =
   let job = get_job_with_id "j3" jobs
   let actual = job.ItemsAbsolute 
                  |> List.map (function
                     | Copy (_, _, _, e, c) -> c, e.Length
                     | Link (_, _, _, e, c) -> c, e.Length
                     | _                    -> Pattern, 0)
                  |> List.sort
   let expected = [File, 0; Pattern, 2; Folder, 3] |> List.sort

   printMethod (expected, actual)
   expected = actual

let expected_from_paths jobs =
   let job = get_job_with_id "j3" jobs
   let actual = job.ItemsAbsolute
                  |> List.map (function
                     | Copy (f, _, _, _, c)  -> f, c
                     | Link (f, _, _, _, c)  -> f, c
                     | Yank (f)              -> f, Pattern)
                  |> List.sort
   let expected = [ 
      @"C:\Source\f1.txt", File
      @"C:\Source\*\cache\*.doc", Pattern
      @"C:\Source\a\b", Folder ] |> List.sort

   printMethod (expected, actual)
   expected = actual

[<Scenario>]
let ``Can find the jobs files`` () =
   Given paths
   |> When checking_existence
   |> It should be all_found
   |> Verify

[<Scenario>]
let ``Can read all jobs in file`` () =
   Given file2
   |> When parsing
   |> It should have (length 3)
   |> Verify

[<Scenario>]
let ``Can populate the ids of all parsed jobs`` () =
   Given file2
   |> When parsing
   |> It should have expected_ids
   |> Verify

[<Scenario>]
let ``Can populate the copy items for each parsed job`` () =
   Given file2
   |> When parsing
   |> It should have expected_item_count
   |> Verify

[<Scenario>]
let ``Can populate the correct copy item type`` () =
   Given file2
   |> When parsing
   |> It should have expected_item_count_by_type
   |> Verify

[<Scenario>]
let ``Can populate the copy exclude items`` () =
   Given file4
   |> When parsing
   |> It should have expected_exclude_items
   |> Verify

[<Scenario>]
let ``Can populate the copy types`` () =
   Given file4
   |> When parsing
   |> It should have expected_item_types
   |> Verify

[<Scenario>]
let ``Can populate the from paths`` () =
   Given file4
   |> When parsing
   |> It should have expected_from_paths
   |> Verify

[<Scenario>]
[<FailsWithType (typeof<ParseException>)>]
let ``Job id is a required attribute`` () =
   Given file3
   |> When parsing
   |> Verify
