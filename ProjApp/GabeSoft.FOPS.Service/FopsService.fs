namespace GabeSoft.FOPS.Service

open System
open System.ComponentModel
open System.Configuration.Install
open System.ServiceProcess

type FopsService() as this = 
    inherit ServiceBase()

    do 
        this.ServiceName <- "File Operations Service"
        this.EventLog.Log <- "Application"

    override this.OnStart(args:string[]) =
        base.OnStart(args)

    override this.OnStop() = 
        base.OnStop()

    static member Main () =
        ServiceBase.Run(new FopsService())

