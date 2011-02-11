module FilterTests

open System
open System.Diagnostics
open System.Text
open System.Text.RegularExpressions
open System.Globalization

open NaturalSpec
open NUnit.Framework

open GabeSoft.FOPS.Core

let paths = 
   FilterTestsData.patterns1
   |> List.map (fun (_, pl) -> pl |> List.map (fun (p, _) -> p))
   |> List.concat

let accepting (path, values) =
   let pattern = Wildcard.toRegex path
   printMethod (path, pattern)

   let actual = values |> List.map fst |> List.filter (Filter.allowed pattern)
   let expected = values |> List.filter snd |> List.map fst
   expected, actual

let matches (expected: string list, actual: string list) =
   printMethod (expected.Length, actual.Length)
   expected = actual

[<ScenarioTemplate(0)>]
[<ScenarioTemplate(1)>]
[<ScenarioTemplate(2)>]
[<ScenarioTemplate(3)>]
[<ScenarioTemplate(4)>]
[<ScenarioTemplate(5)>]
[<ScenarioTemplate(6)>]
[<ScenarioTemplate(7)>]
[<ScenarioTemplate(8)>]
[<ScenarioTemplate(9)>]
[<ScenarioTemplate(10)>]
[<ScenarioTemplate(11)>]
[<ScenarioTemplate(12)>]
let ``Applying a filter should accept only valid paths`` (index) =
   Given FilterTestsData.patterns1.[index]
   |> When accepting
   |> It should have matches
   |> Verify

type TestIOProvider (paths:string list) =
   let clean (path: string) = 
      let root = Path.root path
      if root = path then path else path.TrimEnd('\\')
   let equal a b = String.Equals(a, b, StringComparison.OrdinalIgnoreCase)
   let dirName path = Path.directory path
   let rec dirAll path = seq {
      let d = dirName path
      if d <> null then 
         yield d
         yield! dirAll d }
   let files folder =
      paths
      |> List.filter (fun p -> equal folder (dirName p))
      |> Seq.distinct
      |> Seq.toArray
   let folders folder =
      paths
      |> List.map (fun p -> dirAll p)
      |> Seq.concat
      |> Seq.filter (fun p -> equal folder (dirName p))
      |> Seq.distinct
      |> Seq.toArray
   let hasFile file =
      List.exists (fun p -> equal p file) paths
   let hasFolder folder =
      dirName folder
      |> folders 
      |> Seq.exists (fun p -> equal folder p) 
   interface IOProvider with
      member x.FileExists path = path |> clean |> hasFile
      member x.FolderExists path = path |> clean |> hasFolder 
      member x.GetFiles path = path |> clean |> files 
      member x.GetFolders path = path |> clean |> folders 
      member x.DeleteFile path = failwith "not implemented"
      member x.DeleteFolder path deep = failwith "not implemented"
      member x.CreateFolder path = failwith "not implemented"
      member x.Link source destination = failwith "not implemented"
      member x.Copy source destination = failwith "not implemented"

let io_server paths = 
   let provider = new TestIOProvider(paths)
   new IOServer(provider)

let create_node_and_filter path excludes data =
   let server = io_server (data |> List.map fst)
   let node = Filter.apply {  Pattern = (Wildcard.toRegex path) 
                              Exclude = (excludes |> List.map Wildcard.toRegex)
                              Recursive = Wildcard.isRecursive path }
                           (server.Node (Wildcard.root path))
   node

let filtering (path, excludes, data) =
   printMethod path
   let node = create_node_and_filter path excludes data
   let expected = data |> List.filter snd |> List.map fst
   let actual = node.AllFiles |> Seq.map (fun f -> f.Path) |> Seq.toList
   expected, actual

let applying_file_filter (path, data) =
   printMethod path
   Wildcard.isRecursive path, create_node_and_filter path [] data

let no_folders_if_recursive (isRecursive, node:IONode) =
   printMethod (isRecursive, node.Folders |> Seq.length)
   let noFolders = (0 = (node.Folders |> Seq.length))
   not isRecursive = noFolders

let allowed_matches (expected: string list, actual: string list) =
   printMethod (expected.Length, actual.Length)
   expected = actual

[<Scenario>]
let ``Applying a filter should keep only valid paths`` () =
   Given FilterTestsData.patterns2.[0]
   |> When filtering
   |> It should have allowed_matches
   |> Verify

[<ScenarioTemplate(0)>]
[<ScenarioTemplate(1)>]
[<ScenarioTemplate(2)>]
[<ScenarioTemplate(3)>]
[<ScenarioTemplate(4)>]
[<ScenarioTemplate(5)>]
[<ScenarioTemplate(6)>]
[<ScenarioTemplate(7)>]
[<ScenarioTemplate(8)>]
[<ScenarioTemplate(9)>]
[<ScenarioTemplate(10)>]
[<ScenarioTemplate(11)>]
[<ScenarioTemplate(12)>]
let ``Non recursive filters should leave no folders on parents`` (index) =
   Given FilterTestsData.patterns1.[index]
   |> When applying_file_filter
   |> It should have no_folders_if_recursive
   |> Verify