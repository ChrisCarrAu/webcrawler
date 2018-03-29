open FSharp.Data
open System

type SpiderUri = 
    { 
        Address : Uri;
        Text : string option;
        Depth : int32 
    }
//    | Address of string
//    | Depth of int


[<EntryPoint>]
let main args =

    // Example:
    //Helpers.crawl "http://news.google.com" 25

//    let crawledUri baseUri relative text = 
//        { Address = new Uri(baseUri.Address, relative); Text = Some(text); Depth = baseUri.Depth }

    let parseAnchors (uri : SpiderUri) = 
        HtmlDocument.Load(uri.Address.ToString()).Descendants["a"]
        |> Seq.choose ( fun htmlNode -> 
            htmlNode.TryGetAttribute("href")
            |> Option.map (fun a -> { Text = Some(htmlNode.InnerText()); Address = Uri(uri.Address, a.Value()); Depth = uri.Depth + 1 } ) )

    let rec spiderAgent = MailboxProcessor.Start(fun uriQueue->
        let rec crawlLoop() = async {
            let! uri = uriQueue.Receive()
            
            parseAnchors(uri)
            |> Seq.iter (fun f -> 
                printfn "%A %d" f.Address f.Depth;
                if (f.Depth < 2) then 
                    spiderAgent.Post f
                )

            return! crawlLoop()
        }

        crawlLoop()
    )

    spiderAgent.Post { Address = Uri("http://appthem.com"); Depth = 0; Text = None }
    
    Console.ReadLine() |> ignore

    0 // return an integer exit code

