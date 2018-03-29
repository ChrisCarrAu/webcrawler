open FSharp.Data
open System

type SpiderUri = 
    { 
        Address : Uri;
        Text : string option;
        crawlDepth : int32 
    }

type SpiderResult =
    {
        htmlNode : HtmlNode;
        href : HtmlAttribute;
    }

[<EntryPoint>]
let main args =

    // Example:
    //Helpers.crawl "http://news.google.com" 25

    let parseAnchors (uri : SpiderUri) = 
        HtmlDocument.Load(uri.Address.ToString()).Descendants["a"]

    let parseAnchorLinks (anchors : seq<HtmlNode>) =
        anchors
        |> Seq.choose ( fun htmlNode -> 
            htmlNode.TryGetAttribute("href")
            |> Option.map(fun h -> { htmlNode = htmlNode; href = h } ))

    let spiderUri ( spiderResult ) ( uribase : SpiderUri ) =
        { Text = Some(spiderResult.htmlNode.InnerText()); Address = Uri(uribase.Address, spiderResult.href.Value()); crawlDepth = uribase.crawlDepth + 1 }

    let rec spiderAgent = MailboxProcessor.Start(fun uriQueue->
        let rec crawlLoop() = async {
            let! uri = uriQueue.Receive()
            
            parseAnchors(uri) 
            |> parseAnchorLinks
            |> Seq.map (fun f -> spiderUri f uri)
            |> Seq.iter (fun f -> 
                printfn "%A %d" f.Address f.crawlDepth;
                if f.crawlDepth < 2 then 
                    spiderAgent.Post f
                )

            return! crawlLoop()
        }

        crawlLoop()
    )

    spiderAgent.Post { Address = Uri("http://appthem.com"); crawlDepth = 0; Text = None }
    //spiderAgent.Post { Address = Uri("https://www.bikesales.com.au"); crawlDepth = 0; Text = None }
    
    Console.ReadLine() |> ignore

    0 // return an integer exit code

