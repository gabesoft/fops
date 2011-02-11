#I "Dependencies\Fake.1.42.23.0.Modified"
#r "FakeLib.dll"

open System
open System.Diagnostics
open Fake

let authors = ["Gabriel Adomnicai"]
let projectName = "FOPS"
let projectDesc = "File Operations"

let buildDir = @".\Build\Bin"
let deployDir = @".\Build\Deploy\"
let docsDir = @".\Build\Docs\"
let testDir = @".\Build\Test\"
let nunitPath = @".\Dependencies\NUnit.2.5.9"

let appProjects = !+ @"ProjApp\**\*.*proj" |> Scan
let testProjects = !+ @"ProjTest\**\*.*proj" |> Scan

let version = "1.0"

let testAssemblies = !+ (testDir + "GabeSoft.*.Test.dll") |> Scan

let trimQuotes (s:string) = s.Trim('"')

Target? Clean <- 
   fun _ -> CleanDirs [buildDir; deployDir; docsDir; testDir ]

Target? GetLatest <-
   fun _ -> 
      hgPull |> (fun (_, result, _) -> Log "Pull-Output: " result)
      hgUpdate |> (fun (_, result, _) -> Log "Update-Output: " result)

Target? CheckIn <-
   fun _ ->
      let comment = environVar "checkin_comment" |> trimQuotes
      let user = "gadomnicai" 
      let exitCode, messages, errors = hgCommit comment user
      messages |> Log "Commit-Output: "
      errors |> Log "Commit-ERR-Output: "
      hgPush |> (fun (_, result, _) -> Log "Push-Output: " result)

Target? BuildApp <-
   fun _ -> 
      AssemblyInfo (fun p -> 
            { p with 
               CodeLanguage = FSharp;
               AssemblyVersion = version;
               AssemblyTitle = "FOPS Service";
               AssemblyDescription = "File Operations Creator Service";
               Guid = "F06350B3-3702-49A8-9E85-8DEADE181946";
               OutputFileName = @".\ProjApp\GabeSoft.FOPS.Service\AssemblyInfo.fs"})
      AssemblyInfo (fun p ->
            { p with 
               CodeLanguage = FSharp
               AssemblyVersion = version
               AssemblyTitle = "FOPS Application"
               AssemblyDescription = "File Operations Creator Console Application"
               Guid = "E8F78168-4D85-4D73-9E10-67BA36A05A16"
               OutputFileName = @".\ProjApp\GabeSoft.FOPS.App\AssemblyInfo.fs"})
      MSBuildRelease buildDir "Build" appProjects |> Log "AppBuild-Output: "

Target? BuildTest <-
   fun _ -> 
      MSBuildDebug testDir "Build" testProjects |> Log "TestBuild-Output: "

Target? Test <-
   fun _ -> 
      NUnit (fun p ->
         { p with 
            ToolPath = nunitPath
            DisableShadowCopy = true
            OutputFile = testDir + "TestResults.xml" })
         testAssemblies

Target? Default <- DoNothing

//For? GetLatest <- Dependency? Clean
//For? BuildApp <- Dependency? GetLatest
For? BuildApp <- Dependency? Clean
For? Test <- Dependency? BuildApp |> And? BuildTest
For? CheckIn <- Dependency? Test
For? Default <- Dependency? CheckIn

Run (environVar "build_target" |> trimQuotes)