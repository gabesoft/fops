#r "FSharp.PowerPack.dll"
#r @"..\bin\Debug\Sofgem.DSBK.IO.dll"
#r @"..\bin\Debug\GabeSoft.FOPS.Cmd.dll"
#r @"..\bin\Debug\GabeSoft.FOPS.Core.dll"

open System
open Microsoft.FSharp.Text
open GabeSoft.FOPS.Cmd
open GabeSoft.FOPS.Core

let opts = new Options( [ "-x"; "config.xml"; "/j=123"; "-h"; "-?"; "t=Temp" ])
             
