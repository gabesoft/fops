#r "FSharp.PowerPack.dll"
#r "System.Xml.Linq.dll"
#r @"bin\Debug\SofGem.DSBK.Core.dll"
#r @"bin\Debug\Sofgem.DSBK.IO.dll"
#r @"bin\Debug\Sofgem.DSBK.Domain.dll"
#r @"bin\Debug\GabeSoft.FOPS.Cmd.dll"
#r @"bin\Debug\GabeSoft.FOPS.Core.dll"

open System
open System.IO
open System.Xml.Linq

open Microsoft.FSharp.Text

open SofGem.DSBK.IO
open SofGem.DSBK.Domain

open GabeSoft.FOPS.Cmd
open GabeSoft.FOPS.Core

let path name = 
   let dir = Path.Combine(__SOURCE_DIRECTORY__, @"Files")
   Path.Combine(dir, name)

let xdoc name =
   let file = path name
   XDocument.Load(File.OpenRead(file))

let rules = 
   let rule = new PathFilterRule()
   rule.AddIncludePattern(@"C:\Temp\*\Dir1\*.txt")
//   rule.AddIncludePattern(@"C:\Temp\Source\Dir1\doc2.txt")

   let engine = new FilterEngine()
   engine.AddRule(rule)

   engine

let deleteVssFiles () =
  let server = new IOServer(new IOProviderImpl())
  let item1 = Yank(@"C:\Work\github\clickless\?*\*.vspscc")
  let item2 = Yank(@"C:\Work\github\clickless\?*\*.scc")
  let job = new Job([item1; item2])
  let engine = new Engine(server)
  engine.Run job
