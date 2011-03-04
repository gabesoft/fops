namespace GabeSoft.FOPS.Service

open System
open System.ComponentModel
open System.Configuration.Install
open System.ServiceProcess

module Entry =
    [<EntryPoint>]
    let main (args) = 
        ServiceBase.Run(new FopsService())
        0