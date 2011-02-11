namespace GabeSoft.FOPS.Core

module CopyEngine =

   open System

   let private copy (server: IOServer) (f, t, o, e) =
      // TODO: dest should not be added to excludes if
      //       we're copying a single file                  !
      //       or if the wildcard pattern is not recursive  ?
      let dest = Wildcard.matchAll t
      let spec = {
         Pattern = Wildcard.toRegex f
         Exclude = dest :: (e |> List.map Wildcard.toRegex)
         Recursive = Wildcard.isRecursive f }
      let node = f |> Wildcard.root |> server.Node |> Filter.apply spec
      node.AllFiles 
      |> Seq.iter (fun f -> 
            let dest = Path.combine t (Path.file f.Path)
            server.Provider.Copy f.Path dest)
            
   /// TODO: check file existence if necessary
   let private runItem (server: IOServer) = function
   | Copy (f, t, o, e, c)  -> printfn "copy from %s to %s" f t
   | Link (f, t, o, e, c)  -> printfn "link from %s to %s" f t
   | Yank (f)              -> printfn "yank from %s" f

   let private runJob (server: IOServer) (job: Job) =
      job.ItemsAbsolute |> Seq.iter (runItem server)
         
   let run server jobs = 
      jobs |> Seq.iter (runJob server)
      

