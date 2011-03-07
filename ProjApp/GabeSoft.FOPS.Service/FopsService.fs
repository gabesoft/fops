namespace GabeSoft.FOPS.Service

open System
open System.ComponentModel
open System.Configuration.Install
open System.ServiceProcess
open System.Diagnostics

type FopsService() as this = 
    inherit ServiceBase()

    do 
        this.ServiceName <- "File Operations Service"
        this.EventLog.Log <- "Application"

    override this.OnStart(args:string[]) =
        Debug.WriteLine ("FOPS SERVICE STARTING")
        base.OnStart(args)

    override this.OnStop() = 
        Debug.WriteLine ("FOPS SERVICE STOPPING")
        base.OnStop()

    static member Main () =
        ServiceBase.Run(new FopsService())

