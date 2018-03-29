open FSharp.Data

type SpiderUri = 
    { 
        Address : string;
        Text : string option;
        Depth : int32 
    }
//    | Address of string
//    | Depth of int


[<EntryPoint>]
let main args =

    // Example:
    //Helpers.crawl "http://news.google.com" 25

    let parseAnchors (uri : SpiderUri) = 
        HtmlDocument.Load(uri.Address).Descendants["a"]
        |> Seq.choose ( fun htmlNode -> 
            htmlNode.TryGetAttribute("href")
            |> Option.map (fun a -> { Text = Some(htmlNode.InnerText()); Address = a.Value(); Depth = uri.Depth + 1 } ) )

    let rec spiderAgent = MailboxProcessor.Start(fun uriQueue->
        let rec crawlLoop() = async {
            let! uri = uriQueue.Receive()
            
            parseAnchors(uri)
            |> Seq.iter (fun f -> 
                printfn "%A %d" f.Address f.Depth;
//                if (f.Depth < 3) then 
                spiderAgent.Post f
                )

            return! crawlLoop()
        }

        crawlLoop()
    )

    spiderAgent.Post { Address = "http://appthem.com"; Depth = 0; Text = None }
    
//    let count = spiderAgent.PostAndReply(fun replyChannel -> Get(replyChannel) )
//    printfn "We got %d" count

    parseAnchors { Address = "http://appthem.com"; Depth = 0; Text = None }
    |> Seq.iter (fun f -> printfn "%A" f.Address)
(*
    async { let! html = Http.AsyncRequestString("http://appthem.com")
        printfn "%d" html.Length } 
    |> Async.Start

    printfn "%A" "Electric boogaloo"
*)    

    0 // return an integer exit code

