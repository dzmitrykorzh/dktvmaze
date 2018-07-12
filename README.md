# dk-tvmaze

## Assignment
Create an application that:
1.	Scrapes the [TVMaze API](http://www.tvmaze.com/api) for show and cast information;
2.	Persists the data in storage;
3.	Provides the scraped data using a REST API.

## Solution
  - **TvMaze.ScraperService** - service library to scrap the TVMaze API for shows and cast. Hosted as **IHostedService** inside web api app.
  - **MongoDb** created as Azure Cosmos Db account is used as data storage. 
  - **WebApi** - scraped data REST API. Provides a paginated list of all tv shows containing the id of the TV show and a list of all the cast that are playing in that TV show. The list of the cast is ordered by birthday descending. Available under this link [https://dkshows.azurewebsites.net/](https://dkshows.azurewebsites.net/)
