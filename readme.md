# Web Crawler

A simple web crawler that reports anchor links in real time

## Getting Started

Uses Visual Studio 2017, just load the solution and run the console app, Crawler.

Right now, the starting URI & number of crawl threads are hard coded.

CrawlerFarm is an observable object - Crawler implements IObserver&lt;Anchor&gt; and subscribes to the UriQueue which queues the Uri's to process

### Projects
<ul>
<li><b>Crawler</b> : Console App which uses Crawler.Lib to spider from a given site to a specified depth
<li><b>Crawler.Lib</b> : .NET Core 2.0 library containing the crawler code
<li><b>Crawler.Lib.Tests</b> : Crawler code unit tests
<li><b>FCrawler</b> : Console App written in F# to spider from a given site to a specified depth
</ul>

### Prerequisites

* Visual Studio 2017 (v15.3+)
	* .NET Desktop Development Workload
	* Azure Development Workload
	* .NET Core Cross-Platform Development Workload
* AzureRM PowerShell Module (`Install-Module AzureRM`)
* AzureAD PowerShell Module (`Install-Module AzureAD`)
* (Azure CLI v2) (https://docs.microsoft.com/en-us/cli/azure/install-azure-cli-windows?view=azure-cli-latest)

### Installing

A step by step series of examples that tell you have to get a development env running

Say what the step will be

```
Give the example
```

And repeat

```
until finished
```

End with an example of getting some data out of the system or using it for a little demo

## Running the tests

Automated tests are coming

### Break down into end to end tests

Explain what these tests test and why

```
Give an example
```

### And coding style tests

Explain what these tests test and why

```
Give an example
```

## Deployment

Add additional notes about how to deploy this on a live system

## Built With

The C# projects depend on
* [HtmlAgilityPack](http://html-agility-pack.net/) - An agile HTML parser 
* [Microsoft.Extensions.DependencyInjection](https://www.asp.net/) - Dependency Management
* [NLog](http://nlog-project.org/) - A logging platform for .NET with rich log routing and management capabilities

## Contributing

Just started...

## Versioning

<!--
We use [SemVer](http://semver.org/) for versioning. For the versions available, see the [tags on this repository](https://github.com/your/project/tags). 
-->

## Authors

* **Chris Carr** - *Initial work* - [ChrisCarrAu](https://github.com/ChrisCarrAu)

## License

This project is licensed under the MIT License - see the [LICENSE.md](LICENSE.md) file for details

## Acknowledgments

